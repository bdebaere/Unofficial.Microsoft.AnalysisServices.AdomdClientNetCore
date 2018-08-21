using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal static class SchemaObjectTypeChecker
	{
		internal static bool IsValidSchemaObjectType(SchemaObjectType type)
		{
			return (type >= SchemaObjectType.ObjectTypeDimension && type <= SchemaObjectType.ObjectTypeMiningServiceParameter) || type == SchemaObjectType.ObjectTypeNamedSet;
		}
	}
}
