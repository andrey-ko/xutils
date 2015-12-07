#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace xutils {
	public static class QueueAsyncExtensions {
		public static T TryDequeue<T>(this QueueAsync<T> queue) {
			T res;
			queue.TryDequeue(out res);
			return res;
        }
	}
}
