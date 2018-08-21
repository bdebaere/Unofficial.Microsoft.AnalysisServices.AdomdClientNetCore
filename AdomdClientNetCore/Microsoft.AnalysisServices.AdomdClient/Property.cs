using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Property : ISubordinateObject
	{
		private DataRow dataRow;

		private DataColumn propColumn;

		private int propIndex;

		private object propertyParent;

		private int hashCode;

		private bool hashCodeCalculated;

		public string Name
		{
			get
			{
				return this.propColumn.Caption;
			}
		}

		public object Value
		{
			get
			{
				object property = AdomdUtils.GetProperty(this.dataRow, this.propIndex);
				if (property is DBNull)
				{
					return null;
				}
				return property;
			}
		}

		public Type Type
		{
			get
			{
				return FormattersHelpers.GetColumnType(this.propColumn);
			}
		}

		object ISubordinateObject.Parent
		{
			get
			{
				return this.propertyParent;
			}
		}

		int ISubordinateObject.Ordinal
		{
			get
			{
				return this.propIndex;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(Property);
			}
		}

		internal Property(DataRow dataRow, int propIndex, object propertyParent)
		{
			this.dataRow = dataRow;
			this.propColumn = this.dataRow.Table.Columns[propIndex];
			this.propIndex = propIndex;
			this.propertyParent = propertyParent;
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

		public static bool operator ==(Property o1, Property o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Property o1, Property o2)
		{
			return !(o1 == o2);
		}
	}
}
