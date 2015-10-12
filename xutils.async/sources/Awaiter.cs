using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace xutils {

	public class Awaiter: IAwaiter, IAwaitable {

		public enum State {
			idle, succeded, failed, canceled
		}

		public State state { get; private set; }
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
		public bool canceled {
			get { return state == State.canceled; }
		}

		public void GetResult() {
			if (state != State.succeded) {
				throw state == State.idle ? new OperationCanceledException() : error;
			}
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

		public bool Succeed() {
			if (state != State.idle) {
				return false;
			}
			state = State.succeded;
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

		public IAwaiter GetAwaiter() {
			return this;
		}

		public static SucceededAwaiter<T> CreateSucceeded<T>(T val) {
			return new SucceededAwaiter<T>(val);
		}
		public static FailedAwaiter<T> CreateFailed<T>(Exception ex) {
			return new FailedAwaiter<T>(ex);
		}
		public static CanceledAwaiter<T> CreateCanceled<T>() {
			return new CanceledAwaiter<T>();
		}
	}

}
