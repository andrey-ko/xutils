using System;
using System.Collections.Generic;

namespace xutils {

	public static class Monad {
		public static MonadSinkStep Delay(int ms) {
			return new DelayMonadAction(ms);
		}
	}

}



