using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using System.IO;

namespace xutils {

	public static class EncodingEx {
		public static string ToBase64(this IEnumerable<byte> bytes, bool insertLineBreaks = false) {
			if (bytes == null) {
				return null;
			}
			var opt = insertLineBreaks ? Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None;
			return Convert.ToBase64String(bytes.ToArray(), opt);
		}
		public static string ToBase64(this byte[] bytes, bool insertLineBreaks = false) {
			if (bytes == null) {
				return null;
			}
			var opt = insertLineBreaks ? Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None;
			return Convert.ToBase64String(bytes, opt);
		}
		public static byte[] FromBase64(this string base64) {
			if (base64 == null) {
				return null;
			}
			return Convert.FromBase64String(base64);
		}
		public static byte[] ToUtf8(this string str) {
			if (str == null) {
				return null;
			}
			return Encoding.UTF8.GetBytes(str);
		}
		public static string FromUtf8(this byte[] utf8) {
			if (utf8 == null) {
				return null;
			}
			return Encoding.UTF8.GetString(utf8);
		}
		public static string FromUtf8(this byte[] utf8, int index, int count) {
			return Encoding.UTF8.GetString(utf8, index, count);
		}
		public static string FromUtf8(this byte[] utf8, int count) {
			return Encoding.UTF8.GetString(utf8, 0, count);
		}

		public static byte[] ToAscii(this string str) {
			if (str == null) {
				return null;
			}
			return Encoding.ASCII.GetBytes(str);
		}
		public static string FromAscii(this byte[] ascii) {
			if (ascii == null) {
				return null;
			}
			return Encoding.ASCII.GetString(ascii);
		}
		
		public static string ToHexDigitString(this byte[] bytes, string sep) {
			return String.Join(sep,
				from b in bytes select b.ToString("X2")
			);
		}
		public static string ToHexDigitString(this byte[] bytes) {
			return bytes.ToHexDigitString(" ");
		}
	}
}
