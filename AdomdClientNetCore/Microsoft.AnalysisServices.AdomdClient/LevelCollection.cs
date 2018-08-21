using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class LevelCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private LevelsEnumerator enumer;

			public Level Current
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

			internal Enumerator(LevelCollection levels)
			{
				this.enumer = new LevelsEnumerator(levels.CollectionInternal);
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

		private LevelCollectionInternal levelCollectionInternal;

		public Level this[int index]
		{
			get
			{
				return this.levelCollectionInternal[index];
			}
		}

		public Level this[string index]
		{
			get
			{
				return this.levelCollectionInternal[index];
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
				return this.levelCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.levelCollectionInternal.Count;
			}
		}

		internal LevelCollectionInternal CollectionInternal
		{
			get
			{
				return this.levelCollectionInternal;
			}
		}

		internal LevelCollection(AdomdConnection connection, Hierarchy parentHierarchy)
		{
			this.levelCollectionInternal = new LevelCollectionInternal(connection, parentHierarchy);
		}

		public Level Find(string index)
		{
			return this.levelCollectionInternal.Find(index);
		}

		public void CopyTo(Level[] array, int index)
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

		public LevelCollection.Enumerator GetEnumerator()
		{
			return new LevelCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
