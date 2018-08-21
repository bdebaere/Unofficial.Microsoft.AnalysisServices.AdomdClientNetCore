using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces
{
	internal interface ILinkFile
	{
		string Database
		{
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		string Description
		{
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		bool IsDelegationAllowed
		{
			[return: MarshalAs(UnmanagedType.VariantBool)]
			get;
		}

		bool IsInFarm
		{
			[return: MarshalAs(UnmanagedType.VariantBool)]
			get;
		}

		bool IsFileMalformed
		{
			[return: MarshalAs(UnmanagedType.VariantBool)]
			get;
		}

		string Server
		{
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}
	}
}
