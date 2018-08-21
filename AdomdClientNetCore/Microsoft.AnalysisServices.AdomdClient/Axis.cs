using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Axis : ISubordinateObject
	{
		private CellSet cellset;

		private int axisOrdinal = -1;

		private Set set;

		private PositionCollection positions;

		private int hashCode;

		private bool hashCodeCalculated;

		public string Name
		{
			get
			{
				return this.Set.AxisDataset.DataSetName;
			}
		}

		public Set Set
		{
			get
			{
				return this.set;
			}
		}

		public PositionCollection Positions
		{
			get
			{
				return this.positions;
			}
		}

		object ISubordinateObject.Parent
		{
			get
			{
				return this.cellset;
			}
		}

		int ISubordinateObject.Ordinal
		{
			get
			{
				return this.axisOrdinal;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(Axis);
			}
		}

		internal Axis(AdomdConnection connection, IDSFDataSet dataset, string cubeName, CellSet cellSet, int axisOrdinal)
		{
			this.cellset = cellSet;
			this.axisOrdinal = axisOrdinal;
			this.set = new Set(connection, dataset, cubeName, this);
			this.positions = new PositionCollection(connection, this.set, cubeName);
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

		public static bool operator ==(Axis o1, Axis o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Axis o1, Axis o2)
		{
			return !(o1 == o2);
		}
	}
}
