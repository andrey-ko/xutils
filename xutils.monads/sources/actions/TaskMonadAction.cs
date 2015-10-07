using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace xutils {

	public class TaskMonadAction: MonadSinkAction {
		private IMonadSink parent;
		public Task task;

		public TaskMonadAction(Task task) {
			this.task = task;
		}

		public void Continuation(Task task) {
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
			parent.Succeed();
			parent.Dispose();
		}

		public override void Process(IMonadSink monad) {
			this.parent = monad;
			task.ContinueWith(Continuation);
		}

		public override void Dispose() {
		}
	}


}


