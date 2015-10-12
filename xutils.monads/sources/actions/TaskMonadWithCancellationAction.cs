using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace xutils {

	public class TaskMonadWithCancellationAction: MonadSinkAction {
		CancellationTokenSource cts = new CancellationTokenSource();
		IMonadSink parent;
		Func<CancellationToken, Task> fact;


		public TaskMonadWithCancellationAction(Func<CancellationToken, Task> fact) {
			this.fact = fact;
		}

		public void Continuation(Task task) {
			var parent = this.parent;
			this.parent = null;
			if (parent == null) {
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
			fact(cts.Token).ContinueWith(Continuation);
		}

		public override void Dispose() {
			var parent = this.parent;
			this.parent = null;
			if (parent == null) {
				return;
			}
			try {
				cts.Cancel();
			} catch {
				//swallow error;
			}
			cts = null;
		}
	}


}


