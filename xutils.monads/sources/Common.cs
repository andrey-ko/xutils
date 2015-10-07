using System;
using System.Collections.Generic;

namespace xutils {

	public interface IMonadDoStep {
		void DoStep();
	}

	public interface IMonadFail {
		bool Fail(Exception error);
	}

	public interface IMonadSucceed {
		bool Succeed();
	}

	public interface IMonadSucceed<T> {
		bool Succeed(T val);
    }

	public interface IUnknownMonad : IMonadDoStep, IMonadFail, IDisposable {
    }

	public interface IMonadSink<T> : IMonadFail, IMonadSucceed<T>, IDisposable {
	}

	public interface IMonadSink : IMonadFail, IMonadSucceed, IDisposable {
	}

	public interface IMonad : IUnknownMonad, IMonadSink {
	}

	public interface IMonad<T> : IUnknownMonad, IMonadSink<T> {
	}

	






	public interface IMonadAction<in T> : IDisposable {
		void Process(T monad);
	}

	public abstract class MonadAction<T> : IMonadAction<IMonad<T>> {
		public abstract void Process(IMonad<T> monad);
		public abstract void Dispose();
	}

	public abstract class MonadSinkAction : IMonadAction<IMonadSink> {
		public abstract void Process(IMonadSink monad);
		public abstract void Dispose();

	}

	public abstract class MonadSinkAction<T> : IMonadAction<IMonadSink<T>> {
		public abstract void Process(IMonadSink<T> monad);
		public abstract void Dispose();

	}

	public abstract class MonadAction : IMonadAction<IMonad> {
		public abstract void Process(IMonad monad);
		public abstract void Dispose();
	}

	public abstract class UnknownMonadAction : IMonadAction<IUnknownMonad> {
		public abstract void Process(IUnknownMonad monad);
		public abstract void Dispose();
	}

	public struct UnknownMonadStep {

		public IMonadAction<IUnknownMonad> act;

		public UnknownMonadStep(IMonadAction<IUnknownMonad> act) {
			this.act = act;
		}

		public static implicit operator UnknownMonadStep(UnknownMonadAction act) {
			return new UnknownMonadStep(act);
		}
	}

	public struct MonadSinkStep {

		public IMonadAction<IMonadSink> act;

		public MonadSinkStep(IMonadAction<IMonadSink> act) {
			this.act = act;
		}

		public void Process(IMonadSink monad) {
			act.Process(monad);
		}

		public void Dispose() {
			act.Dispose();
		}

		public UnknownMonadStep Await() {
			return new ProxyMonadAction(this);
		}

		public static implicit operator MonadSinkStep(MonadSinkAction act) {
			return new MonadSinkStep(act);
		}
	}

	public struct MonadSinkStep<T> {

		public readonly IMonadAction<IMonadSink<T>> act;

		public MonadSinkStep(IMonadAction<IMonadSink<T>> act) {
			this.act = act;
		}

		public void Process(IMonadSink<T> monad) {
			act.Process(monad);
		}

		public void Dispose() {
			act.Dispose();
		}

		public UnknownMonadStep Await(Action<T> success) {
			return new ProxyMonadStep<T>(this, success);
		}

		public static implicit operator MonadSinkStep<T>(MonadSinkAction<T> act) {
			return new MonadSinkStep<T>(act);
		}

		public static implicit operator MonadSinkStep<T>(T val) {
			return new SucceedMonadSinkAction<T>(val);
		}
	}

}



