using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Tuple : ISubordinateObject
	{
		private Set axis;

		private int tupleOrdinal;

		private MemberCollection members;

		private int hashCode;

		private bool hashCodeCalculated;

		internal Set Axis
		{
			get
			{
				return this.axis;
			}
		}

		public int TupleOrdinal
		{
			get
			{
				return this.tupleOrdinal;
			}
		}

		public MemberCollection Members
		{
			get
			{
				return this.members;
			}
		}

		object ISubordinateObject.Parent
		{
			get
			{
				return this.Axis.ParentAxis;
			}
		}

		int ISubordinateObject.Ordinal
		{
			get
			{
				return this.TupleOrdinal;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(Tuple);
			}
		}

		internal Tuple(AdomdConnection connection, Set axis, int tupleOrdinal, string cubeName)
		{
			this.axis = axis;
			this.tupleOrdinal = tupleOrdinal;
			this.members = new MemberCollection(connection, this, cubeName);
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

		public static bool operator ==(Tuple o1, Tuple o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Tuple o1, Tuple o2)
		{
			return !(o1 == o2);
		}
	}
}
