#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xutils {
	//TODO: current implementation is vulnerable to out-of-band exceptions
	public class TrampolineSynCtx: SynchronizationContext {

		[Serializable]
		public sealed class UnhandledException: InvalidOperationException {
			public UnhandledException(Exception error) : base("trampoline unhandled exception", error) {
			}
			public UnhandledException(SerializationInfo info, StreamingContext context) : base(info, context) {
			}
		}

		interface IPostedAction {
			void Process();
		}

		class PostedAction: IPostedAction {
			SynchronizationContext synctx;
			ExecutionContext ectx;
			object state;
			SendOrPostCallback callback;

			public PostedAction(
				ExecutionContext ectx, SynchronizationContext synctx,
				SendOrPostCallback callback, object state
			) {
				this.ectx = ectx;
				this.synctx = synctx;
				this.callback = callback;
				this.state = state;
			}

			void Invoke() {
				if (Current == null) {
					SetSynchronizationContext(synctx);
				}
				try {
					callback(state);
				} catch {
					//swallow exception
				}
			}

			static void Proxy(object o) {
				var s = (PostedAction)o;
				s.Invoke();
			}

			public void Process() {
				if (ectx != null) {
					try {
						ExecutionContext.Run(ectx, Proxy, this);
					} finally {
						ectx.Dispose();
						ectx = null;
					}
				} else {
					//flow suppressed
					var origin = Current;
					SetSynchronizationContext(synctx);
					Invoke();
					SetSynchronizationContext(origin);
				}
			}
		}

		class PostedAction<T>: IPostedAction {
			SynchronizationContext synctx;
			ExecutionContext ectx;
			T state;
			Action<T> callback;

			public PostedAction(
				ExecutionContext ectx, SynchronizationContext synctx,
				Action<T> callback, T state
			) {
				this.ectx = ectx;
				this.synctx = synctx;
				this.callback = callback;
				this.state = state;
			}

			void Invoke() {
				if (Current == null) {
					SetSynchronizationContext(synctx);
				}
				try {
					callback(state);
				} catch {
					//swallow exception
				}
			}

			static void Proxy(object o) {
				var s = (PostedAction<T>)o;
				s.Invoke();
			}

			public void Process() {
				if (ectx != null) {
					try {
						ExecutionContext.Run(ectx, Proxy, this);
					} finally {
						ectx.Dispose();
						ectx = null;
					}
					ectx = null;
				} else {
					//flow suppressed
					var origin = Current;
					SetSynchronizationContext(synctx);
					Invoke();
					SetSynchronizationContext(origin);
				}
			}
		}

		private bool processing = false;
		QueueEx<IPostedAction> queue = new QueueEx<IPostedAction>();

		public override void Send(SendOrPostCallback d, object state) {
			d(state);
		}

		private void ProcessQueue() {

			while (true) {
				IPostedAction act;
				lock (queue) {
					if (!queue.TryDequeue(out act)) {
						processing = false;
						return;
					}
				}
				act.Process();
			}
		}

		public override void Post(SendOrPostCallback d, object state) {
			var synctx = Current;
			if (synctx == null) {
				synctx = this;
			}
			var ectx = ExecutionContext.Capture();

			var act = new PostedAction(
				ectx, synctx, d, state
			);

			lock (queue) {
				queue.Enqueue(act);
				if (processing) {
					return;
				}
				processing = true;
			}
			ProcessQueue();
		}

		public void Post<T>(Action<T> d, T state) {
			var synctx = Current;
			if (synctx == null) {
				synctx = this;
			}
			var ectx = ExecutionContext.Capture();

			var act = new PostedAction<T>(
				ectx, synctx, d, state
			);

			lock (queue) {
				queue.Enqueue(act);
				if (processing) {
					return;
				}
				processing = true;
			}
			ProcessQueue();
		}


		public override SynchronizationContext CreateCopy() {
			return this;
		}
	}
}
