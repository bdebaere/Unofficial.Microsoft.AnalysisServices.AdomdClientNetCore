using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class LevelPropertyCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private LevelPropsEnumerator enumer;

			public LevelProperty Current
			{
				get
				{
					return this.enumer.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(LevelPropertyCollection props)
			{
				this.enumer = new LevelPropsEnumerator(props.levelPropertyCollectionInternal);
			}

			public bool MoveNext()
			{
				return this.enumer.MoveNext();
			}

			public void Reset()
			{
				this.enumer.Reset();
			}
		}

		private LevelPropertyCollectionInternal levelPropertyCollectionInternal;

		public LevelProperty this[int index]
		{
			get
			{
				return this.levelPropertyCollectionInternal[index];
			}
		}

		public LevelProperty this[string index]
		{
			get
			{
				return this.levelPropertyCollectionInternal[index];
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
				return this.levelPropertyCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.levelPropertyCollectionInternal.Count;
			}
		}

		internal LevelPropertyCollection(AdomdConnection connection, Level parentLevel)
		{
			this.levelPropertyCollectionInternal = new LevelPropertyCollectionInternal(connection, parentLevel);
		}

		public LevelProperty Find(string index)
		{
			return this.levelPropertyCollectionInternal.Find(index);
		}

		public void CopyTo(LevelProperty[] array, int index)
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

		public LevelPropertyCollection.Enumerator GetEnumerator()
		{
			return new LevelPropertyCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
