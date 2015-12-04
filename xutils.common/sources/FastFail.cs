using System;
using System.Threading;

namespace xutils {

	public static class FastFail {
		public static event Action<Exception> onError;
		static readonly object sync = new object();
		public static bool swallow = true;
		public static bool Swallow(Exception exception) {
			var handlers = Volatile.Read(ref FastFail.onError);
			if(handlers != null) {
				handlers(exception);
			}
			return swallow;
        }
		
	}
}
