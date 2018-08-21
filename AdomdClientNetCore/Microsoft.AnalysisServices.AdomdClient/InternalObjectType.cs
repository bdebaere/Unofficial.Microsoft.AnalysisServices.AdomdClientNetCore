using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal enum InternalObjectType
	{
		InternalTypeDimension = 1,
		InternalTypeHierarchy,
		InternalTypeLevel,
		InternalTypeMember,
		InternalTypeMeasure = 6,
		InternalTypeKpi,
		InternalTypeMiningModel = 9,
		InternalTypeMiningStructure = 8,
		InternalTypeMiningModelColumn = 10,
		InternalTypeMiningStructureColumn,
		InternalTypeMiningContentNode,
		InternalTypeMiningDistribution,
		InternalTypeMiningService,
		InternalTypeMiningServiceParameter,
		InternalTypeNamedSet = 20,
		InternalTypeLevelProperty,
		InternalTypeMemberProperty,
		InternalTypeCube,
		InternalMaxType = 22
	}
}
