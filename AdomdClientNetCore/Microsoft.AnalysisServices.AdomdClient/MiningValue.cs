using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningValue
	{
		private MiningValueType valueType;

		private object objValue;

		private int index;

		public MiningValueType ValueType
		{
			get
			{
				return this.valueType;
			}
		}

		public object Value
		{
			get
			{
				return this.objValue;
			}
		}

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		internal MiningValue()
		{
			this.valueType = MiningValueType.PreRenderedString;
		}

		internal MiningValue(MiningValueType valueType, int index, object objValue)
		{
			this.valueType = valueType;
			this.index = index;
			this.objValue = objValue;
		}

		public override string ToString()
		{
			if (this.objValue != null)
			{
				return this.objValue.ToString();
			}
			return string.Empty;
		}
	}
}
