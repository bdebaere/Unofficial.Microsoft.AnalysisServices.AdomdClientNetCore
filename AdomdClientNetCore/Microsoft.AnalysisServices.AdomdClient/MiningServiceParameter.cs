using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningServiceParameter : IAdomdBaseObject, IMetadataObject
	{
		private BaseObjectData baseData;

		private PropertyCollection propertiesCollection;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string serviceNameColumn = "SERVICE_NAME";

		internal static string serviceParameterColumn = "PARAMETER_NAME";

		internal static string descriptionColumn = "DESCRIPTION";

		private static string isRequiredColumn = "IS_REQUIRED";

		private static string defaultValueColumn = "DEFAULT_VALUE";

		private static string valueEnumerationColumn = "VALUE_ENUMERATION";

		private static string parameterTypeColumn = "PARAMETER_TYPE";

		public string ServiceName
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceParameterRow, MiningServiceParameter.serviceNameColumn).ToString();
			}
		}

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceParameterRow, MiningServiceParameter.serviceParameterColumn).ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceParameterRow, MiningServiceParameter.descriptionColumn).ToString();
			}
		}

		public bool IsRequired
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningServiceParameterRow, MiningServiceParameter.isRequiredColumn), CultureInfo.InvariantCulture);
			}
		}

		public string DefaultValue
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceParameterRow, MiningServiceParameter.defaultValueColumn).ToString();
			}
		}

		public string ValueEnumeration
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceParameterRow, MiningServiceParameter.valueEnumerationColumn).ToString();
			}
		}

		public string ParameterType
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceParameterRow, MiningServiceParameter.parameterTypeColumn).ToString();
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertiesCollection == null)
				{
					this.propertiesCollection = new PropertyCollection(this.MiningServiceParameterRow, this);
				}
				return this.propertiesCollection;
			}
		}

		AdomdConnection IAdomdBaseObject.Connection
		{
			get
			{
				return this.baseData.Connection;
			}
		}

		bool IAdomdBaseObject.IsMetadata
		{
			get
			{
				return this.baseData.IsMetadata;
			}
			set
			{
				this.baseData.IsMetadata = value;
			}
		}

		object IAdomdBaseObject.MetadataData
		{
			get
			{
				return this.baseData.MetadataData;
			}
			set
			{
				this.baseData.MetadataData = value;
			}
		}

		IAdomdBaseObject IAdomdBaseObject.ParentObject
		{
			get
			{
				return this.baseData.ParentObject;
			}
			set
			{
				this.baseData.ParentObject = value;
			}
		}

		string IAdomdBaseObject.CubeName
		{
			get
			{
				return this.baseData.ParentObject.CubeName;
			}
		}

		SchemaObjectType IAdomdBaseObject.SchemaObjectType
		{
			get
			{
				return SchemaObjectType.ObjectTypeMiningServiceParameter;
			}
		}

		string IAdomdBaseObject.InternalUniqueName
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceParameterRow, MiningServiceParameter.serviceParameterColumn).ToString();
			}
		}

		AdomdConnection IMetadataObject.Connection
		{
			get
			{
				return this.baseData.Connection;
			}
		}

		string IMetadataObject.Catalog
		{
			get
			{
				return this.baseData.Catalog;
			}
		}

		string IMetadataObject.SessionId
		{
			get
			{
				return this.baseData.SessionID;
			}
		}

		string IMetadataObject.CubeName
		{
			get
			{
				return ((IAdomdBaseObject)this).CubeName;
			}
		}

		string IMetadataObject.UniqueName
		{
			get
			{
				return this.Name;
			}
		}

		Type IMetadataObject.Type
		{
			get
			{
				return typeof(MiningServiceParameter);
			}
		}

		internal DataRow MiningServiceParameterRow
		{
			get
			{
				return (DataRow)this.baseData.MetadataData;
			}
		}

		internal MiningServiceParameter(AdomdConnection connection, DataRow miningServiceParameterRow, IAdomdBaseObject parentObject, string catalog, string sessionId)
		{
			this.baseData = new BaseObjectData(connection, true, null, miningServiceParameterRow, parentObject, null, catalog, sessionId);
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
			return AdomdUtils.Equals(this, obj as IMetadataObject);
		}

		public static bool operator ==(MiningServiceParameter o1, MiningServiceParameter o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(MiningServiceParameter o1, MiningServiceParameter o2)
		{
			return !(o1 == o2);
		}
	}
}
