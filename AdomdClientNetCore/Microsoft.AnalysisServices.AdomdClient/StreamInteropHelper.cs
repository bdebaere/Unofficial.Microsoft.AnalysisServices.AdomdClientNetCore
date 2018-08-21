using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class StreamInteropHelper
	{
		private StreamInteropHelper()
		{
		}

		public static Stream ProcessRequest(XASC comClass, Stream requestStream)
		{
			return new NativeIStream((IStream)((IXASC)comClass).ProcessRequest(new ManagedIStream(requestStream)));
		}
	}
}
