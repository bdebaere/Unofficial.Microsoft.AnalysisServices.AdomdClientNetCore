using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MiningValueCollectionInternal : ICollection, IEnumerable
	{
		private ArrayList internalObjectCollection;

		public MiningValue this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (MiningValue)this.internalObjectCollection[index];
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

		internal MiningValueCollectionInternal()
		{
			this.internalObjectCollection = new ArrayList();
		}

		internal MiningValueCollectionInternal(MiningModelColumn column)
		{
			this.internalObjectCollection = new ArrayList();
			if (column.IsTable)
			{
				return;
			}
			AdomdCommand adomdCommand = new AdomdCommand();
			adomdCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "SELECT DISTINCT {0} FROM [{1}]", new object[]
			{
				column.FullyQualifiedName,
				column.ParentMiningModel.Name
			});
			adomdCommand.Connection = column.ParentMiningModel.ParentConnection;
			AdomdDataReader adomdDataReader = adomdCommand.ExecuteReader();
			int num = -1;
			while (adomdDataReader.Read())
			{
				num++;
				object objValue = adomdDataReader[0];
				string content = column.Content;
				MiningValueType valueType = MiningValueType.Missing;
				if (num == 0 && content.IndexOf("key", StringComparison.OrdinalIgnoreCase) < 0)
				{
					valueType = MiningValueType.Missing;
				}
				else if (string.Compare(content, "discrete", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(content, "key", StringComparison.OrdinalIgnoreCase) == 0)
				{
					valueType = MiningValueType.Discrete;
				}
				else if (content.IndexOf("discretized", StringComparison.OrdinalIgnoreCase) == 0)
				{
					valueType = MiningValueType.Discretized;
				}
				else if (string.Compare(content, "continuous", StringComparison.OrdinalIgnoreCase) == 0)
				{
					valueType = MiningValueType.Continuous;
				}
				MiningValue newValue = new MiningValue(valueType, num, objValue);
				this.Add(newValue);
			}
			adomdDataReader.Close();
			adomdDataReader.Dispose();
			adomdCommand.Dispose();
		}

		internal void Add(MiningValue newValue)
		{
			this.internalObjectCollection.Add(newValue);
		}

		public IEnumerator GetEnumerator()
		{
			return new MiningValuesEnumerator(this);
		}

		public void CopyTo(Array array, int index)
		{
			this.internalObjectCollection.CopyTo(array, index);
		}
	}
}
