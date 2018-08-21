using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningDistribution : ISubordinateObject
	{
		private DataRow miningDistributionRow;

		private MiningContentNode parentNode;

		private AdomdConnection connection;

		private PropertyCollection propertiesCollection;

		private MiningAttribute attribute;

		internal int ordinal;

		private MiningValue distValue;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string miningDistributionAttrNameColumn = "ATTRIBUTE_NAME";

		internal static string miningDistributionAttrValueColumn = "ATTRIBUTE_VALUE";

		internal static string miningDistributionSupportColumn = "SUPPORT";

		internal static string miningDistributionProbabilityColumn = "PROBABILITY";

		internal static string miningDistributionVarianceColumn = "VARIANCE";

		internal static string miningDistributionValueTypeColumn = "VALUETYPE";

		public MiningModel ParentMiningModel
		{
			get
			{
				return this.parentNode.ParentMiningModel;
			}
		}

		public MiningContentNode ParentNode
		{
			get
			{
				return this.parentNode;
			}
		}

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.miningDistributionRow, MiningDistribution.miningDistributionAttrNameColumn).ToString();
			}
		}

		public MiningAttribute Attribute
		{
			get
			{
				if (this.attribute == null)
				{
					string attributeDisplayName = AdomdUtils.GetProperty(this.miningDistributionRow, MiningDistribution.miningDistributionAttrNameColumn).ToString();
					this.attribute = new MiningAttribute(this.ParentMiningModel, attributeDisplayName);
				}
				return this.attribute;
			}
		}

		public MiningValue Value
		{
			get
			{
				if (this.distValue == null)
				{
					object property = AdomdUtils.GetProperty(this.miningDistributionRow, MiningDistribution.miningDistributionAttrValueColumn);
					int index = -1;
					if (this.ValueType == MiningValueType.Missing)
					{
						index = 0;
					}
					else if (this.ValueType == MiningValueType.Continuous)
					{
						index = 1;
					}
					this.distValue = new MiningValue(this.ValueType, index, property);
				}
				return this.distValue;
			}
		}

		public double Probability
		{
			get
			{
				object property = AdomdUtils.GetProperty(this.miningDistributionRow, MiningDistribution.miningDistributionProbabilityColumn);
				return Convert.ToDouble(property, CultureInfo.InvariantCulture);
			}
		}

		public double Variance
		{
			get
			{
				object property = AdomdUtils.GetProperty(this.miningDistributionRow, MiningDistribution.miningDistributionVarianceColumn);
				return Convert.ToDouble(property, CultureInfo.InvariantCulture);
			}
		}

		public double Support
		{
			get
			{
				object property = AdomdUtils.GetProperty(this.miningDistributionRow, MiningDistribution.miningDistributionSupportColumn);
				return Convert.ToDouble(property, CultureInfo.InvariantCulture);
			}
		}

		public MiningValueType ValueType
		{
			get
			{
				return (MiningValueType)Convert.ToInt32(AdomdUtils.GetProperty(this.miningDistributionRow, MiningDistribution.miningDistributionValueTypeColumn).ToString(), CultureInfo.InvariantCulture);
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertiesCollection == null)
				{
					this.propertiesCollection = new PropertyCollection(this.miningDistributionRow, this);
				}
				return this.propertiesCollection;
			}
		}

		object ISubordinateObject.Parent
		{
			get
			{
				return this.parentNode;
			}
		}

		int ISubordinateObject.Ordinal
		{
			get
			{
				return this.ordinal;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(MiningDistribution);
			}
		}

		internal MiningDistribution(AdomdConnection connection, DataRow miningDistributionRow, MiningContentNode parentNode)
		{
			this.connection = connection;
			this.miningDistributionRow = miningDistributionRow;
			this.parentNode = parentNode;
			this.propertiesCollection = null;
			this.distValue = null;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override int GetHashCode()
		{
			if (!this.hashCodeCalculated)
			{
				this.hashCode = AdomdUtils.GetHashCode(this);
				this.hashCodeCalculated = true;
			}
			return this.hashCode;
		}

		public override bool Equals(object obj)
		{
			return AdomdUtils.Equals(this, obj as ISubordinateObject);
		}

		public static bool operator ==(MiningDistribution o1, MiningDistribution o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(MiningDistribution o1, MiningDistribution o2)
		{
			return !(o1 == o2);
		}
	}
}
