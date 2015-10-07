using System.Runtime.CompilerServices;

namespace xutils {

	public interface IAwaiter : INotifyCompletion {
		bool IsCompleted { get; }
		bool IsSucceeded { get; }
		bool IsFailed { get; }
		void GetResult();
	}

	public interface IAwaiter<out TResult> : INotifyCompletion {
		bool IsCompleted { get; }
		bool IsSucceeded { get; }
		bool IsFailed { get; }
		TResult GetResult();
	}

	public interface IAwaitable {
		IAwaiter GetAwaiter();
	}

	public interface IAwaitable<out TResult> {
		IAwaiter<TResult> GetAwaiter();
    }

}
