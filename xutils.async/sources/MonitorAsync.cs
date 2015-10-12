using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace xutils {

	public class MonitorAsync: IDisposable {

		struct Rec {
			public TaskCompletionSource<IDisposable> tcs;
			public CancellationToken ct;
		};

		bool entered;
		Queue<Rec> waiting = new Queue<Rec>();

		object sync { get { return waiting; } }

		public MonitorAsync() {
		}

		public MonitorAsync(bool entered) {
			this.entered = entered;
		}

		public Task<IDisposable> Enter() {
			TaskCompletionSource<IDisposable> tcs;
			lock (sync) {
				if (!entered) {
					entered = true;
					return Task.FromResult<IDisposable>(this);
				}
				tcs = new TaskCompletionSource<IDisposable>();
				waiting.Enqueue(new Rec { tcs = tcs, ct = CancellationToken.None });
			}
			return tcs.Task;
		}

		public Task<bool> TryEnter(CancellationToken ct) {
			TaskCompletionSource<IDisposable> tcs;
			if (ct.IsCancellationRequested) {
				return Task.FromResult(false);
			}
			lock (sync) {
				if (!entered) {
					entered = true;
					return Task.FromResult(true);
				}
				tcs = new TaskCompletionSource<IDisposable>();
				waiting.Enqueue(new Rec { tcs = tcs, ct = ct });
			}
			return tcs.Task.ContinueWith(
				t => (t.Status == TaskStatus.RanToCompletion && t.Result != null),
				TaskContinuationOptions.ExecuteSynchronously
			);
		}

		public void Leave() {
			Rec rec;
			while (true) {
				lock (sync) {
					if (!entered) {
						throw new Exception("MonitorAsync unbalanced leave call");
					}
					if (waiting.Count == 0) {
						entered = false;
						return;
					}
					rec = waiting.Dequeue();
				}
				if (!rec.ct.IsCancellationRequested) {
					if (rec.tcs.TrySetResult(this)) {
						return;
					}
				} else {
					rec.tcs.TrySetResult(null);
				}
			}
		}

		//TODO: This is very confusing....
		public void Dispose() {
			Leave();
		}
	}

}
