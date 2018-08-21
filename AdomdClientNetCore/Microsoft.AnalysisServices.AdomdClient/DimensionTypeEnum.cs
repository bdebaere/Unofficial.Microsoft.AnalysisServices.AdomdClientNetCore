using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public enum DimensionTypeEnum
	{
		Unknown,
		Time,
		Measure,
		Other,
		Quantitative = 5,
		Accounts,
		Customers,
		Products,
		Scenario,
		Utility,
		Currency,
		Rates,
		Channel,
		Promotion,
		Organization,
		BillOfMaterials,
		Geography
	}
}
