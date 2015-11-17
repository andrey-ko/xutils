using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace xutils {

	public class SuccessfulAwaiter: ISuccessfulAwaitable, ISuccessfulAwaiter {

		public event Action onCompleted {
			add {
				if (!IsCompleted) {
					m_onCompleted += value;
				} else {
					value();
				}
			}
			remove {
				if (!IsCompleted) {
					m_onCompleted -= value;
				}
			}
		}

		event Action m_onCompleted;

		public SuccessfulAwaiter(bool completed = false) {
			IsCompleted = completed;
		}

		public bool IsCompleted {
			get; private set;
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

		public bool Complete() {
			if (IsCompleted) {
				return false;
			}
			IsCompleted = true;
			NotifyOnCompleted();
			return true;
		}

		public void GetResult() {
		}

		public ISuccessfulAwaiter GetAwaiter() {
			return this;
		}

		public void UnsafeOnCompleted(Action cont) {
			onCompleted += cont;
		}
	}

}
