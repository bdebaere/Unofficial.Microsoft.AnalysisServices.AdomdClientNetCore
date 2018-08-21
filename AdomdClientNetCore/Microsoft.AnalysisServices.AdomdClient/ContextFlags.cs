using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Flags]
	internal enum ContextFlags
	{
		Delegate = 1,
		MutualAuth = 2,
		ReplayDetect = 4,
		SequenceDetect = 8,
		Confidentiality = 16,
		UseSuppliedCreds = 128,
		AllocateMemory = 256,
		Connection = 2048,
		Stream = 32768,
		Integrity = 65536,
		Identify = 131072,
		NoIntegrity = 8388608
	}
}
