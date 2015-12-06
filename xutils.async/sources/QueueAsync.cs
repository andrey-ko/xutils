#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace xutils {

	public class QueueAsync<T>: IDisposable {
		Queue<T> queue = new Queue<T>();
		Queue<TaskCompletionSource<T>> awaiters = new Queue<TaskCompletionSource<T>>();
		object sync = new object();

		public bool disposed {
			get {
				return queue == null;
            }
		}

		public Task<T> Dequeue() {
			lock (sync) {
				if (queue == null) {
					throw new OperationCanceledException();
				}
				if (queue.Count == 0) {
					var tcs = new TaskCompletionSource<T>();
					awaiters.Enqueue(tcs);
					return tcs.Task;
				}
				return Task.FromResult(queue.Dequeue());
			}
		}

		/// <summary>
		/// try to immediately get item out of the queue if any
		/// </summary>
		/// <param name="res">hold dequeued item if succeded, or default value otherwise</param>
		/// <returns>true if item was retrieved, false otherwise</returns>
		public bool TryDequeue(out T res) {
			lock (sync) {
				if (queue == null || queue.Count == 0) {
					res = default(T);
					return false;
				}
				res = queue.Dequeue();
				return true;
			}
		}

		/// <summary>
		/// put item to queue
		/// </summary>
		/// <param name="item">item to enqueue</param>
		/// <returns>false is queue was disposed, true otherwise</returns>
		public bool TryEnqueue(T item) {
			TaskCompletionSource<T> tcs;
			lock (sync) {
				if (queue == null) {
					return false;
				}
				if (awaiters.Count == 0) {
					queue.Enqueue(item);
					return true;
				}
				tcs = awaiters.Dequeue();
			}
			if (!tcs.TrySetResult(item)) {
				//TODO: log error
			};
			return true;
		}

		/// <summary>
		/// put item to queue
		/// </summary>
		/// <param name="item">item to enqueue</param>
		/// <exception cref="OperationCanceledException">if queue was disposed</exception>
		public void Enqueue(T item) {
			TaskCompletionSource<T> tcs;
			lock (sync) {
				if (queue == null) {
					throw new OperationCanceledException();
				}
				if (awaiters.Count == 0) {
					queue.Enqueue(item);
					return;
				}
				tcs = awaiters.Dequeue();
			}
			if (!tcs.TrySetResult(item)) {
				//TODO: log error
			}
		}

		/// <summary>
		/// dispose queue, it will cancel all awaiters if any, 
		/// all further attempts to enqueue or dequeue will fail 
		/// with OperationCanceledException exception
		/// </summary>
		/// <returns>items that were not dequed</returns>
		public Queue<T> Cancel() {
			Queue<TaskCompletionSource<T>> toCancel;
			Queue<T> unhandled;
            lock (sync) {
				toCancel = awaiters;
				unhandled = queue;
				awaiters = null;
				queue = null;
			}
			if (toCancel != null) {
				while (toCancel.Count > 0) {
					if (!toCancel.Dequeue().TrySetCanceled()) {
						//TODO: log error
					}
				}
			}
			return unhandled;
		}

		public void Dispose() {
			Cancel();
		}


	}
}
