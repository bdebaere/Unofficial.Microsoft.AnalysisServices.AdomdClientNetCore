using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningAttribute
	{
		internal MiningModel parentModel;

		internal string name;

		internal string shortName;

		internal MiningModelColumn valueColumn;

		internal MiningModelColumn keyColumn;

		internal MiningModelColumn parentColumn;

		internal int attributeID;

		internal MiningFeatureSelection featureSelection;

		internal bool isInput;

		internal bool isPredictable;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string ShortName
		{
			get
			{
				return this.shortName;
			}
		}

		public int AttributeID
		{
			get
			{
				if (this.attributeID == -1 && this.parentModel != null && this.parentModel.Attributes != null && this.parentModel.Attributes.hashAttrIDs != null && this.parentModel.Attributes.hashAttrIDs.ContainsKey(this.Name))
				{
					this.attributeID = (int)this.parentModel.Attributes.hashAttrIDs[this.Name];
				}
				return this.attributeID;
			}
		}

		public MiningModelColumn ValueColumn
		{
			get
			{
				return this.valueColumn;
			}
		}

		public MiningModelColumn KeyColumn
		{
			get
			{
				return this.keyColumn;
			}
		}

		public bool IsInput
		{
			get
			{
				return this.isInput;
			}
		}

		public bool IsPredictable
		{
			get
			{
				return this.isPredictable;
			}
		}

		public MiningFeatureSelection FeatureSelection
		{
			get
			{
				return this.featureSelection;
			}
		}

		internal MiningAttribute(MiningModel parentModel, string attributeDisplayName)
		{
			this.attributeID = -1;
			this.parentModel = parentModel;
			this.name = attributeDisplayName;
			this.shortName = "";
			this.valueColumn = null;
			this.keyColumn = null;
			this.featureSelection = MiningFeatureSelection.NotSelected;
			this.isInput = false;
			this.isPredictable = false;
			this.ParseAttributeName();
			if (this.valueColumn != null)
			{
				this.isInput = this.valueColumn.IsInput;
				this.isPredictable = this.valueColumn.IsPredictable;
				return;
			}
			if (this.parentColumn != null)
			{
				this.isInput = this.parentColumn.IsInput;
				this.isPredictable = this.parentColumn.IsPredictable;
			}
		}

		internal MiningAttribute(MiningModel parentModel)
		{
			this.attributeID = -1;
			this.parentModel = parentModel;
			this.name = "";
			this.shortName = "";
			this.valueColumn = null;
			this.keyColumn = null;
			this.featureSelection = MiningFeatureSelection.NotSelected;
		}

		private void ParseAttributeName()
		{
			if (this.name.Length == 0)
			{
				return;
			}
			int num = this.name.IndexOf('(');
			int num2 = this.name.IndexOf(')');
			int num3 = this.name.IndexOf('.');
			bool flag = false;
			if (num > 0 && num2 > num && (num3 < 0 || num3 == num2 + 1))
			{
				flag = true;
			}
			if (!flag)
			{
				this.valueColumn = this.parentModel.Columns[this.name];
				if (this.valueColumn != null)
				{
					this.shortName = this.name;
					return;
				}
			}
			else
			{
				bool flag2 = num3 < 0;
				string index = this.name.Substring(0, num);
				this.parentColumn = this.parentModel.Columns[index];
				if (this.parentColumn == null)
				{
					return;
				}
				if (flag2)
				{
					this.valueColumn = null;
					MiningModelColumnCollection.Enumerator enumerator = this.parentColumn.Columns.GetEnumerator();
					while (enumerator.MoveNext())
					{
						MiningModelColumn current = enumerator.Current;
						if (current.Content == "KEY")
						{
							this.keyColumn = current;
							this.shortName = this.name;
						}
					}
					return;
				}
				string index2 = this.name.Substring(num3 + 1);
				this.valueColumn = this.parentColumn.Columns[index2];
				this.shortName = index2;
			}
		}
	}
}
