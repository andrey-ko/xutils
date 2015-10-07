using System;
using System.Collections.Generic;
using System.Text;

namespace xutils {

	public static class Disposable {
		public static readonly DummyDisposable dummy = new DummyDisposable();
		public static IDisposable Create(IEnumerable<IDisposable> disposables) {
			return new EnumDisposable(disposables);
		}

		public static IDisposable Create(Action act) {
			return new ActionDisposable(act);
		}
	}

	public class ActionDisposable : IDisposable {
		Action act;

		public ActionDisposable(Action act) {
			this.act = act;
		}

		public void Dispose() {
			var t = act;
			act = null;
			if (t != null) {
				try {
					t();
				} catch (Exception exn) {
					//swallow exception
					//TODO: log error
				}
			}
		}
	}

	public class EnumDisposable : IDisposable {
		IEnumerable<IDisposable> disposables;

		public EnumDisposable(IEnumerable<IDisposable> disposables) {
			this.disposables = disposables;
		}

		public void Dispose() {
			var t = disposables;
			disposables = null;
			if (t != null) {
				foreach (var d in disposables) {
					try {
						d.Dispose();
					} catch(Exception exn) {
						//swallow exception
						//TODO: log error
					}
				}
			}
		}
	}

	public class DummyDisposable : IDisposable {
		public void Dispose() {
		}
	}
}
