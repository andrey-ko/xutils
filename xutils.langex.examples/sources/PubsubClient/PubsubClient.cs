#pragma warning disable 1591
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
namespace xutils.langex.examples {

	public partial class PubsubClient {
		public void OnSubscribeError(SubscribeError err) {
			err.Match(
				unsolicited: u => {
					Console.WriteLine($"unsolicited event on channel: '{u.channel}'");
				},
				errorOutcome: o => {
					Console.WriteLine($"subscribe request failed: '{o.request}'");
				}
			);
		}

		public void OnSubscribeRequestFailed(object request) {
			OnSubscribeError(
				new SubscribeError.ErrorOutcome(request)
			);
		}

		public void OnUnsolicitedError(string channel) {
			OnSubscribeError(
				new SubscribeError.Unsolicited(channel)
			);
		}

	}
}
