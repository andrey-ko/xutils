using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace xutils
{
    /// <summary>
    /// Lock-free gate to delay some cleanup ('disposal') until after something (eg operation) is complete.
    /// </summary>
    /// <remarks>Warning: this is a struct </remarks>
	public struct DGate {
        //1st bit is 'dispose' marker and the rest (unsigned porition) is the entered counter
        int state;

        /// <summary>
        /// Register hold to prevent gated cleanup.
        /// </summary>
        /// <returns> If enter succeded. Failed enter means that the gated target is cleaned and so could no longer be entered.
        /// Internally, if we're not in "disposed" state, entering increments the entry count and returns true. 
        /// If we are in "disposed" state, entry fails and false is returned.</returns>
		public bool Enter() {
            var origin = state;
            while (true) {
                if ((origin & int.MinValue) != 0) { //if "disposed"
                    return false;
                }
                var tmp = Interlocked.CompareExchange(ref state, origin + 1, origin);
                if (tmp == origin) {
                    return true;
                }
                origin = tmp;
            }
        }

        /// <summary>
        /// Leave must follow previous entry: release previous hold.
        /// </summary>
        /// <returns>True if the gated target is to be cleaned up: nothing else is entered and the gate is in the "disposed" state.
        /// False if the entity is still "active": either because there is more (unrleased) entries, or that target hasn't been set for disposal.
        /// </returns>
        public bool Leave() {
            return Interlocked.Decrement(ref state) == int.MinValue;
        }

        /// <summary>
        /// Mark the target for cleanup. 
        /// </summary>
        /// <returns>True if nothing is entered: in other words, there is no holders and so cleanup should be perfomed. 
        /// False if there is current (unleft) entries: this signals that entity cannot be "disposed" yet, till everything leaves</returns>
        /// <remarks>Callers should perform immidiate cleanup if the call returns true. 
        /// Otherwise cleanup is performed on the first Leave() returning true </remarks>
        public bool Dispose() {
            return InterlockedEx.Or(ref state, int.MinValue) == int.MinValue;
        }

    }
}
