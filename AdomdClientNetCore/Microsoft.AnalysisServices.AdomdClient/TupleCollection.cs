using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class TupleCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private TupleCollection tuples;

			public Tuple Current
			{
				get
				{
					Tuple result;
					try
					{
						result = this.tuples[this.currentIndex];
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

			internal Enumerator(TupleCollection tuples)
			{
				this.tuples = tuples;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.tuples.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private DataRowCollection internalCollection;

		private Set axis;

		private string cubeName;

		private AdomdConnection connection;

		public Tuple this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return new Tuple(this.connection, this.axis, index, this.cubeName);
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

		internal TupleCollection(AdomdConnection connection, Set axis, string cubeName)
		{
			this.connection = connection;
			this.axis = axis;
			this.cubeName = cubeName;
			if (axis.AxisDataset.Count > 0)
			{
				this.internalCollection = axis.AxisDataset[0].Rows;
				return;
			}
			this.internalCollection = null;
		}

		public void CopyTo(Tuple[] array, int index)
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

		public TupleCollection.Enumerator GetEnumerator()
		{
			return new TupleCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
