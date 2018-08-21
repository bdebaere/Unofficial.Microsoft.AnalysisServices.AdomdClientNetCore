using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient.MsoID
{
	internal sealed class MsoIDAuthenticationProvider : CriticalFinalizerObject
	{
		private enum ExtendedNameFormat
		{
			NameUnknown,
			NameFullyQualifiedDN,
			NameSamCompatible,
			NameDisplay,
			NameUniqueId = 6,
			NameCanonical,
			NameUserPrincipal,
			NameCanonicalEx,
			NameServicePrincipal,
			NameDnsDomain = 12
		}

		private const string AzureAnalysisServicesTargetName = "analysis.windows.net";

		private const string AzureAnalysisServicesPolicy = "MBI_SSL";

		private static readonly Guid XmlaClientApplicationID = new Guid("CB0AA5A2-A483-499d-9521-C3CEB6E5AB23");

		private static readonly MsoIDAuthenticationProvider instance = new MsoIDAuthenticationProvider();

		private readonly object idcrlLock = new object();

		private MsoIDCRL idcrl;

		public static MsoIDAuthenticationProvider Instance
		{
			get
			{
				return MsoIDAuthenticationProvider.instance;
			}
		}

		private MsoIDAuthenticationProvider()
		{
		}

		public string Authenticate(string userName, string password)
		{
			if (this.idcrl == null)
			{
				lock (this.idcrlLock)
				{
					if (this.idcrl == null)
					{
						Guid xmlaClientApplicationID = MsoIDAuthenticationProvider.XmlaClientApplicationID;
						try
						{
							MsoIDCRL msoIDCRL = new MsoIDCRL();
							msoIDCRL.InitializeEx(ref xmlaClientApplicationID, 1, 2u);
							this.idcrl = msoIDCRL;
						}
						catch (BadImageFormatException innerException)
						{
							throw new AdomdConnectionException(XmlaSR.Authentication_MsoID_MissingSignInAssistant, innerException);
						}
					}
				}
			}
			bool flag2;
			if (string.IsNullOrEmpty(userName))
			{
				flag2 = true;
				StringBuilder stringBuilder = new StringBuilder();
				int capacity = 0;
				MsoIDAuthenticationProvider.GetUserNameEx(MsoIDAuthenticationProvider.ExtendedNameFormat.NameUserPrincipal, stringBuilder, ref capacity);
				stringBuilder.Capacity = capacity;
				MsoIDAuthenticationProvider.GetUserNameEx(MsoIDAuthenticationProvider.ExtendedNameFormat.NameUserPrincipal, stringBuilder, ref capacity);
				userName = stringBuilder.ToString();
				if (string.IsNullOrEmpty(userName))
				{
					throw new AdomdConnectionException(XmlaSR.Authentication_MsoID_SsoFailedNonDomainUser);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(password))
				{
					throw new AdomdConnectionException(XmlaSR.ConnectionString_MissingPassword);
				}
				flag2 = false;
			}
			IntPtr zero = IntPtr.Zero;
			string result = null;
			try
			{
				MsoIDCRL.RSTParams[] array = new MsoIDCRL.RSTParams[1];
				array[0].cbSize = (uint)Marshal.SizeOf(typeof(MsoIDCRL.RSTParams));
				array[0].serviceName = "analysis.windows.net";
				array[0].servicePolicy = "MBI_SSL";
				try
				{
					this.idcrl.CreateIdentityHandle(userName, 255u, out zero);
					if (!flag2)
					{
						this.idcrl.SetCredential(zero, "ps:password", password);
					}
					this.idcrl.LogonIdentityEx(zero, null, 0u, array, 1u);
					MsoIDCRL.LogonState logonState;
					this.idcrl.GetAuthState(zero, out logonState);
					if (logonState.AuthState != 296963)
					{
						if (flag2)
						{
							throw new AdomdConnectionException(XmlaSR.Authentication_MsoID_SsoFailed);
						}
						throw new AdomdConnectionException(XmlaSR.Authentication_MsoID_InvalidCredentials);
					}
				}
				catch (IDCRLException innerException2)
				{
					throw new AdomdConnectionException(XmlaSR.Authentication_MsoID_InternalError, innerException2);
				}
				try
				{
					MsoIDCRL.AuthState authState;
					this.idcrl.AuthIdentityToService(zero, "analysis.windows.net", "MBI_SSL", 65536u, out authState);
					result = authState.Token;
				}
				catch (IDCRLException innerException3)
				{
					throw new AdomdConnectionException(XmlaSR.Authentication_MsoID_InternalError, innerException3);
				}
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					this.idcrl.CloseIdentityHandle(zero);
				}
			}
			return result;
		}

		//protected override void Finalize()
		//{
		//	try
		//	{
		//		lock (this.idcrlLock)
		//		{
		//			if (this.idcrl != null)
		//			{
		//				this.idcrl.Uninitialize();
		//				this.idcrl = null;
		//			}
		//		}
		//	}
		//	finally
		//	{
		//		//base.Finalize();
		//	}
		//}

		[DllImport("Secur32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool GetUserNameEx(MsoIDAuthenticationProvider.ExtendedNameFormat extendedNameFormat, StringBuilder userName, ref int userNameSize);
	}
}
