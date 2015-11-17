using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace xutils {

	/// <note>
	/// No Execution/Synchronization context are cuptured/restored
	/// Continuation after awaiting on the trigger will happen on completion thread
	/// </note>
	public class Trigger: IAwaiter {
		#region State
		public abstract class State {
			private State() { }
			public virtual void OnCompleted(ref State state, Action cont) {
				cont();
			}
			public virtual bool IsCompleted {
				get { return false; }
			}
			public virtual bool IsSucceeded {
				get { return false; }
			}
			public virtual bool IsFailed {
				get { return false; }
			}
			public virtual void GetResult() {
				throw new InvalidOperationException();
			}
			public virtual bool CompleteAs(ref State state, Completed completedState) {
				return false;
			}

			public bool CompleteAsSucceeded(ref State state) {
				return CompleteAs(ref state, State.succeeded);
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
						} catch (Exception err) {
							//swallow error
							//TODO: log error
						}
						x = x.next;
					} while (x != null);
					return true;
				}
			}

			public abstract class Completed: State {
			}

			public sealed class Succeeded: Completed {
				public override bool IsCompleted {
					get { return true; }
				}
				public override bool IsSucceeded {
					get { return true; }
				}
				public override void GetResult() {
					return;
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
				public override bool IsFailed {
					get { return true; }
				}
				public override void GetResult() {
					error.Throw();
				}
			}
			public static Started started = new Started();
			public static Succeeded succeeded = new Succeeded();
		}
		#endregion

		State state = State.started;

		public Trigger() {
		}

		public bool IsCompleted {
			get { return state.IsCompleted; }
		}

		public bool IsSucceeded {
			get { return state.IsSucceeded; }
		}

		public bool IsFailed {
			get { return state.IsFailed; }
		}

		public void GetResult() {
			state.GetResult();
		}

		public void OnCompleted(Action cont) {
			state.OnCompleted(ref state, cont);
		}

		bool CompleteAs(State.Completed completedState) {
			return state.CompleteAs(ref state, completedState);
		}

		public bool CompleteAsSucceeded() {
			return state.CompleteAsSucceeded(ref state);
		}

		public bool CompleteAsFailed(Exception error) {
			return state.CompleteAsFailed(ref state, error);
		}

		public bool CompleteAsCanceled() {
			return state.CompleteAsCanceled(ref state);
		}

		//public bool CompleteAsCanceled(CancellationToken ct) {
		//	return state.CompleteAsCanceled(ref state, ct);
		//}

		public void UnsafeOnCompleted(Action cont) {
			state.OnCompleted(ref state, cont);
		}
	}

}
