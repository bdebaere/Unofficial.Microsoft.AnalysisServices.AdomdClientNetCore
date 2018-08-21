using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public enum MiningValueType
	{
		PreRenderedString = -1,
		Missing = 1,
		Existing,
		Continuous,
		Discrete,
		Discretized,
		Boolean,
		Coefficient,
		ScoreGain,
		RegressorStatistics,
		NodeUniqueName,
		Intercept,
		Periodicity,
		AutoRegressiveOrder,
		MovingAverageOrder,
		DifferenceOrder,
		Other
	}
}
