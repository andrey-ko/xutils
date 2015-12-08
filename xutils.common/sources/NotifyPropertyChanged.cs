#pragma warning disable 1591
using System;
using System.ComponentModel;
//using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Threading;

namespace xutils {

	public static class NotifyPropertyChangedExtensions {
		public class Subscription: IDisposable {
			INotifyPropertyChanged comp;
			string propName;
			Action act;
			CancellationTokenRegistration ctreg;


			public Subscription(
				INotifyPropertyChanged comp, CancellationToken ct,
				string propName, Action act
			) {
				//Contract.Requires(comp != null);
				this.comp = comp;
				this.propName = propName;
				this.act = act;
				comp.PropertyChanged += OnPropertyChanged;
				ctreg = ct.Register(Dispose);
			}

			public Subscription(INotifyPropertyChanged comp, string propName, Action act) {
				this.comp = comp;
				this.propName = propName;
				this.act = act;
				comp.PropertyChanged += OnPropertyChanged;
			}

			void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
				var cb = Volatile.Read(ref act);
				if (cb != null && e.PropertyName == propName) {
					foreach (Action c in cb.GetInvocationList()) {
						try {
							c();
						} catch (Exception exn) {
							if (!FastFail.Swallow(exn)) {
								throw;
							}
						}
					};
				}
			}

			public void Dispose() {
				var c = Interlocked.Exchange(ref comp, null);
				if (c != null) {
					c.PropertyChanged -= OnPropertyChanged;
					propName = null;
					act = null;
					ctreg.Dispose();
					ctreg = default(CancellationTokenRegistration);
				}
			}
		}

		/// <summary>
		/// Subscribe to specific property changed.  
		/// </summary>
		/// <param name="comp"></param>
		/// <param name="propName"></param>
		/// <param name="act"></param>
		/// <returns></returns>
		public static IDisposable Subscribe(
			this INotifyPropertyChanged comp, string propName, Action act
		) {

			return new Subscription(comp, propName, act);
		}

		public static IDisposable Subscribe<TComp, TProp>(
			this TComp comp, Expression<Func<TComp, TProp>> property, Action act
		) where TComp : INotifyPropertyChanged {

			string propName = ((MemberExpression)property.Body).Member.Name;
			return new Subscription(comp, propName, act);
		}

		public static IDisposable Subscribe<TComp, TProp>(
			this TComp comp, CancellationToken ct,
			Expression<Func<TComp, TProp>> property, Action act
		) where TComp : INotifyPropertyChanged {

			string propName = ((MemberExpression)property.Body).Member.Name;
			return new Subscription(comp, ct, propName, act);
		}
	}
}
