using System;
using System.Runtime.CompilerServices;

namespace xutils {

	public interface IAwaiter: ISuccessfulAwaiter {
		bool IsSucceeded { get; }
		bool IsFailed { get; }
	}

	public interface IAwaiter<out TResult>: ISuccessfulAwaiter<TResult> {
		bool IsSucceeded { get; }
		bool IsFailed { get; }
	}

	public interface IAwaitable {
		IAwaiter GetAwaiter();
	}

	public interface IAwaitable<out TResult> {
		IAwaiter<TResult> GetAwaiter();
	}

	public interface ISuccessfulAwaiter: INotifyCompletion, ICriticalNotifyCompletion {
		bool IsCompleted { get; }
		void GetResult();
	}

	public interface ISuccessfulAwaiter<out TResult>: INotifyCompletion, ICriticalNotifyCompletion {
		bool IsCompleted { get; }
		TResult GetResult();
	}

	public interface ISuccessfulAwaitable {
		ISuccessfulAwaiter GetAwaiter();
	}

	public interface ISuccessfulAwaitable<out TResult> {
		ISuccessfulAwaiter<TResult> GetAwaiter();
	}

	public class SuccessfulAwaitableTransform<I, O>: ISuccessfulAwaitable<O>, ISuccessfulAwaiter<O> {
		readonly Func<I, O> transform;
		readonly ISuccessfulAwaiter<I> awaiter;

		public SuccessfulAwaitableTransform(ISuccessfulAwaiter<I> awaiter, Func<I, O> transform) {
			this.awaiter = awaiter;
			this.transform = transform;

		}

		public SuccessfulAwaitableTransform(ISuccessfulAwaitable<I> awaitable, Func<I, O> transform):
			this(awaitable.GetAwaiter(), transform) {
		}

		public bool IsCompleted {
			get {
				return awaiter.IsCompleted;
            }
		}

		public ISuccessfulAwaiter<O> GetAwaiter() {
			return this;
		}

		public O GetResult() {
			return transform(awaiter.GetResult());
        }

		public void OnCompleted(Action continuation) {
			awaiter.OnCompleted(continuation);
        }

		public void UnsafeOnCompleted(Action continuation) {
			awaiter.UnsafeOnCompleted(continuation);
		}
	}

	public class SuccessfulAwaitableTransform<O>: ISuccessfulAwaitable<O>, ISuccessfulAwaiter<O> {
		readonly Func<O> transform;
		readonly ISuccessfulAwaiter awaiter;

		public SuccessfulAwaitableTransform(ISuccessfulAwaiter awaiter, Func<O> transform) {
			this.awaiter = awaiter;
			this.transform = transform;

		}

		public SuccessfulAwaitableTransform(ISuccessfulAwaitable awaitable, Func<O> transform) :
			this(awaitable.GetAwaiter(), transform) {
		}

		public bool IsCompleted {
			get {
				return awaiter.IsCompleted;
			}
		}

		public ISuccessfulAwaiter<O> GetAwaiter() {
			return this;
		}

		public O GetResult() {
			return transform();
		}

		public void OnCompleted(Action continuation) {
			awaiter.OnCompleted(continuation);
		}

		public void UnsafeOnCompleted(Action continuation) {
			awaiter.UnsafeOnCompleted(continuation);
		}
	}

	public static class SuccessfulAwaitableExtensions {
		public static ISuccessfulAwaitable<O> Map<I, O>(this ISuccessfulAwaitable<I> awaitable, Func<I, O> transform) {
			return new SuccessfulAwaitableTransform<I, O>(awaitable, transform);
        }
		public static ISuccessfulAwaitable<O> Map<O>(this ISuccessfulAwaitable awaitable, Func<O> transform) {
			return new SuccessfulAwaitableTransform<O>(awaitable, transform);
		}
	}

}
