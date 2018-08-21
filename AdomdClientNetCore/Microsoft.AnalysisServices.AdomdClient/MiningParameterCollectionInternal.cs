using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MiningParameterCollectionInternal : ICollection, IEnumerable
	{
		private ArrayList internalObjectCollection;

		public MiningParameter this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (MiningParameter)this.internalObjectCollection[index];
			}
		}

		public int Count
		{
			get
			{
				return this.internalObjectCollection.Count;
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
				return this.internalObjectCollection.SyncRoot;
			}
		}

		internal MiningParameterCollectionInternal(string parameters)
		{
			this.internalObjectCollection = new ArrayList();
			ArrayList arrayList = new ArrayList();
			int i = 0;
			char c = ' ';
			char c2 = ' ';
			while (i < parameters.Length)
			{
				if (parameters[i] == '=' || parameters[i] == ',')
				{
					if (c == parameters[i])
					{
						if (parameters[i] == ',')
						{
							arrayList[arrayList.Count - 1] = i;
						}
					}
					else
					{
						if (arrayList.Count == 0)
						{
							c2 = parameters[i];
						}
						arrayList.Add(i);
						c = parameters[i];
					}
				}
				i++;
			}
			if (c2 == ',')
			{
				arrayList.RemoveAt(0);
			}
			if (arrayList.Count % 2 == 1)
			{
				arrayList.Add(parameters.Length);
			}
			i = 0;
			for (int j = 0; j < arrayList.Count; j += 2)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				int num = Convert.ToInt32(arrayList[j], CultureInfo.InvariantCulture);
				int num2 = Convert.ToInt32(arrayList[j + 1], CultureInfo.InvariantCulture);
				text = parameters.Substring(i, num - i);
				text2 = parameters.Substring(num + 1, num2 - num - 1);
				i = num2 + 1;
				if (!string.IsNullOrEmpty(text))
				{
					this.internalObjectCollection.Add(new MiningParameter(text.Trim(), text2.Trim()));
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return new MiningParametersEnumerator(this);
		}

		public void CopyTo(Array array, int index)
		{
			this.internalObjectCollection.CopyTo(array, index);
		}
	}
}
