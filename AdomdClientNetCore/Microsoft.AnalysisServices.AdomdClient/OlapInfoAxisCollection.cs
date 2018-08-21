using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class OlapInfoAxisCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private OlapInfoAxisCollection axes;

			public OlapInfoAxis Current
			{
				get
				{
					OlapInfoAxis result;
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

			internal Enumerator(OlapInfoAxisCollection axes)
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

		private MDDatasetFormatter formatter;

		private OlapInfoAxis[] axesCach;

		private Hashtable namesHash;

		public OlapInfoAxis this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (this.axesCach[index] == null)
				{
					this.axesCach[index] = new OlapInfoAxis(this.formatter.AxesList[index]);
				}
				return this.axesCach[index];
			}
		}

		public OlapInfoAxis this[string name]
		{
			get
			{
				OlapInfoAxis olapInfoAxis = this.Find(name);
				if (olapInfoAxis == null)
				{
					throw new ArgumentException(SR.ICollection_ItemWithThisNameDoesNotExistInTheCollection, "name");
				}
				return olapInfoAxis;
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
				return this.axesCach.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.axesCach.Length;
			}
		}

		internal OlapInfoAxisCollection(MDDatasetFormatter formatter)
		{
			this.formatter = formatter;
			this.axesCach = new OlapInfoAxis[formatter.AxesList.Count];
			for (int i = 0; i < this.axesCach.Length; i++)
			{
				this.axesCach[i] = null;
			}
		}

		public OlapInfoAxis Find(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this.namesHash == null)
			{
				this.namesHash = new Hashtable(this.Count);
				for (int i = 0; i < this.formatter.AxesList.Count; i++)
				{
					string dataSetName = this.formatter.AxesList[i].DataSetName;
					if (this.namesHash[dataSetName] == null)
					{
						this.namesHash[dataSetName] = i;
					}
				}
			}
			if (!this.namesHash.ContainsKey(name))
			{
				return null;
			}
			int index = (int)this.namesHash[name];
			return this[index];
		}

		public void CopyTo(OlapInfoAxis[] array, int index)
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

		public OlapInfoAxisCollection.Enumerator GetEnumerator()
		{
			return new OlapInfoAxisCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
