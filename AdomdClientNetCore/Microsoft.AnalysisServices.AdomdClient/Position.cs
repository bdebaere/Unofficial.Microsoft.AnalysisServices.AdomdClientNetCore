using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Position : ISubordinateObject
	{
		private MemberCollection members;

		private Tuple tuple;

		private int hashCode;

		private bool hashCodeCalculated;

		public int Ordinal
		{
			get
			{
				return this.tuple.TupleOrdinal;
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
				return this.tuple;
			}
		}

		int ISubordinateObject.Ordinal
		{
			get
			{
				return this.Ordinal;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(Position);
			}
		}

		internal Position(AdomdConnection connection, Tuple tuple, string cubeName)
		{
			this.tuple = tuple;
			this.members = new MemberCollection(connection, tuple, cubeName);
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

		public static bool operator ==(Position o1, Position o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Position o1, Position o2)
		{
			return !(o1 == o2);
		}
	}
}
