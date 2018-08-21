using System;
using System.Collections;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class CellCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private CellCollection cells;

			private int currentCellOrdinal;

			private int currentRowOrdinal;

			private int currentRowIndex;

			private DataRow currentRow;

			private Cell currentCell;

			public Cell Current
			{
				get
				{
					if (this.currentCell == null)
					{
						throw new InvalidOperationException();
					}
					return this.currentCell;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(CellCollection cells)
			{
				this.cells = cells;
				this.currentCellOrdinal = -1;
				this.currentRowIndex = -1;
				this.currentRowOrdinal = -1;
				this.currentRow = null;
				this.currentCell = null;
			}

			public bool MoveNext()
			{
				if (this.currentCellOrdinal >= this.cells.Count - 1)
				{
					this.currentCell = null;
					return false;
				}
				this.currentCellOrdinal++;
				DataRow cellRow = null;
				if (this.currentCellOrdinal >= this.cells.firstCellOrdinal && this.currentCellOrdinal <= this.cells.lastCellOrdinal)
				{
					if (this.currentRowOrdinal < this.currentCellOrdinal)
					{
						this.currentRowIndex++;
						this.currentRow = null;
						DataRowCollection rows = this.cells.cellset.Formatter.CellTable.Rows;
						if (this.currentRowIndex < rows.Count)
						{
							this.currentRow = rows[this.currentRowIndex];
						}
						this.currentRowOrdinal = (int)this.currentRow[this.cells.ordinalColumnIndex];
					}
					if (this.currentRowOrdinal == this.currentCellOrdinal)
					{
						cellRow = this.currentRow;
					}
				}
				this.currentCell = new Cell(this.cells.cellset.Formatter.CellTable, this.currentCellOrdinal, cellRow, this.cells.cellset);
				return true;
			}

			public void Reset()
			{
				this.currentCellOrdinal = -1;
				this.currentRowIndex = -1;
				this.currentRow = null;
				this.currentRowOrdinal = -1;
				this.currentRow = null;
				this.currentCell = null;
			}
		}

		private DataRowCollection internalCollection;

		private CellSet cellset;

		private int count = 1;

		private int valueColumnIndex = -1;

		private int fmtValueColumnIndex = -1;

		private int ordinalColumnIndex = -1;

		private int firstCellOrdinal = -1;

		private int lastCellOrdinal = -1;

		private int previouslyIndexedCellOrdinal = -1;

		private int previouslyIndexedCellRowIndex = -1;

		public Cell this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return new Cell(this.cellset.Formatter.CellTable, index, this.GetRowByOrdinal(index), this.cellset);
			}
		}

		public Cell this[int index1, int index2]
		{
			get
			{
				if (2 != this.cellset.Axes.Count)
				{
					throw new ArgumentException(SR.CellIndexer_InvalidNumberOfAxesIndexers(this.cellset.Axes.Count, 2));
				}
				int num = this.cellset.Formatter.AxesList[0][0].Rows.Count;
				int num2 = this.cellset.Formatter.AxesList[1][0].Rows.Count;
				if (index1 < 0 || index1 >= num)
				{
					throw new ArgumentOutOfRangeException("index1", index1, SR.CellIndexer_IndexOutOfRange(0, num));
				}
				if (index2 < 0 || index2 >= num2)
				{
					throw new ArgumentOutOfRangeException("index2", index2, SR.CellIndexer_IndexOutOfRange(1, num2));
				}
				int index3 = index1 + num * index2;
				return this[index3];
			}
		}

		public Cell this[params int[] indexes]
		{
			get
			{
				return this[indexes];
			}
		}

		public Cell this[ICollection indexes]
		{
			get
			{
				if (indexes.Count != this.cellset.Axes.Count)
				{
					throw new ArgumentException(SR.CellIndexer_InvalidNumberOfAxesIndexers(this.cellset.Axes.Count, indexes.Count), "indexes");
				}
				int num = 0;
				int num2 = 1;
				IEnumerator enumerator = indexes.GetEnumerator();
				for (int i = 0; i < indexes.Count; i++)
				{
					enumerator.MoveNext();
					IDSFDataSet iDSFDataSet = this.cellset.Formatter.AxesList[i];
					int num3 = iDSFDataSet[0].Rows.Count;
					if (!(enumerator.Current is int))
					{
						throw new ArgumentException(SR.CellIndexer_InvalidIndexType(i), "indexes");
					}
					int num4 = (int)enumerator.Current;
					if (num4 < 0 || num4 >= num3)
					{
						throw new ArgumentOutOfRangeException("index #" + i.ToString(CultureInfo.CurrentCulture), num4, SR.CellIndexer_IndexOutOfRange(i, num3));
					}
					num += num2 * num4;
					num2 *= num3;
				}
				return new Cell(this.cellset.Formatter.CellTable, num, this.GetRowByOrdinal(num), this.cellset);
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
				return this.internalCollection.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		internal CellCollection(CellSet cellset)
		{
			this.cellset = cellset;
			this.internalCollection = cellset.Formatter.CellTable.Rows;
			AxisCollection.Enumerator enumerator = cellset.Axes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Axis current = enumerator.Current;
				this.count *= current.Set.Tuples.Count;
			}
			this.valueColumnIndex = cellset.Formatter.CellTable.Columns.IndexOf("Value");
			this.fmtValueColumnIndex = cellset.Formatter.CellTable.Columns.IndexOf("FmtValue");
			this.ordinalColumnIndex = cellset.Formatter.CellTable.Columns.IndexOf("CellOrdinal");
			this.firstCellOrdinal = -1;
			this.lastCellOrdinal = -1;
			if (cellset.Formatter.CellTable.Rows.Count > 0)
			{
				this.firstCellOrdinal = (int)cellset.Formatter.CellTable.Rows[0][this.ordinalColumnIndex];
				this.lastCellOrdinal = (int)cellset.Formatter.CellTable.Rows[cellset.Formatter.CellTable.Rows.Count - 1][this.ordinalColumnIndex];
			}
		}

		private DataRow GetRowByOrdinal(int index)
		{
			if (index < this.firstCellOrdinal || index > this.lastCellOrdinal)
			{
				return null;
			}
			int num = this.cellset.Formatter.CellTable.Rows.Count;
			int num2 = index - this.firstCellOrdinal;
			num2 = ((num - 1 < index) ? (num - 1) : index);
			int num3 = 0;
			if (this.previouslyIndexedCellOrdinal != -1 && this.previouslyIndexedCellRowIndex != -1)
			{
				if (index == this.previouslyIndexedCellOrdinal)
				{
					return this.cellset.Formatter.CellTable.Rows[this.previouslyIndexedCellRowIndex];
				}
				if (index > this.previouslyIndexedCellOrdinal)
				{
					num3 = this.previouslyIndexedCellRowIndex + 1;
				}
				else
				{
					num2 = Math.Min(num2, this.previouslyIndexedCellOrdinal - 1);
				}
			}
			DataRow dataRow = this.cellset.Formatter.CellTable.Rows[num3];
			int num4 = (int)dataRow[this.ordinalColumnIndex];
			if (num4 == index)
			{
				this.previouslyIndexedCellOrdinal = index;
				this.previouslyIndexedCellRowIndex = num3;
				return dataRow;
			}
			dataRow = this.cellset.Formatter.CellTable.Rows[num2];
			num4 = (int)dataRow[this.ordinalColumnIndex];
			if (num4 == index)
			{
				this.previouslyIndexedCellOrdinal = index;
				this.previouslyIndexedCellRowIndex = num2;
				return dataRow;
			}
			int i = num3 + 1;
			int num5 = num2 - 1;
			while (i <= num5)
			{
				int num6 = i + (num5 - i) / 2;
				dataRow = this.cellset.Formatter.CellTable.Rows[num6];
				num4 = (int)dataRow[this.ordinalColumnIndex];
				if (num4 == index)
				{
					this.previouslyIndexedCellOrdinal = index;
					this.previouslyIndexedCellRowIndex = num6;
					return dataRow;
				}
				if (num4 < index)
				{
					i = num6 + 1;
				}
				else
				{
					num5 = num6 - 1;
				}
			}
			return null;
		}

		public void CopyTo(Cell[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			AdomdUtils.CheckCopyToParameters(array, index, this.Count);
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index + i);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public CellCollection.Enumerator GetEnumerator()
		{
			return new CellCollection.Enumerator(this);
		}

		internal object GetCellValue(DataRow row)
		{
			return this.GetProperty(row, this.valueColumnIndex);
		}

		internal string GetCellFmtValue(DataRow row)
		{
			object property = this.GetProperty(row, this.fmtValueColumnIndex);
			if (property == null)
			{
				return string.Empty;
			}
			return property.ToString();
		}

		private object GetProperty(DataRow row, int index)
		{
			if (index < 0 || row == null)
			{
				return null;
			}
			object obj = row[index];
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
}
