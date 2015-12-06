#pragma warning disable 1591
using System;
using System.Collections.Generic;

namespace xutils {

	public static class DictionaryExtensions {

		public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key) {
			V result;
			dict.TryGetValue(key, out result);
			return result;
		}

		public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key, V value) {
			V result;
			if (!dict.TryGetValue(key, out result)) {
				return value;
			}
			return result;
		}

		public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key, Func<V> factory) {
			V result;
			if (!dict.TryGetValue(key, out result)) {
				return factory();
			}
			return result;
		}
	}
}
