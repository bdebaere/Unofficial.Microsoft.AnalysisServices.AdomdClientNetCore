using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XmlaMessageCollection : ICollection, IEnumerable
	{
		private ArrayList items = new ArrayList();

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public XmlaMessage this[int index]
		{
			get
			{
				return (XmlaMessage)this.items[index];
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.items.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		internal XmlaMessageCollection()
		{
		}

		internal void Add(XmlaMessage item)
		{
			this.items.Add(item);
		}
	}
}
