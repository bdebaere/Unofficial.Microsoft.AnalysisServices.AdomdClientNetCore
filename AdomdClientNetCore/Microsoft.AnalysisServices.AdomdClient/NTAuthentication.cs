using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class NTAuthentication
	{
		private const int ERROR_BUFFER_OVERFLOW = 111;

		private const int ERROR_INVALID_PARAMETER = 87;

		private const string ServiceClassName = "MSOLAPSvc.3";

		private const string SqlBrowserClassName = "MSOLAPDisco.3";

		private static SecurityPackageInfoClass[] m_SupportedSecurityPackages = SSPIWrapper.EnumerateSecurityPackages();

		private CredentialsContext m_CredentialsHandle;

		private SecurityContext m_SecurityContext;

		private Endianness m_Endianness;

		private string m_RemotePeerId;

		private int m_TokenSize;

		private int m_Capabilities;

		private int m_ContextFlags;

		private ContextFlags m_RequestedFlags;

		private bool m_isSchannel;

		internal SecurityContext SecurityContext
		{
			get
			{
				return this.m_SecurityContext;
			}
		}

		public bool IsSchannel
		{
			get
			{
				return this.m_isSchannel;
			}
		}

		internal NTAuthentication(ConnectionInfo connectionInfo, SecurityContextMode securityContextMode)
		{
			this.m_isSchannel = connectionInfo.IsSchannelSspi();
			this.InitializeSPN(connectionInfo);
			this.InitializePackageSpecificMembers(connectionInfo.Sspi);
			this.InitializeFlags(connectionInfo, securityContextMode);
			this.InitializeCredentialsHandle(connectionInfo);
			this.m_Endianness = Endianness.Network;
			this.m_SecurityContext = new SecurityContext(securityContextMode);
		}

		private void InitializeSPN(ConnectionInfo connectionInfo)
		{
			if (this.m_isSchannel)
			{
				this.m_RemotePeerId = connectionInfo.Server;
				return;
			}
			if (connectionInfo.InstanceName != null)
			{
				string server = connectionInfo.Server;
				if (string.Compare(server, "localhost", StringComparison.OrdinalIgnoreCase) == 0 || connectionInfo.IsServerLocal || string.Compare(server, Environment.MachineName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.m_RemotePeerId = null;
					return;
				}
				this.m_RemotePeerId = "MSOLAPSvc.3/" + server + ":" + connectionInfo.InstanceName;
				return;
			}
			else
			{
				string strName = connectionInfo.IsServerLocal ? Environment.MachineName : connectionInfo.Server;
				uint num = 0u;
				string strClass;
				ushort iPort;
				if (connectionInfo.IsForSqlBrowser)
				{
					strClass = "MSOLAPDisco.3";
					iPort = 0;
				}
				else
				{
					strClass = "MSOLAPSvc.3";
					iPort = (ushort) ((connectionInfo.Port == null) ? 0 : ushort.Parse(connectionInfo.Port, CultureInfo.InvariantCulture));
				}
				int num2 = UnsafeNclNativeMethods.NativeNTSSPI.DsMakeSpnW(strClass, strName, null, iPort, null, ref num, null);
				int num3 = num2;
				if (num3 == 87)
				{
					this.m_RemotePeerId = string.Empty;
					return;
				}
				if (num3 != 111)
				{
					throw new Win32Exception(num2);
				}
				StringBuilder stringBuilder = new StringBuilder((int)num, (int)num);
				num2 = UnsafeNclNativeMethods.NativeNTSSPI.DsMakeSpnW(strClass, strName, null, iPort, null, ref num, stringBuilder);
				if (num2 != 0)
				{
					throw new Win32Exception(num2);
				}
				this.m_RemotePeerId = stringBuilder.ToString();
				return;
			}
		}

		private void InitializePackageSpecificMembers(string packageName)
		{
			if (NTAuthentication.m_SupportedSecurityPackages != null)
			{
				int i = 0;
				int num = NTAuthentication.m_SupportedSecurityPackages.Length;
				while (i < num)
				{
					if (string.Compare(NTAuthentication.m_SupportedSecurityPackages[i].Name, packageName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.m_TokenSize = NTAuthentication.m_SupportedSecurityPackages[i].MaxToken;
						this.m_Capabilities = NTAuthentication.m_SupportedSecurityPackages[i].Capabilities;
						return;
					}
					i++;
				}
			}
			throw new AdomdConnectionException(XmlaSR.Authentication_Sspi_PackageNotFound(packageName), null, ConnectionExceptionCause.AuthenticationFailed);
		}

		private void InitializeFlags(ConnectionInfo connectionInfo, SecurityContextMode securityContextMode)
		{
			string sspi = connectionInfo.Sspi;
			ImpersonationLevel impersonationLevel = connectionInfo.ImpersonationLevel;
			if (this.m_isSchannel)
			{
				switch (securityContextMode)
				{
				case SecurityContextMode.block:
					this.CheckIfCapabilityIsSupported(sspi, PackageCapabilities.Connection);
					this.m_RequestedFlags = ContextFlags.Connection;
					break;
				case SecurityContextMode.stream:
					this.CheckIfCapabilityIsSupported(sspi, PackageCapabilities.Stream);
					this.m_RequestedFlags = ContextFlags.Stream;
					break;
				default:
					throw new AdomdConnectionException("Unsupported TLS mode " + securityContextMode.ToString(), null, ConnectionExceptionCause.AuthenticationFailed);
				}
				switch (impersonationLevel)
				{
				case ImpersonationLevel.Anonymous:
				case ImpersonationLevel.Identify:
				case ImpersonationLevel.Impersonate:
					switch (connectionInfo.ProtectionLevel)
					{
					case ProtectionLevel.None:
					case ProtectionLevel.Connection:
					case ProtectionLevel.Integrity:
						throw new AdomdConnectionException(XmlaSR.Authentication_Sspi_SchannelSupportsOnlyPrivacyLevel, null, ConnectionExceptionCause.AuthenticationFailed);
					case ProtectionLevel.Privacy:
						this.m_RequestedFlags |= (ContextFlags.ReplayDetect | ContextFlags.SequenceDetect | ContextFlags.Confidentiality | ContextFlags.UseSuppliedCreds | ContextFlags.Integrity);
						return;
					default:
						throw new AdomdConnectionException(XmlaSR.Authentication_Sspi_SchannelUnsupportedProtectionLevel, null, ConnectionExceptionCause.AuthenticationFailed);
					}
					
				case ImpersonationLevel.Delegate:
					throw new AdomdConnectionException(XmlaSR.Authentication_Sspi_SchannelCantDelegate, null, ConnectionExceptionCause.AuthenticationFailed);
				default:
					throw new AdomdConnectionException(XmlaSR.Authentication_Sspi_SchannelUnsupportedImpersonationLevel, null, ConnectionExceptionCause.AuthenticationFailed);
				}
			}
			else
			{
				this.m_RequestedFlags = (ContextFlags.MutualAuth | ContextFlags.Connection);
				switch (impersonationLevel)
				{
				case ImpersonationLevel.Identify:
					this.m_RequestedFlags |= ContextFlags.Identify;
					break;
				case ImpersonationLevel.Impersonate:
					this.CheckIfCapabilityIsSupported(sspi, PackageCapabilities.Impersonation);
					break;
				case ImpersonationLevel.Delegate:
					this.m_RequestedFlags |= ContextFlags.Delegate;
					break;
				}
				switch (connectionInfo.ProtectionLevel)
				{
				case ProtectionLevel.Connection:
					this.m_RequestedFlags |= ContextFlags.NoIntegrity;
					return;
				case ProtectionLevel.Integrity:
					this.CheckIfCapabilityIsSupported(sspi, PackageCapabilities.Integrity);
					this.m_RequestedFlags |= (ContextFlags.ReplayDetect | ContextFlags.SequenceDetect | ContextFlags.Integrity);
					return;
				case ProtectionLevel.Privacy:
					this.CheckIfCapabilityIsSupported(sspi, PackageCapabilities.Privacy);
					this.m_RequestedFlags |= (ContextFlags.ReplayDetect | ContextFlags.SequenceDetect | ContextFlags.Confidentiality | ContextFlags.Integrity);
					return;
				default:
					return;
				}
			}
		}

		private void InitializeCredentialsHandle(ConnectionInfo connectionInfo)
		{
			string sspi = connectionInfo.Sspi;
			if (connectionInfo.ImpersonationLevel == ImpersonationLevel.Anonymous)
			{
				if (this.m_isSchannel)
				{
					if (string.IsNullOrEmpty(connectionInfo.Certificate))
					{
						this.m_CredentialsHandle = new CredentialsContext(null);
						return;
					}
					throw new AdomdConnectionException(XmlaSR.Authentication_Sspi_SchannelAnonymousAmbiguity, null, ConnectionExceptionCause.AuthenticationFailed);
				}
				else
				{
					IntPtr currentThread = UnsafeNclNativeMethods.GetCurrentThread();
					if (!UnsafeNclNativeMethods.ImpersonateAnonymousToken(currentThread))
					{
						uint lastError = UnsafeNclNativeMethods.GetLastError();
						throw new Win32Exception((int)lastError);
					}
					try
					{
						this.m_CredentialsHandle = new CredentialsContext(sspi, CredentialUse.Outgoing);
						return;
					}
					finally
					{
						if (!UnsafeNclNativeMethods.RevertToSelf())
						{
							Process.GetCurrentProcess().Kill();
						}
					}
				}
			}
			if (this.m_isSchannel)
			{
				string certificate = connectionInfo.Certificate;
				if (string.IsNullOrEmpty(certificate))
				{
					this.m_CredentialsHandle = new CredentialsContext(null);
					return;
				}
				this.m_CredentialsHandle = new CredentialsContext(CertUtils.LoadCertificateByThumbprint(certificate, true));
				return;
			}
			else
			{
				this.m_CredentialsHandle = new CredentialsContext(sspi, CredentialUse.Outgoing);
			}
		}

		public byte[] GetOutgoingBlob(byte[] incomingBlob, out bool handshakeComplete)
		{
			handshakeComplete = true;
			if (this.m_SecurityContext.Handle.Initialized && incomingBlob == null)
			{
				return null;
			}
			SecurityBufferClass inputBuffer = null;
			if (incomingBlob != null)
			{
				inputBuffer = new SecurityBufferClass(incomingBlob, BufferType.Token);
			}
			SecurityBufferClass securityBufferClass = new SecurityBufferClass(this.m_TokenSize, BufferType.Token);
			int num = SSPIWrapper.InitializeSecurityContext(ref this.m_CredentialsHandle.Handle, ref this.m_SecurityContext.Handle, this.m_RemotePeerId, (int)this.m_RequestedFlags, this.m_Endianness, inputBuffer, ref this.m_SecurityContext.Handle, securityBufferClass, ref this.m_ContextFlags, ref this.m_SecurityContext.TimeStamp);
			int num2 = num & -2147483648;
			if (num2 != 0)
			{
				throw new Win32Exception(num);
			}
			if (num != 0 && this.m_SecurityContext.Handle.Initialized)
			{
				handshakeComplete = false;
			}
			else
			{
				switch (this.m_SecurityContext.SecurityContextMode)
				{
				case SecurityContextMode.stream:
					this.CheckIfFlagWasObtainedIfRequested(ContextFlags.Stream);
					break;
				}
				this.CheckIfFlagWasObtainedIfRequested(ContextFlags.MutualAuth);
				this.CheckIfFlagWasObtainedIfRequested(ContextFlags.ReplayDetect);
				this.CheckIfFlagWasObtainedIfRequested(ContextFlags.SequenceDetect);
				this.CheckIfFlagWasObtainedIfRequested(ContextFlags.Confidentiality);
			}
			return securityBufferClass.token;
		}

		private void CheckIfCapabilityIsSupported(string packageName, PackageCapabilities capability)
		{
			if ((this.m_Capabilities & (int)capability) == 0)
			{
				throw new AdomdConnectionException(XmlaSR.Authentication_Sspi_PackageDoesntSupportCapability(packageName, capability.ToString()), null, ConnectionExceptionCause.AuthenticationFailed);
			}
		}

		private void CheckIfFlagWasObtainedIfRequested(ContextFlags flag)
		{
			if ((this.m_RequestedFlags & flag) != (ContextFlags)0 && (this.m_ContextFlags & (int)flag) == 0)
			{
				throw new AdomdConnectionException(XmlaSR.Authentication_Sspi_FlagNotEstablished(flag.ToString()), null, ConnectionExceptionCause.AuthenticationFailed);
			}
		}
	}
}
