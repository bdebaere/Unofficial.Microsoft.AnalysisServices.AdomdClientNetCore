using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class OlapInfoCubeCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private OlapInfoCubeCollection cubes;

			public OlapInfoCube Current
			{
				get
				{
					OlapInfoCube result;
					try
					{
						result = this.cubes[this.currentIndex];
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

			internal Enumerator(OlapInfoCubeCollection cubes)
			{
				this.cubes = cubes;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.cubes.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private DataTable cubesTable;

		private OlapInfoCube[] cubesCach;

		public OlapInfoCube this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (this.cubesCach[index] == null)
				{
					this.cubesCach[index] = new OlapInfoCube(this.cubesTable.Rows[index]);
				}
				return this.cubesCach[index];
			}
		}

		public OlapInfoCube this[string name]
		{
			get
			{
				OlapInfoCube olapInfoCube = this.Find(name);
				if (olapInfoCube == null)
				{
					throw new ArgumentException(SR.ICollection_ItemWithThisNameDoesNotExistInTheCollection, "name");
				}
				return olapInfoCube;
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
				return this.cubesCach.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.cubesCach.Length;
			}
		}

		internal OlapInfoCubeCollection(MDDatasetFormatter formatter)
		{
			this.cubesTable = formatter.CubesInfos;
			int num = 0;
			if (this.cubesTable != null)
			{
				num = this.cubesTable.Rows.Count;
			}
			this.cubesCach = new OlapInfoCube[num];
			for (int i = 0; i < this.cubesCach.Length; i++)
			{
				this.cubesCach[i] = null;
			}
		}

		public OlapInfoCube Find(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			int num = -1;
			for (int i = 0; i < this.Count; i++)
			{
				if ((string)this.cubesTable.Rows[i]["CubeName"] == name)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return null;
			}
			if (this.cubesCach[num] == null)
			{
				this.cubesCach[num] = new OlapInfoCube(this.cubesTable.Rows[num]);
			}
			return this.cubesCach[num];
		}

		public void CopyTo(OlapInfoCube[] array, int index)
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

		public OlapInfoCubeCollection.Enumerator GetEnumerator()
		{
			return new OlapInfoCubeCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
