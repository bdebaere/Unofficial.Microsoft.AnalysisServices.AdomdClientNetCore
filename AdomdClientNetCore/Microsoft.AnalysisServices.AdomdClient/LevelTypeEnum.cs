using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public enum LevelTypeEnum
	{
		Regular,
		All,
		Calculated,
		Time = 4,
		Reserved1 = 8,
		TimeYears = 20,
		TimeHalfYears = 36,
		TimeQuarters = 68,
		TimeMonths = 132,
		TimeWeeks = 260,
		TimeDays = 516,
		TimeHours = 772,
		TimeMinutes = 1028,
		TimeSeconds = 2052,
		TimeUndefined = 4100,
		GeoContinent = 8193,
		GeoRegion,
		GeoCountry,
		GeoStateOrProvince,
		GeoCounty,
		GeoCity,
		GeoPostalCode,
		GeoPoint,
		OrgUnit = 4113,
		BomResource,
		Quantitative,
		Account,
		Customer = 4129,
		CustomerGroup,
		CustomerHousehold,
		Product = 4145,
		ProductGroup,
		Scenario = 4117,
		Utility,
		Person = 4161,
		Company,
		CurrencySource = 4177,
		CurrencyDestination,
		Channel = 4193,
		Representative,
		Promotion = 4209
	}
}
