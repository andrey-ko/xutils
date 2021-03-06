﻿#pragma warning disable 1591
using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace xutils {

	/// <summary>
	/// Thread safe version of Awaiter
	/// </summary>
	/// <remarks>
	/// Subscribers will be called in FIFO order
	/// </remarks>
	/// <note>
	/// No Execution/Synchronization context are cuptured/restored
	/// Continuation after awaiting on the trigger will happen on completion thread
	/// </note>
	public partial class SuccessfulTrigger {
		public abstract class State {
			private State() { }
			public abstract bool IsCompleted { get; }
			public abstract void GetResult();
			public abstract void OnCompleted(ref State state, Action cont);
			public abstract bool Complete(ref State state, Completed completedState);

			public sealed class Idle: State {
				public Idle() { }
				public override bool IsCompleted {
					get { return false; }
				}
				public override void GetResult() {
				}
				public override void OnCompleted(ref State state, Action cont) {
					var newState = new Subscribed(cont, null);
					var s = Interlocked.CompareExchange(ref state, newState, this);
					if (s != this) {
						s.OnCompleted(ref state, cont);
					}
				}
				public override bool Complete(ref State state, Completed completedState) {
					var s = Interlocked.CompareExchange(ref state, completedState, this);
					if (s != this) {
						return s.Complete(ref state, completedState);
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

				public override void GetResult() {
				}

				public override void OnCompleted(ref State state, Action cont) {
					var newState = new State.Subscribed(cont, this);
					var s = Interlocked.CompareExchange(ref state, newState, this);
					if (s != this) {
						//log mismatch... ?
						s.OnCompleted(ref state, cont);
					}
				}

				public override bool Complete(ref State state, Completed completedState) {
					var s = Interlocked.CompareExchange(ref state, completedState, this);
					if (s != this) {
						//log mismatch... ?
						return s.Complete(ref state, completedState);
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

			public sealed class Completed: State {
				public Completed() {
				}
				public override bool IsCompleted {
					get { return true; }
				}
				public override void GetResult() {
				}
				public override void OnCompleted(ref State state, Action cont) {
					cont();
				}
				public override bool Complete(ref State state, Completed completedState) {
					return false;
				}
			}
			
			public static Idle started = new Idle();
		}

		State state;

		public SuccessfulTrigger() {
			state = State.started;
        }

	}

	public partial class SuccessfulTrigger: ISuccessfulAwaiter {

		public bool IsCompleted {
			get { return state.IsCompleted; }
		}


		public void GetResult() {
		}

		public void OnCompleted(Action cont) {
			state.OnCompleted(ref state, cont);
		}

		public void UnsafeOnCompleted(Action cont) {
			state.OnCompleted(ref state, cont);
		}
	}

	public partial class SuccessfulTrigger: ISuccessfulCompletionSink {
	
		public bool Succeed() {
			return state.Complete(ref state, new State.Completed());
		}

	}
}
