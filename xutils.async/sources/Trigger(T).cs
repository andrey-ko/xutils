﻿#pragma warning disable 1591
using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace xutils {

	/// <summary>
	/// Thread safe version of Awaiter(T)
	/// </summary>
	/// <typeparam name="TResult">Type of Result</typeparam>
	/// <remarks>
	/// Subscribers will be called in FIFO order
	/// </remarks>
	/// <note>
	/// No Execution/Synchronization context are cuptured/restored
	/// Continuation after awaiting on the trigger will happen on completion thread
	/// </note>
	public partial class Trigger<TResult> {
		public abstract class State {
			private State() { }
			public abstract void OnCompleted(ref State state, Action cont);
			public abstract bool IsCompleted { get; }
			public abstract bool IsSucceeded { get; }
			public abstract bool IsFailed { get; }
			public abstract TResult GetResult();
			public abstract bool CompleteAs(ref State state, Completed completedState);

			public bool CompleteAsSucceeded(ref State state, TResult result) {
				return CompleteAs(ref state, new State.Succeeded(result));
			}

			public bool CompleteAsFailed(ref State state, Exception error) {
				return CompleteAs(ref state, new State.Failed(ExceptionDispatchInfo.Capture(error)));
			}

			//public bool CompleteAsCanceled(ref State state, CancellationToken ct) {
			//	return CompleteAs(ref state, new State.Failed(ExceptionDispatchInfo.Capture(new OperationCanceledException(ct))));
			//}

			public bool CompleteAsCanceled(ref State state) {
				return CompleteAs(ref state, new State.Failed(ExceptionDispatchInfo.Capture(new OperationCanceledException())));
			}


			public sealed class Started: State {
				public Started() { }
				public override bool IsCompleted {
					get { return false; }
				}
				public override bool IsSucceeded {
					get { return false; }
				}
				public override bool IsFailed {
					get { return false; }
				}
				public override TResult GetResult() {
					throw new InvalidOperationException();
				}
				public override void OnCompleted(ref State state, Action cont) {
					var newState = new Subscribed(cont, null);
					var s = Interlocked.CompareExchange(ref state, newState, this);
					if (s != this) {
						//log mismatch... ?
						s.OnCompleted(ref state, cont);
					}
				}
				public override bool CompleteAs(ref State state, Completed completedState) {
					var s = Interlocked.CompareExchange(ref state, completedState, this);
					if (s != this) {
						//log mismatch... ?
						return s.CompleteAs(ref state, completedState);
					}
					return true;
				}
			}

			public sealed class Subscribed: State {
				public readonly Action cont;
				public readonly Subscribed next;

				public Subscribed(Action cont, Subscribed next) {
					this.cont = cont;
					this.next = next;
				}
				public override bool IsCompleted {
					get { return false; }
				}
				public override bool IsSucceeded {
					get { return false; }
				}
				public override bool IsFailed {
					get { return false; }
				}
				public override TResult GetResult() {
					throw new InvalidOperationException();
				}
				public override void OnCompleted(ref State state, Action cont) {
					var newState = new State.Subscribed(cont, this);
					var s = Interlocked.CompareExchange(ref state, newState, this);
					if (s != this) {
						//log mismatch... ?
						s.OnCompleted(ref state, cont);
					}
				}
				public override bool CompleteAs(ref State state, Completed completedState) {
					var s = Interlocked.CompareExchange(ref state, completedState, this);
					if (s != this) {
						//log mismatch... ?
						return s.CompleteAs(ref state, completedState);
					}
					var x = this;
					do {
						try {
							x.cont();
						} catch (Exception exn) {
							if (!FastFail.Swallow(exn)) {
								throw;
							}
						}
						x = x.next;
					} while (x != null);
					return true;
				}
			}

			public abstract class Completed: State {
			}

			public sealed class Succeeded: Completed {
				private TResult result;
				public Succeeded(TResult result) {
					this.result = result;
				}
				public override bool IsCompleted {
					get { return true; }
				}
				public override bool IsSucceeded {
					get { return true; }
				}
				public override bool IsFailed {
					get { return false; }
				}
				public override TResult GetResult() {
					return result;
				}
				public override void OnCompleted(ref State state, Action cont) {
					cont();
				}
				public override bool CompleteAs(ref State state, Completed completedState) {
					return false;
				}
			}

			public sealed class Failed: Completed {
				private ExceptionDispatchInfo error;
				public Failed(ExceptionDispatchInfo error) {
					this.error = error;
				}
				public override bool IsCompleted {
					get { return true; }
				}
				public override bool IsSucceeded {
					get { return false; }
				}
				public override bool IsFailed {
					get { return true; }
				}
				public override TResult GetResult() {
					error.Throw();
					return default(TResult);
				}
				public override void OnCompleted(ref State state, Action cont) {
					cont();
				}
				public override bool CompleteAs(ref State state, Completed completedState) {
					return false;
				}
			}

			public static Started started = new Started();
		}

		State state;

		public Trigger() {
			state = State.started;
		}

		public Trigger(TResult result) {
			state = new State.Succeeded(result);
		}
	}

	public partial class Trigger<TResult>: IAwaiter<TResult> {

		public bool IsSucceeded {
			get { return state.IsSucceeded; }
		}

		public bool IsFailed {
			get { return state.IsFailed; }
		}

		public bool IsCompleted {
			get { return state.IsCompleted; }
		}

		public TResult GetResult() {
			return state.GetResult();
		}

		public void OnCompleted(Action cont) {
			state.OnCompleted(ref state, cont);
		}
		public void UnsafeOnCompleted(Action cont) {
			state.OnCompleted(ref state, cont);
		}
	}

	public partial class Trigger<TResult>: ICompletionSink<TResult> {

		public bool Succeed(TResult result) {
			return state.CompleteAsSucceeded(ref state, result);
		}

		public bool Fail(Exception error) {
			return state.CompleteAsFailed(ref state, error);
		}

		public bool Cancel() {
			return state.CompleteAsCanceled(ref state);
		}
	}
}
