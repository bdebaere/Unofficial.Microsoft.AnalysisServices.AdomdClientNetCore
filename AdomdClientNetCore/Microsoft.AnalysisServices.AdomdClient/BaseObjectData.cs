using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal struct BaseObjectData
	{
		private AdomdConnection connection;

		private bool isMetadata;

		private object axisData;

		private object metadataData;

		private IAdomdBaseObject parentObject;

		private string cubeName;

		private string catalog;

		private string sessionId;

		public AdomdConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		public string Catalog
		{
			get
			{
				return this.catalog;
			}
		}

		public string SessionID
		{
			get
			{
				return this.sessionId;
			}
		}

		public bool IsMetadata
		{
			get
			{
				return this.isMetadata;
			}
			set
			{
				this.isMetadata = value;
			}
		}

		public object AxisData
		{
			get
			{
				return this.axisData;
			}
		}

		public object MetadataData
		{
			get
			{
				return this.metadataData;
			}
			set
			{
				this.metadataData = value;
			}
		}

		public IAdomdBaseObject ParentObject
		{
			get
			{
				return this.parentObject;
			}
			set
			{
				this.parentObject = value;
			}
		}

		public string CubeName
		{
			get
			{
				return this.cubeName;
			}
		}

		internal BaseObjectData(AdomdConnection connection, bool isMetadata, object axisData, object metadataData, IAdomdBaseObject parentObject, string cubeName, string catalog, string sessionId)
		{
			this.connection = connection;
			this.isMetadata = isMetadata;
			this.axisData = axisData;
			this.metadataData = metadataData;
			this.parentObject = parentObject;
			this.cubeName = cubeName;
			this.catalog = catalog;
			this.sessionId = sessionId;
		}
	}
}
