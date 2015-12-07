#pragma warning disable 1591
using System.Threading.Tasks;

namespace xutils {
	public static class TaskExtensions {

		public static T WaitAndGetResult<T>(this Task<T> task) {
			task.Wait();
			return task.Result;
        }
			
		public static void ContinueWith<T>(this Task<T> task, TaskCompletionSource<T> tcs) {
			task.ContinueWith(t => {
				if (t.IsFaulted) {
					tcs.TrySetException(t.Exception);
				} else if (t.IsCanceled) {
					tcs.TrySetCanceled();
				} else {
					tcs.TrySetResult(t.Result);
				}
			}, TaskContinuationOptions.ExecuteSynchronously);
		}

	}
}
