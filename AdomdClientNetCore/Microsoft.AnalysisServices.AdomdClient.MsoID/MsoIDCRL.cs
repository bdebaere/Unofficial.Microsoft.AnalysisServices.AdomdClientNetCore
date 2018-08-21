using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient.MsoID
{
	internal sealed class MsoIDCRL
	{
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		internal struct PassportCredUIInfo
		{
			internal IntPtr hwndParent;

			internal Point ptPosition;

			internal Size szSize;

			internal bool bShow;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		internal struct PassportCredCustomUI
		{
			internal uint cElements;

			internal IntPtr customValues;

			internal PassportCredCustomUI(uint cElements, IntPtr customValues)
			{
				this.cElements = cElements;
				this.customValues = customValues;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct POINT
		{
			internal int x;

			internal int y;

			internal POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct SIZE
		{
			internal int cx;

			internal int cy;

			internal SIZE(int cx, int cy)
			{
				this.cx = cx;
				this.cy = cy;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct PassportIdentityHandle
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct PassportUIAuthContextHandle
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct PassportEnumIdentitiesHandle
		{
		}

		internal delegate uint CallBackDelegateWithData(IntPtr Identity, IntPtr ptr, bool CanContinue);

		internal delegate void UserStateChangedCallback(uint dwChangeID, IntPtr changeData);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct IDCRL_OPTION
		{
			internal MsoIDCRL.IDCRL_OPTION_ID m_dwId;

			internal IntPtr m_pValue;

			internal uint m_cbValue;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct IDCRL_OPTIONS
		{
			internal uint m_dwCount;

			internal MsoIDCRL.IDCRL_OPTION m_arrOptions;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct INTERNET_PROXY_INFO
		{
			internal uint dwAccessType;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string proxyName;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string ProxyBypassList;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct RSTParams
		{
			internal uint cbSize;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string serviceName;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string servicePolicy;

			internal uint tokenFlags;

			internal uint tokenParams;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal sealed class IDCRL_STATUS_CURRENT
		{
			internal int hrAuthState;

			internal int hrAuthRequired;

			internal int hrRequestStatus;

			internal int hrUIError;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string wszWebFlowUrl;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal sealed class UIParam
		{
			internal uint uiFlag;

			internal IntPtr hwndParent;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string cobrandingText;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string appName;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string signUpText;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string cobrandingLogoPath;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string headerBgImage;

			internal uint bgColor;

			internal uint urlColor;

			internal uint tileBgColor;

			internal uint tileBdColor;

			internal uint fieldBdColor;

			internal uint checkboxLbColor;

			internal uint btTxtColor;

			internal uint tileLbColor;

			internal int lWinLeft;

			internal int lWinTop;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string signupUrl;
		}

		internal struct AuthState
		{
			internal string token;

			internal uint resultFlags;

			internal string sessionKey;

			internal uint sessionKeyLength;

			internal string Token
			{
				get
				{
					return this.token;
				}
			}

			internal uint ResultFlags
			{
				get
				{
					return this.resultFlags;
				}
			}

			internal string SessionKey
			{
				get
				{
					return this.sessionKey;
				}
			}

			internal uint SessionKeyLength
			{
				get
				{
					return this.sessionKeyLength;
				}
			}
		}

		internal struct LogonState
		{
			internal int AuthState;

			internal int AuthRequired;

			internal int RequestStatus;

			internal string WebFlowUrl;
		}

		internal struct MD5Data
		{
			internal string md5Url;

			internal string postData;

			internal string MD5Url
			{
				get
				{
					return this.md5Url;
				}
			}

			internal string PostData
			{
				get
				{
					return this.postData;
				}
			}
		}

		internal struct SHA1
		{
			internal string sha1Url;

			internal string sha1PostData;

			internal string Sha1Url
			{
				get
				{
					return this.sha1Url;
				}
			}

			internal string Sha1PostData
			{
				get
				{
					return this.sha1PostData;
				}
			}
		}

		internal struct CertSet
		{
			internal IntPtr ppcertContext;

			internal IntPtr ppCACertContext;

			internal uint popSize;

			internal IntPtr popBlob;

			internal X509Certificate x509Cert;

			internal X509Certificate x509CACert;

			internal X509Certificate2 x509Cert2;

			internal X509Certificate2 x509CACert2;

			internal string x509CertStr;

			internal string x509CACertStr;

			internal string popBlobStr;

			internal IntPtr PPCertContext
			{
				get
				{
					return this.ppcertContext;
				}
			}

			internal IntPtr PPCACertContext
			{
				get
				{
					return this.ppCACertContext;
				}
			}

			internal IntPtr PopBlob
			{
				get
				{
					return this.popBlob;
				}
			}

			internal uint PopSize
			{
				get
				{
					return this.popSize;
				}
			}

			internal X509Certificate X509Cert
			{
				get
				{
					return this.x509Cert;
				}
			}

			internal X509Certificate X509CACert
			{
				get
				{
					return this.x509CACert;
				}
			}

			internal X509Certificate2 X509Cert2
			{
				get
				{
					return this.x509Cert2;
				}
			}

			internal X509Certificate2 X509CACert2
			{
				get
				{
					return this.x509CACert2;
				}
			}

			internal string PopBlobStr
			{
				get
				{
					return this.popBlobStr;
				}
			}

			internal string X509CertStr
			{
				get
				{
					return this.x509CertStr;
				}
			}

			internal string X509CACertStr
			{
				get
				{
					return this.x509CACertStr;
				}
			}

			internal CertSet(IntPtr certContext, IntPtr popBlob, uint popSize, IntPtr caCertContext)
			{
				if (certContext != IntPtr.Zero)
				{
					this.ppcertContext = certContext;
					this.x509Cert = new X509Certificate(certContext);
					this.x509CertStr = Marshal.PtrToStringUni(certContext);
					this.x509Cert2 = new X509Certificate2(certContext);
				}
				else
				{
					this.ppcertContext = IntPtr.Zero;
					this.x509Cert = new X509Certificate();
					this.x509Cert2 = new X509Certificate2();
					this.x509CertStr = string.Empty;
				}
				if (caCertContext != IntPtr.Zero)
				{
					this.ppCACertContext = caCertContext;
					this.x509CACert = ((caCertContext == IntPtr.Zero) ? new X509Certificate() : new X509Certificate(caCertContext));
					this.x509CACertStr = Marshal.PtrToStringUni(caCertContext);
					this.x509CACert2 = ((caCertContext == IntPtr.Zero) ? new X509Certificate2() : new X509Certificate2(caCertContext));
				}
				else
				{
					this.ppCACertContext = IntPtr.Zero;
					this.x509CACert = new X509Certificate();
					this.x509CACertStr = string.Empty;
					this.x509CACert2 = new X509Certificate2();
				}
				this.popBlob = popBlob;
				if (popBlob != IntPtr.Zero)
				{
					this.popBlobStr = Marshal.PtrToStringUni(popBlob);
				}
				else
				{
					this.popBlobStr = string.Empty;
				}
				this.popSize = popSize;
			}
		}

		internal struct ExtendedError
		{
			internal MsoIDCRL.IDCRL_ERROR_CATEGORY category;

			internal uint errorCode;

			internal string errorBlob;

			internal MsoIDCRL.IDCRL_ERROR_CATEGORY Category
			{
				get
				{
					return this.category;
				}
			}

			internal uint ErrorCode
			{
				get
				{
					return this.errorCode;
				}
			}

			internal string ErrorBlob
			{
				get
				{
					return this.errorBlob;
				}
			}
		}

		internal struct Initialize_Options
		{
			internal MsoIDCRL.IDCRL_OPTION_ID optionType;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string proxyUserInfo;

			internal int[] timeOut;

			internal MsoIDCRL.INTERNET_PROXY_INFO proxyInfo;
		}

		internal enum INTERNET_CONNECTION_OPTIONS
		{
			INTERNET_OPEN_TYPE_PRECONFIG,
			INTERNET_OPEN_TYPE_DIRECT,
			INTERNET_OPEN_TYPE_PROXY = 3,
			INTERNET_OPEN_TYPE_PRECONFIG_WITH_NO_AUTOPROXY
		}

		internal enum AUTHENTICATION_STATE
		{
			AUTHENTICATED_USING_PASSWORD = 296963
		}

		internal enum IDCRL_OPTION_ID
		{
			IDCRL_OPTION_ALL_BIT = 127,
			IDCRL_OPTION_PROXY = 1,
			IDCRL_OPTION_CONNECT_TIMEOUT,
			IDCRL_OPTION_SEND_TIMEOUT = 4,
			IDCRL_OPTION_RECEIVE_TIMEOUT = 8,
			IDCRL_OPTION_PROXY_PASSWORD = 16,
			IDCRL_OPTION_PROXY_USERNAME = 32,
			IDCRL_OPTION_ENVIRONMENT = 64,
			IDCRL_OPTION_MSC_TIMEOUT = 128
		}

		internal enum PassportIdentityProperty
		{
			IDENTITY_MEMBER_NAME = 1,
			IDENTITY_PUIDSTR
		}

		internal enum UpdateFlag : uint
		{
			UPDATE_FLAG_ALL_BIT = 15u,
			DEFAULT_UPDATE_POLICY = 0u,
			OFFLINE_MODE_ALLOWED,
			NO_UI,
			SKIP_CONNECTION_CHECK = 4u,
			SET_EXTENDED_ERROR = 8u,
			SEND_VERSION = 16u,
			UPDATE_DEFAULT = 0u
		}

		internal enum LogonFlag : uint
		{
			LOGONIDENTITY_ALL_BIT = 511u,
			LOGONIDENTITY_DEFAULT = 0u,
			LOGONIDENTITY_ALLOW_OFFLINE,
			LOGONIDENTITY_FORCE_OFFLINE,
			LOGONIDENTITY_CREATE_OFFLINE_HASH = 4u,
			LOGONIDENTITY_ALLOW_PERSISTENT_COOKIES = 8u,
			LOGONIDENTITY_USE_EID_AUTH = 16u,
			LOGONIDENTITY_USE_LINKED_ACCOUNTS = 32u,
			LOGONIDENTITY_FEDERATED = 64u,
			LOGONIDENTITY_WLID = 128u,
			LOGONIDENTITY_AUTO_PARTNER_REDIRECT = 256u,
			LOGONIDENTITY_IGNORE_CACHED_TOKENS = 512u
		}

		internal enum IdentityFlag : uint
		{
			IDENTITY_ALL_BIT = 1023u,
			IDENTITY_SHARE_ALL = 255u,
			IDENTITY_LOAD_FROM_PERSISTED_STORE,
			IDENTITY_AUTHSTATE_ENCRYPTED = 512u
		}

		internal enum OfflineKeyMethod
		{
			OFFLINE_KEY_PBKDF2_ON_PASSWORD = 1
		}

		internal enum ServiceTokenFlags : uint
		{
			SERVICE_TOKEN_TYPE_LEGACY_PASSPORT = 1u,
			SERVICE_TOKEN_TYPE_WEBSSO,
			SERVICE_TOKEN_TYPE_COMPACT_WEBSSO = 4u,
			SERVICE_TOKEN_TYPE_ANY = 255u,
			SERVICE_TOKEN_FROM_CACHE = 65536u,
			SERVICE_TOKEN_TYPE_X509V3 = 8u,
			SERVICE_TOKEN_CERT_IN_MEMORY_PRIVATE_KEY = 16u,
			SERVICE_TOKEN_REQUEST_TYPE_NONE = 0u,
			SERVICE_TOKEN_TYPE_PROPRIETARY,
			SERVICE_TOKEN_TYPE_SAML
		}

		internal enum CertRequestFlags : uint
		{
			CERT_FROM_CACHE = 65536u,
			CERT_FROM_SERVER = 131072u
		}

		internal enum ResponseType : uint
		{
			PPCRL_RESPONSE_TYPE_AUTH,
			PPCRL_RESPONSE_TYPE_SERVICE
		}

		internal enum WebAuthOptions : uint
		{
			IDCRL_WEBAUTH_NONE,
			IDCRL_WEBAUTH_REAUTH,
			IDCRL_WEBAUTH_PERSISTENT
		}

		internal enum IDCRL_ERROR_CATEGORY : uint
		{
			IDCRL_REQUEST_BUILD_ERROR = 1u,
			IDCRL_REQUEST_SEND_ERROR,
			IDCRL_RESPONSE_RECEIVE_ERROR,
			IDCRL_RESPONSE_READ_ERROR,
			IDCRL_REPSONSE_PARSE_ERROR,
			IDCRL_RESPONSE_SIG_DECRYPT_ERROR,
			IDCRL_RESPONSE_PARSE_HEADER_ERROR,
			IDCRL_RESPONSE_PARSE_TOKEN_ERROR,
			IDCRL_RESPONSE_PUTCERT_ERROR
		}

		internal enum NOTIFICATION_CHANGE_TYPE : uint
		{
			NOTIFICATION_CHANGE_ALL_BIT = 3u,
			IDS_USER_ACCOUNT_CHANGE = 1u,
			IDS_USER_PROPERTY_CHANGE
		}

		internal enum NOTIFICATION_ACTION_TYPE : uint
		{
			IDS_NOTIFY_ACTION_ALL_BIT = 7u,
			IDS_NOTIFY_ACTION_ADD = 1u,
			IDS_NOTIFY_ACTION_UPDATE,
			IDS_NOTIFY_ACTION_DELETE = 4u
		}

		internal enum IDCRL_SETOPTIONS_FLAG : uint
		{
			IDCRL_SETOPTIONS_DEFAULT,
			IDCRL_SETOPTIONS_SET = 0u,
			IDCRL_SETOPTIONS_RESET
		}

		internal enum IDCRL_DEVICE_ID_OPTIONS
		{
			IDCRL_DEVICE_ID_FORCENEWPASSWORD = 1,
			IDCRL_DEVICE_ID_IMPERSONATE,
			IDCRL_DEVICE_ID_KEYROLLOVER = 4,
			IDCRL_DEVICE_ID_PHYSICAL = 8,
			IDCRL_DEVICE_ID_LOGICAL = 0,
			IDCRL_DEVICE_ID_FROMCACHE = 16,
			IDCRL_DEVICE_ID_ACCESSCHECK = 32,
			IDCRL_DEVICE_ID_NO_SIGNUP = 256,
			IDCRL_DEVICE_ID_RENEW_CERT = 512
		}

		internal enum IDCRL_DEVICE_CONSENT_OPTIONS
		{
			IDCRL_DEVICE_ID_CONSENT_MIN,
			IDCRL_DEVICE_ID_CONSENT_GRANT,
			IDCRL_DEVICE_ID_CONSENT_REVOKE,
			IDCRL_DEVICE_ID_CONSENT_REMOVE,
			IDCRL_DEVICE_ID_CONSENT_MAX
		}

		internal enum UIFlag : uint
		{
			WLIDUI_ALL_BIT = 1023u,
			WLIDUI_DEFAULT = 0u,
			WLIDUI_DISABLE_REMEBERME,
			WLIDUI_DISABLE_SAVEPASSWORD,
			WLIDUI_DISABLE_DIFFERENTUSER = 4u,
			WLIDUI_DISABLE_EID = 32u,
			WLIDUI_DISABLE_SIGNUPLINK = 64u,
			WLIDUI_DISABLE_SAVEDUSERS = 128u,
			WLIDUI_FORCE_SAVEPASSWORD = 256u,
			WLIDUI_FORCE_SMARTCARD = 512u
		}

		internal enum SSOFlags : uint
		{
			SSO_ALL_BIT = 15u,
			SSO_DEFAULT = 0u,
			SSO_NO_UI,
			SSO_ALWAYS_SHOW_UI,
			SSO_HANDLE_ERROR = 4u
		}

		internal enum WINHTTP_INTERNET_CONNECTION_OPTIONS
		{
			WINHTTP_ACCESS_TYPE_DEFAULT_PROXY,
			WINHTTP_ACCESS_TYPE_NO_PROXY,
			WINHTTP_ACCESS_TYPE_NAMED_PROXY = 3
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct IDSUserNotification
		{
			internal uint type;

			internal uint action;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string accountName;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string credType;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct IDSUserPropertyNotification
		{
			internal uint type;

			internal uint action;

			private bool bValueTooLarge;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string accountName;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string propertyName;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string propertyValue;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct IDCRLCertInfo
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
			internal char[] thumbprint;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
			internal char[] description;
		}

		internal struct CertificateInfo
		{
			internal string thumbprint;

			internal string description;
		}

		internal const int IDCRL_API_VERSION_1 = 1;

		internal const int IDCRL_API_VERSION_CURRENT = 1;

		internal const string PPCRL_CREDTYPE_PASSWORD = "ps:password";

		internal const string PPCRL_CREDTYPE_MEMBERNAMEONLY = "ps:membernameonly";

		internal const string PPCRL_CREDTYPE_ACTIVEUSER = "ps:active";

		internal const string PPCRL_CREDTYPE_VIRUTUALAPPPrefix = "ps:virtualapp=";

		internal const string PPCRL_CREDTYPE_PIN = "ps:pin";

		internal const string PPCRL_CREDTYPE_EID = "ps:eid";

		internal const string EXT_USER_PROPERTY_NAME_USERTILEURL = "UserTileUrl";

		internal const string EXT_USER_PROPERTY_NAME_IDTILETIMESTAMP = "IDTileTimestamp";

		internal const string EXT_USER_PROPERTY_VALUE_REMOVE = null;

		internal const string PPCRL_CREDPROPERTY_MEMBER_NAME = "MemberName";

		internal const string PPCRL_CREDPROPERTY_PUIDSTR = "PUID";

		internal const string PPCRL_CREDPROPERTY_ONETIMECREDENTIAL = "OneTimeCredential";

		internal const string PPCRL_CREDPROPERTY_CID = "CID";

		internal const string PPCRL_CREDPROPERTY_MAINBRANDID = "MainBrandID";

		internal const string PCRL_CREDPROPERTY_BRANDIDLIST = "BrandIDList";

		internal const string PPCRL_CREDPROPERTY_ISWINLIVEUSER = "IsWinLiveUser";

		internal const string PPCRL_CREDPROPERTY_EID = "EID";

		internal const string PPCRL_CREDPROPERTY_LINKID = "LinkId";

		internal const string PPCRL_CREDPROPERTY_LINKVER = "LinkVer";

		internal const string PPCRL_CREDPROPERTY_LINKEDIDS = "LinkedIds";

		internal const string PPCRL_CREDPROPERTY_ISDOMAINUSER = "IsDomainUser";

		internal const int S_OK = 0;

		internal const int Default_Flag = 0;

		internal const string EXT_PROPERTY_NAME_internalCOMPUTER = "internalComputer";

		internal const string EXT_PROPERTY_VALUE_TRUE = "1";

		internal const string EXT_PROPERTY_VALUE_FALSE = "0";

		internal const string EXT_PROPERTY_VALUE_REMOVE = null;

		internal MsoIDCRL()
		{
		}

		internal int Initialize(ref Guid appGuid, int ppclrVersion, uint updateFlags)
		{
			int num = NativeMethods.Initialize(ref appGuid, ppclrVersion, updateFlags);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int InitializeEx(ref Guid appGuid, int ppcrlVersion, uint updateFlags)
		{
			int num = NativeMethods.InitializeEx(ref appGuid, ppcrlVersion, updateFlags, null, 0u);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int InitializeEx(ref Guid appGuid, int ppcrlVersion, uint updateFlags, MsoIDCRL.IDCRL_OPTION[] options, uint count)
		{
			int num = NativeMethods.InitializeEx(ref appGuid, ppcrlVersion, updateFlags, options, count);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int InitializeEx(ref Guid appGuid, int ppcrlVersion, uint updateFlags, MsoIDCRL.Initialize_Options[] initOptions)
		{
			MsoIDCRL.IDCRL_OPTION[] array = new MsoIDCRL.IDCRL_OPTION[initOptions.Length];
			IntPtr intPtr = IntPtr.Zero;
			GCHandle[] array2 = new GCHandle[initOptions.Length];
			int i = 0;
			while (i < initOptions.Length)
			{
				MsoIDCRL.IDCRL_OPTION_ID optionType = initOptions[i].optionType;
				int num;
				if (optionType <= MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY_PASSWORD)
				{
					switch (optionType)
					{
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY:
					{
						num = Marshal.SizeOf(initOptions[i].proxyInfo);
						byte[] value = new byte[num];
						array2[i] = GCHandle.Alloc(value, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.StructureToPtr(initOptions[i].proxyInfo, intPtr, true);
						break;
					}
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_CONNECT_TIMEOUT:
					{
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value2 = new byte[num];
						array2[i] = GCHandle.Alloc(value2, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					case (MsoIDCRL.IDCRL_OPTION_ID)3:
						goto IL_431;
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_SEND_TIMEOUT:
					{
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value3 = new byte[num];
						array2[i] = GCHandle.Alloc(value3, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					default:
						if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_RECEIVE_TIMEOUT)
						{
							if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY_PASSWORD)
							{
								goto IL_431;
							}
							byte[] bytes = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
							num = bytes.Length;
							byte[] value4 = new byte[bytes.Length];
							array2[i] = GCHandle.Alloc(value4, GCHandleType.Pinned);
							intPtr = array2[i].AddrOfPinnedObject();
							Marshal.Copy(bytes, 0, intPtr, bytes.Length);
						}
						else
						{
							num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
							byte[] value5 = new byte[num];
							array2[i] = GCHandle.Alloc(value5, GCHandleType.Pinned);
							intPtr = array2[i].AddrOfPinnedObject();
							Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						}
						break;
					}
				}
				else if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY_USERNAME)
				{
					if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_ENVIRONMENT)
					{
						if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_MSC_TIMEOUT)
						{
							goto IL_431;
						}
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value6 = new byte[num];
						array2[i] = GCHandle.Alloc(value6, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
					}
					else
					{
						byte[] bytes2 = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
						num = bytes2.Length;
						byte[] value7 = new byte[bytes2.Length];
						array2[i] = GCHandle.Alloc(value7, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(bytes2, 0, intPtr, bytes2.Length);
					}
				}
				else
				{
					byte[] bytes3 = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
					num = bytes3.Length;
					byte[] value8 = new byte[bytes3.Length];
					array2[i] = GCHandle.Alloc(value8, GCHandleType.Pinned);
					intPtr = array2[i].AddrOfPinnedObject();
					Marshal.Copy(bytes3, 0, intPtr, bytes3.Length);
				}
				IL_43A:
				array[i].m_dwId = initOptions[i].optionType;
				array[i].m_pValue = intPtr;
				array[i].m_cbValue = (uint)num;
				intPtr = IntPtr.Zero;
				i++;
				continue;
				IL_431:
				intPtr = IntPtr.Zero;
				num = 0;
				goto IL_43A;
			}
			int num2 = NativeMethods.InitializeEx(ref appGuid, ppcrlVersion, updateFlags, array, (uint)array.Length);
			for (int j = 0; j < initOptions.Length; j++)
			{
				array[j].m_pValue = IntPtr.Zero;
				if (array2[j].IsAllocated)
				{
					array2[j].Free();
				}
			}
			if (num2 < 0)
			{
				IDCRLException ex = new IDCRLException(num2);
				throw ex;
			}
			return num2;
		}

		internal int InitializeEx(ref Guid appGuid, int ppcrlVersion, uint updateFlags, MsoIDCRL.Initialize_Options[] initOptions, uint count)
		{
			MsoIDCRL.IDCRL_OPTION[] array = new MsoIDCRL.IDCRL_OPTION[initOptions.Length];
			IntPtr intPtr = IntPtr.Zero;
			GCHandle[] array2 = new GCHandle[initOptions.Length];
			int i = 0;
			while (i < initOptions.Length)
			{
				MsoIDCRL.IDCRL_OPTION_ID optionType = initOptions[i].optionType;
				int num;
				if (optionType <= MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_RECEIVE_TIMEOUT)
				{
					switch (optionType)
					{
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY:
					{
						num = Marshal.SizeOf(initOptions[i].proxyInfo);
						byte[] value = new byte[num];
						array2[i] = GCHandle.Alloc(value, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.StructureToPtr(initOptions[i].proxyInfo, intPtr, true);
						break;
					}
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_CONNECT_TIMEOUT:
					{
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value2 = new byte[num];
						array2[i] = GCHandle.Alloc(value2, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					case (MsoIDCRL.IDCRL_OPTION_ID)3:
						goto IL_396;
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_SEND_TIMEOUT:
					{
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value3 = new byte[num];
						array2[i] = GCHandle.Alloc(value3, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					default:
					{
						if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_RECEIVE_TIMEOUT)
						{
							goto IL_396;
						}
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value4 = new byte[num];
						array2[i] = GCHandle.Alloc(value4, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					}
				}
				else if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY_PASSWORD)
				{
					if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY_USERNAME)
					{
						if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_ENVIRONMENT)
						{
							goto IL_396;
						}
						byte[] bytes = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
						num = bytes.Length;
						byte[] value5 = new byte[bytes.Length];
						array2[i] = GCHandle.Alloc(value5, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(bytes, 0, intPtr, bytes.Length);
					}
					else
					{
						byte[] bytes2 = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
						num = bytes2.Length;
						byte[] value6 = new byte[bytes2.Length];
						array2[i] = GCHandle.Alloc(value6, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(bytes2, 0, intPtr, bytes2.Length);
					}
				}
				else
				{
					byte[] bytes3 = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
					num = bytes3.Length;
					byte[] value7 = new byte[bytes3.Length];
					array2[i] = GCHandle.Alloc(value7, GCHandleType.Pinned);
					intPtr = array2[i].AddrOfPinnedObject();
					Marshal.Copy(bytes3, 0, intPtr, bytes3.Length);
				}
				IL_39F:
				array[i].m_dwId = initOptions[i].optionType;
				array[i].m_pValue = intPtr;
				array[i].m_cbValue = (uint)num;
				intPtr = IntPtr.Zero;
				i++;
				continue;
				IL_396:
				intPtr = IntPtr.Zero;
				num = 0;
				goto IL_39F;
			}
			int num2 = NativeMethods.InitializeEx(ref appGuid, ppcrlVersion, updateFlags, array, count);
			for (int j = 0; j < initOptions.Length; j++)
			{
				array[j].m_pValue = IntPtr.Zero;
				if (array2[j].IsAllocated)
				{
					array2[j].Free();
				}
			}
			if (num2 < 0)
			{
				IDCRLException ex = new IDCRLException(num2);
				throw ex;
			}
			return num2;
		}

		internal int InitializeApp(string appID, int ppcrlVersion, uint updateFlags, MsoIDCRL.Initialize_Options[] initOptions)
		{
			MsoIDCRL.IDCRL_OPTION[] array = new MsoIDCRL.IDCRL_OPTION[initOptions.Length];
			IntPtr intPtr = IntPtr.Zero;
			GCHandle[] array2 = new GCHandle[initOptions.Length];
			int i = 0;
			while (i < initOptions.Length)
			{
				MsoIDCRL.IDCRL_OPTION_ID optionType = initOptions[i].optionType;
				int num;
				if (optionType <= MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_RECEIVE_TIMEOUT)
				{
					switch (optionType)
					{
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY:
					{
						num = Marshal.SizeOf(initOptions[i].proxyInfo);
						byte[] value = new byte[num];
						array2[i] = GCHandle.Alloc(value, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.StructureToPtr(initOptions[i].proxyInfo, intPtr, true);
						break;
					}
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_CONNECT_TIMEOUT:
					{
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value2 = new byte[num];
						array2[i] = GCHandle.Alloc(value2, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					case (MsoIDCRL.IDCRL_OPTION_ID)3:
						goto IL_396;
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_SEND_TIMEOUT:
					{
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value3 = new byte[num];
						array2[i] = GCHandle.Alloc(value3, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					default:
					{
						if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_RECEIVE_TIMEOUT)
						{
							goto IL_396;
						}
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value4 = new byte[num];
						array2[i] = GCHandle.Alloc(value4, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					}
				}
				else if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY_PASSWORD)
				{
					if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY_USERNAME)
					{
						if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_ENVIRONMENT)
						{
							goto IL_396;
						}
						byte[] bytes = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
						num = bytes.Length;
						byte[] value5 = new byte[bytes.Length];
						array2[i] = GCHandle.Alloc(value5, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(bytes, 0, intPtr, bytes.Length);
					}
					else
					{
						byte[] bytes2 = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
						num = bytes2.Length;
						byte[] value6 = new byte[bytes2.Length];
						array2[i] = GCHandle.Alloc(value6, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(bytes2, 0, intPtr, bytes2.Length);
					}
				}
				else
				{
					byte[] bytes3 = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
					num = bytes3.Length;
					byte[] value7 = new byte[bytes3.Length];
					array2[i] = GCHandle.Alloc(value7, GCHandleType.Pinned);
					intPtr = array2[i].AddrOfPinnedObject();
					Marshal.Copy(bytes3, 0, intPtr, bytes3.Length);
				}
				IL_39F:
				array[i].m_dwId = initOptions[i].optionType;
				array[i].m_pValue = intPtr;
				array[i].m_cbValue = (uint)num;
				intPtr = IntPtr.Zero;
				i++;
				continue;
				IL_396:
				intPtr = IntPtr.Zero;
				num = 0;
				goto IL_39F;
			}
			int num2 = NativeMethods.InitializeApp(appID, ppcrlVersion, updateFlags, array, (uint)array.Length);
			for (int j = 0; j < initOptions.Length; j++)
			{
				array[j].m_pValue = IntPtr.Zero;
				if (array2[j].IsAllocated)
				{
					array2[j].Free();
				}
			}
			if (num2 < 0)
			{
				IDCRLException ex = new IDCRLException(num2);
				throw ex;
			}
			return num2;
		}

		internal int SetIdcrlOptions(MsoIDCRL.Initialize_Options[] initOptions, uint updateFlags)
		{
			MsoIDCRL.IDCRL_OPTION[] array = new MsoIDCRL.IDCRL_OPTION[initOptions.Length];
			IntPtr intPtr = IntPtr.Zero;
			GCHandle[] array2 = new GCHandle[initOptions.Length];
			int i = 0;
			while (i < initOptions.Length)
			{
				MsoIDCRL.IDCRL_OPTION_ID optionType = initOptions[i].optionType;
				int num;
				if (optionType <= MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_RECEIVE_TIMEOUT)
				{
					switch (optionType)
					{
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY:
					{
						num = Marshal.SizeOf(initOptions[i].proxyInfo);
						byte[] value = new byte[num];
						array2[i] = GCHandle.Alloc(value, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.StructureToPtr(initOptions[i].proxyInfo, intPtr, true);
						break;
					}
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_CONNECT_TIMEOUT:
					{
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value2 = new byte[num];
						array2[i] = GCHandle.Alloc(value2, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					case (MsoIDCRL.IDCRL_OPTION_ID)3:
						goto IL_382;
					case MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_SEND_TIMEOUT:
					{
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value3 = new byte[num];
						array2[i] = GCHandle.Alloc(value3, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					default:
					{
						if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_RECEIVE_TIMEOUT)
						{
							goto IL_382;
						}
						num = Marshal.SizeOf(initOptions[i].timeOut[0].GetType()) * initOptions[i].timeOut.Length;
						byte[] value4 = new byte[num];
						array2[i] = GCHandle.Alloc(value4, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(initOptions[i].timeOut, 0, intPtr, initOptions[i].timeOut.Length);
						break;
					}
					}
				}
				else if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY_PASSWORD)
				{
					if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_PROXY_USERNAME)
					{
						if (optionType != MsoIDCRL.IDCRL_OPTION_ID.IDCRL_OPTION_ENVIRONMENT)
						{
							goto IL_382;
						}
						byte[] bytes = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
						num = bytes.Length;
						byte[] value5 = new byte[bytes.Length];
						array2[i] = GCHandle.Alloc(value5, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(bytes, 0, intPtr, bytes.Length);
					}
					else
					{
						byte[] bytes2 = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
						num = bytes2.Length;
						byte[] value6 = new byte[bytes2.Length];
						array2[i] = GCHandle.Alloc(value6, GCHandleType.Pinned);
						intPtr = array2[i].AddrOfPinnedObject();
						Marshal.Copy(bytes2, 0, intPtr, bytes2.Length);
					}
				}
				else
				{
					byte[] bytes3 = Encoding.Unicode.GetBytes(initOptions[i].proxyUserInfo);
					num = bytes3.Length;
					byte[] value7 = new byte[bytes3.Length];
					array2[i] = GCHandle.Alloc(value7, GCHandleType.Pinned);
					intPtr = array2[i].AddrOfPinnedObject();
					Marshal.Copy(bytes3, 0, intPtr, bytes3.Length);
				}
				IL_38B:
				array[i].m_dwId = initOptions[i].optionType;
				array[i].m_pValue = intPtr;
				array[i].m_cbValue = (uint)num;
				intPtr = IntPtr.Zero;
				i++;
				continue;
				IL_382:
				intPtr = IntPtr.Zero;
				num = 0;
				goto IL_38B;
			}
			int num2 = NativeMethods.SetIdcrlOptions(array, (uint)array.Length, updateFlags);
			for (int j = 0; j < initOptions.Length; j++)
			{
				array[j].m_pValue = IntPtr.Zero;
				if (array2[j].IsAllocated)
				{
					array2[j].Free();
				}
			}
			if (num2 < 0)
			{
				IDCRLException ex = new IDCRLException(num2);
				throw ex;
			}
			return num2;
		}

		internal int Uninitialize()
		{
			int num = NativeMethods.Uninitialize();
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int CreateIdentityHandle(string memberName, uint identityFlags, out IntPtr hIdentity)
		{
			hIdentity = IntPtr.Zero;
			int num = NativeMethods.CreateIdentityHandle(memberName, identityFlags, out hIdentity);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int AuthIdentityToService(IntPtr hIdentity, string serviceTarget, string servicePolicy, uint serviceTokenFlags, out MsoIDCRL.AuthState authState)
		{
			IntPtr zero = IntPtr.Zero;
			uint resultFlags = 0u;
			IntPtr zero2 = IntPtr.Zero;
			uint sessionKeyLength = 0u;
			int num = NativeMethods.AuthIdentityToService(hIdentity, serviceTarget, servicePolicy, serviceTokenFlags, out zero, out resultFlags, out zero2, out sessionKeyLength);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			authState = default(MsoIDCRL.AuthState);
			authState.token = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			authState.resultFlags = resultFlags;
			authState.sessionKey = ((zero2 == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero2));
			authState.sessionKeyLength = sessionKeyLength;
			this.InternalPassportFreeMemory(zero);
			this.InternalPassportFreeMemory(zero2);
			return num;
		}

		internal int LogonIdentity(IntPtr hIdentity, string servicePolicy, uint logonFlags)
		{
			int num = NativeMethods.LogonIdentity(hIdentity, servicePolicy, logonFlags);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int LogonIdentityEx(IntPtr hIdentity, string authPolicy, uint logonFlags, MsoIDCRL.RSTParams[] rstParam, uint paramCount)
		{
			int num = NativeMethods.LogonIdentityEx(hIdentity, authPolicy, logonFlags, rstParam, paramCount);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int SetCredential(IntPtr hIdentity, string credType, string credValue)
		{
			int num = NativeMethods.SetCredential(hIdentity, credType, credValue);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GetAuthState(IntPtr hIdentity, out MsoIDCRL.LogonState state)
		{
			int authState = 0;
			int authRequired = 0;
			int requestStatus = 0;
			IntPtr zero = IntPtr.Zero;
			int authState2 = NativeMethods.GetAuthState(hIdentity, out authState, out authRequired, out requestStatus, out zero);
			if (authState2 < 0)
			{
				IDCRLException ex = new IDCRLException(authState2);
				throw ex;
			}
			state = default(MsoIDCRL.LogonState);
			state.AuthRequired = authRequired;
			state.AuthState = authState;
			state.RequestStatus = requestStatus;
			state.WebFlowUrl = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			this.InternalPassportFreeMemory(zero);
			return authState2;
		}

		internal int AuthIdentityToServiceEx(IntPtr hIdentity, uint serviceTokenFlags, MsoIDCRL.RSTParams[] rstParams, uint rstParamCount)
		{
			int num = NativeMethods.AuthIdentityToServiceEx(hIdentity, serviceTokenFlags, rstParams, rstParamCount);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GetCertificate(IntPtr hIdentity, MsoIDCRL.RSTParams pcRSTParams, uint dwMinTTL, uint certRequestFlags, out IntPtr certContext)
		{
			IntPtr zero = IntPtr.Zero;
			uint num = 0u;
			certContext = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			int certificate = NativeMethods.GetCertificate(hIdentity, ref pcRSTParams, ref dwMinTTL, certRequestFlags, out certContext, out zero, out num, out zero2);
			if (certificate < 0)
			{
				IDCRLException ex = new IDCRLException(certificate);
				throw ex;
			}
			return certificate;
		}

		internal int GetCertificate(IntPtr hIdentity, MsoIDCRL.RSTParams pcRSTParams, uint dwMinTTL, uint certRequestFlags, out X509Certificate cert)
		{
			IntPtr zero = IntPtr.Zero;
			uint num = 0u;
			IntPtr zero2 = IntPtr.Zero;
			IntPtr zero3 = IntPtr.Zero;
			int certificate = NativeMethods.GetCertificate(hIdentity, ref pcRSTParams, ref dwMinTTL, certRequestFlags, out zero2, out zero, out num, out zero3);
			if (certificate < 0)
			{
				IDCRLException ex = new IDCRLException(certificate);
				throw ex;
			}
			cert = new X509Certificate(zero2);
			return certificate;
		}

		internal int GetCertificate(IntPtr hIdentity, MsoIDCRL.RSTParams pcRSTParams, uint dwMinTTL, uint certRequestFlags, out IntPtr certContext, IntPtr popBlob, out uint popBlobSize, out IntPtr ppCACertContext)
		{
			popBlob = IntPtr.Zero;
			popBlobSize = 0u;
			certContext = IntPtr.Zero;
			ppCACertContext = IntPtr.Zero;
			int certificate = NativeMethods.GetCertificate(hIdentity, ref pcRSTParams, ref dwMinTTL, certRequestFlags, out certContext, out popBlob, out popBlobSize, out ppCACertContext);
			if (certificate < 0)
			{
				IDCRLException ex = new IDCRLException(certificate);
				throw ex;
			}
			return certificate;
		}

		internal int GetCertificate(IntPtr hIdentity, MsoIDCRL.RSTParams pcRSTParams, uint dwMinTTL, uint certRequestFlags, out MsoIDCRL.CertSet certSet)
		{
			IntPtr zero = IntPtr.Zero;
			uint popSize = 0u;
			IntPtr zero2 = IntPtr.Zero;
			IntPtr zero3 = IntPtr.Zero;
			int certificate = NativeMethods.GetCertificate(hIdentity, ref pcRSTParams, ref dwMinTTL, certRequestFlags, out zero2, out zero, out popSize, out zero3);
			if (certificate < 0)
			{
				IDCRLException ex = new IDCRLException(certificate);
				throw ex;
			}
			if (certificate == 297031)
			{
				certSet = default(MsoIDCRL.CertSet);
			}
			else
			{
				certSet = new MsoIDCRL.CertSet(zero2, zero, popSize, zero3);
			}
			return certificate;
		}

		internal int GetAssertion(IntPtr hIdentity, MsoIDCRL.RSTParams pcRSTParams, uint dwMinTTL, uint certRequestFlags, out MsoIDCRL.CertSet certSet)
		{
			IntPtr zero = IntPtr.Zero;
			uint popSize = 0u;
			IntPtr zero2 = IntPtr.Zero;
			IntPtr zero3 = IntPtr.Zero;
			int assertion = NativeMethods.GetAssertion(hIdentity, ref pcRSTParams, ref dwMinTTL, certRequestFlags, out zero2, out zero, out popSize, out zero3);
			if (assertion < 0)
			{
				IDCRLException ex = new IDCRLException(assertion);
				throw ex;
			}
			if (assertion == 297031)
			{
				certSet = default(MsoIDCRL.CertSet);
			}
			else
			{
				certSet = new MsoIDCRL.CertSet(zero2, zero, popSize, zero3);
			}
			return assertion;
		}

		internal int GetAuthStateEx(IntPtr hIdentity, string serviceTarget, out MsoIDCRL.LogonState state)
		{
			int authState = 0;
			int authRequired = 0;
			int requestStatus = 0;
			IntPtr zero = IntPtr.Zero;
			int authStateEx = NativeMethods.GetAuthStateEx(hIdentity, serviceTarget, out authState, out authRequired, out requestStatus, out zero);
			if (authStateEx < 0)
			{
				IDCRLException ex = new IDCRLException(authStateEx);
				throw ex;
			}
			state = default(MsoIDCRL.LogonState);
			state.AuthState = authState;
			state.AuthRequired = authRequired;
			state.RequestStatus = requestStatus;
			state.WebFlowUrl = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			this.InternalPassportFreeMemory(zero);
			return authStateEx;
		}

		internal int GetAuthenticationStatus(IntPtr hIdentity, string wzServiceTarget, uint dwVersion, out MsoIDCRL.IDCRL_STATUS_CURRENT currentStatus)
		{
			IntPtr zero = IntPtr.Zero;
			int authenticationStatus = NativeMethods.GetAuthenticationStatus(hIdentity, wzServiceTarget, dwVersion, out zero);
			if (authenticationStatus < 0)
			{
				IDCRLException ex = new IDCRLException(authenticationStatus);
				throw ex;
			}
			currentStatus = new MsoIDCRL.IDCRL_STATUS_CURRENT();
			Marshal.PtrToStructure(zero, currentStatus);
			this.InternalPassportFreeMemory(zero);
			return authenticationStatus;
		}

		internal int VerifyCertificate(IntPtr pcertContext, ref uint dwMinTTL, IntPtr popBlob, uint popSize, out IntPtr ppCACertContext)
		{
			ppCACertContext = IntPtr.Zero;
			int num = NativeMethods.VerifyCertificate(pcertContext, ref dwMinTTL, popBlob, popSize, out ppCACertContext);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int VerifyCertificate(IntPtr pcertContext, ref uint dwMinTTL, IntPtr popBlob, uint popSize, out X509Certificate ppCACert)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeMethods.VerifyCertificate(pcertContext, ref dwMinTTL, popBlob, popSize, out zero);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			ppCACert = new X509Certificate(zero);
			return num;
		}

		internal int VerifyCertificate(IntPtr pcertContext, ref uint dwMinTTL, out X509Certificate ppCACert)
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			uint cbPOP = 0u;
			int num = NativeMethods.VerifyCertificate(pcertContext, ref dwMinTTL, zero2, cbPOP, out zero);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			ppCACert = new X509Certificate(zero);
			return num;
		}

		internal int VerifyAssertion(IntPtr pcertContext, ref uint dwMinTTL, IntPtr popBlob, uint popSize, out IntPtr ppCACertContext)
		{
			ppCACertContext = IntPtr.Zero;
			int num = NativeMethods.VerifyAssertion(pcertContext, ref dwMinTTL, popBlob, popSize, out ppCACertContext);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int SetIdentityCallback(IntPtr hIdentity, MsoIDCRL.CallBackDelegateWithData CallbackFunction)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeMethods.SetIdentityCallback(hIdentity, CallbackFunction, zero);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int SetIdentityCallback(IntPtr hIdentity, MsoIDCRL.CallBackDelegateWithData CallbackFunction, IntPtr callBackData)
		{
			int num = NativeMethods.SetIdentityCallback(hIdentity, CallbackFunction, callBackData);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int SetChangeNotificationCallback(string virtualApp, uint flags, MsoIDCRL.UserStateChangedCallback callback)
		{
			int num = NativeMethods.SetChangeNotificationCallback(virtualApp, flags, callback);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int SetChangeNotificationCallback(MsoIDCRL.UserStateChangedCallback callback)
		{
			int num = NativeMethods.SetChangeNotificationCallback(string.Empty, 0u, callback);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int RemoveChangeNotificationCallback()
		{
			int num = NativeMethods.RemoveChangeNotificationCallback();
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int CancelPendingRequest(IntPtr hIdentity)
		{
			int num = NativeMethods.CancelPendingRequest(hIdentity);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int CloseIdentityHandle(IntPtr hIdentity)
		{
			int num = NativeMethods.CloseIdentityHandle(hIdentity);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int PersistCredential(IntPtr hIdentity, string credType)
		{
			int num = NativeMethods.PersistCredential(hIdentity, credType);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int EnumIdentitiesWithCachedCredentials(string credType, out long credHandle)
		{
			IntPtr zero = IntPtr.Zero;
			credHandle = 0L;
			int num = NativeMethods.EnumIdentitiesWithCachedCredentials(credType, out zero);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			credHandle = zero.ToInt64();
			return num;
		}

		internal string NextIdentity(int enumHandle)
		{
			IntPtr ptr = IntPtr.Zero;
			IntPtr enumHandle2 = new IntPtr(enumHandle);
			int num = NativeMethods.NextIdentity(enumHandle2, ref ptr);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return Marshal.PtrToStringUni(ptr);
		}

		internal int NextIdentity(int enumHandle, out string userName)
		{
			IntPtr ptr = IntPtr.Zero;
			IntPtr enumHandle2 = new IntPtr(enumHandle);
			int num = NativeMethods.NextIdentity(enumHandle2, ref ptr);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			userName = Marshal.PtrToStringUni(ptr);
			return num;
		}

		internal int CloseEnumIdentitiesHandle(int enumHandle)
		{
			IntPtr enumHandle2 = new IntPtr(enumHandle);
			int num = NativeMethods.CloseEnumIdentitiesHandle(enumHandle2);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int RemovePersistedCredential(IntPtr hIdentity, string credType)
		{
			int num = NativeMethods.RemovePersistedCredential(hIdentity, credType);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GetIdentityPropertyByName(IntPtr hIdentity, string PropertyName, out string strPropertyVal)
		{
			IntPtr zero = IntPtr.Zero;
			strPropertyVal = string.Empty;
			int identityPropertyByName = NativeMethods.GetIdentityPropertyByName(hIdentity, PropertyName, out zero);
			if (identityPropertyByName < 0)
			{
				IDCRLException ex = new IDCRLException(identityPropertyByName);
				throw ex;
			}
			strPropertyVal = Marshal.PtrToStringUni(zero);
			return identityPropertyByName;
		}

		internal string GetIdentityPropertyByName(IntPtr hIdentity, string PropertyName)
		{
			IntPtr zero = IntPtr.Zero;
			string empty = string.Empty;
			int identityPropertyByName = NativeMethods.GetIdentityPropertyByName(hIdentity, PropertyName, out zero);
			if (identityPropertyByName < 0)
			{
				IDCRLException ex = new IDCRLException(identityPropertyByName);
				throw ex;
			}
			return (zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero);
		}

		internal int GetIdentityProperty(IntPtr hIdentity, MsoIDCRL.PassportIdentityProperty PropertyName, out string strPropertyVal)
		{
			IntPtr zero = IntPtr.Zero;
			strPropertyVal = string.Empty;
			int identityProperty = NativeMethods.GetIdentityProperty(hIdentity, PropertyName, out zero);
			if (identityProperty < 0)
			{
				IDCRLException ex = new IDCRLException(identityProperty);
				throw ex;
			}
			strPropertyVal = Marshal.PtrToStringUni(zero);
			return identityProperty;
		}

		internal string GetIdentityProperty(IntPtr hIdentity, MsoIDCRL.PassportIdentityProperty PropertyName)
		{
			IntPtr zero = IntPtr.Zero;
			string result = string.Empty;
			int identityProperty = NativeMethods.GetIdentityProperty(hIdentity, PropertyName, out zero);
			if (identityProperty < 0)
			{
				IDCRLException ex = new IDCRLException(identityProperty);
				throw ex;
			}
			result = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			this.InternalPassportFreeMemory(zero);
			return result;
		}

		internal int GetPreferredAuthUIContextSize(IntPtr hIdentity, out Size size)
		{
			size = default(Size);
			int preferredAuthUIContextSize = NativeMethods.GetPreferredAuthUIContextSize(hIdentity, out size);
			if (preferredAuthUIContextSize < 0)
			{
				IDCRLException ex = new IDCRLException(preferredAuthUIContextSize);
				throw ex;
			}
			return preferredAuthUIContextSize;
		}

		internal int CreatePassportAuthUIContext(ref MsoIDCRL.PassportCredUIInfo credUIInfo, ref MsoIDCRL.PassportCredCustomUI credUICustomUI, out long authUIContextHandle)
		{
			IntPtr zero = IntPtr.Zero;
			authUIContextHandle = 0L;
			int num = NativeMethods.CreatePassportAuthUIContext(ref credUIInfo, ref credUICustomUI, out zero);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			authUIContextHandle = zero.ToInt64();
			return num;
		}

		internal int DestroyPassportAuthUIContext(int authUIContextHandle)
		{
			IntPtr authContext = new IntPtr(authUIContextHandle);
			int num = NativeMethods.DestroyPassportAuthUIContext(authContext);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int LogonIdentityWithUI(int authContext, IntPtr hIdentity, string policy, uint logonFlags)
		{
			IntPtr authContext2 = new IntPtr(authContext);
			int num = NativeMethods.LogonIdentityWithUI(authContext2, hIdentity, policy, logonFlags);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int MoveAuthUIContext(int authContext, Point position, Size newSize)
		{
			IntPtr authContext2 = new IntPtr(authContext);
			int num = NativeMethods.MoveAuthUIContext(authContext2, position, newSize);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GetWebAuthUrl(IntPtr hIdentity, string targetService, string policy, string additionalParams, string sourceService, out MsoIDCRL.MD5Data md5data)
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			md5data = default(MsoIDCRL.MD5Data);
			int webAuthUrl = NativeMethods.GetWebAuthUrl(hIdentity, targetService, policy, additionalParams, sourceService, out zero, out zero2);
			if (webAuthUrl < 0)
			{
				IDCRLException ex = new IDCRLException(webAuthUrl);
				throw ex;
			}
			if (zero != IntPtr.Zero && zero2 != IntPtr.Zero)
			{
				md5data.md5Url = Marshal.PtrToStringUni(zero);
				md5data.postData = Marshal.PtrToStringUni(zero2);
			}
			this.InternalPassportFreeMemory(zero);
			this.InternalPassportFreeMemory(zero2);
			return webAuthUrl;
		}

		internal bool HasPersistedCredentials(IntPtr hIdentity, string CredType)
		{
			long num = 0L;
			int num2 = NativeMethods.HasPersistedCredential(hIdentity, CredType, out num);
			if (num2 != 0)
			{
				IDCRLException ex = new IDCRLException(num2);
				throw ex;
			}
			return num == 1L;
		}

		internal int PassportFreeMemory(IntPtr buffer)
		{
			int num = NativeMethods.PassportFreeMemory(buffer);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int SetIdentityProperty(IntPtr hIdentity, MsoIDCRL.PassportIdentityProperty iProperty, string propertyValue)
		{
			int num = NativeMethods.SetIdentityProperty(hIdentity, iProperty, propertyValue);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GetExtendedError(IntPtr hIdentity, out MsoIDCRL.IDCRL_ERROR_CATEGORY category, out uint errorCode, out string errorBlob)
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			uint num = 0u;
			int extendedError = NativeMethods.GetExtendedError(hIdentity, zero, out num, out errorCode, out zero2);
			if (extendedError < 0)
			{
				IDCRLException ex = new IDCRLException(extendedError);
				throw ex;
			}
			errorBlob = ((zero2 == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero2));
			switch (num)
			{
			case 1u:
				category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_REQUEST_BUILD_ERROR;
				break;
			case 2u:
				category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_REQUEST_SEND_ERROR;
				break;
			case 3u:
				category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_RECEIVE_ERROR;
				break;
			case 4u:
				category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_READ_ERROR;
				break;
			case 5u:
				category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_REPSONSE_PARSE_ERROR;
				break;
			case 6u:
				category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_SIG_DECRYPT_ERROR;
				break;
			case 7u:
				category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_PARSE_HEADER_ERROR;
				break;
			case 8u:
				category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_PARSE_TOKEN_ERROR;
				break;
			case 9u:
				category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_PUTCERT_ERROR;
				break;
			default:
				category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_RECEIVE_ERROR;
				break;
			}
			this.InternalPassportFreeMemory(zero2);
			return extendedError;
		}

		internal int GetExtendedError(IntPtr hIdentity, out MsoIDCRL.ExtendedError extentedError)
		{
			int num = 0;
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			uint num2 = 0u;
			uint errorCode = 0u;
			extentedError = default(MsoIDCRL.ExtendedError);
			try
			{
				num = NativeMethods.GetExtendedError(hIdentity, zero, out num2, out errorCode, out zero2);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			if (num < 0)
			{
				IDCRLException ex2 = new IDCRLException(num);
				throw ex2;
			}
			extentedError.errorBlob = ((zero2 == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero2));
			extentedError.errorCode = errorCode;
			switch (num2)
			{
			case 1u:
				extentedError.category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_REQUEST_BUILD_ERROR;
				break;
			case 2u:
				extentedError.category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_REQUEST_SEND_ERROR;
				break;
			case 3u:
				extentedError.category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_RECEIVE_ERROR;
				break;
			case 4u:
				extentedError.category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_READ_ERROR;
				break;
			case 5u:
				extentedError.category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_REPSONSE_PARSE_ERROR;
				break;
			case 6u:
				extentedError.category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_SIG_DECRYPT_ERROR;
				break;
			case 7u:
				extentedError.category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_PARSE_HEADER_ERROR;
				break;
			case 8u:
				extentedError.category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_PARSE_TOKEN_ERROR;
				break;
			case 9u:
				extentedError.category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_PUTCERT_ERROR;
				break;
			default:
				extentedError.category = MsoIDCRL.IDCRL_ERROR_CATEGORY.IDCRL_RESPONSE_RECEIVE_ERROR;
				break;
			}
			return num;
		}

		private void InternalPassportFreeMemory(IntPtr o)
		{
			int num = NativeMethods.PassportFreeMemory(o);
			StackTrace stackTrace = new StackTrace();
			string name = stackTrace.GetFrame(1).GetMethod().Name;
			if (num < 0)
			{
				string message = "PassportFreeMemory Called from " + name + " threw an Exception " + num.ToString("X", CultureInfo.InvariantCulture);
				IDCRLException ex = new IDCRLException(num, message, null);
				throw ex;
			}
		}

		internal IntPtr ConvertStrToPtr(string str)
		{
			IntPtr intPtr = IntPtr.Zero;
			GCHandle gCHandle = default(GCHandle);
			byte[] bytes = Encoding.Unicode.GetBytes(str);
			byte[] value = new byte[bytes.Length];
			intPtr = GCHandle.Alloc(value, GCHandleType.Pinned).AddrOfPinnedObject();
			Marshal.Copy(bytes, 0, intPtr, bytes.Length);
			return intPtr;
		}

		internal int CacheAuthState(IntPtr hIdentity, string virtualAppName)
		{
			int num = NativeMethods.CacheAuthState(hIdentity, virtualAppName, 0u);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int CacheAuthState(IntPtr hIdentity, string virtualAppName, uint flags)
		{
			int num = NativeMethods.CacheAuthState(hIdentity, virtualAppName, flags);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int RemoveAuthStateFromCache(string username, string virtualAppName, uint flags)
		{
			int num = NativeMethods.RemoveAuthStateFromCache(username, virtualAppName, flags);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int RemoveAuthStateFromCache(string username, string virtualAppName)
		{
			int num = NativeMethods.RemoveAuthStateFromCache(username, virtualAppName, 0u);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int CreateIdentityHandleFromCachedAuthState(string username, string virtualAppName, uint flags, out IntPtr hIdentity)
		{
			hIdentity = IntPtr.Zero;
			int num = NativeMethods.CreateIdentityHandleFromCachedAuthState(username, virtualAppName, flags, out hIdentity);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int CreateIdentityHandleFromCachedAuthState(string virtualAppName, uint flags, out IntPtr hIdentity)
		{
			hIdentity = IntPtr.Zero;
			int num = NativeMethods.CreateIdentityHandleFromCachedAuthState(virtualAppName, flags, out hIdentity);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int CreateIdentityHandleFromAuthState(string authToken, uint flags, out IntPtr hIdentity)
		{
			hIdentity = IntPtr.Zero;
			int num = NativeMethods.CreateIdentityHandleFromAuthState(authToken, flags, out hIdentity);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GetWebAuthUrlEx(IntPtr hIdentity, MsoIDCRL.WebAuthOptions authType, string targetService, string policy, string additionalParams, out MsoIDCRL.SHA1 sha1Data)
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			sha1Data = default(MsoIDCRL.SHA1);
			int webAuthUrlEx = NativeMethods.GetWebAuthUrlEx(hIdentity, (uint)authType, targetService, policy, additionalParams, out zero, out zero2);
			if (webAuthUrlEx < 0)
			{
				IDCRLException ex = new IDCRLException(webAuthUrlEx);
				throw ex;
			}
			if (zero != IntPtr.Zero && zero2 != IntPtr.Zero)
			{
				sha1Data.sha1Url = Marshal.PtrToStringUni(zero);
				sha1Data.sha1PostData = Marshal.PtrToStringUni(zero2);
			}
			this.InternalPassportFreeMemory(zero);
			this.InternalPassportFreeMemory(zero2);
			return webAuthUrlEx;
		}

		internal int ExportAuthState(IntPtr hIdentity, uint flags, out string authToken)
		{
			authToken = string.Empty;
			IntPtr zero = IntPtr.Zero;
			int num = NativeMethods.ExportAuthState(hIdentity, flags, out zero);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			authToken = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			return num;
		}

		internal int ExportAuthState(IntPtr hIdentity, out string authToken)
		{
			authToken = string.Empty;
			IntPtr zero = IntPtr.Zero;
			int num = NativeMethods.ExportAuthState(hIdentity, 0u, out zero);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			authToken = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			this.InternalPassportFreeMemory(zero);
			return num;
		}

		internal string GetExtendedProperty(string propertyName)
		{
			IntPtr zero = IntPtr.Zero;
			string result = string.Empty;
			int extendedProperty = NativeMethods.GetExtendedProperty(propertyName, out zero);
			if (extendedProperty < 0)
			{
				IDCRLException ex = new IDCRLException(extendedProperty);
				throw ex;
			}
			result = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			this.InternalPassportFreeMemory(zero);
			return result;
		}

		internal int SetExtendedProperty(string propertyName, string propertyValue)
		{
			int num = NativeMethods.SetExtendedProperty(propertyName, propertyValue);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal string GetUserExtendedProperty(string userName, string property)
		{
			IntPtr zero = IntPtr.Zero;
			string result = string.Empty;
			int userExtendedProperty = NativeMethods.GetUserExtendedProperty(userName, property, out zero);
			if (userExtendedProperty < 0)
			{
				IDCRLException ex = new IDCRLException(userExtendedProperty);
				throw ex;
			}
			result = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			this.InternalPassportFreeMemory(zero);
			return result;
		}

		internal int SetUserExtendedProperty(string username, string property, string propertyValue)
		{
			int num = NativeMethods.SetUserExtendedProperty(username, property, propertyValue);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal string GetServiceConfig(string valueName)
		{
			IntPtr zero = IntPtr.Zero;
			string result = string.Empty;
			int serviceConfig = NativeMethods.GetServiceConfig(valueName, out zero);
			if (serviceConfig < 0)
			{
				IDCRLException ex = new IDCRLException(serviceConfig);
				throw ex;
			}
			result = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			this.InternalPassportFreeMemory(zero);
			return result;
		}

		internal int EncryptWithSessionKey(IntPtr hIdentity, string serviceName, uint algIdEncrypt, uint algIdHash, string data, out IntPtr cipherPtr, out uint cipherSize)
		{
			IntPtr intPtr = IntPtr.Zero;
			GCHandle gCHandle = default(GCHandle);
			int length = data.Length;
			byte[] value = new byte[data.Length];
			intPtr = GCHandle.Alloc(value, GCHandleType.Pinned).AddrOfPinnedObject();
			ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
			byte[] bytes = aSCIIEncoding.GetBytes(data);
			Marshal.Copy(bytes, 0, intPtr, length);
			cipherPtr = IntPtr.Zero;
			int num = NativeMethods.EncryptWithSessionKey(hIdentity, serviceName, algIdEncrypt, algIdHash, intPtr, (uint)length, out cipherPtr, out cipherSize);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int DecryptWithSessionKey(IntPtr hIdentity, string serviceName, uint algIdEncrypt, uint algIdHash, IntPtr cipherPtr, uint cipherSize, out string data, out uint dataSize)
		{
			IntPtr zero = IntPtr.Zero;
			dataSize = 0u;
			int num = NativeMethods.DecryptWithSessionKey(hIdentity, serviceName, algIdEncrypt, algIdHash, cipherPtr, cipherSize, out zero, out dataSize);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			byte[] array = new byte[dataSize];
			Marshal.Copy(zero, array, 0, (int)dataSize);
			ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
			data = aSCIIEncoding.GetString(array);
			this.InternalPassportFreeMemory(zero);
			return num;
		}

		internal int MigratePersistedCredentials(ref Guid appGuid, bool keepOldCreds, out uint migratedUserCount)
		{
			migratedUserCount = 0u;
			int num = NativeMethods.MigratePersistedCredentials(ref appGuid, keepOldCreds, out migratedUserCount);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GetDeviceID(int dwFlags, string additionalParams, out string strDeviceId, out X509Certificate2 didCert)
		{
			didCert = null;
			IntPtr zero = IntPtr.Zero;
			strDeviceId = string.Empty;
			IntPtr zero2 = IntPtr.Zero;
			int deviceId = NativeMethods.GetDeviceId(dwFlags, additionalParams, out zero, out zero2);
			if (deviceId == 0)
			{
				strDeviceId = Marshal.PtrToStringUni(zero);
				didCert = new X509Certificate2(zero2);
			}
			return deviceId;
		}

		internal int GetDeviceID(int dwFlags, string additionalParams, out string strDeviceId, out MsoIDCRL.CertSet certSet)
		{
			certSet = default(MsoIDCRL.CertSet);
			IntPtr zero = IntPtr.Zero;
			strDeviceId = string.Empty;
			IntPtr zero2 = IntPtr.Zero;
			int deviceId = NativeMethods.GetDeviceId(dwFlags, additionalParams, out zero, out zero2);
			if (deviceId < 0)
			{
				IDCRLException ex = new IDCRLException(deviceId);
				throw ex;
			}
			strDeviceId = Marshal.PtrToStringUni(zero);
			this.InternalPassportFreeMemory(zero);
			if (deviceId == 297031)
			{
				certSet = default(MsoIDCRL.CertSet);
			}
			else
			{
				certSet = new MsoIDCRL.CertSet(zero2, IntPtr.Zero, 0u, IntPtr.Zero);
			}
			return deviceId;
		}

		internal int SetDeviceConsent(int dwFlags, int dwConsentSetting, string additionalParams)
		{
			return NativeMethods.SetDeviceConsent(dwFlags, dwConsentSetting, additionalParams);
		}

		internal int SetDeviceConsentExcpt(int dwFlags, int dwConsentSetting, string additionalParams)
		{
			int num = NativeMethods.SetDeviceConsent(dwFlags, dwConsentSetting, additionalParams);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int CloseDeviceID(int dwFlags, string additionalParams)
		{
			return NativeMethods.CloseDeviceID(dwFlags, additionalParams);
		}

		internal int CloseDeviceIDExcpt(int dwFlags, string additionalParams)
		{
			int num = NativeMethods.CloseDeviceID(dwFlags, additionalParams);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GenerateDeviceToken(int dwFlags, string additionalParams, string audience, out string strDeviceToken)
		{
			IntPtr zero = IntPtr.Zero;
			strDeviceToken = string.Empty;
			int num = NativeMethods.GenerateDeviceToken(dwFlags, additionalParams, audience, out zero);
			if (num == 0)
			{
				strDeviceToken = Marshal.PtrToStringUni(zero);
				this.InternalPassportFreeMemory(zero);
			}
			return num;
		}

		internal int GenerateDeviceTokenExcpt(int dwFlags, string additionalParams, string audience, out string strDeviceToken)
		{
			IntPtr zero = IntPtr.Zero;
			strDeviceToken = string.Empty;
			int num = NativeMethods.GenerateDeviceToken(dwFlags, additionalParams, audience, out zero);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			if (num == 0)
			{
				strDeviceToken = Marshal.PtrToStringUni(zero);
				this.InternalPassportFreeMemory(zero);
			}
			return num;
		}

		internal int GenerateCertToken(IntPtr hIdentity, uint dwFlags, ref string strCertToken)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeMethods.GenerateCertToken(hIdentity, dwFlags, out zero);
			if (num == 0)
			{
				strCertToken = Marshal.PtrToStringUni(zero);
			}
			return num;
		}

		internal int EnumerateCertificates(uint dwFlags, string prgwszCSPs, string prgwszThumbprints, ref MsoIDCRL.IDCRLCertInfo[] idcrlCertInfo)
		{
			IntPtr ptr = IntPtr.Zero;
			uint num = 0u;
			int result = NativeMethods.EnumerateCertificates(dwFlags, prgwszCSPs, prgwszThumbprints, out num, out ptr);
			idcrlCertInfo = new MsoIDCRL.IDCRLCertInfo[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				idcrlCertInfo[num2] = (MsoIDCRL.IDCRLCertInfo)Marshal.PtrToStructure(ptr, typeof(MsoIDCRL.IDCRLCertInfo));
				string str = new string(idcrlCertInfo[num2].description, 0, new string(idcrlCertInfo[num2].description).IndexOf('\0'));
				string str2 = new string(idcrlCertInfo[num2].thumbprint, 0, new string(idcrlCertInfo[num2].thumbprint).IndexOf('\0'));
				Console.WriteLine("\nDESC: " + str);
				Console.WriteLine("\nTP: " + str2);
				ptr = new IntPtr(ptr.ToInt64() + (long)Marshal.SizeOf(idcrlCertInfo[num2]));
				num2++;
			}
			return result;
		}

		internal int EnumerateCertificates(uint dwFlags, string prgwszCSPs, string prgwszThumbprints, out MsoIDCRL.CertificateInfo[] certInfoList)
		{
			IntPtr ptr = IntPtr.Zero;
			uint num = 0u;
			int result = NativeMethods.EnumerateCertificates(dwFlags, prgwszCSPs, prgwszThumbprints, out num, out ptr);
			MsoIDCRL.IDCRLCertInfo[] array = new MsoIDCRL.IDCRLCertInfo[num];
			certInfoList = new MsoIDCRL.CertificateInfo[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = (MsoIDCRL.IDCRLCertInfo)Marshal.PtrToStructure(ptr, typeof(MsoIDCRL.IDCRLCertInfo));
				certInfoList[num2].description = new string(array[num2].description, 0, new string(array[num2].description).IndexOf('\0'));
				certInfoList[num2].thumbprint = new string(array[num2].thumbprint, 0, new string(array[num2].thumbprint).IndexOf('\0'));
				ptr = new IntPtr(ptr.ToInt64() + (long)Marshal.SizeOf(array[num2]));
				num2++;
			}
			return result;
		}

		internal int EnumerateDeviceID(int dwFlags, string additionalParams, out string[] deviceIDs, out MsoIDCRL.CertSet[] certSet)
		{
			uint num = 0u;
			IntPtr source;
			IntPtr source2;
			int num2 = NativeMethods.EnumerateDeviceID(dwFlags, additionalParams, out num, out source, out source2);
			if (num2 < 0)
			{
				IDCRLException ex = new IDCRLException(num2);
				throw ex;
			}
			IntPtr[] array = new IntPtr[num];
			IntPtr[] array2 = new IntPtr[num];
			if (num > 0u)
			{
				Marshal.Copy(source, array, 0, (int)num);
				Marshal.Copy(source2, array2, 0, (int)num);
			}
			deviceIDs = new string[num];
			certSet = new MsoIDCRL.CertSet[num];
			int num3 = 0;
			while ((long)num3 < (long)((ulong)num))
			{
				certSet[num3] = new MsoIDCRL.CertSet(array2[num3], IntPtr.Zero, 0u, IntPtr.Zero);
				deviceIDs[num3] = Marshal.PtrToStringUni(array[num3]);
				num3++;
			}
			return num2;
		}

		internal int CreateLinkedIdentityHandle(IntPtr hIdentity, uint dwFlags, string wszMemberName, out long linkedIdentity)
		{
			IntPtr zero = IntPtr.Zero;
			linkedIdentity = 0L;
			int num = NativeMethods.CreateLinkedIdentityHandle(hIdentity, dwFlags, wszMemberName, out zero);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			linkedIdentity = zero.ToInt64();
			return num;
		}

		internal int SaveTweenerCreds(string strUserName, string strPwd)
		{
			IntPtr intPtr = IntPtr.Zero;
			uint dwLen = (uint)(strPwd.Length * 2);
			intPtr = Marshal.StringToCoTaskMemUni(strPwd);
			int num = NativeMethods.SaveTweenerCreds(strUserName, intPtr, dwLen);
			Marshal.FreeCoTaskMem(intPtr);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int DeleteTweenerCreds(string strUserName)
		{
			int num = NativeMethods.DeleteTweenerCreds(strUserName);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int OpenAuthenticatedBrowser(IntPtr hIdentity, MsoIDCRL.WebAuthOptions authType, string targetService, string policy, string additionalParams)
		{
			int num = NativeMethods.OpenAuthenticatedBrowser(hIdentity, (uint)authType, targetService, policy, additionalParams);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int InitializeIDCRLTraceBuffer(uint dwMaxBufferSizeBytes)
		{
			int num = NativeMethods.InitializeIDCRLTraceBuffer(dwMaxBufferSizeBytes, null, 0u);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int FlushIDCRLTraceBuffer(string wszFileLocation, out string pwszTraceBuf)
		{
			pwszTraceBuf = string.Empty;
			IntPtr zero = IntPtr.Zero;
			int num = NativeMethods.FlushIDCRLTraceBuffer(wszFileLocation, null, 0u, out zero);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			pwszTraceBuf = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			this.InternalPassportFreeMemory(zero);
			return num;
		}

		internal int LogonIdentityExWithUI(IntPtr hIdentity, string authPolicy, uint logonFlags, uint ssoFlags, MsoIDCRL.UIParam pcUIParam, MsoIDCRL.RSTParams[] rstParam, uint paramCount)
		{
			int num = NativeMethods.LogonIdentityExWithUI(hIdentity, authPolicy, logonFlags, ssoFlags, pcUIParam, rstParam, paramCount);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GetResponseForHttpChallenge(IntPtr hIdentity, uint logonFlags, uint ssoFlags, MsoIDCRL.UIParam pcUIParam, string wszServiceTarget, string wszChallenge, out string pwszResponse)
		{
			pwszResponse = string.Empty;
			IntPtr zero = IntPtr.Zero;
			int responseForHttpChallenge = NativeMethods.GetResponseForHttpChallenge(hIdentity, logonFlags, ssoFlags, pcUIParam, wszServiceTarget, wszChallenge, out zero);
			if (responseForHttpChallenge < 0)
			{
				IDCRLException ex = new IDCRLException(responseForHttpChallenge);
				throw ex;
			}
			pwszResponse = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			return responseForHttpChallenge;
		}

		internal int SetDefaultUserForTarget(IntPtr hIdentity, uint dwFlags, string wszServiceTarget)
		{
			int num = NativeMethods.SetDefaultUserForTarget(hIdentity, dwFlags, wszServiceTarget);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GetDefaultUserForTarget(uint dwFlags, string wszServiceTarget, out string pwszMemberName)
		{
			pwszMemberName = string.Empty;
			IntPtr zero = IntPtr.Zero;
			int defaultUserForTarget = NativeMethods.GetDefaultUserForTarget(dwFlags, wszServiceTarget, out zero);
			if (defaultUserForTarget < 0)
			{
				IDCRLException ex = new IDCRLException(defaultUserForTarget);
				throw ex;
			}
			pwszMemberName = ((zero == IntPtr.Zero) ? string.Empty : Marshal.PtrToStringUni(zero));
			return defaultUserForTarget;
		}

		internal int AssociateDeviceToUser(IntPtr hIdentity, string pwszFriendlyName, uint dwAssocType)
		{
			int num = NativeMethods.AssociateDeviceToUser(hIdentity, pwszFriendlyName, dwAssocType);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int DisassociateDeviceFromUser(IntPtr hIdentity, uint dwAssocType)
		{
			int num = NativeMethods.DisassociateDeviceFromUser(hIdentity, dwAssocType);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int EnumerateUserAssociatedDevices(IntPtr hIdentity, uint dwAssocType, string pwzOwnerUserName, out uint pdwCount, out string[] paNames, out string[] paFriendlyNames)
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			int num = NativeMethods.EnumerateUserAssociatedDevices(hIdentity, dwAssocType, pwzOwnerUserName, out pdwCount, out zero, out zero2);
			IntPtr[] array = new IntPtr[pdwCount];
			IntPtr[] array2 = new IntPtr[pdwCount];
			if (array.Length > 0)
			{
				Marshal.Copy(zero, array, 0, (int)pdwCount);
			}
			if (array2.Length > 0)
			{
				Marshal.Copy(zero2, array2, 0, (int)pdwCount);
			}
			paNames = new string[pdwCount];
			paFriendlyNames = new string[pdwCount];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)pdwCount))
			{
				paNames[num2] = Marshal.PtrToStringUni(array[num2]);
				paFriendlyNames[num2] = Marshal.PtrToStringUni(array2[num2]);
				num2++;
			}
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int UpdateUserAssociatedDeviceProperties(IntPtr hIdentity, uint dwAssocType, uint pdwCount, string[] paPropertyNames, string[] paPropertyValues)
		{
			int num = NativeMethods.UpdateUserAssociatedDeviceProperties(hIdentity, dwAssocType, pdwCount, paPropertyNames, paPropertyValues);
			if (num < 0)
			{
				IDCRLException ex = new IDCRLException(num);
				throw ex;
			}
			return num;
		}

		internal int GetDeviceShortLivedToken(out string ppszDIDDAtoken)
		{
			ppszDIDDAtoken = string.Empty;
			IntPtr zero = IntPtr.Zero;
			int deviceShortLivedToken = NativeMethods.GetDeviceShortLivedToken(out zero);
			if (deviceShortLivedToken == 0)
			{
				ppszDIDDAtoken = Marshal.PtrToStringUni(zero);
			}
			return deviceShortLivedToken;
		}

		internal int ProvisionDeviceId(uint dwDeviceType, uint dwFlags)
		{
			return NativeMethods.ProvisionDeviceId(dwDeviceType, dwFlags);
		}

		internal int DeProvisionDeviceId(uint dwDeviceType, uint dwFlags)
		{
			return NativeMethods.DeProvisionDeviceId(dwDeviceType, dwFlags);
		}

		internal int RenewDeviceId(uint dwDeviceType, uint dwFlags)
		{
			return NativeMethods.RenewDeviceId(dwDeviceType, dwFlags);
		}

		internal int GetDeviceIdEx(uint dwDeviceType, uint dwFlags, string additionalParams, out string strDeviceId, out MsoIDCRL.CertSet certSet)
		{
			certSet = default(MsoIDCRL.CertSet);
			IntPtr zero = IntPtr.Zero;
			strDeviceId = string.Empty;
			IntPtr zero2 = IntPtr.Zero;
			int deviceIdEx = NativeMethods.GetDeviceIdEx(dwDeviceType, dwFlags, additionalParams, out zero, out zero2);
			if (deviceIdEx < 0)
			{
				return deviceIdEx;
			}
			strDeviceId = Marshal.PtrToStringUni(zero);
			this.InternalPassportFreeMemory(zero);
			if (deviceIdEx == 297031)
			{
				certSet = default(MsoIDCRL.CertSet);
			}
			else
			{
				certSet = new MsoIDCRL.CertSet(zero2, IntPtr.Zero, 0u, IntPtr.Zero);
			}
			return deviceIdEx;
		}
	}
}
