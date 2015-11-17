using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace xutils {

	public class Awaiter<T>: IAwaiter<T>, IAwaitable<T> {

		public enum State {
			idle, succeded, failed, canceled
		}

		public State state { get; private set; }
		public T result { get; private set; }
		public Exception error { get; private set; }

		public event Action onCompleted {
			add {
				if (state != State.idle) {
					value();
				} else {
					m_onCompleted += value;
				}
			}
			remove {
				if (state != State.idle) {
					m_onCompleted -= value;
				}
			}
		}

		event Action m_onCompleted;
		CancellationTokenRegistration ctr;


		public Awaiter() {
			state = State.idle;
		}

		public Awaiter(CancellationToken ct, bool useSyncCtx = false) {
			if (ct.IsCancellationRequested) {
				state = State.canceled;
			} else {
				this.state = State.idle;
				this.ctr = ct.Register(CancellationRequested, useSyncCtx);
			}
		}

		void CancellationRequested() {
			Cancel();
		}

		public bool IsCompleted {
			get { return state != State.idle; }
		}

		public bool IsSucceeded {
			get { return state == State.succeded; }
		}

		public bool IsFailed {
			get { return state == State.failed; }
		}
		public bool IsCanceled {
			get { return state == State.canceled; }
		}

		public T GetResult() {
			if (state != State.succeded) {
				throw state == State.idle ? new OperationCanceledException() : error;
			}
			return result;
		}

		public void OnCompleted(Action cont) {
			onCompleted += cont;
		}

		void ProcessOnCompleted() {

			ctr.Dispose();
			ctr = default(CancellationTokenRegistration);

			if (m_onCompleted != null) {
				foreach (Action cb in m_onCompleted.GetInvocationList()) {
					try {
						cb();
					} catch (Exception exn) {
						//swallow exception
						//TODO: log error
					}
				}
				m_onCompleted = null;
			}
		}

		public bool Succeed(T result) {
			if (state != State.idle) {
				return false;
			}
			state = State.succeded;
			this.result = result;
			ProcessOnCompleted();
			return true;
		}

		public bool Fail(Exception error) {
			if (state != State.idle) {
				return false;
			}
			state = State.succeded;
			this.error = error;
			ProcessOnCompleted();
			return true;
		}

		public bool Cancel() {
			if (state != State.idle) {
				return false;
			}
			state = State.canceled;
			this.error = new OperationCanceledException();
			ProcessOnCompleted();
			return true;
		}

		//public bool Cancel(CancellationToken ct) {
		//	if(state != State.idle) {
		//		return false;
		//	}
		//	state = State.canceled;
		//	this.error = new OperationCanceledException(ct);
		//	ProcessOnCompleted();
		//	return true;
		//}

		public IAwaiter<T> GetAwaiter() {
			return this;
		}

		public void UnsafeOnCompleted(Action cont) {
			onCompleted += cont;
		}
	}

}
