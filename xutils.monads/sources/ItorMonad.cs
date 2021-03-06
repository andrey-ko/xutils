﻿using System;
using System.Collections.Generic;

namespace xutils {

	public partial class ItorMonad: IMonad {

		public IEnumerator<MonadStep> itor;

		protected bool completed;

		//protected abstract void OnFailed(Exception err);
		//protected abstract void OnSucceeded(T res);
		//protected abstract void OnDisposing();

		public ItorMonad(IEnumerator<MonadStep> itor) {
			this.itor = itor;
		}

		public virtual void Dispose() {
			if (itor != null) {
				itor.Dispose();
				itor = null;
			}

			if (!completed) {
				completed = true;
			}
		}

		public virtual bool Fail(Exception error) {
			if (completed) {
				return false;
			}
			completed = true;
			try {
				//OnFailed(error);
			} catch {
				// swallow
				return false;
			}
			return true;
		}

		public virtual bool Succeed() {
			if (completed) {
				return false;
			}
			completed = true;
			Exception error;
			try {
				//OnSucceeded(val);
				return true;
			} catch (Exception ex) {
				error = ex;
			}
			//OnFailed(error);
			return false;
		}

		public void DoStep() {
			Exception error;
			try {
				var current = itor.Current;
				current.Dispose();

				if (!itor.MoveNext()) {
					// dispose
					Dispose();
					return;
				}

				current = itor.Current;
				current.Process(this);
				return;

			} catch (Exception ex) {
				error = ex;
			}
			Fail(error);
			Dispose();
		}
	}

}


