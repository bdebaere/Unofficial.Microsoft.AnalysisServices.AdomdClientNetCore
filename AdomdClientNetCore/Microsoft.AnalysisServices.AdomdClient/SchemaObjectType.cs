using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public enum SchemaObjectType
	{
		ObjectTypeDimension = 1,
		ObjectTypeHierarchy,
		ObjectTypeLevel,
		ObjectTypeMember,
		ObjectTypeMeasure = 6,
		ObjectTypeKpi,
		ObjectTypeMiningStructure,
		ObjectTypeMiningModel,
		ObjectTypeMiningModelColumn,
		ObjectTypeMiningStructureColumn,
		ObjectTypeMiningContentNode,
		ObjectTypeMiningDistribution,
		ObjectTypeMiningService,
		ObjectTypeMiningServiceParameter,
		ObjectTypeNamedSet = 20
	}
}
