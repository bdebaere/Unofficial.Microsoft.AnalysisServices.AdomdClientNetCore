using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal static class XmlaDefaults
	{
		internal const bool Batch_TransactionDefault = true;

		internal const bool Batch_ProcessAffecteObjectsDefault = false;

		internal const bool Batch_SkipVolatileObjects = false;

		internal const long Batch_KeyErrorLimitDefault = 0L;

		internal const bool Cancel_CancelAssociatedDefault = false;
	}
}
