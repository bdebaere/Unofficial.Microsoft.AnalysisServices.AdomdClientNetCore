using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Cell : ISubordinateObject
	{
		private DataTable cellsTable;

		private int cellOrdinal;

		private CellPropertyCollection cellProps;

		private DataRow cellRow;

		private CellSet cellSet;

		private int hashCode;

		private bool hashCodeCalculated;

		internal int Ordinal
		{
			get
			{
				return this.cellOrdinal;
			}
		}

		public object Value
		{
			get
			{
				return this.cellSet.Cells.GetCellValue(this.cellRow);
			}
		}

		public string FormattedValue
		{
			get
			{
				return this.cellSet.Cells.GetCellFmtValue(this.cellRow);
			}
		}

		public CellPropertyCollection CellProperties
		{
			get
			{
				if (this.cellProps == null)
				{
					this.cellProps = new CellPropertyCollection(this.cellsTable, this.cellRow, this);
				}
				return this.cellProps;
			}
		}

		object ISubordinateObject.Parent
		{
			get
			{
				return this.cellSet;
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
				return typeof(Cell);
			}
		}

		internal Cell(DataTable cellsTable, int cellOrdinal, DataRow cellRow, CellSet cellSet)
		{
			this.cellsTable = cellsTable;
			this.cellOrdinal = cellOrdinal;
			this.cellRow = cellRow;
			this.cellSet = cellSet;
			this.cellProps = null;
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

		public static bool operator ==(Cell o1, Cell o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Cell o1, Cell o2)
		{
			return !(o1 == o2);
		}
	}
}
