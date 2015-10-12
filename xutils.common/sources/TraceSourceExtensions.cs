using System;
using System.Diagnostics;

namespace xutils {

	/// <summary>
	/// extension methods for tracing events using TraceSource
	/// </summary>
	public static class TraceSourceExtensions {

		/// <summary>
		/// trace general event to TraceSource
		/// </summary>
		/// <param name="trace">TraceSource</param>
		/// <param name="level">level of event</param>
		/// <param name="message">event message</param>
		[Conditional("TRACE")]
		private static void TraceEvent(TraceSource trace, TraceEventType level, string message) {
			if (trace == null || !trace.Switch.ShouldTrace(level)) {
				return;
			}
			trace.TraceEvent(level, 0, message);
		}

		/// <summary>
		/// trace general event to TraceSource, with formatting one parameter
		/// </summary>
		/// <typeparam name="T">type of parameter</typeparam>
		/// <param name="trace">tracer source instance</param>
		/// <param name="level">level of event</param>
		/// <param name="format">format string for event message</param>
		/// <param name="arg">parameter for formatting</param>
		[Conditional("TRACE")]
		private static void TraceEvent<T>(TraceSource trace, TraceEventType level, string format, T arg) {
			if (trace == null || !trace.Switch.ShouldTrace(level)) {
				return;
			}
			trace.TraceEvent(level, 0, format, arg);
		}


		/// <summary>
		/// trace general event to TraceSource, with formatting two parameters
		/// </summary>
		/// <typeparam name="T1"> type of first parameter</typeparam>
		/// <typeparam name="T2"> type of second parameter</typeparam>
		/// <param name="trace"> tracer source instance</param>
		/// <param name="level"> level of event</param>
		/// <param name="format"> format string for event message</param>
		/// <param name="arg1"> frist parameter for formatting</param>
		/// <param name="arg2"> second parameter for formatting</param>
		[Conditional("TRACE")]
		private static void TraceEvent<T1, T2>(TraceSource trace, TraceEventType level, string format, T1 arg1, T2 arg2) {
			if (trace == null || !trace.Switch.ShouldTrace(level)) {
				return;
			}
			trace.TraceEvent(level, 0, format, arg1, arg2);
		}

		/// <summary>
		/// trace general event to TraceSource, with formatting three parameters
		/// </summary>
		/// <typeparam name="T1"> type of first parameter</typeparam>
		/// <typeparam name="T2"> type of second parameter</typeparam>
		/// <typeparam name="T3"> type of third parameter</typeparam>
		/// <param name="trace"> tracer source instance</param>
		/// <param name="level"> level of event</param>
		/// <param name="format"> format string for event message</param>
		/// <param name="arg1"> frist parameter for formatting</param>
		/// <param name="arg2"> second parameter for formatting</param>
		/// <param name="arg3"> third parameter for formatting</param>
		[Conditional("TRACE")]
		private static void TraceEvent<T1, T2, T3>(TraceSource trace, TraceEventType level, string format, T1 arg1, T2 arg2, T3 arg3) {
			if (trace == null || !trace.Switch.ShouldTrace(level)) {
				return;
			}
			trace.TraceEvent(level, 0, format, arg1, arg2, arg3);
		}


		/// <summary>
		/// trace error to TraceSource
		/// </summary>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="exn"> error to trace </param>
		[Conditional("TRACE")]
		public static void e(this TraceSource trace, Exception exn) {
			TraceEvent(trace, TraceEventType.Error, exn.Message);
		}

		/// <summary>
		/// trace error to TraceSource
		/// </summary>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="message"> error message to trace </param>
		[Conditional("TRACE")]
		public static void e(this TraceSource trace, string message) {
			TraceEvent(trace, TraceEventType.Error, message);
		}

		/// <summary>
		/// trace error to TraceSource, with formatting one parameter
		/// </summary>
		/// <typeparam name="T"> type of the parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg"> parameter for formatting </param>
		[Conditional("TRACE")]
		public static void e<T>(this TraceSource trace, string format, T arg) {
			TraceEvent(trace, TraceEventType.Error, format, arg);
		}

		/// <summary>
		/// trace error to TraceSource, with formatting two parameters
		/// </summary>
		/// <typeparam name="T1"> type of the first parameter </typeparam>
		/// <typeparam name="T2"> type of the second parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg1"> first parameter for formatting </param>
		/// <param name="arg2"> second parameter for formatting </param>
		[Conditional("TRACE")]
		public static void e<T1, T2>(this TraceSource trace, string format, T1 arg1, T2 arg2) {
			TraceEvent(trace, TraceEventType.Error, format, arg1, arg2);
		}

		/// <summary>
		/// trace error to TraceSource, with formatting three parameters
		/// </summary>
		/// <typeparam name="T1"> type of the first parameter </typeparam>
		/// <typeparam name="T2"> type of the second parameter </typeparam>
		/// <typeparam name="T3"> type of the third parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg1"> first parameter for formatting </param>
		/// <param name="arg2"> second parameter for formatting </param>
		/// <param name="arg3"> third parameter for formatting </param>
		[Conditional("TRACE")]
		public static void e<T1, T2, T3>(this TraceSource trace, string format, T1 arg1, T2 arg2, T3 arg3) {
			TraceEvent(trace, TraceEventType.Error, format, arg1, arg2, arg3);
		}

		/// <summary>
		/// trace verbose message to TraceSource
		/// </summary>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="message"> message to trace </param>
		[Conditional("TRACE")]
		public static void v(this TraceSource trace, string message) {
			TraceEvent(trace, TraceEventType.Verbose, message);
		}

		/// <summary>
		/// trace verbose message to TraceSource, with formatting one parameter
		/// </summary>
		/// <typeparam name="T"> type of the first parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg"> parameter for formatting </param>
		[Conditional("TRACE")]
		public static void v<T>(this TraceSource trace, string format, T arg) {
			TraceEvent(trace, TraceEventType.Verbose, format, arg);
		}

		/// <summary>
		/// trace verbose message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T1"> type of the first parameter </typeparam>
		/// <typeparam name="T2"> type of the second parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg1"> first parameter for formatting </param>
		/// <param name="arg2"> second parameter for formatting </param>
		[Conditional("TRACE")]
		public static void v<T1, T2>(this TraceSource trace, string format, T1 arg1, T2 arg2) {
			TraceEvent(trace, TraceEventType.Verbose, format, arg1, arg2);
		}

		/// <summary>
		/// trace verbose message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T1"> type of the first parameter </typeparam>
		/// <typeparam name="T2"> type of the second parameter </typeparam>
		/// <typeparam name="T3"> type of the third parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg1"> first parameter for formatting </param>
		/// <param name="arg2"> second parameter for formatting </param>
		/// <param name="arg3"> third parameter for formatting </param>
		[Conditional("TRACE")]
		public static void v<T1, T2, T3>(this TraceSource trace, string format, T1 arg1, T2 arg2, T3 arg3) {
			TraceEvent(trace, TraceEventType.Verbose, format, arg1, arg2, arg3);
		}


		/// <summary>
		/// trace fatal error message to TraceSource
		/// </summary>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="message"> message text </param>
		[Conditional("TRACE")]
		public static void f(this TraceSource trace, string message) {
			TraceEvent(trace, TraceEventType.Critical, message);
		}

		/// <summary>
		/// trace fatal error message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T"> type of the parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg"> parameter for formatting </param>
		[Conditional("TRACE")]
		public static void f<T>(this TraceSource trace, string format, T arg) {
			TraceEvent(trace, TraceEventType.Critical, format, arg);
		}

		/// <summary>
		/// trace fatal error message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T1"> type of the first parameter </typeparam>
		/// <typeparam name="T2"> type of the second parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg1"> first parameter for formatting </param>
		/// <param name="arg2"> second parameter for formatting </param>
		[Conditional("TRACE")]
		public static void f<T1, T2>(this TraceSource trace, string format, T1 arg1, T2 arg2) {
			TraceEvent(trace, TraceEventType.Critical, format, arg1, arg2);
		}

		/// <summary>
		/// trace fatal error message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T1"> type of the first parameter </typeparam>
		/// <typeparam name="T2"> type of the second parameter </typeparam>
		/// <typeparam name="T3"> type of the third parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg1"> first parameter for formatting </param>
		/// <param name="arg2"> second parameter for formatting </param>
		/// <param name="arg3"> third parameter for formatting </param>
		[Conditional("TRACE")]
		public static void f<T1, T2, T3>(this TraceSource trace, string format, T1 arg1, T2 arg2, T3 arg3) {
			TraceEvent(trace, TraceEventType.Critical, format, arg1, arg2, arg3);
		}


		/// <summary>
		/// trace warning message to TraceSource
		/// </summary>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="message"> message text </param>
		[Conditional("TRACE")]
		public static void w(this TraceSource trace, string message) {
			TraceEvent(trace, TraceEventType.Warning, message);
		}

		/// <summary>
		/// trace warning message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T"> type of the parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg"> parameter for formatting </param>
		[Conditional("TRACE")]
		public static void w<T>(this TraceSource trace, string format, T arg) {
			TraceEvent(trace, TraceEventType.Warning, format, arg);
		}

		/// <summary>
		/// trace warning message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T1"> type of the first parameter </typeparam>
		/// <typeparam name="T2"> type of the second parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg1"> first parameter for formatting </param>
		/// <param name="arg2"> second parameter for formatting </param>
		[Conditional("TRACE")]
		public static void w<T1, T2>(this TraceSource trace, string format, T1 arg1, T2 arg2) {
			TraceEvent(trace, TraceEventType.Warning, format, arg1, arg2);
		}

		/// <summary>
		/// trace warning message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T1"> type of the first parameter </typeparam>
		/// <typeparam name="T2"> type of the second parameter </typeparam>
		/// <typeparam name="T3"> type of the third parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg1"> first parameter for formatting </param>
		/// <param name="arg2"> second parameter for formatting </param>
		/// <param name="arg3"> third parameter for formatting </param>
		[Conditional("TRACE")]
		public static void w<T1, T2, T3>(this TraceSource trace, string format, T1 arg1, T2 arg2, T3 arg3) {
			TraceEvent(trace, TraceEventType.Warning, format, arg1, arg2, arg3);
		}

		/// <summary>
		/// trace warning exception to TraceSource
		/// </summary>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="exn"> exception that should be traced as warning </param>
		[Conditional("TRACE")]
		public static void w(this TraceSource trace, Exception exn) {
			TraceEvent(trace, TraceEventType.Warning, exn.Message);
		}

		/// <summary>
		/// trace information message to TraceSource
		/// </summary>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="message"> message text </param>
		[Conditional("TRACE")]
		public static void i(this TraceSource trace, string message) {
			TraceEvent(trace, TraceEventType.Information, message);
		}

		/// <summary>
		/// trace information message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T"> type of the parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg"> parameter for formatting </param>
		[Conditional("TRACE")]
		public static void i<T>(this TraceSource trace, string format, T arg) {
			TraceEvent(trace, TraceEventType.Information, format, arg);
		}

		/// <summary>
		/// trace information message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T1"> type of the first parameter </typeparam>
		/// <typeparam name="T2"> type of the second parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg1"> first parameter for formatting </param>
		/// <param name="arg2"> second parameter for formatting </param>
		[Conditional("TRACE")]
		public static void i<T1, T2>(this TraceSource trace, string format, T1 arg1, T2 arg2) {
			TraceEvent(trace, TraceEventType.Information, format, arg1, arg2);
		}

		/// <summary>
		/// trace information message to TraceSource, with formatting
		/// </summary>
		/// <typeparam name="T1"> type of the first parameter </typeparam>
		/// <typeparam name="T2"> type of the second parameter </typeparam>
		/// <typeparam name="T3"> type of the third parameter </typeparam>
		/// <param name="trace"> tracer source instance </param>
		/// <param name="format"> format string for error message </param>
		/// <param name="arg1"> first parameter for formatting </param>
		/// <param name="arg2"> second parameter for formatting </param>
		/// <param name="arg3"> third parameter for formatting </param>
		[Conditional("TRACE")]
		public static void i<T1, T2, T3>(this TraceSource trace, string format, T1 arg1, T2 arg2, T3 arg3) {
			TraceEvent(trace, TraceEventType.Information, format, arg1, arg2, arg3);
		}
	}

}
