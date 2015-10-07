using System;
using System.Collections;
using System.Collections.Generic;

namespace xutils {
	public class CircularBufferFix<T> : IEnumerable<T> {
		protected T[] _buf = null;
		protected uint _head = 0;
		
		public uint length {get; private set;}

		public T first {
			get {
				return this[0];
			}
		}

		public T last {
			get {
				return this[length - 1];
			}
		}

		public T this[uint index] {
			get {
				if ((uint)index >= length) {
					throw new ArgumentOutOfRangeException("index");
				}
				return _buf[GetRealIndex(index)];
			}
		}

		public CircularBufferFix(uint size) {
			_buf = new T[size];
		}

		public void Enqueue(T value) {
			_buf[GetRealIndex(length)] = value;
			if (length < capacity) {
				++length;
			} else {
				_head = (_head + 1) % capacity;
			}
		}
		
		public bool TryDequeue(out T value) {
			if (length == 0) {
				value = default(T);
				return false;
			}
			length = length-1;
			var pos = GetRealIndex(0);
			value = _buf[pos];
			_buf[pos] = default(T);
			if (length == 0) {
				_head = 0;
			} else {
				_head = (_head + 1) % capacity;
			}
			return true;
		}

		public T DequeueOrDefault() {
			T value;
			TryDequeue(out value);
			return value;
		}

		public T Dequeue() {
			T value;
			if (!TryDequeue(out value)) {
				throw new InvalidOperationException();
			}
			return value;
		}

		public void Clear() {
			length = 0;
			_head = 0;
		}

		public uint capacity {
			get {
				return (uint)_buf.Length;
			}
			set {
				if(value < length) {
					throw new InvalidOperationException();
				}
				var newBuf = new T[value];
				if(length > 0) {
					//copy data to new buffer
					uint end = (_head + length) % capacity;
					if(_head < end) {
						Array.Copy(_buf, _head, newBuf, 0, length);
					} else {
						var s = length - _head;
						Array.Copy(_buf, _head, newBuf, 0, s);
						Array.Copy(_buf, 0, newBuf, s, end);
					}
				}
				_buf = newBuf;
				_head = 0;
				//leave length the same
			}
		}

		public IEnumerator<T> GetEnumerator() {
			for (uint i = 0; i < length; ++i) {
				yield return this[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			for (uint i = 0; i < length; ++i) {
				yield return this[i];
			}
		}

		private uint GetRealIndex(uint index) {
			return (_head + index) % capacity;
		}
	}

}
