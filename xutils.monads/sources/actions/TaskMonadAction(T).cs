using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace xutils {

	public class TaskMonadAction<T>: MonadSinkAction<T> {
		private IMonadSink<T> parent;
		public Task<T> task;

		public TaskMonadAction(Task<T> task) {
			this.task = task;
		}

		public void Continuation(Task<T> task) {
			var parent = this.parent;
			this.parent = null;
			if(parent == null) {
				return;
			}
			if (task.Status != TaskStatus.RanToCompletion) {
				parent.Fail(task.Exception);
				parent.Dispose();
				return;
			}
			parent.Succeed(task.Result);
			parent.Dispose();
		}

		public override void Process(IMonadSink<T> monad) {
			this.parent = monad;
			task.ContinueWith(Continuation);
		}

		public override void Dispose() {
		}
		
	}


}


