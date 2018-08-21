using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class EmptyMembersCollection : IMemberCollectionInternal
	{
		int IMemberCollectionInternal.Count
		{
			get
			{
				return 0;
			}
		}

		bool IMemberCollectionInternal.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object IMemberCollectionInternal.SyncRoot
		{
			get
			{
				return this;
			}
		}

		Member IMemberCollectionInternal.this[int index]
		{
			get
			{
				throw new ArgumentOutOfRangeException("index");
			}
		}

		internal EmptyMembersCollection()
		{
		}

		Member IMemberCollectionInternal.Find(string index)
		{
			return null;
		}
	}
}
