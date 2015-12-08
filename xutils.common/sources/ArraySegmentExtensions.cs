#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Text;

namespace xutils {
	public static class ArraySegmentExtensions {

		public static bool CheckIfValidNotEmpty<T>(this ArraySegment<T> seg) {
			if (seg.Array == null) {
				return false;
			}
			if (seg.Offset < 0 || seg.Offset > seg.Array.Length) {
				return false;
			}
			if (seg.Count < 0 || seg.Count > seg.Array.Length - seg.Offset) {
				return false;
			}
			return true;
		}
		public static T[] ToArray<T>(this ArraySegment<T> seg) {
			var res = new T[seg.Count];
			Array.Copy(seg.Array, seg.Offset, res, 0, seg.Count);
			return res;
		}
	}
}
