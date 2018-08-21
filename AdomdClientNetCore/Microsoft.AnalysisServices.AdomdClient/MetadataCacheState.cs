using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal enum MetadataCacheState
	{
		Empty,
		UpToDate,
		NeedsValidnessCheck,
		Invalid,
		Abandoned
	}
}
