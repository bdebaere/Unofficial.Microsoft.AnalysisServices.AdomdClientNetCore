using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class OlapInfoProperty
	{
		private DataColumn propertyColumn;

		public string Name
		{
			get
			{
				return this.propertyColumn.ExtendedProperties["MemberPropertyUnqualifiedName"] as string;
			}
		}

		public string Namespace
		{
			get
			{
				return this.propertyColumn.Namespace;
			}
		}

		public Type Type
		{
			get
			{
				return FormattersHelpers.GetColumnType(this.propertyColumn);
			}
		}

		internal OlapInfoProperty(DataColumn propertyColumn)
		{
			this.propertyColumn = propertyColumn;
		}
	}
}
