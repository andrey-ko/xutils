using System;
using System.Collections.Generic;

namespace xutils {

	public partial class ItorMonadSink<T>: MonadSinkAction<T>, IMonad<T> {

		public IEnumerator<MonadStep<T>> itor;
		IMonadSink<T> parent;

		protected bool completed;

		public ItorMonadSink(IEnumerable<MonadStep<T>> itor) {
			this.itor = itor.GetEnumerator();
		}

		public override void Dispose() {
			if (completed) {
				return;
			}
			completed = true;
			itor.Dispose();
		}

		public virtual bool Fail(Exception error) {
			if (completed) {
				return false;
			}
			completed = true;
			parent.Fail(error);
			itor.Dispose();
			parent.Dispose();
			return true;
		}

		public virtual bool Succeed(T val) {
			if (completed) {
				return false;
			}
			completed = true;
			parent.Succeed(val);
			itor.Dispose();
			parent.Dispose();
			return true;
		}

		public void DoStep() {
			Exception error;
			try {
				var current = itor.Current;
				current.Dispose();

				if (!itor.MoveNext()) {
					//TODO: log error
					// dispose
					Dispose();
					parent.Fail(new Exception("monad was unexpectedly ended"));
					//parent.Succeed();
					return;
				}

				current = itor.Current;
				current.Process(this);
				return;

			} catch (Exception ex) {
				error = ex;
			}
			Fail(error);
			Dispose();
		}

		public override void Process(IMonadSink<T> monad) {
			parent = monad;
			DoStep();
		}
	}

}


