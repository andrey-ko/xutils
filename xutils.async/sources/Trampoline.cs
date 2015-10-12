using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace xutils {
	public class Trampoline {
		bool acquired = false;
		Queue<Action> queue = new Queue<Action>();

		void ProcessQueue(Action act) {

			while (true) {

				try {
					act();
				} catch (Exception err) {
					//TODO: log error
				}

				lock (queue) {
					if (queue.Count <= 0) {
						acquired = false;
						return;
					}
					act = queue.Dequeue();
				}
			}
		}

		public void Post(Action act) {

			lock (queue) {
				if (acquired) {
					queue.Enqueue(act);
					return;
				}
				acquired = true;
			}

			ProcessQueue(act);
		}
	}

}
