﻿#pragma warning disable 1591
using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace xutils {

	public partial class SuccessfulAwaiter {
		bool completed = false;
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
			this.completed = completed;
		}

		void NotifyOnCompleted() {
			if (m_onCompleted != null) {
				foreach (Action cb in m_onCompleted.GetInvocationList()) {
					try {
						cb();
					} catch (Exception exn) {
						if (!FastFail.Swallow(exn)) {
							throw;
						}
					}
				}
				m_onCompleted = null;
			}
		}
	}

	public partial class SuccessfulAwaiter: ISuccessfulAwaiter {

		public bool IsCompleted {
			get {
				return completed;
			}
		}

		public bool IsSucceeded {
			get {
				throw new NotImplementedException();
			}
		}

		public bool IsFailed {
			get {
				throw new NotImplementedException();
			}
		}

		public void OnCompleted(Action cont) {
			onCompleted += cont;
		}

		public void GetResult() {
		}


		public void UnsafeOnCompleted(Action cont) {
			onCompleted += cont;
		}

	}

	public partial class SuccessfulAwaiter: ISuccessfulCompletionSink {
		
		public bool Succeed() {
			if (IsCompleted) {
				return false;
			}
			completed = true;
			NotifyOnCompleted();
			return true;
		}

	}
}
