using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace xutils {
	public class QueueEx<T>: IEnumerable<T> {
		T[] _buf;
		uint _head = 0;
		uint _length = 0;
		uint _mask;

		/// <summary>
		/// total elemnts in the queue
		/// </summary>
		public uint length {
			get { return _length; }
		}

		/// <summary>
		/// total allocated element slots
		/// </summary>
		public uint capacity {
			get { return (uint)_buf.Length; }
		}

		/// <summary>
		/// first element or default if the queue is empty
		/// </summary>
		public T first {
			get { return _buf[_head]; }
		}

		/// <summary>
		/// last element or default if the queue is empty
		/// </summary>
		public T last {
			get { return _buf[(_head + _length - 1) & _mask]; }
		}


		public T this[uint index] {
			get {
				return _buf[(_head + index) & _mask];
			}
		}

		public QueueEx(uint size) {
			var s = 4u;
			while (s < size) {
				s *= 2;
			}
			_buf = new T[s];
			_mask = s - 1;
		}

		public QueueEx() {
			_buf = new T[4];
			_mask = 3;
		}

		public uint GetPosition(uint index) {
			return (_head + index) & _mask;
		}

		/// <summary>
		/// add new element at the end of the queue
		/// </summary>
		/// <param name="value">element to add to the queue</param>
		public void Enqueue(T value) {
			var cap = (_mask + 1);
			if (_length != cap) {
				_buf[(_head + _length) & _mask] = value;
				_length += 1;
				return;
			}
			//internal buffer needs to be expanded
			cap = cap * 2;
			var newBuf = new T[cap];
			var afterHead = _length - _head;
			Array.Copy(_buf, _head, newBuf, 0, afterHead);
			Array.Copy(_buf, 0, newBuf, afterHead, _head);
			newBuf[_length] = value;
			_buf = newBuf;
			_mask = cap - 1;
			_head = 0;
			_length += 1;
		}

		/// <summary>
		/// rerieve first element from the queue
		/// </summary>
		/// <returns></returns>
		public T Dequeue() {
			T res;
			switch (_length) {
				default:
					res = _buf[_head];
					_buf[_head] = default(T);
					_length -= 1;
					_head = (_head + 1) & _mask;
					return res;
				case 0:
					throw new InvalidOperationException("nothing to dequeue");
				case 1:
					res = _buf[_head];
					_buf[_head] = default(T);
					_length = 0;
					_head = 0;
					return res;
			}
		}

		public bool TryDequeue(out T value) {
			switch (_length) {
				default:
					value = _buf[_head];
					_buf[_head] = default(T);
					_length -= 1;
					_head = (_head + 1) & _mask;
					return true;
				case 0:
					value = default(T);
					return false;
				case 1:
					value = _buf[_head];
					_buf[_head] = default(T);
					_length = 0;
					_head = 0;
					return true;
			}
		}

		public T DequeueOrDefault() {
			T value;
			TryDequeue(out value);
			return value;
		}

		public uint RemoveAll(T value) {
			EqualityComparer<T> comp;
			switch (_length) {
				default:
					comp = EqualityComparer<T>.Default;
					uint i = 1;
					uint pos = _head;
					do {
						if (comp.Equals(_buf[pos], value)) {
							break;
						}
						pos = (pos + 1) & _mask;

						while (!comp.Equals(_buf[pos], value)) {
							i += 1;
							switch (_length - i) {
								case 0:
									return 0;
								case 1:
									pos = (pos + 1) & _mask;
									if (!comp.Equals(_buf[pos], value)) {
										goto case 0;
									}
									_buf[pos] = default(T);
									_length -= 1;
									return 1;
								default:
									pos = (pos + 1) & _mask;
									break;
							}
						}
					} while (false);

					uint d = 1;
					uint pos_d = pos;
					i += 1;
					pos = (pos + 1) & _mask;
					while (i < _length) {
						var v = _buf[pos];
						if (comp.Equals(v, value)) {
							d += 1;
						} else {
							_buf[pos_d] = v;
							pos_d = (pos_d + 1) & _mask;
						}
						i += 1;
						pos = (pos + 1) & _mask;
					}

					do {
						_buf[pos_d] = default(T);
						pos_d = (pos_d + 1) & _mask;
					} while (pos_d < pos);

					_length -= d;
					return d;
				case 0:
					return 0;
				case 1:
					comp = EqualityComparer<T>.Default;
					if (!comp.Equals(_buf[_head], value)) {
						goto case 0;
					}
					_buf[_head] = default(T);
					_head = 0;
					_length = 0;
					return 1;
			}
		}

		public bool RemoveFirst(T value) {
			EqualityComparer<T> comp;
			switch (_length) {
				default:
					comp = EqualityComparer<T>.Default;
					uint pos = _head;
					uint middle = (_head + _length / 2) & _mask;
					do {
						if (comp.Equals(_buf[pos], value)) {
							while (pos != _head) {
								uint n = (pos - 1) & _mask;
								_buf[pos] = _buf[n];
								pos = n;
							}
							_buf[_head] = default(T);
							_head = (_head + 1) & _mask;
							_length -= 1;
							return true;
						}
						pos = (pos + 1) & _mask;
					} while (pos != middle);

					uint end = (_head + _length - 1) & _mask;
					do {
						if (comp.Equals(_buf[pos], value)) {
							while (pos != end) {
								var n = (pos + 1) & _mask;
								_buf[pos] = _buf[n];
								pos = n;
							}
							_buf[end] = default(T);
							_length -= 1;
							return true;
						}
						if (pos == end) {
							goto case 0;
						}
						pos = (pos + 1) & _mask;
					} while (true);
				//goto case 0;
				case 0:
					return false;
				case 1:
					comp = EqualityComparer<T>.Default;
					if (!comp.Equals(_buf[_head], value)) {
						goto case 0;
					}
					_buf[_head] = default(T);
					_head = 0;
					_length = 0;
					return true;
			}
		}

		public bool RemoveFirst() {
			if (_length > 0) {
				_head = (_head + 1) & _mask;
				_length -= 1;
				return true;
			}
			return false;
		}

		/// <summary>
		/// removes elements in the queue from beginning as long as condition is true
		/// </summary>
		/// <param name="pred">predicate</param>
		/// <returns>count of removed elements</returns>
		/// <remarks>
		/// if predicate throw exception this will be interpreted as false 
		/// and exception will be propogated to caller
		/// </remarks>
		public uint RemoveWhile(Func<T, bool> pred) {
			uint res = _length;
			while (_length > 0 && pred(_buf[_head])) {
				_head = (_head + 1) & _mask;
				_length -= 1;
			}
			return res - _length;
		}

		public bool Contains(T value) {
			var comp = EqualityComparer<T>.Default;
			switch (_length) {
				default:
					uint pos = _head;
					uint end = (_head + _length) & _mask;
					do {
						if (comp.Equals(_buf[pos], value)) {
							return true;
						}
						pos = (pos + 1) & _mask;
					} while (pos != end);
					return false;
				case 0:
					return false;
				case 1:
					return comp.Equals(_buf[_head], value);
			}
		}

		public void Clear() {
			switch (_length) {
				default:
					uint pos = _head;
					uint end = (_head + length) & _mask;
					do {
						_buf[pos] = default(T);
						pos = (pos + 1) & _mask;
					} while (pos != end);
					return;
				case 0:
					return;
				case 1:
					_buf[_head] = default(T);
					_length = 0;
					_head = 0;
					return;
			}
		}

		public IEnumerable<T> ReverseEnumerable() {
			switch (_length) {
				default:
					uint start = (_head - 1) & _mask;
					uint pos = (_head + length - 1) & _mask;
					do {
						yield return _buf[pos];
						//TODO: validate version
						pos = (pos - 1) & _mask;
					} while (pos != start);
					break;
				case 0:
					break;
				case 1:
					yield return _buf[_head];
					break;
			}
		}

		public IEnumerator<T> GetEnumerator() {
			switch (_length) {
				default:
					uint pos = _head;
					uint end = (_head + length) & _mask;
					do {
						yield return _buf[pos];
						//TODO: validate version
						pos = (pos + 1) & _mask;
					} while (pos != end);
					break;
				case 0:
					break;
				case 1:
					yield return _buf[_head];
					break;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public T[] ToArray() {
			switch (_length) {
				default:
					T[] array = new T[_length];
					uint end = (_head + _length) & _mask;
					if (_head < end) {
						Array.Copy(_buf, _head, array, 0, _length);
					} else {
						var s = _length - _head;
						Array.Copy(_buf, _head, array, 0, s);
						Array.Copy(_buf, 0, array, s, end);
					}
					return array;
				case 0:
					return new T[0];
				case 1:
					return new T[] { _buf[_head] };
			}
		}
	}

}
