#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace xutils {

	public static partial class Intrinsics {
		public static void print(string txt) {
			Console.Write(txt);
		}

		public static void printn(string txt) {
			Console.WriteLine(txt);
		}

		//public static void getc() {
		//	Console.In.ReadKey(true);
		//}

		//public static void getc(bool intercept) {
		//	Console.ReadKey(intercept);
		//}

		public static IDisposable disposable(IEnumerable<IDisposable> disposables) {
			return Disposable.Create(disposables);
		}

		public static IDisposable disposable(Action act) {
			return Disposable.Create(act);
		}

		public static T run<T>(Func<T> func) {
			return func();
		}

		public static Task<T> ForkJoin<T>(params Task<T>[] tasks) {
			var tcs = new TaskCompletionSource<T>();
			foreach (var task in tasks) {
				task.ContinueWith(tcs);
			}
			return tcs.Task;
		}

		public static Task ForkJoin(params Task[] tasks) {
			var tcs = new TaskCompletionSource<int>();
			foreach (var task in tasks) {
				task.ContinueWith(t => {
					if (t.IsFaulted) {
						tcs.TrySetException(t.Exception);
					} else if (t.IsCanceled) {
						tcs.TrySetCanceled();
					} else {
						tcs.TrySetResult(0);
					}
				}, TaskContinuationOptions.ExecuteSynchronously);
			}
			return tcs.Task;
		}
	}
}
