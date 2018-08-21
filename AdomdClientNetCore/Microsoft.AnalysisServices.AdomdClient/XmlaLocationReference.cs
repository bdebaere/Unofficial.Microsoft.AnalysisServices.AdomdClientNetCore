using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XmlaLocationReference
	{
		private string dimension;

		private string hierarchy;

		private string attribute;

		private string cube;

		private string measureGroup;

		private string memberName;

		private string role;

		private string tableName;

		private string columnName;

		private string partitionName;

		private string measureName;

		private string roleName;

		public string Dimension
		{
			get
			{
				return this.dimension;
			}
		}

		public string Hierarchy
		{
			get
			{
				return this.hierarchy;
			}
		}

		public string Attribute
		{
			get
			{
				return this.attribute;
			}
		}

		public string Cube
		{
			get
			{
				return this.cube;
			}
		}

		public string MeasureGroup
		{
			get
			{
				return this.measureGroup;
			}
		}

		public string MemberName
		{
			get
			{
				return this.memberName;
			}
		}

		public string Role
		{
			get
			{
				return this.role;
			}
		}

		public string TableName
		{
			get
			{
				return this.tableName;
			}
		}

		public string ColumnName
		{
			get
			{
				return this.columnName;
			}
		}

		public string PartitionName
		{
			get
			{
				return this.partitionName;
			}
		}

		public string MeasureName
		{
			get
			{
				return this.measureName;
			}
		}

		public string RoleName
		{
			get
			{
				return this.roleName;
			}
		}

		internal XmlaLocationReference(string dimension, string hierarchy, string attribute, string cube, string measureGroup, string memberName, string role, string tableName, string columnName, string partitionName, string measureName, string roleName)
		{
			this.dimension = dimension;
			this.hierarchy = hierarchy;
			this.attribute = attribute;
			this.cube = cube;
			this.measureGroup = measureGroup;
			this.memberName = memberName;
			this.role = role;
			this.tableName = tableName;
			this.columnName = columnName;
			this.partitionName = partitionName;
			this.measureName = measureName;
			this.roleName = roleName;
		}
	}
}
