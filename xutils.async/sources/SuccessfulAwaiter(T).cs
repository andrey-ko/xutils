﻿#pragma warning disable 1591
using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace xutils {
	
	public partial class SuccessfulAwaiter<T> {

		bool completed;
		public T result { get; private set; }

		public event Action onCompleted {
			add {
				if (completed) {
					value();
				} else {
					m_onCompleted += value;
				}
			}
			remove {
				if (!completed) {
					m_onCompleted -= value;
				}
			}
		}

		event Action m_onCompleted;

		public SuccessfulAwaiter() {
			completed = false;
		}

		public SuccessfulAwaiter(T result) {
			completed = true;
			this.result = result;
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
	
	public partial class SuccessfulAwaiter<T>: ISuccessfulAwaiter<T> {

		public bool IsCompleted {
			get { return completed; }
		}

		public T GetResult() {
			return result;
		}

		public void OnCompleted(Action cont) {
			onCompleted += cont;
		}

		public void UnsafeOnCompleted(Action cont) {
			onCompleted += cont;
		}
	}

	public partial class SuccessfulAwaiter<T>: ISuccessfulCompletionSink<T> {

		public bool Succeed(T result) {
			if (completed) {
				return false;
			}
			this.result = result;
			completed = true;
			NotifyOnCompleted();
			return true;
		}
	}
}
