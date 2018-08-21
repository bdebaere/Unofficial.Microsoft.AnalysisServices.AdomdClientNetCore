using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IMemberCollectionInternal
	{
		int Count
		{
			get;
		}

		bool IsSynchronized
		{
			get;
		}

		object SyncRoot
		{
			get;
		}

		Member this[int index]
		{
			get;
		}

		Member Find(string index);
	}
}
