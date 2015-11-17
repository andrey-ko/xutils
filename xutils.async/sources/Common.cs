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

	public interface ISuccessfulAwaiter: INotifyCompletion, ICriticalNotifyCompletion {
		bool IsCompleted { get; }
		void GetResult();
	}

	public interface ISuccessfulAwaiter<out TResult>: INotifyCompletion, ICriticalNotifyCompletion {
		bool IsCompleted { get; }
		TResult GetResult();
	}

	public class SuccessfulAwaiterTransform<I, O>: ISuccessfulAwaiter<O> {
		readonly Func<I, O> transform;
		readonly ISuccessfulAwaiter<I> awaiter;

		public SuccessfulAwaiterTransform(ISuccessfulAwaiter<I> awaiter, Func<I, O> transform) {
			this.awaiter = awaiter;
			this.transform = transform;

		}

		public bool IsCompleted {
			get {
				return awaiter.IsCompleted;
            }
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

	public class SuccessfulAwaiterTransform<O>: ISuccessfulAwaiter<O> {
		readonly Func<O> transform;
		readonly ISuccessfulAwaiter awaiter;

		public SuccessfulAwaiterTransform(ISuccessfulAwaiter awaiter, Func<O> transform) {
			this.awaiter = awaiter;
			this.transform = transform;

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

	public static class AwaiterExtensions {

		public static ISuccessfulAwaiter<O> Map<I, O>(this ISuccessfulAwaiter<I> awaiter, Func<I, O> transform) {
			return new SuccessfulAwaiterTransform<I, O>(awaiter, transform);
        }

		public static ISuccessfulAwaiter<O> Map<O>(this ISuccessfulAwaiter awaiter, Func<O> transform) {
			return new SuccessfulAwaiterTransform<O>(awaiter, transform);
		}

		public static ISuccessfulAwaiter<T> GetAwaiter<T>(this ISuccessfulAwaiter<T> awaiter) {
			return awaiter;
		}

		public static ISuccessfulAwaiter GetAwaiter(this ISuccessfulAwaiter awaiter) {
			return awaiter;
		}

		public static IAwaiter GetAwaiter(this IAwaiter awaiter) {
			return awaiter;
		}

		public static IAwaiter<T> GetAwaiter<T>(this IAwaiter<T> awaiter) {
			return awaiter;
		}
	}

}
