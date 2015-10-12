using System;
using System.Collections.Generic;
using System.Threading;

namespace xutils {

	public class DelayMonadAction: MonadSinkAction {
		Timer timer;
		int ms;

		public DelayMonadAction(int ms) {
			this.ms = ms;
		}

		public override void Dispose() {
			var timer = Interlocked.Exchange(ref this.timer, null);
			if (timer == null) {
				return;
			}
			Console.WriteLine("monad: disposing timer ....");
			timer.Dispose();
		}

		void TimerCallback(object state) {
			Console.WriteLine("monad: on timer callback....");
			var timer = Interlocked.Exchange(ref this.timer, null);
			if (timer == null) {
				Console.WriteLine("monad: timer has been disposed...");
			}
			timer.Dispose();
			var monad = (IMonadSink)state;
			monad.Succeed();
		}

		public override void Process(IMonadSink monad) {
			//TODO;: not thread safe
			timer = new Timer(TimerCallback, monad, ms, -1);
		}

		//public static implicit operator MonadSinkStep(DelayMonadStep step) {
		//	return new MonadSinkStep(step);
		//}
	}


}


