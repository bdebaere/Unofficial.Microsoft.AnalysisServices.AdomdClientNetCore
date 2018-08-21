using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class PositionCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private PositionCollection positions;

			public Position Current
			{
				get
				{
					Position result;
					try
					{
						result = this.positions[this.currentIndex];
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

			internal Enumerator(PositionCollection positions)
			{
				this.positions = positions;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.positions.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private DataRowCollection internalCollection;

		private Set set;

		private string cubeName;

		private AdomdConnection connection;

		public Position this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				Tuple tuple = new Tuple(this.connection, this.set, index, this.cubeName);
				return new Position(this.connection, tuple, this.cubeName);
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
				if (this.internalCollection != null)
				{
					return this.internalCollection.Count;
				}
				return 0;
			}
		}

		internal PositionCollection(AdomdConnection connection, Set set, string cubeName)
		{
			this.connection = connection;
			this.set = set;
			this.cubeName = cubeName;
			if (set.AxisDataset.Count > 0)
			{
				this.internalCollection = set.AxisDataset[0].Rows;
				return;
			}
			this.internalCollection = null;
		}

		public void CopyTo(Position[] array, int index)
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

		public PositionCollection.Enumerator GetEnumerator()
		{
			return new PositionCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
