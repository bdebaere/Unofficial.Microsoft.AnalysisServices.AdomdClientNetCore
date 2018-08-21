using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal static class MemberFilterTypeChecker
	{
		internal static bool IsValidMemberFilterType(MemberFilterType type)
		{
			return type >= MemberFilterType.Equals && type <= MemberFilterType.Contains;
		}
	}
}
