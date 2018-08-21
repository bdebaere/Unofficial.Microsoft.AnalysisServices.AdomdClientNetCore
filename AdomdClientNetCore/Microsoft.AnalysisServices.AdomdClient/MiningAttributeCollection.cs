using System;
using System.Collections;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningAttributeCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private MiningAttributeCollection attributes;

			public MiningAttribute Current
			{
				get
				{
					MiningAttribute result;
					try
					{
						result = this.attributes[this.currentIndex];
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
					return this.attributes[this.currentIndex];
				}
			}

			internal Enumerator(MiningAttributeCollection miningModelAttributes)
			{
				this.currentIndex = -1;
				this.attributes = miningModelAttributes;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.attributes.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private ArrayList arAttributesInternal;

		internal Hashtable hashAttrIDs;

		internal static string attribQueryStmt = "CALL System.GetModelAttributes('{0}')";

		internal static int attIdIndex = 0;

		internal static int nameIndex = 1;

		internal static int shortNameIndex = 2;

		internal static int isInputIndex = 3;

		internal static int isPredictableIndex = 4;

		internal static int featureSelectionIndex = 5;

		internal static int keyColumnIndex = 6;

		internal static int valueColumnIndex = 7;

		public MiningAttribute this[int index]
		{
			get
			{
				return (MiningAttribute)this.arAttributesInternal[index];
			}
		}

		public MiningAttribute this[string index]
		{
			get
			{
				return this.Find(index);
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
				return this.arAttributesInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.arAttributesInternal.Count;
			}
		}

		internal MiningAttributeCollection(MiningModel parentModel)
		{
			this.arAttributesInternal = new ArrayList();
			this.PopulateCollection(parentModel);
		}

		internal MiningAttributeCollection(MiningModel parentModel, MiningFeatureSelection filter)
		{
			this.arAttributesInternal = new ArrayList();
			this.FilterCollection(parentModel.Attributes, filter);
		}

		private MiningModelColumn ModelColumnFromName(string colName, MiningModel parentModel)
		{
			MiningModelColumn result = null;
			if (colName == null || colName.Length == 0)
			{
				return result;
			}
			int num = colName.IndexOf(".", StringComparison.OrdinalIgnoreCase);
			string text = string.Empty;
			string text2 = string.Empty;
			if (num < 0)
			{
				text = colName;
				if (text.IndexOf("[", StringComparison.OrdinalIgnoreCase) == 0)
				{
					text = text.Substring(1, text.Length - 2);
				}
				result = parentModel.Columns[text];
			}
			else
			{
				text = colName.Substring(0, num);
				text2 = colName.Substring(num + 1, colName.Length - num - 1);
				if (text.IndexOf("[", StringComparison.OrdinalIgnoreCase) == 0)
				{
					text = text.Substring(1, text.Length - 2);
				}
				MiningModelColumn miningModelColumn = parentModel.Columns[text];
				if (text2.IndexOf("[", StringComparison.OrdinalIgnoreCase) == 0)
				{
					text2 = text2.Substring(1, text2.Length - 2);
				}
				result = miningModelColumn.Columns[text2];
			}
			return result;
		}

		private void PopulateCollection(MiningModel parentModel)
		{
			this.hashAttrIDs = new Hashtable();
			AdomdCommand adomdCommand = new AdomdCommand();
			adomdCommand.Connection = parentModel.ParentConnection;
			int num = 0;
			MiningModelColumnCollection.Enumerator enumerator = parentModel.Columns.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MiningModelColumn current = enumerator.Current;
				if (current.IsTable)
				{
					MiningModelColumnCollection.Enumerator enumerator2 = current.Columns.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						MiningModelColumn arg_54_0 = enumerator2.Current;
						num++;
					}
				}
			}
			adomdCommand.CommandText = string.Format(CultureInfo.InvariantCulture, MiningAttributeCollection.attribQueryStmt, new object[]
			{
				parentModel.Name
			});
			AdomdDataReader adomdDataReader = adomdCommand.ExecuteReader(CommandBehavior.SequentialAccess);
			while (adomdDataReader.Read())
			{
				int @int = adomdDataReader.GetInt32(MiningAttributeCollection.attIdIndex);
				string @string = adomdDataReader.GetString(MiningAttributeCollection.nameIndex);
				string string2 = adomdDataReader.GetString(MiningAttributeCollection.shortNameIndex);
				bool boolean = adomdDataReader.GetBoolean(MiningAttributeCollection.isInputIndex);
				bool boolean2 = adomdDataReader.GetBoolean(MiningAttributeCollection.isPredictableIndex);
				int int2 = adomdDataReader.GetInt32(MiningAttributeCollection.featureSelectionIndex);
				string string3 = adomdDataReader.GetString(MiningAttributeCollection.keyColumnIndex);
				string string4 = adomdDataReader.GetString(MiningAttributeCollection.valueColumnIndex);
				MiningAttribute miningAttribute = new MiningAttribute(parentModel);
				miningAttribute.attributeID = @int;
				miningAttribute.name = @string;
				miningAttribute.shortName = string2;
				miningAttribute.isInput = boolean;
				miningAttribute.isPredictable = boolean2;
				miningAttribute.featureSelection = (MiningFeatureSelection)int2;
				miningAttribute.keyColumn = this.ModelColumnFromName(string3, parentModel);
				miningAttribute.valueColumn = this.ModelColumnFromName(string4, parentModel);
				this.hashAttrIDs.Add(miningAttribute.name, miningAttribute.attributeID);
				this.arAttributesInternal.Add(miningAttribute);
			}
			adomdDataReader.Close();
			adomdCommand.Dispose();
		}

		private void FilterCollection(MiningAttributeCollection parentCollection, MiningFeatureSelection filter)
		{
			MiningAttributeCollection.Enumerator enumerator = parentCollection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MiningAttribute current = enumerator.Current;
				bool flag = false;
				switch (filter)
				{
				case MiningFeatureSelection.All:
					flag = true;
					break;
				case MiningFeatureSelection.NotSelected:
					flag = (current.FeatureSelection == MiningFeatureSelection.NotSelected);
					break;
				case MiningFeatureSelection.Selected:
					flag = (current.FeatureSelection == MiningFeatureSelection.Input || current.FeatureSelection == MiningFeatureSelection.Output || current.FeatureSelection == MiningFeatureSelection.InputAndOutput);
					break;
				case (MiningFeatureSelection)3:
				case (MiningFeatureSelection)5:
				case (MiningFeatureSelection)6:
				case (MiningFeatureSelection)7:
					break;
				case MiningFeatureSelection.Input:
					flag = (current.FeatureSelection == MiningFeatureSelection.Input);
					break;
				case MiningFeatureSelection.Output:
					flag = (current.FeatureSelection == MiningFeatureSelection.Output);
					break;
				default:
					if (filter == MiningFeatureSelection.InputAndOutput)
					{
						flag = (current.FeatureSelection == MiningFeatureSelection.InputAndOutput);
					}
					break;
				}
				if (flag)
				{
					this.arAttributesInternal.Add(current);
				}
			}
		}

		public MiningAttribute Find(string index)
		{
			return null;
		}

		public void CopyTo(MiningAttribute[] array, int index)
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

		public MiningAttributeCollection.Enumerator GetEnumerator()
		{
			return new MiningAttributeCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
