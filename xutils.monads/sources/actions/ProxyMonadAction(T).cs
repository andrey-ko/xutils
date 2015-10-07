using System;
using System.Collections.Generic;

namespace xutils {

	public class ProxyMonadAction: UnknownMonadAction, IMonadSink {
		IUnknownMonad parent = null;
		MonadSinkStep step;

		public ProxyMonadAction(MonadSinkStep step) {
			this.step = step;
		}

		public override void Process(IUnknownMonad monad) {
			parent = monad;
			step.Process(this);
		}

		public bool Fail(Exception error) {
			parent.FailAndDispose(error);
			return true;
		}

		public bool Succeed() {
			//resume parent
			parent.DoStep();
			return true;
		}

		public override void Dispose() {
			step.Dispose();
		}
	}

	public class ProxyMonadStep<T> : UnknownMonadAction, IMonadSink<T> {
		IUnknownMonad parent = null;
		Action<T> onSucceed;
		Action<Exception> onFail;
		MonadSinkStep<T> step;

		public ProxyMonadStep(MonadSinkStep<T> step, Action<T> onSucceed = null, Action<Exception> onFail = null) {
			this.step = step;
			this.onSucceed = onSucceed;
			this.onFail = onFail;
		}

		public override void Process(IUnknownMonad monad) {
			parent = monad;
			step.Process(this);
        }

		public bool Fail(Exception error) {
			do {
				if (onFail != null) {
					try {
						onFail(error);
					} catch (Exception ex) {
						error = ex;
						break;
					}
				}

				//we can't continue if trigger throw exceptinon
				parent.FailAndDispose(error);
				return false;

			} while (false);

			//resume parent
			parent.DoStep();
			return true;
		}

		public bool Succeed(T val) {
			//fire trigger
			while (onSucceed != null) {
				Exception error;
				try {
					onSucceed(val);
					break;
				} catch (Exception ex) {
					error = ex;
				}
				//fail parent if trigger throws exception
				parent.FailAndDispose(error);
				return false;
			}

			//resume parent
			parent.DoStep();
			return true;
		}

		public override void Dispose() {
			step.Dispose();
		}
	}

}
