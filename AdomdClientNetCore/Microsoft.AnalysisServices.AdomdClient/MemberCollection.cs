using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MemberCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private MemberCollection members;

			public Member Current
			{
				get
				{
					Member result;
					try
					{
						result = this.members[this.currentIndex];
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
					return this.members[this.currentIndex];
				}
			}

			internal Enumerator(MemberCollection members)
			{
				this.members = members;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.members.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private IMemberCollectionInternal memberCollectionInternal;

		public Member this[int index]
		{
			get
			{
				return this.memberCollectionInternal[index];
			}
		}

		public Member this[string index]
		{
			get
			{
				Member member = this.Find(index);
				if (null == member)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return member;
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
				return this.memberCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.memberCollectionInternal.Count;
			}
		}

		internal MemberCollection(AdomdConnection connection, Tuple tuple, string cubeName)
		{
			this.memberCollectionInternal = new AxisTupleMemberCollection(connection, tuple, cubeName);
		}

		internal MemberCollection(AdomdConnection connection, DataTable memberHierarchyDataTable, string cubeName, Level parentLevel, Member parentMember)
		{
			if (memberHierarchyDataTable == null)
			{
				this.memberCollectionInternal = new EmptyMembersCollection();
				return;
			}
			this.memberCollectionInternal = new AxisHierarchyMemberCollection(connection, memberHierarchyDataTable, cubeName, parentLevel, parentMember);
		}

		public Member Find(string index)
		{
			return this.memberCollectionInternal.Find(index);
		}

		public void CopyTo(Member[] array, int index)
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

		public MemberCollection.Enumerator GetEnumerator()
		{
			return new MemberCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
