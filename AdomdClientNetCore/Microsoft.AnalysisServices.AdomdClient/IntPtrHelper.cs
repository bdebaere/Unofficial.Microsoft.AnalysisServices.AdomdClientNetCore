using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class IntPtrHelper
	{
		private IntPtrHelper()
		{
		}

		internal static IntPtr Add(IntPtr a, int b)
		{
			return (IntPtr)((long)a + (long)b);
		}
	}
}
