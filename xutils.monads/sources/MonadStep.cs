using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace xutils {

	public struct MonadStep {

		public IMonadAction<IMonad> act;

		public MonadStep(IMonadAction<IMonad> act) {
			this.act = act;
		}

		public void Process(IMonad monad) {
			act.Process(monad);
		}

		public void Dispose() {
			if (act != null) {
				act.Dispose();
			}
		}

		public static implicit operator MonadStep(MonadAction act) {
			return new MonadStep(act);
		}

		public static implicit operator MonadStep(MonadSinkAction act) {
			return new MonadStep(act);
		}

		public static implicit operator MonadStep(UnknownMonadAction proc) {
			return new MonadStep(proc);
		}

		public static implicit operator MonadStep(UnknownMonadStep unk) {
			return new MonadStep(unk.act);
		}

		public static MonadStep<T> Succeeded<T>(T val) {
			return new SucceedMonadAction<T>(val);
		}

		public static MonadSinkStep<T> FromTask<T>(Task<T> task) {
			return new TaskMonadAction<T>(task);
		}
		public static MonadSinkStep FromTask(Task task) {
			return new TaskMonadAction(task);
		}

		public static MonadSinkStep FromTask(Func<CancellationToken, Task> fact) {
			return new TaskMonadWithCancellationAction(fact);
		}

		//public static UnknownMonadStep From<T>(Task<T> task, Proc<T> onSucceed) {
		//	return new MonadTaskClosure<T>(task, onSucceed);
		//}

		//public static UnknownMonadStep From<T>(Task<T> task, Proc<T> onSucceed, Proc<Exception> onFail) {
		//	return new MonadTaskClosure<T>(task, onSucceed);
		//}

		//public static UnknownMonadStep From<T>(IEnumerable<MonadStep<T>> monad) {
		//	return new TriggerMonad<T>(monad);
		//}

		//public static UnknownMonadStep From<T>(IEnumerable<MonadStep<T>> monad, Proc<T> onSucceed) {
		//	return new TriggerMonad<T>(monad, onSucceed);
		//}
		//public static UnknownMonadStep From<T>(IEnumerable<MonadStep<T>> monad, Proc<T> onSucceed, Proc<Exception> onFail) {
		//	return new TriggerMonad<T>(monad, onSucceed, onFail);
		//}
	}

	//public static class MonadStep {

	//	public class Base : MonadStep {
	//		protected bool proceeded;
	//		public Base() {
	//			this.proceeded = false;
	//		}

	//		public Base(bool proceeded) {
	//			this.proceeded = proceeded;
	//		}

	//		public void Abort(Monad monad) {
	//			if (proceeded) {
	//				return;
	//			}
	//			proceeded = true;
	//			OnAbort(monad);
	//		}

	//		public void Process(Monad monad) {
	//			if (proceeded) {
	//				return;
	//			}
	//			proceeded = true;
	//			OnProcess(monad);
	//           }

	//		public virtual void OnAbort(Monad monad) {
	//			monad.Dispose();
	//		}
	//		public virtual void OnProcess(Monad monad) {
	//			monad.DoStep();
	//		}
	//	}

	//	public class Succeeded : Base {
	//		public override void OnProcess(Monad monad) {
	//			monad.Succeed();
	//		}
	//	}

	//	public class Failed : Base {
	//		public readonly Exception error;
	//		public Failed(Exception error) {
	//			this.error = error;

	//		}
	//		public override void OnProcess(Monad monad) {
	//			monad.Fail(error);
	//		}
	//	}

	//	public class Anonymous : Base {
	//		public readonly Action<Monad> callback;
	//		public Anonymous(Action<Monad> callback) {
	//			this.callback = callback;
	//		}
	//		public override void OnProcess(Monad monad) {
	//			callback(monad);
	//		}
	//	}

	//	public static readonly MonadStep moveNext = new Base();
	//	public static readonly MonadStep succeeded = new Succeeded();


	//}

	//public struct MonadStep {

	//	public static readonly MonadStep MoveNext = new MonadStep(MoveNextStep);

	//	public Action<Monad> cont;

	//	public MonadStep(Action<Monad> cont) {
	//		this.cont = cont;
	//	}

	//	public MonadStep(IEnumerable<MonadStep> monad) {
	//		var itor = monad.GetEnumerator();
	//		cont = (m) => {
	//			new LinkedMonad() {
	//				itor = itor,
	//				parent = m
	//			}.DoStep();
	//		};
	//	}

	//	public static void MoveNextStep(Monad monad) {
	//		monad.DoStep();
	//	}

	//	//public static implicit operator MonadStep(Action<Monad> cont) {
	//	//	return new MonadStep(cont);
	//	//}


	//	//public static explicit operator MonadStep(IEnumerable<MonadStep> fact) {
	//	//	return new MonadStep(fact());
	//	//}

	//	public void Process(Monad monad) {
	//		cont(monad);
	//	}
	//}


}


