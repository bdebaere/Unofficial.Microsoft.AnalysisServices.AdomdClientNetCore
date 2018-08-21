using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal enum SecurityStatus
	{
		OK,
		InsufficientMemory = -2146893056,
		InvalidHandle,
		UnsupportedFunction,
		ContinueNeeded = 590610,
		CompleteNeeded,
		CompleteAndContinue,
		IncompleteMessage = -2146893032,
		IncompleteCredentials = -2146893024
	}
}
