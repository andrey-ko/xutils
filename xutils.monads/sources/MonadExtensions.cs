using System;
using System.Collections.Generic;

namespace xutils {
	public static class MonadExtensions {

		public static void FailAndDispose(this IUnknownMonad monad, Exception error) {
			try {
				monad.Fail(error);
			} catch {
				//swallow error
			}
			try {
				monad.Dispose();
			} catch {
				//swallow error
			}
		}

		public static IDisposable Run<T>(this IEnumerable<MonadStep<T>> impl) {
			var itor = impl.GetEnumerator();
			if (itor != null) {
				var monad = new ItorMonad<T>(itor);
				monad.DoStep();
				return monad;
			}
			return Disposable.dummy;
		}

		public static IDisposable Run(this IEnumerable<MonadStep> impl) {
			var itor = impl.GetEnumerator();
			if (itor != null) {
				var monad = new ItorMonad(itor);
				monad.DoStep();
				return monad;
			}
			return Disposable.dummy;
		}

	}

}

