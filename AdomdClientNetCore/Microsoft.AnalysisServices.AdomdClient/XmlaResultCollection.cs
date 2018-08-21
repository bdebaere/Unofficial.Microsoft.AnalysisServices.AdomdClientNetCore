using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XmlaResultCollection : ICollection, IEnumerable
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

		public XmlaResult this[int index]
		{
			get
			{
				return (XmlaResult)this.items[index];
			}
		}

		public bool ContainsErrors
		{
			get
			{
				int i = 0;
				int count = this.items.Count;
				while (i < count)
				{
					if (((XmlaResult)this.items[i]).ContainsErrors)
					{
						return true;
					}
					i++;
				}
				return false;
			}
		}

		internal bool ContainsInvalidSessionError
		{
			get
			{
				int i = 0;
				int count = this.items.Count;
				while (i < count)
				{
					if (((XmlaResult)this.items[i]).ContainsInvalidSessionError)
					{
						return true;
					}
					i++;
				}
				return false;
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

		internal XmlaResultCollection()
		{
		}

		internal void Add(XmlaResult item)
		{
			this.items.Add(item);
		}

		internal static Exception ExceptionOnError(XmlaResultCollection col)
		{
			XmlaException ex = new XmlaException(col);
			if (col.ContainsInvalidSessionError)
			{
				return new XmlaStreamException(ex);
			}
			return ex;
		}

		internal static Exception ExceptionOnError(XmlaResult res)
		{
			XmlaException ex = new XmlaException(res);
			if (res.ContainsInvalidSessionError)
			{
				return new XmlaStreamException(ex);
			}
			return ex;
		}
	}
}
