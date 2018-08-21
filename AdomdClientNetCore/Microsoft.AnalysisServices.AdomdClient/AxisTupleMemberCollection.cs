using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class AxisTupleMemberCollection : IMemberCollectionInternal
	{
		private Tuple tuple;

		private AdomdConnection connection;

		private IDSFDataSet internalAxisMembers;

		private string cubeName;

		public int Count
		{
			get
			{
				return this.internalAxisMembers.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.internalAxisMembers.SyncRoot;
			}
		}

		public Member this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataTable dataTable = this.internalAxisMembers[index];
				return new Member(this.connection, dataTable.Rows[this.tuple.TupleOrdinal], null, null, MemberOrigin.UserQuery, this.cubeName, this.tuple, index, null, null);
			}
		}

		internal AxisTupleMemberCollection(AdomdConnection connection, Tuple tuple, string cubeName)
		{
			this.connection = connection;
			this.tuple = tuple;
			this.internalAxisMembers = tuple.Axis.AxisDataset;
			this.cubeName = cubeName;
		}

		public Member Find(string index)
		{
			int num = 0;
			foreach (DataTable dataTable in this.internalAxisMembers)
			{
				if (dataTable.Columns.Contains("UName") && dataTable.Rows[this.tuple.TupleOrdinal]["UName"].ToString() == index)
				{
					return new Member(this.connection, dataTable.Rows[this.tuple.TupleOrdinal], null, null, MemberOrigin.UserQuery, this.cubeName, this.tuple, num, null, null);
				}
				num++;
			}
			return null;
		}
	}
}
