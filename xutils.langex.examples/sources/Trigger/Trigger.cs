#pragma warning disable 1591
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
namespace xutils.langex.examples {
	partial class Trigger<T> {
		partial class State {
			public static readonly State idle = new Idle();
		}

		State state = State.idle;

		public bool Succeed(T val) {
			return state.Match(
				idle: _ => {
					state = new State.Completed.Succeded(val);
					return true;
				},
				subscribed: s => {
					state = new State.Completed.Succeded(val);
					while (s != null) {
						try {
							s.cont();
						} catch {
							// swallow exception
						}
						s = s.next;
					}
					return true;
				},
				completed: _ => {
					return false;
				}
			);
		}

		public bool Fail(Exception err) {
			switch (state.id) {
				case State.Id.idle:
					state = new State.Completed.Failed(err);
					return true;
				case State.Id.subscribed:
					var subscribed = state.AsSubscribed();
                    state = new State.Completed.Failed(err);
					while (subscribed != null) {
						try {
							subscribed.cont();
						} catch {
							// swallow exception
						}
						subscribed = subscribed.next;
					}
					return true;
				case State.Id.completed:
					return false;
				default:
					throw new Exception("invalid state");
			}
		}

		public void OnCompleted(Action cont) {
			switch (state.id) {
				case State.Id.idle:
					state = new State.Subscribed(cont, null);
					break;
				case State.Id.subscribed:
					var subscribed = state.AsSubscribed();
					state = new State.Subscribed(cont, subscribed);
					break;
				case State.Id.completed:
					try {
						cont();
					} catch {
						// swallow exception
					}
					break;
				default:
					throw new Exception("invalid state");
			}
		}

	}
}
