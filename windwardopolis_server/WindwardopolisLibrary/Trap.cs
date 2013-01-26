namespace WindwardopolisLibrary
{
	/// <summary>
	/// Used to set code coverage breakpoints in the code in DEBUG mode only.
	/// </summary>
	public static class Trap
	{
		// to turn breaks off in a debug session set this to false
		private static bool stopOnBreak = true;

		/// <summary>Will break in to the debugger (debug builds only).</summary>
		public static void trap()
		{
#if DEBUG
			if (stopOnBreak)
				System.Diagnostics.Debugger.Break();
#endif
		}

		/// <summary>Will break in to the debugger if breakOn is true (debug builds only).</summary>
		/// <param name="breakOn">Will break if this boolean value is true.</param>
		public static void trap(bool breakOn)
		{
#if DEBUG
			if (stopOnBreak && breakOn)
				System.Diagnostics.Debugger.Break();
#endif
		}
	}
}
