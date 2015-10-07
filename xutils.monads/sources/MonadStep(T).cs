using System;


namespace xutils {
	
	public struct MonadStep<T> {

		public IMonadAction<IMonad<T>> act;

		public MonadStep(IMonadAction<IMonad<T>> act) {
			this.act = act;
		}

		public MonadStep(IMonadAction<IUnknownMonad> act) {
			this.act = act;
		}

		public void Process(IMonad<T> monad) {
			act.Process(monad);
		}

		public void Dispose() {
			if(act != null) {
				act.Dispose();
			}
		}

		public static implicit operator MonadStep<T>(MonadAction<T> proc) {
			return new MonadStep<T>(proc);
		}

		public static implicit operator MonadStep<T>(UnknownMonadAction proc) {
			return new MonadStep<T>(proc);
		}

		public static implicit operator MonadStep<T>(UnknownMonadStep unk) {
			return new MonadStep<T>(unk.act);
		}

		public static implicit operator MonadStep<T>(MonadSinkStep<T> unk) {
			return new MonadStep<T>(unk.act);
		}

		public static implicit operator MonadStep<T>(T val) {
			return new SucceedMonadAction<T>(val);
		}
		

		//public UnknownMonadStep Await(Action<T> success, Action<Exception> fail) {
		//	return new ProxyMonad<T>(this, success, fail);
		//}

		//public UnknownMonadStep Await() {
		//	return new ProxyMonad<T>(this);
		//}

		//public MonadStep(IEnumerable<MonadStep> monad) {
		//	var itor = monad.GetEnumerator();
		//	cont = (m) => {
		//		new SubscribedMonad<T>() {
		//			itor = itor,
		//			parent = m
		//		}.DoStep();
		//	};
		//}
	}

}


