using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal enum BufferType
	{
		Empty,
		Data,
		Token,
		Parameters,
		Missing,
		Extra,
		Trailer,
		Header
	}
}
