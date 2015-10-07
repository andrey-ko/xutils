using System;


namespace xutils {

	public class SucceedMonadSinkAction<T> : MonadSinkAction<T> {
		public readonly T val;
		public SucceedMonadSinkAction(T val) {
			this.val = val;
		}
		public override void Process(IMonadSink<T> monad) {
			monad.Succeed(val);
		}
		public override void Dispose() {
		}
	}

	public class SucceedMonadAction<T> : MonadAction<T> {
		public readonly T val;
		public SucceedMonadAction(T val) {
			this.val = val;
		}
		public override void Process(IMonad<T> monad) {
			monad.Succeed(val);
        }
		public override void Dispose() {
		}
	}

	public class SucceedMonadAction : MonadAction {
		public SucceedMonadAction() {
		}
		public override void Process(IMonad monad) {
			monad.Succeed();
		}
		public override void Dispose() {
		}
	}


}


