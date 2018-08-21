using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal enum ComputerNameFormat
	{
		ComputerNameNetBIOS,
		ComputerNameDnsHostname,
		ComputerNameDnsDomain,
		ComputerNameDnsFullyQualified,
		ComputerNamePhysicalNetBIOS,
		ComputerNamePhysicalDnsHostname,
		ComputerNamePhysicalDnsDomain,
		ComputerNamePhysicalDnsFullyQualified,
		ComputerNameMax
	}
}
