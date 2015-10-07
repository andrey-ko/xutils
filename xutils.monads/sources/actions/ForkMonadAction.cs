using System;
using System.Collections.Generic;
using System.Threading;

namespace xutils {
	
	public class ForkMonadAction : MonadSinkAction, IMonadSink {
		IMonadSink parent = null;
		MonadSinkStep[] childs = null;

		public ForkMonadAction(MonadSinkStep[] childs) {
			this.childs = childs;
		}

		public override void Process(IMonadSink monad) {
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

		public bool Succeed() {
			var parent = Interlocked.Exchange(ref this.parent, null);
			if (parent == null) {
				return false;
			}
			parent.Succeed();
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


