using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xutils {

	public struct ByteArrayReader {

		public int start;
		public int end;
		public byte[] data;

		public int size {
			get { return end - start; }
		}

		public ByteArrayReader(byte[] array, int offset, int count) {
			data = array;
			start = offset;
			end = offset + count;
		}

		public ByteArrayReader(ArraySegment<byte> segment) {
			data = segment.Array;
			start = segment.Offset;
			end = start + segment.Count;
		}

		public bool TryReadInt8Signed(ref int value) {
			if (size < 1) {
				return false;
			}
			value = (sbyte)data[start++];
			return true;
		}

		public bool TryReadInt8(ref int value) {
			if (size < 1) {
				return false;
			}
			value = data[start++];
			return true;
		}

		public int ReadInt16() {
			int res = data[start];
			res |= data[start + 1] << 8;
			start += 2;
			return res;
		}

		public int ReadInt16BigEndian() {
			int res = data[start] << 8;
			res |= data[start + 1];
			start += 2;
			return res;
		}

		public bool TryReadInt16(ref int value) {
			if (size < 2) {
				return false;
			}
			value = ReadInt16();
			return true;
		}

		public bool TryReadInt16BigEndian(ref int value) {
			if (size < 2) {
				return false;
			}
			value = ReadInt16BigEndian();
			return true;
		}

		public int ReadInt32() {
			int res = data[start];
			res |= data[start + 1] << 8;
			res |= data[start + 2] << 16;
			res |= data[start + 3] << 24;
			start += 4;
			return res;
		}

		public bool TryReadInt32(ref int value) {
			if (size < 2) {
				return false;
			}
			value = ReadInt32();
			return true;
		}

		public bool TryReadSegment(ref ArraySegment<byte> segment, int count) {
			if (size < count) {
				return false;
			}
			segment = new ArraySegment<byte>(data, start, count);
			start += count;
			return true;
		}

		public byte[] CopyToArray() {
			var res = new byte[size];
			Array.Copy(data, start, res, 0, size);
			return res;
		}
	}

}
