using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal static class LocalExcelVar
	{
		internal const string MSMGDSRV = "msmgdsrv.dll";

		internal const bool IsLocalExcel = false;

		internal static readonly string MSMDLOCAL_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Microsoft Analysis Services\\AS OLEDB\\130\\msmdlocal.dll";

		internal static readonly string MSMDLOCAL_FALLBACK_PATH = LocalExcelVar.MSMDLOCAL_PATH;
	}
}
