using System;
using System.Collections.Generic;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class SchemaRowsetCacheData
	{
		private string requestType;

		private string relationColumnName;

		private string uniqueNameColumnName;

		private string[] primaryKeyColumns;

		private InternalObjectType objectType;

		private KeyValuePair<string, string>[] additionalStaticRestrictions;

		internal string RequestType
		{
			get
			{
				return this.requestType;
			}
		}

		internal string UniqueNameColumnName
		{
			get
			{
				return this.uniqueNameColumnName;
			}
		}

		internal string RelationColumnName
		{
			get
			{
				return this.relationColumnName;
			}
		}

		internal string[] PrimaryKeyColumns
		{
			get
			{
				return this.primaryKeyColumns;
			}
		}

		internal InternalObjectType ObjectType
		{
			get
			{
				return this.objectType;
			}
		}

		internal KeyValuePair<string, string>[] AdditionalStaticRestrictions
		{
			get
			{
				return this.additionalStaticRestrictions;
			}
		}

		internal bool HasAdditionalRestrictions
		{
			get
			{
				return this.additionalStaticRestrictions != null;
			}
		}

		internal SchemaRowsetCacheData(InternalObjectType objectType, string requestType, string relationColumnName, string[] primaryKeyColumns, string uniqueNameColumnName) : this(objectType, requestType, relationColumnName, primaryKeyColumns, uniqueNameColumnName, null)
		{
		}

		internal SchemaRowsetCacheData(InternalObjectType objectType, string requestType, string relationColumnName, string[] primaryKeyColumns, string uniqueNameColumnName, KeyValuePair<string, string>[] additionalStaticRestrictions)
		{
			this.objectType = objectType;
			this.requestType = requestType;
			this.relationColumnName = relationColumnName;
			this.primaryKeyColumns = primaryKeyColumns;
			this.uniqueNameColumnName = uniqueNameColumnName;
			this.additionalStaticRestrictions = additionalStaticRestrictions;
		}
	}
}
