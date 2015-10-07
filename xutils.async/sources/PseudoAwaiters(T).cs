using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xutils {

	public sealed class SucceededAwaiter<T> : IAwaitable<T>, IAwaiter<T> {
		public readonly T result;

		public SucceededAwaiter(T result) {
			this.result = result;
		}

		public IAwaiter<T> GetAwaiter() {
			return this;
		}

		public bool IsCompleted {
			get { return true; }
		}

		public bool IsSucceeded {
			get { return true; }
		}

		public bool IsFailed {
			get { return false; }
		}

		public T GetResult() {
			return result;
		}

		public void OnCompleted(Action continuation) {
			continuation();
		}
	}

	public sealed class FailedAwaiter<T> : IAwaitable<T>, IAwaiter<T> {
		public readonly Exception error;

		public FailedAwaiter(Exception error) {
			this.error = error;
		}

		public IAwaiter<T> GetAwaiter() {
			return this;
		}

		public bool IsCompleted {
			get { return true; }
		}

		public bool IsSucceeded {
			get { return false; }
		}

		public bool IsFailed {
			get { return true; }
		}

		public T GetResult() {
			throw error;
		}

		public void OnCompleted(Action continuation) {
			continuation();
		}
	}

	public sealed class CanceledAwaiter<T> : IAwaitable<T>, IAwaiter<T> {
		public CanceledAwaiter() {
		}

		public IAwaiter<T> GetAwaiter() {
			return this;
		}

		public bool IsCompleted {
			get { return true; }
		}

		public bool IsSucceeded {
			get { return false; }
		}

		public bool IsFailed {
			get { return true; }
		}

		public T GetResult() {
			throw new OperationCanceledException();
		}

		public void OnCompleted(Action continuation) {
			continuation();
		}
	}
}
