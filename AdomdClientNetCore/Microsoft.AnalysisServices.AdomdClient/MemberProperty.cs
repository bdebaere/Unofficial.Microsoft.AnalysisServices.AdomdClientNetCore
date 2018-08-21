using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MemberProperty : ISubordinateObject
	{
		private DataRow memberAxisRow;

		private int index = -1;

		private Member parentMember;

		private int hashCode;

		private bool hashCodeCalculated;

		public string Name
		{
			get
			{
				if (this.index >= this.memberAxisRow.Table.Columns.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.memberAxisRow.Table.Columns[this.index].ExtendedProperties["MemberPropertyUnqualifiedName"] as string;
			}
		}

		public string UniqueName
		{
			get
			{
				if (this.index >= this.memberAxisRow.Table.Columns.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.memberAxisRow.Table.Columns[this.index].Caption;
			}
		}

		public object Value
		{
			get
			{
				if (this.index >= this.memberAxisRow.Table.Columns.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				object property = AdomdUtils.GetProperty(this.memberAxisRow, this.index);
				if (property is DBNull)
				{
					return null;
				}
				return property;
			}
		}

		object ISubordinateObject.Parent
		{
			get
			{
				return this.parentMember;
			}
		}

		int ISubordinateObject.Ordinal
		{
			get
			{
				return this.index;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(MemberProperty);
			}
		}

		internal MemberProperty(DataRow memberAxisRow, int index, Member parentMember)
		{
			if (memberAxisRow == null)
			{
				throw new ArgumentNullException("memberAxisRow");
			}
			if (index >= memberAxisRow.Table.Columns.Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.memberAxisRow = memberAxisRow;
			this.index = index;
			this.parentMember = parentMember;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override int GetHashCode()
		{
			if (!this.hashCodeCalculated)
			{
				this.hashCode = AdomdUtils.GetHashCode(this);
				this.hashCodeCalculated = true;
			}
			return this.hashCode;
		}

		public override bool Equals(object obj)
		{
			return AdomdUtils.Equals(this, obj as ISubordinateObject);
		}

		public static bool operator ==(MemberProperty o1, MemberProperty o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(MemberProperty o1, MemberProperty o2)
		{
			return !(o1 == o2);
		}
	}
}
