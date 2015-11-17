using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace xutils {

	public class SuccessfulAwaiter<T>: ISuccessfulAwaitable<T>, ISuccessfulAwaiter<T> {

		public enum State {
			idle, completed
		}

		public State state { get; private set; }
		public T result { get; private set; }

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

		public SuccessfulAwaiter() {
			state = State.idle;
		}

		public SuccessfulAwaiter(T result) {
			this.result = result;
			state = State.completed;
		}

		public bool IsCompleted {
			get { return state != State.idle; }
		}
		
		
		public void OnCompleted(Action cont) {
			onCompleted += cont;
		}

		void NotifyOnCompleted() {
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

		public bool Complete(T result) {
			if (state != State.idle) {
				return false;
			}
			this.result = result;
			state = State.completed;
			NotifyOnCompleted();
			return true;
		}

		public T GetResult() {
			return result;
		}

		public ISuccessfulAwaiter<T> GetAwaiter() {
			return this;
		}

		public void UnsafeOnCompleted(Action cont) {
			onCompleted += cont;
		}
	}

}
