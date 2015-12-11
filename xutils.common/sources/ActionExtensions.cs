using System;
using System.Threading;

namespace xutils {

	public static class ActionExtensions {
		public static void InvokeSafe<T>(this Action<T> action, T p1) {
			var cb = Volatile.Read(ref action);
			if (cb == null) {
				return;
			}
			foreach (Action<T> a in action.GetInvocationList()) {
				try {
					a(p1);
				} catch (Exception exn) {
					if (!FastFail.Swallow(exn)) {
						throw;
					}
				}
			}
		}
		public static void InvokeSafe<T1, T2>(this Action<T1, T2> action, T1 p1, T2 p2) {
			var cb = Volatile.Read(ref action);
			if (cb == null) {
				return;
			}
			foreach (Action<T1, T2> a in action.GetInvocationList()) {
				try {
					a(p1, p2);
				} catch (Exception exn) {
					if (!FastFail.Swallow(exn)) {
						throw;
					}
				}
			}
		}
		public static void InvokeSafe<T1, T2, T3>(this Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3) {
			var cb = Volatile.Read(ref action);
			if (cb == null) {
				return;
			}
			foreach (Action<T1, T2, T3> a in action.GetInvocationList()) {
				try {
					a(p1, p2, p3);
				} catch (Exception exn) {
					if (!FastFail.Swallow(exn)) {
						throw;
					}
				}
			}
		}
	}
}

