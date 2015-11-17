using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace xutils {

	public struct TaskSwitcher: INotifyCompletion {
		public void OnCompleted(Action continuation) {
			Task.Factory.StartNew(continuation);
		}

		public bool IsCompleted {
			get { return false; }
		}

		public TaskSwitcher GetAwaiter() {
			return this;
		}
		public void GetResult() {
		}
	}

}
