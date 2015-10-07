using System;
using System.Collections.Generic;
using System.Threading;

namespace xutils {

	public class ForkMonadAction<T> : MonadSinkAction<T>, IMonadSink<T> {
		IMonadSink<T> parent = null;
		MonadSinkStep<T>[] childs = null;

		public ForkMonadAction(MonadSinkStep<T>[] childs) {
			this.childs = childs;
		}

		public override void Process(IMonadSink<T> monad) {
			parent = monad;
			//TODO: not thread safe, not even reenterable
			foreach (var c in childs) {
				c.Process(this);
			}
		}

		public bool Fail(Exception error) {
			var parent = Interlocked.Exchange(ref this.parent, null);
			if (parent == null) {
				return false;
			}
			parent.Fail(error);
			foreach (var c in childs) {
				c.Dispose();
			}
			childs = null;
			parent.Dispose();
			return true;
		}

		public bool Succeed(T val) {
			var parent = Interlocked.Exchange(ref this.parent, null);
			if (parent == null) {
				return false;
			}
			parent.Succeed(val);
			foreach (var c in childs) {
				c.Dispose();
			}
			childs = null;
			parent.Dispose();
			return true;
		}

		public override void Dispose() {
			var parent = Interlocked.Exchange(ref this.parent, null);
			if (parent == null) {
				return;
			}
			foreach (var c in childs) {
				c.Dispose();
			}
			childs = null;
		}
	}


}


