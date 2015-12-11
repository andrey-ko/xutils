using System;
using System.Collections.Generic;
using System.Text;

namespace xutils {
	public static class StringExtensions {

		/// <summary>
		/// Truncate string, adding ellipses when 'overlfow'
		/// </summary>
		/// <param name="maxChars">must be >= 0</param>
		/// <returns>trucated string of total length (including ellipses) less or equal to maxChars. </returns>
		public static string Truncate(this string str, int maxChars, string overlfowTail = "...") {
			if(str.Length <= maxChars) {
				return str;
			}
			var len = maxChars - overlfowTail.Length;
			if (len <= 0) {
				return overlfowTail.Substring(0, maxChars);
			}
			return str.Substring(0, len) + overlfowTail;
		}
	}
}
