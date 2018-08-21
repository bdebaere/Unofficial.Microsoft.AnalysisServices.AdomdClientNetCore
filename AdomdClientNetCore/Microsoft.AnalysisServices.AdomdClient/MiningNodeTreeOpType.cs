using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal enum MiningNodeTreeOpType
	{
		TreeopChildren = 1,
		TreeopSiblings,
		TreeopParent = 4,
		TreeopSelf = 8,
		TreeopDescendants = 16,
		TreeopAncestors = 32
	}
}
