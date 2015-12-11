using System.Diagnostics;

namespace xutils {

	public static partial class dbg {

		[DebuggerHidden]
		[Conditional("DEBUG")]
		public static void Break() {
			if (Debugger.IsAttached) {
				Debugger.Break();
			}
		}

		[DebuggerHidden]
		[Conditional("DEBUG")]
		public static void BreakIf(bool Condition) {
			if (Debugger.IsAttached && Condition) {
				Debugger.Break();
			}
		}

	}

}

