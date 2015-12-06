#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xutils {
	public class InterlockedEx {
		public static bool IncrementIf(ref int loc, Func<int, bool> test) {
			var origin = loc;
			while (true) {
				if (!test(origin)) {
					return false;
				}
				var tmp = Interlocked.CompareExchange(ref loc, origin + 1, origin);
				if (tmp == origin) {
					return true;
				}
				origin = tmp;
			}
		}
		public static int Or(ref int loc, int val) {
			var origin = loc;
			var newVal = origin | val;
			if (newVal == origin) {
				return newVal;
			}
			while (true) {

				var tmp = Interlocked.CompareExchange(ref loc, newVal, origin);
				if (tmp == origin || tmp == newVal) {
					return newVal;
				}
				origin = tmp;
				newVal = origin | val;
			}
		}
	}
}
