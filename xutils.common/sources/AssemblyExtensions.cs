using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace xutils {
	public static class AssemblyExtensions {
		public static Guid GetTypeLibGuid(this Assembly assembly){
			return Marshal.GetTypeLibGuidForAssembly(assembly);
		}
	}
}
