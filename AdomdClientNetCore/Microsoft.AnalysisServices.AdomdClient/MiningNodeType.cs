using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public enum MiningNodeType
	{
		Model = 1,
		Tree,
		Interior,
		Distribution,
		Cluster,
		Unknown,
		ItemSet,
		AssociationRule,
		PredictableAttribute,
		InputAttribute,
		InputAttributeState,
		Sequence = 13,
		Transition,
		TimeSeries,
		TsTree,
		NNetSubnetwork,
		NNetInputLayer,
		NNetHiddenLayer,
		NNetOutputLayer,
		NNetInputNode,
		NNetHiddenNode,
		NNetOutputNode,
		NNetMarginalNode,
		RegressionTreeRoot,
		NaiveBayesMarginalStatNode,
		ArimaRoot,
		ArimaPeriodicStructure,
		ArimaAutoRegressive,
		ArimaMovingAverage,
		CustomBase = 1000
	}
}
