using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class SchannelAlgorithms
	{
		public static readonly uint ALG_CLASS_DATA_ENCRYPT = 24576u;

		public static readonly uint ALG_TYPE_STREAM = 2048u;

		public static readonly uint ALG_SID_RC4 = 1u;

		public static readonly uint CALG_RC4 = SchannelAlgorithms.ALG_CLASS_DATA_ENCRYPT | SchannelAlgorithms.ALG_TYPE_STREAM | SchannelAlgorithms.ALG_SID_RC4;
	}
}
