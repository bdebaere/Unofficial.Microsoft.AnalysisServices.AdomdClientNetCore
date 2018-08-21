using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Serializable]
	public sealed class AdomdErrorCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private IEnumerator enumer;

			public AdomdError Current
			{
				get
				{
					return (AdomdError)this.enumer.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(AdomdErrorCollection errors)
			{
				this.enumer = errors.errors.GetEnumerator();
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

		private ArrayList errors = new ArrayList();

		public AdomdError this[int index]
		{
			get
			{
				return (AdomdError)this.errors[index];
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
				return this.errors.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.errors.Count;
			}
		}

		internal AdomdErrorCollection()
		{
		}

		public void CopyTo(AdomdError[] array, int index)
		{
			this.errors.CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.errors.CopyTo(array, index);
		}

		public AdomdErrorCollection.Enumerator GetEnumerator()
		{
			return new AdomdErrorCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal void Add(AdomdError error)
		{
			this.errors.Add(error);
		}
	}
}
