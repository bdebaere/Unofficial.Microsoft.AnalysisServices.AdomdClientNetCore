using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class AxisHierarchyMemberCollection : IMemberCollectionInternal
	{
		private AdomdConnection connection;

		private DataTable memberHierarchyDataTable;

		private string cubeName;

		private Level parentLevel;

		private Member parentMember;

		private string catalog;

		private string sessionId;

		public int Count
		{
			get
			{
				return this.memberHierarchyDataTable.Rows.Count;
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
				return this.memberHierarchyDataTable.Rows.SyncRoot;
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
				return new Member(this.connection, this.memberHierarchyDataTable.Rows[index], this.parentLevel, this.parentMember, MemberOrigin.InternalMemberQuery, this.cubeName, null, index, this.catalog, this.sessionId);
			}
		}

		internal AxisHierarchyMemberCollection(AdomdConnection connection, DataTable memberHierarchyDataTable, string cubeName, Level parentLevel, Member parentMember)
		{
			this.connection = connection;
			this.memberHierarchyDataTable = memberHierarchyDataTable;
			this.cubeName = cubeName;
			this.parentLevel = parentLevel;
			this.parentMember = parentMember;
			this.catalog = this.connection.CatalogConnectionStringProperty;
			this.sessionId = this.connection.SessionID;
		}

		public Member Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			if (!this.memberHierarchyDataTable.Columns.Contains("MEMBER_NAME"))
			{
				throw new NotSupportedException();
			}
			string dataTableFilter = AdomdUtils.GetDataTableFilter("MEMBER_NAME", index);
			DataRow[] array = this.memberHierarchyDataTable.Select(dataTableFilter);
			Member result;
			if (array.Length > 0)
			{
				result = new Member(this.connection, array[0], this.parentLevel, this.parentMember, MemberOrigin.InternalMemberQuery, this.cubeName, null, -1, this.catalog, this.sessionId);
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
