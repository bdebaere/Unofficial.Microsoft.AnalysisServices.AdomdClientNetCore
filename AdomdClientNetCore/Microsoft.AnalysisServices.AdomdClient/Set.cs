using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Set : ISubordinateObject
	{
		private HierarchyCollection hierarchies;

		private TupleCollection tuples;

		private IDSFDataSet axisDataset;

		private string cubeName;

		private Axis axis;

		private int hashCode;

		private bool hashCodeCalculated;

		internal IDSFDataSet AxisDataset
		{
			get
			{
				return this.axisDataset;
			}
		}

		public HierarchyCollection Hierarchies
		{
			get
			{
				return this.hierarchies;
			}
		}

		public TupleCollection Tuples
		{
			get
			{
				return this.tuples;
			}
		}

		object ISubordinateObject.Parent
		{
			get
			{
				return this.axis;
			}
		}

		int ISubordinateObject.Ordinal
		{
			get
			{
				return 0;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(Set);
			}
		}

		internal Axis ParentAxis
		{
			get
			{
				return this.axis;
			}
		}

		internal Set(AdomdConnection connection, IDSFDataSet dataset, string cubeName, Axis axis)
		{
			this.axisDataset = dataset;
			this.cubeName = cubeName;
			this.hierarchies = new HierarchyCollection(connection, this, cubeName);
			this.tuples = new TupleCollection(connection, this, cubeName);
			this.axis = axis;
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

		public static bool operator ==(Set o1, Set o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Set o1, Set o2)
		{
			return !(o1 == o2);
		}
	}
}
