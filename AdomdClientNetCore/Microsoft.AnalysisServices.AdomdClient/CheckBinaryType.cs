using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class CheckBinaryType
	{
		internal static bool Is64BitProcess
		{
			get
			{
				return IntPtr.Size == 8;
			}
		}
	}
}
