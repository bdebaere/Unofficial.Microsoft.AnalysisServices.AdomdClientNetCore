using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class CellProperty : ISubordinateObject
	{
		private DataTable cellsTable;

		private DataRow cellRow;

		private int propOrdinal;

		private Cell cell;

		private int hashCode;

		private bool hashCodeCalculated;

		public string Name
		{
			get
			{
				return this.cellsTable.Columns[this.propOrdinal].Caption;
			}
		}

		public string Namespace
		{
			get
			{
				return this.cellsTable.Columns[this.propOrdinal].Namespace;
			}
		}

		public object Value
		{
			get
			{
				object obj = null;
				if (this.cellRow != null)
				{
					obj = this.cellRow[this.propOrdinal];
				}
				else if (this.Name == "CellOrdinal")
				{
					obj = this.cell.Ordinal;
				}
				if (obj is XmlaError)
				{
					throw new AdomdErrorResponseException((XmlaError)obj);
				}
				if (obj is DBNull)
				{
					return null;
				}
				return obj;
			}
		}

		object ISubordinateObject.Parent
		{
			get
			{
				return this.cell;
			}
		}

		int ISubordinateObject.Ordinal
		{
			get
			{
				return this.propOrdinal;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(CellProperty);
			}
		}

		internal CellProperty(DataTable cellsTable, DataRow cellRow, int propOrdinal, Cell cell)
		{
			this.cellsTable = cellsTable;
			this.cellRow = cellRow;
			this.propOrdinal = propOrdinal;
			this.cell = cell;
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

		public static bool operator ==(CellProperty o1, CellProperty o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(CellProperty o1, CellProperty o2)
		{
			return !(o1 == o2);
		}
	}
}
