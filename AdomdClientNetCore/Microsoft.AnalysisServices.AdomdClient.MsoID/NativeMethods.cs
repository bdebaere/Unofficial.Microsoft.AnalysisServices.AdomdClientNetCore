using Microsoft.Win32;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient.MsoID
{
	internal sealed class NativeMethods
	{
		private const string IdcrlFileName = "msoidcli.dll";

		//static NativeMethods()
		//{
		//	string text = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\MSOIdentityCRL", "TargetDir", null) as string;
		//	if (text == null || NativeMethods.LoadLibrary(Path.Combine(text, "msoidcli.dll")) == IntPtr.Zero)
		//	{
		//		throw new IDCRLException(string.Format(CultureInfo.CurrentCulture, "Failed to load {0}.\nMicrosoft Online Services Sign-in Assistant should be installed.", new object[]
		//		{
		//			"msoidcli.dll"
		//		}));
		//	}
		//}

		private NativeMethods()
		{
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr LoadLibrary(string filePath);

		[DllImport("msoidcli.dll")]
		internal static extern int Initialize([In] ref Guid AppGuid, [In] int PpcrlVersion, [In] uint Flags);

		[DllImport("msoidcli.dll")]
		internal static extern int CreateIdentityHandle([MarshalAs(UnmanagedType.LPWStr)] [In] string MemberName, [In] uint Flags, out IntPtr Identity);

		[DllImport("msoidcli.dll")]
		internal static extern int AuthIdentityToService([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string ServiceTarget, [MarshalAs(UnmanagedType.LPWStr)] [In] string ServicePolicy, [In] uint TokenRequestFlags, out IntPtr Token, out uint ResultFlags, out IntPtr SessionKey, out uint SessionKeyLength);

		[DllImport("msoidcli.dll")]
		internal static extern int Uninitialize();

		[DllImport("msoidcli.dll")]
		internal static extern int MoveAuthUIContext([In] IntPtr AuthContext, [In] Point Position, [In] Size NewSize);

		[DllImport("msoidcli.dll")]
		internal static extern int DestroyPassportAuthUIContext([In] IntPtr AuthContext);

		[DllImport("msoidcli.dll")]
		internal static extern int CreatePassportAuthUIContext(ref MsoIDCRL.PassportCredUIInfo CredUIInfo, ref MsoIDCRL.PassportCredCustomUI CredUICustomUI, out IntPtr AuthContext);

		[DllImport("msoidcli.dll")]
		internal static extern int GetPreferredAuthUIContextSize([In] IntPtr Identity, out Size Size);

		[DllImport("msoidcli.dll")]
		internal static extern int CloseIdentityHandle([In] IntPtr Identity);

		[DllImport("msoidcli.dll")]
		internal static extern int LogonIdentityWithUI([In] IntPtr AuthContext, [In] IntPtr User, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszPolicy, [In] uint dwAuthFlags);

		[DllImport("msoidcli.dll")]
		internal static extern int GetAuthState([In] IntPtr Identity, out int AuthState, out int AuthRequired, out int RequestStatus, out IntPtr WebFlowUrl);

		[DllImport("msoidcli.dll")]
		internal static extern int PassportFreeMemory([In] [Out] IntPtr o);

		[DllImport("msoidcli.dll")]
		internal static extern int PersistCredential([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string CredType);

		[DllImport("msoidcli.dll")]
		internal static extern int HasPersistedCredential([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string CredType, out long HasPersistentCred);

		[DllImport("msoidcli.dll")]
		internal static extern int EnumIdentitiesWithCachedCredentials([MarshalAs(UnmanagedType.LPWStr)] [In] string CredType, out IntPtr EnumHandle);

		[DllImport("msoidcli.dll")]
		internal static extern int NextIdentity([In] IntPtr EnumHandle, ref IntPtr MemberName);

		[DllImport("msoidcli.dll")]
		internal static extern int SetIdentityProperty([In] IntPtr Identity, MsoIDCRL.PassportIdentityProperty Property, [MarshalAs(UnmanagedType.LPWStr)] [In] string PropertyValue);

		[DllImport("msoidcli.dll")]
		internal static extern int GetIdentityProperty([In] IntPtr Identity, MsoIDCRL.PassportIdentityProperty Property, out IntPtr PropertyValue);

		[DllImport("msoidcli.dll")]
		internal static extern int GetIdentityPropertyByName([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string PropertyName, out IntPtr PropertyValue);

		[DllImport("msoidcli.dll")]
		internal static extern int LogonIdentity([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string Policy, uint AuthFlags);

		[DllImport("msoidcli.dll")]
		internal static extern int InitializeEx([In] ref Guid AppGuid, [In] int PpcrlVersion, [In] uint Flags, [MarshalAs(UnmanagedType.LPArray)] [In] MsoIDCRL.IDCRL_OPTION[] pOptions, [In] uint dwOptions = default(uint));

		[DllImport("msoidcli.dll")]
		internal static extern int InitializeApp([MarshalAs(UnmanagedType.LPWStr)] [In] string AppID, [In] int PpcrlVersion, [In] uint Flags, [MarshalAs(UnmanagedType.LPArray)] [In] MsoIDCRL.IDCRL_OPTION[] pOptions, [In] uint dwOptions);

		[DllImport("msoidcli.dll")]
		internal static extern int SetIdcrlOptions([MarshalAs(UnmanagedType.LPArray)] [In] MsoIDCRL.IDCRL_OPTION[] pOptions, [In] uint dwOptions, [In] uint Flags);

		[DllImport("msoidcli.dll")]
		internal static extern int SetCredential([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string CredType, [MarshalAs(UnmanagedType.LPWStr)] [In] string CredValue);

		[DllImport("msoidcli.dll")]
		internal static extern int BuildAuthTokenRequest([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string Policy, uint AuthFlags, ref IntPtr RequestXML);

		[DllImport("msoidcli.dll")]
		internal static extern int GetWebAuthUrl([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string TargetServiceName, [MarshalAs(UnmanagedType.LPWStr)] [In] string ServicePolicy, [MarshalAs(UnmanagedType.LPWStr)] [In] string AdditionalPostParams, [MarshalAs(UnmanagedType.LPWStr)] [In] string SourceServiceName, out IntPtr WebAuthUrl, out IntPtr PostData);

		[DllImport("msoidcli.dll")]
		internal static extern int RemovePersistedCredential([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string CredType);

		[DllImport("msoidcli.dll")]
		internal static extern int CloseEnumIdentitiesHandle([In] IntPtr EnumHandle);

		[DllImport("msoidcli.dll")]
		internal static extern int BuildServiceTokenRequest([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string ServiceTarget, [MarshalAs(UnmanagedType.LPWStr)] [In] string ServicePolicy, uint TokenRequestFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string RequestXML);

		[DllImport("msoidcli.dll")]
		internal static extern int PutTokenResponseEx([In] IntPtr Identity, uint tokenType, [MarshalAs(UnmanagedType.LPWStr)] [In] string Response);

		[DllImport("msoidcli.dll")]
		internal static extern int DeriveOfflineKey([In] IntPtr Identity, MsoIDCRL.OfflineKeyMethod Method, ref object OfflineKeyParameters, ref object OfflineKeyMaterial, [In] IntPtr CryptProv, [In] IntPtr SessionKey);

		[DllImport("msoidcli.dll")]
		internal static extern int GetCertificate([In] IntPtr Identity, [In] ref MsoIDCRL.RSTParams pcRSTParams, [In] [Out] ref uint dwMinTTL, [In] uint dwRequestFlags, out IntPtr pcertContext, out IntPtr ppbPOP, out uint pcbPOP, out IntPtr ppCACertContext);

		[DllImport("msoidcli.dll")]
		internal static extern int GetAuthStateEx([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string ServiceTarget, out int AuthState, out int AuthRequired, out int RequestStatus, out IntPtr WebFlowUrl);

		[DllImport("msoidcli.dll")]
		internal static extern int AuthIdentityToServiceEx([In] IntPtr Identity, [In] uint swRequestFlags, [MarshalAs(UnmanagedType.LPArray)] [In] MsoIDCRL.RSTParams[] pcRSTParams, [In] uint dwpcRSTParamsCount);

		[DllImport("msoidcli.dll")]
		internal static extern int LogonIdentityEx([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string authPolicy, [In] uint dwAuthFlags, [MarshalAs(UnmanagedType.LPArray)] [In] MsoIDCRL.RSTParams[] pcRSTParams, [In] uint dwpcRSTParamsCount);

		[DllImport("msoidcli.dll")]
		internal static extern int VerifyCertificate([In] IntPtr pcertContext, [In] [Out] ref uint dwMinTTL, [In] IntPtr pbPOP, [In] uint cbPOP, out IntPtr ppCACertContext);

		[DllImport("msoidcli.dll")]
		internal static extern int SetIdentityCallback([In] IntPtr Identity, MsoIDCRL.CallBackDelegateWithData CallBackFunction, IntPtr callBackData);

		[DllImport("msoidcli.dll")]
		internal static extern int CancelPendingRequest([In] IntPtr Identity);

		[DllImport("msoidcli.dll")]
		internal static extern int GetExtendedError([In] IntPtr Identity, [In] IntPtr pReserved, out uint errorCategory, out uint errorCode, out IntPtr errorBlob);

		[DllImport("msoidcli.dll")]
		internal static extern int SaveTweenerCreds([MarshalAs(UnmanagedType.LPWStr)] [In] string strUserName, [In] IntPtr pEncPwd, uint dwLen);

		[DllImport("msoidcli.dll")]
		internal static extern int DeleteTweenerCreds([MarshalAs(UnmanagedType.LPWStr)] [In] string strUserName);

		[DllImport("msoidcli.dll")]
		internal static extern int GetExtendedProperty([MarshalAs(UnmanagedType.LPWStr)] [In] string propertyName, out IntPtr propertyValue);

		[DllImport("msoidcli.dll")]
		internal static extern int SetExtendedProperty([MarshalAs(UnmanagedType.LPWStr)] [In] string propertyName, [MarshalAs(UnmanagedType.LPWStr)] [In] string propertyValue);

		[DllImport("msoidcli.dll")]
		internal static extern int GetWebAuthUrlEx([In] IntPtr Identity, [In] uint webAuthFlag, [MarshalAs(UnmanagedType.LPWStr)] [In] string TargetServiceName, [MarshalAs(UnmanagedType.LPWStr)] [In] string ServicePolicy, [MarshalAs(UnmanagedType.LPWStr)] [In] string AdditionalPostParams, out IntPtr WebAuthUrl, out IntPtr PostData);

		[DllImport("msoidcli.dll")]
		internal static extern int CacheAuthState([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string VirtualAppName, [In] uint Flags);

		[DllImport("msoidcli.dll")]
		internal static extern int RemoveAuthStateFromCache([MarshalAs(UnmanagedType.LPWStr)] [In] string username, [MarshalAs(UnmanagedType.LPWStr)] [In] string VirtualAppName, [In] uint Flags);

		[DllImport("msoidcli.dll")]
		internal static extern int CreateIdentityHandleFromCachedAuthState([MarshalAs(UnmanagedType.LPWStr)] [In] string username, [MarshalAs(UnmanagedType.LPWStr)] [In] string VirtualAppName, [In] uint Flags, out IntPtr identity);

		[DllImport("msoidcli.dll")]
		internal static extern int CreateIdentityHandleFromCachedAuthState([MarshalAs(UnmanagedType.LPWStr)] [In] string VirtualAppName, [In] uint Flags, out IntPtr identity);

		[DllImport("msoidcli.dll")]
		internal static extern int CreateIdentityHandleFromAuthState([MarshalAs(UnmanagedType.LPWStr)] [In] string authToken, [In] uint flags, out IntPtr identity);

		[DllImport("msoidcli.dll")]
		internal static extern int ExportAuthState([In] IntPtr identity, [In] uint flags, out IntPtr authToken);

		[DllImport("msoidcli.dll")]
		internal static extern int SetUserExtendedProperty([MarshalAs(UnmanagedType.LPWStr)] [In] string userName, [MarshalAs(UnmanagedType.LPWStr)] [In] string propertyName, [MarshalAs(UnmanagedType.LPWStr)] [In] string propertyValue);

		[DllImport("msoidcli.dll")]
		internal static extern int GetUserExtendedProperty([MarshalAs(UnmanagedType.LPWStr)] [In] string userName, [MarshalAs(UnmanagedType.LPWStr)] [In] string propertyName, out IntPtr propertyValue);

		[DllImport("msoidcli.dll")]
		internal static extern int GetServiceConfig([MarshalAs(UnmanagedType.LPWStr)] [In] string valueName, out IntPtr UrlValue);

		[DllImport("msoidcli.dll")]
		internal static extern int EncryptWithSessionKey([In] IntPtr identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string serviceName, [In] uint algIdEncrypt, [In] uint algIdHash, [In] IntPtr data, [In] uint dataSize, out IntPtr cipher, out uint cipherSize);

		[DllImport("msoidcli.dll")]
		internal static extern int DecryptWithSessionKey([In] IntPtr identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string serviceName, [In] uint algIdEncrypt, [In] uint algIdHash, [In] IntPtr cipher, [In] uint cipherSize, out IntPtr data, out uint dataSize);

		[DllImport("msoidcli.dll")]
		internal static extern int MigratePersistedCredentials([In] ref Guid AppGuid, [In] bool keepOldCreds, out uint userCount);

		[DllImport("msoidcli.dll")]
		internal static extern int SetChangeNotificationCallback([MarshalAs(UnmanagedType.LPWStr)] [In] string virtualApp, [In] uint reservedFlag, MsoIDCRL.UserStateChangedCallback callBackFunction);

		[DllImport("msoidcli.dll")]
		internal static extern int RemoveChangeNotificationCallback();

		[DllImport("msoidcli.dll")]
		internal static extern int GetDeviceId([In] int dwFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string pvAdditionalParams, out IntPtr deviceId, out IntPtr didCertContext);

		[DllImport("msoidcli.dll")]
		internal static extern int GenerateDeviceToken([In] int dwFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string pvAdditionalParams, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszAudience, out IntPtr pwszDeviceToken);

		[DllImport("msoidcli.dll")]
		internal static extern int SetDeviceConsent([In] int dwFlags, [In] int dwConsentSetting, [MarshalAs(UnmanagedType.LPWStr)] [In] string pvAdditionalParams);

		[DllImport("msoidcli.dll")]
		internal static extern int CloseDeviceID([In] int dwFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string pvAdditionalParams);

		[DllImport("msoidcli.dll")]
		internal static extern int GenerateCertToken([In] IntPtr hExternalIdentity, [In] uint dwFlags, out IntPtr certToken);

		[DllImport("msoidcli.dll")]
		internal static extern int EnumerateCertificates([In] uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string prgwszCSPs, [MarshalAs(UnmanagedType.LPWStr)] [In] string prgwszThumbprints, out uint dwCerts, out IntPtr CertInfo);

		[DllImport("msoidcli.dll")]
		internal static extern int EnumerateDeviceID([In] int dwFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string pvAdditionalParams, out uint dwCount, out IntPtr deviceId, out IntPtr didCertContext);

		[DllImport("msoidcli.dll")]
		internal static extern int CreateLinkedIdentityHandle([In] IntPtr Identity, [In] uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszMemberName, out IntPtr LinkedIdentity);

		[DllImport("msoidcli.dll")]
		internal static extern int OpenAuthenticatedBrowser([In] IntPtr Identity, [In] uint webAuthFlag, [MarshalAs(UnmanagedType.LPWStr)] [In] string TargetServiceName, [MarshalAs(UnmanagedType.LPWStr)] [In] string ServicePolicy, [MarshalAs(UnmanagedType.LPWStr)] [In] string AdditionalPostParams);

		[DllImport("msoidcli.dll")]
		internal static extern int GetAssertion([In] IntPtr Identity, [In] ref MsoIDCRL.RSTParams pcRSTParams, [In] [Out] ref uint dwMinTTL, [In] uint dwRequestFlags, out IntPtr pcertContext, out IntPtr ppbPOP, out uint pcbPOP, out IntPtr ppCACertContext);

		[DllImport("msoidcli.dll")]
		internal static extern int VerifyAssertion([In] IntPtr pcertContext, [In] [Out] ref uint dwMinTTL, [In] IntPtr pbPOP, [In] uint cbPOP, out IntPtr ppCACertContext);

		[DllImport("msoidcli.dll")]
		internal static extern int InitializeIDCRLTraceBuffer([In] uint dwMaxBufferSizeBytes, [MarshalAs(UnmanagedType.LPArray)] [In] MsoIDCRL.IDCRL_OPTION[] pOptions, [In] uint dwOptions = default(uint));

		[DllImport("msoidcli.dll")]
		internal static extern int FlushIDCRLTraceBuffer([MarshalAs(UnmanagedType.LPWStr)] [In] string wszFileLocation, [MarshalAs(UnmanagedType.LPArray)] [In] MsoIDCRL.IDCRL_OPTION[] pOptions, [In] uint dwOptions, out IntPtr pwszTraceBuf);

		[DllImport("msoidcli.dll")]
		internal static extern int LogonIdentityExWithUI([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string authPolicy, [In] uint dwAuthFlags, [In] uint dwSSOFlags, [In] MsoIDCRL.UIParam pcUIParam, [MarshalAs(UnmanagedType.LPArray)] [In] MsoIDCRL.RSTParams[] pcRSTParams, [In] uint dwpcRSTParamsCount = default(uint));

		[DllImport("msoidcli.dll")]
		internal static extern int GetResponseForHttpChallenge([In] IntPtr Identity, [In] uint dwAuthFlags, [In] uint dwSSOFlags, [In] MsoIDCRL.UIParam pcUIParam, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszServiceTarget, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszChallenge, out IntPtr pwszResponse);

		[DllImport("msoidcli.dll")]
		internal static extern int SetDefaultUserForTarget([In] IntPtr Identity, [In] uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszServiceTarget);

		[DllImport("msoidcli.dll")]
		internal static extern int GetDefaultUserForTarget([In] uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszServiceTarget, out IntPtr pwszMemberName);

		[DllImport("msoidcli.dll")]
		internal static extern int AssociateDeviceToUser([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string pwszFriendlyName, [In] uint dwAssocType);

		[DllImport("msoidcli.dll")]
		internal static extern int DisassociateDeviceFromUser([In] IntPtr Identity, [In] uint dwAssocType);

		[DllImport("msoidcli.dll")]
		internal static extern int EnumerateUserAssociatedDevices([In] IntPtr Identity, [In] uint dwAssocType, [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzOwnerUserName, out uint pdwCount, out IntPtr paNames, out IntPtr paFriendlyNames);

		[DllImport("msoidcli.dll")]
		internal static extern int UpdateUserAssociatedDeviceProperties([In] IntPtr Identity, [In] uint dwAssocType, [In] uint pdwCount, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)] [In] string[] paPropertyNames, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)] [In] string[] paPropertyValues);

		[DllImport("msoidcli.dll")]
		internal static extern int GetAuthenticationStatus([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] string ServiceTarget, [In] uint dwVersion, out IntPtr ppStatus);

		[DllImport("msoidcli.dll")]
		internal static extern int GetDeviceShortLivedToken(out IntPtr ppszDIDDAtoken);

		[DllImport("msoidcli.dll")]
		internal static extern int ProvisionDeviceId([In] uint dwDeviceType, [In] uint dwFlags);

		[DllImport("msoidcli.dll")]
		internal static extern int DeProvisionDeviceId([In] uint dwDeviceType, [In] uint dwFlags);

		[DllImport("msoidcli.dll")]
		internal static extern int RenewDeviceId([In] uint dwDeviceType, [In] uint dwFlags);

		[DllImport("msoidcli.dll")]
		internal static extern int GetDeviceIdEx([In] uint dwDeviceType, [In] uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string pvAdditionalParams, out IntPtr deviceId, out IntPtr didCertContext);
	}
}
