using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xutils {

    /// <summary>
    /// TrampolineFifo serializes execution of actions (lock-free).
    /// Warining: !!!! no context flow (sync, exec and etc.), assuming to be implemented in FifoTrampolineSyncCtx
    /// </summary>

    public class TrampolineFifo {
		public class State {
			public enum Id {
				idle, acquired, queued, disposed
			}

			public static readonly State idle = new State(Id.idle);
			public static readonly State acquired = new State(Id.acquired);
			public static readonly State disposed = new State(Id.disposed);

			public readonly Id id;

			protected State(Id id) {
				this.id = id;
			}

			public class Queued : State {
				public Action act;
				public Queued next;
				public Queued(): base(Id.queued) {
				}
			}
		}

		State state = State.idle;

		void ProcessQueueAndRelease() {

			var s = Interlocked.CompareExchange(ref state, State.idle, State.acquired);
			if (s == State.acquired) {
				return;
			}
			State.Queued end = null;

			// we have some queued operations to run

			while (true) {

				var tmp = (State.Queued)s;
				var head = tmp;

				do {
					try {
						head.act();
						head.act = null;
					} catch (Exception err) {
						//TODO: log error 
					}
					head = head.next;
				} while (head != end);

				end = tmp;

				// check if there any new operations, if not, set state as idle and finish
				s = Interlocked.CompareExchange(ref state, State.idle, end);
				if (s == end) {
					return;
				}

				// still have some queued operations to run
			}
		}

		void OnDispose(){

		}
		
		public void Post(Action act) {
			var newState = default(State.Queued);
			var tmp = default(State);
			var s = state;
			while (true) {
				switch (s.id) {
					case State.Id.idle:
						s = Interlocked.CompareExchange(ref state, State.acquired, State.idle);
						if (s == State.idle) {
							act();
							ProcessQueueAndRelease();
							return;
						}
						break;
					case State.Id.acquired:
						if (newState == null) {
							newState = new State.Queued { act = act, next = null };
						} else {
							newState.next = null;
						}
						tmp = Interlocked.CompareExchange(ref state, newState, s);
						if (tmp == s) {
							return;
						}
						s = tmp;
						break;
					case State.Id.queued:
						if (newState == null) {
							newState = new State.Queued {
								act = act,
								next = (State.Queued)s
							};
						} else {
							newState.next = (State.Queued)s;
						}
						tmp = Interlocked.CompareExchange(ref state, newState, s);
						if (tmp == s) {
							return;
						}
						s = tmp;
						break;
				}
			}
		}
		
	}

}
