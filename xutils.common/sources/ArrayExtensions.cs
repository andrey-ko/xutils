using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xutils {
	public static class ArrayExtensions {


		public static void CopyTo<TSrc, TDst>(this TSrc[] src, TDst[] dst, int index, Func<TSrc, TDst> transform) {
			var mappedArray = new TDst[src.Length];
			for (int i = 0; i < src.Length; ++i) {
				dst[i] = transform(src[i]);
			}
		}

		public static T[] Append<T,E>(this T[] array, E[] elements, Func<E, T> transform) {
			if (array == null) {
				return elements.Copy(transform);
			}
			if (elements == null) {
				return array.Copy();
			}
			T[] result = new T[array.Length + elements.Length];
			array.CopyTo(result, 0);
			elements.CopyTo(result, array.Length, transform);
			return result;
		}

		public static T[] Append<T>(this T[] array, params T[] elements) {
			if (array == null) {
				return elements.Copy();
			}
			if(elements == null) {
				return array.Copy();
			}
			T[] result = new T[array.Length + elements.Length];
			array.CopyTo(result, 0);
			elements.CopyTo(result, array.Length);
			return result;
		}

		public static T[] Copy<T>(this T[] array) {
			if(array == null) {
				return null;
			}
			T[] result = new T[array.Length];
			Array.Copy(array, 0, result, 0, array.Length);
			return result;
		}

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
