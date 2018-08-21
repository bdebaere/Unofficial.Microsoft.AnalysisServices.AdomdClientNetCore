using Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;

namespace Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces
{
	internal static class ASSPClientProxyFactory
	{
		private sealed class InvalidASSPClientProxy : IASSPClientProxy
		{
			public bool IsWorkbookInFarm(string in_bstrWorkbookPath)
			{
				return false;
			}

			public bool IsFarmRunning()
			{
				return false;
			}

			public ILinkFile GetLinkFile(string in_bstrLinkFilePath)
			{
				throw new InvalidOperationException();
			}

			public IWorkbookSession OpenWorkbookModel(string in_bstrWorkbookPath)
			{
				throw new InvalidOperationException();
			}

			public IWorkbookSession OpenWorkbookModel(string in_bstrWorkbookPath, SessionLifetimePolicy in_lifetimePolicy)
			{
				throw new InvalidOperationException();
			}

			public IWorkbookSession OpenWorkbookModelForRefresh(string in_bstrWorkbookPath)
			{
				throw new InvalidOperationException();
			}

			public IWorkbookSession OpenWorkbookModelForRefresh(string in_bstrWorkbookPath, SessionLifetimePolicy in_lifetimePolicy)
			{
				throw new InvalidOperationException();
			}

			public IWorkbookSession OpenWorkbookSession(string in_bstrWorkbookPath, string in_bstrSessionId)
			{
				throw new InvalidOperationException();
			}

			public IWorkbookSession OpenWorkbookSession(string in_bstrWorkbookPath, string in_bstrSessionId, SessionLifetimePolicy in_lifetimePolicy)
			{
				throw new InvalidOperationException();
			}

			public bool IsRunningInFarm(int majorVersion)
			{
				return false;
			}

			public WindowsIdentity GetWindowsIdentityFromCurrentPrincipal()
			{
				throw new InvalidOperationException();
			}

			public void Log(TraceLevel tl, string message)
			{
			}

			public void Log1(TraceLevel tl, string message, string param1)
			{
			}

			public void Log2(TraceLevel tl, string message, string param1, string param2)
			{
			}

			public void Log3(TraceLevel tl, string message, string param1, string param2, string param3)
			{
			}

			public void Log4(TraceLevel tl, string message, string param1, string param2, string param3, string param4)
			{
			}

			public void TraceError(string message, params object[] args)
			{
			}

			public void TraceVerbose(string message, params object[] args)
			{
			}

			public void TraceWarning(string message, params object[] args)
			{
			}
		}

		private sealed class StaticASSPClientProxy : IASSPClientProxy
		{
			private Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.IASSPClientProxy staticProxy;

			public StaticASSPClientProxy(Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.IASSPClientProxy staticProxy)
			{
				this.staticProxy = staticProxy;
			}

			public bool IsWorkbookInFarm(string in_bstrWorkbookPath)
			{
				return this.staticProxy.IsWorkbookInFarm(in_bstrWorkbookPath);
			}

			public bool IsFarmRunning()
			{
				return this.staticProxy.IsFarmRunning();
			}

			public ILinkFile GetLinkFile(string in_bstrLinkFilePath)
			{
				return new ASSPClientProxyFactory.StaticLinkFile(this.staticProxy.GetLinkFile(in_bstrLinkFilePath));
			}

			public IWorkbookSession OpenWorkbookModel(string in_bstrWorkbookPath)
			{
				return new ASSPClientProxyFactory.StaticWorkbookSession(this.staticProxy.OpenWorkbookModel(in_bstrWorkbookPath));
			}

			public IWorkbookSession OpenWorkbookModel(string in_bstrWorkbookPath, SessionLifetimePolicy in_lifetimePolicy)
			{
				return new ASSPClientProxyFactory.StaticWorkbookSession(this.staticProxy.OpenWorkbookModel(in_bstrWorkbookPath, (Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.SessionLifetimePolicy)in_lifetimePolicy));
			}

			public IWorkbookSession OpenWorkbookModelForRefresh(string in_bstrWorkbookPath)
			{
				return new ASSPClientProxyFactory.StaticWorkbookSession(this.staticProxy.OpenWorkbookModelForRefresh(in_bstrWorkbookPath));
			}

			public IWorkbookSession OpenWorkbookModelForRefresh(string in_bstrWorkbookPath, SessionLifetimePolicy in_lifetimePolicy)
			{
				return new ASSPClientProxyFactory.StaticWorkbookSession(this.staticProxy.OpenWorkbookModelForRefresh(in_bstrWorkbookPath, (Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.SessionLifetimePolicy)in_lifetimePolicy));
			}

			public IWorkbookSession OpenWorkbookSession(string in_bstrWorkbookPath, string in_bstrSessionId)
			{
				return new ASSPClientProxyFactory.StaticWorkbookSession(this.staticProxy.OpenWorkbookSession(in_bstrWorkbookPath, in_bstrSessionId));
			}

			public IWorkbookSession OpenWorkbookSession(string in_bstrWorkbookPath, string in_bstrSessionId, SessionLifetimePolicy in_lifetimePolicy)
			{
				return new ASSPClientProxyFactory.StaticWorkbookSession(this.staticProxy.OpenWorkbookSession(in_bstrWorkbookPath, in_bstrSessionId, (Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.SessionLifetimePolicy)in_lifetimePolicy));
			}

			public bool IsRunningInFarm(int majorVersion)
			{
				return this.staticProxy.IsRunningInFarm(majorVersion);
			}

			public WindowsIdentity GetWindowsIdentityFromCurrentPrincipal()
			{
				return this.staticProxy.GetWindowsIdentityFromCurrentPrincipal();
			}

			public void TraceError(string message, params object[] args)
			{
				this.staticProxy.TraceError(message, args);
			}

			public void TraceVerbose(string message, params object[] args)
			{
				this.staticProxy.TraceVerbose(message, args);
			}

			public void TraceWarning(string message, params object[] args)
			{
				this.staticProxy.TraceWarning(message, args);
			}

			public void Log(TraceLevel tl, string message)
			{
				this.staticProxy.Log(tl, message);
			}

			public void Log1(TraceLevel tl, string message, string param1)
			{
				this.staticProxy.Log1(tl, message, param1);
			}

			public void Log2(TraceLevel tl, string message, string param1, string param2)
			{
				this.staticProxy.Log2(tl, message, param1, param2);
			}

			public void Log3(TraceLevel tl, string message, string param1, string param2, string param3)
			{
				this.staticProxy.Log3(tl, message, param1, param2, param3);
			}

			public void Log4(TraceLevel tl, string message, string param1, string param2, string param3, string param4)
			{
				this.staticProxy.Log4(tl, message, param1, param2, param3, param4);
			}
		}

		private sealed class StaticLinkFile : ILinkFile
		{
			private Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.ILinkFile staticLinkFile;

			public string Database
			{
				get
				{
					return this.staticLinkFile.Database;
				}
			}

			public string Description
			{
				get
				{
					return this.staticLinkFile.Description;
				}
			}

			public bool IsDelegationAllowed
			{
				get
				{
					return this.staticLinkFile.IsDelegationAllowed;
				}
			}

			public bool IsInFarm
			{
				get
				{
					return this.staticLinkFile.IsInFarm;
				}
			}

			public bool IsFileMalformed
			{
				get
				{
					return this.staticLinkFile.IsFileMalformed;
				}
			}

			public string Server
			{
				get
				{
					return this.staticLinkFile.Server;
				}
			}

			public StaticLinkFile(Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.ILinkFile staticLinkFile)
			{
				this.staticLinkFile = staticLinkFile;
			}
		}

		private sealed class StaticWorkbookSession : IWorkbookSession, IDisposable
		{
			private Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.IWorkbookSession staticWorkbookSession;

			public ConnectionStyle ConnectionStyle
			{
				get
				{
					return (ConnectionStyle)this.staticWorkbookSession.ConnectionStyle;
				}
			}

			public string Database
			{
				get
				{
					return this.staticWorkbookSession.Database;
				}
			}

			public string Server
			{
				get
				{
					return this.staticWorkbookSession.Server;
				}
			}

			public string SessionId
			{
				get
				{
					return this.staticWorkbookSession.SessionId;
				}
			}

			public string UserName
			{
				get
				{
					return this.staticWorkbookSession.UserName;
				}
			}

			public WorkbookFileFormat WorkbookFormatVersion
			{
				get
				{
					return (WorkbookFileFormat)this.staticWorkbookSession.WorkbookFormatVersion;
				}
			}

			public string WorkbookPath
			{
				get
				{
					return this.staticWorkbookSession.WorkbookPath;
				}
			}

			public StaticWorkbookSession(Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.IWorkbookSession staticWorkbookSession)
			{
				this.staticWorkbookSession = staticWorkbookSession;
			}

			public void BeginActivity()
			{
				this.staticWorkbookSession.BeginActivity();
			}

			public IStream CreateNativeStream()
			{
				return this.staticWorkbookSession.CreateNativeStream();
			}

			public void EndActivity()
			{
				this.staticWorkbookSession.EndActivity();
			}

			public void EndSession()
			{
				this.staticWorkbookSession.EndSession();
			}

			public void EnsureValidSession()
			{
				this.staticWorkbookSession.EnsureValidSession();
			}

			public void Refresh(string in_bstrTargetApplicationId, string in_bstrConnectionName)
			{
				this.staticWorkbookSession.Refresh(in_bstrTargetApplicationId, in_bstrConnectionName);
			}

			public void RefreshEmbeddedModel()
			{
				this.staticWorkbookSession.RefreshEmbeddedModel();
			}

			public string[] GetWorkbookConnections()
			{
				return this.staticWorkbookSession.GetWorkbookConnections();
			}

			public Stream CreateManagedStream()
			{
				return this.staticWorkbookSession.CreateManagedStream();
			}

			public void Save()
			{
				this.staticWorkbookSession.Save();
			}

			public void Dispose()
			{
				this.staticWorkbookSession.Dispose();
			}

			public void TraceError(string message, params object[] args)
			{
				this.staticWorkbookSession.TraceError(message, args);
			}

			public void TraceVerbose(string message, params object[] args)
			{
				this.staticWorkbookSession.TraceVerbose(message, args);
			}

			public void TraceWarning(string message, params object[] args)
			{
				this.staticWorkbookSession.TraceWarning(message, args);
			}

			public void Log(TraceLevel tl, string message)
			{
				this.staticWorkbookSession.Log(tl, message);
			}

			public void Log1(TraceLevel tl, string message, string param1)
			{
				this.staticWorkbookSession.Log1(tl, message, param1);
			}

			public void Log2(TraceLevel tl, string message, string param1, string param2)
			{
				this.staticWorkbookSession.Log2(tl, message, param1, param2);
			}

			public void Log3(TraceLevel tl, string message, string param1, string param2, string param3)
			{
				this.staticWorkbookSession.Log3(tl, message, param1, param2, param3);
			}

			public void Log4(TraceLevel tl, string message, string param1, string param2, string param3, string param4)
			{
				this.staticWorkbookSession.Log4(tl, message, param1, param2, param3, param4);
			}

			public void ReportQueryExecution(int elapsedTime, string in_bstrQuery)
			{
				this.staticWorkbookSession.ReportQueryExecution(elapsedTime, in_bstrQuery);
			}
		}

		private const string ConcreteProxyTypeName13 = "Microsoft.AnalysisServices.SPClient.ASSPClientProxy, Microsoft.AnalysisServices.SPClient, PublicKeyToken=89845dcd8080cc91, Culture=neutral, Version=13.0.0.0";

		private const string ConcreteProxyTypeName16 = "Microsoft.AnalysisServices.SPClient16.ASSPClient16Proxy, Microsoft.AnalysisServices.SPClient16, PublicKeyToken=89845dcd8080cc91, Culture=neutral, Version=13.0.0.0";

		public static IASSPClientProxy CreateProxy()
		{
			IASSPClientProxy result;
			try
			{
				Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.IASSPClientProxy staticProxy = Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces.ASSPClientProxyFactory.CreateProxy();
				result = new ASSPClientProxyFactory.StaticASSPClientProxy(staticProxy);
			}
			catch
			{
				result = new ASSPClientProxyFactory.InvalidASSPClientProxy();
			}
			return result;
		}
	}
}
