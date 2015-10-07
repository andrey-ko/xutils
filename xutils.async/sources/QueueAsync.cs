using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace xutils {

	public class QueueAsync<T>: IDisposable {
		Queue<T> queue = new Queue<T>();
		Queue<TaskCompletionSource<T>> awaiters = new Queue<TaskCompletionSource<T>>();
		object gate = new object();

		public Task<T> Dequeue() {
			lock (gate) {
				if(queue == null) {
					throw new Exception();
				}
				if(queue.Count == 0) {
					var tcs = new TaskCompletionSource<T>();
					awaiters.Enqueue(tcs);
					return tcs.Task;
				}
				return Task.FromResult(queue.Dequeue());
			}
		}

		public bool TryDequeue(out T res) {
			lock (gate) {
				if(queue == null) {
					throw new Exception();
				}
				if(queue.Count == 0) {
					res = default(T);
                    return false;
				}
				res = queue.Dequeue();
				return true;
			}
		}

		public void Enqueue(T item) {
			TaskCompletionSource<T> tcs;
			lock (gate) {
				if(queue == null) {
					throw new Exception();
				}
				if(awaiters.Count == 0) {
					queue.Enqueue(item);
					return;
				}
				tcs = awaiters.Dequeue();
			}
			tcs.TrySetResult(item);
		}

		public void Dispose() {
			Queue<TaskCompletionSource<T>> toCancel;
			lock (gate) {
				toCancel = awaiters;
				awaiters = null;
				queue = null;
			}
			if(toCancel != null) {
				while(toCancel.Count > 0) {
					toCancel.Dequeue().TrySetCanceled();
				}
			}
		}


	}
}
