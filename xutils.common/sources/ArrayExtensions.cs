using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xutils {
	public static class ArrayExtensions {

		public static T[] SubArray<T>(this T[] array, int offset, int count) {
			T[] result = new T[count];
			Array.Copy(array, offset, result, 0, count);
			return result;
		}

		public static ArraySegment<T> Segment<T>(this T[] array, int offset, int count) {
			return new ArraySegment<T>(array, offset, count);
		}

		public static ArraySegment<T> Segment<T>(this T[] array, int offset) {
			return new ArraySegment<T>(array, offset, array.Length - offset);
		}

		public static ArraySegment<T> Segment<T>(this T[] array) {
			return new ArraySegment<T>(array);
		}

	}
}
