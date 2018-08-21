using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AxisCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private AxisCollection axes;

			public Axis Current
			{
				get
				{
					Axis result;
					try
					{
						result = this.axes[this.currentIndex];
					}
					catch (ArgumentException)
					{
						throw new InvalidOperationException();
					}
					return result;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(AxisCollection axes)
			{
				this.axes = axes;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.axes.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private IDSFAxisCollection internalCollection;

		private AdomdConnection connection;

		private CellSet cellset;

		private string cubeName;

		public Axis this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				IDSFDataSet dataset = this.internalCollection[index];
				return new Axis(this.connection, dataset, this.cubeName, this.cellset, index);
			}
		}

		public Axis this[string index]
		{
			get
			{
				Axis axis = this.Find(index);
				if (null == axis)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return axis;
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
				return this.internalCollection.Count;
			}
		}

		internal AxisCollection(AdomdConnection connection, CellSet cellset, string cubeName)
		{
			this.connection = connection;
			this.cellset = cellset;
			this.cubeName = cubeName;
			this.internalCollection = cellset.Formatter.AxesList;
		}

		public Axis Find(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			int num = 0;
			foreach (IDSFDataSet iDSFDataSet in this.internalCollection)
			{
				if (iDSFDataSet.DataSetName == name)
				{
					return new Axis(this.connection, iDSFDataSet, this.cubeName, this.cellset, num);
				}
				num++;
			}
			return null;
		}

		public void CopyTo(Axis[] array, int index)
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

		public AxisCollection.Enumerator GetEnumerator()
		{
			return new AxisCollection.Enumerator(this);
		}
	}
}
