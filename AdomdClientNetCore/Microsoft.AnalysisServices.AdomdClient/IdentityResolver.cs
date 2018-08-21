using System;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class IdentityResolver : IDisposable
	{
		//private WindowsImpersonationContext ctx;

		private IPrincipal originalPrincipal;

		private IdentityResolver(ConnectionInfo connInfo)
		{
			if (!connInfo.RevertToProcessAccountForConnection)
			{
				if (connInfo.UserIdentity == UserIdentityType.SharePointPrincipal)
				{
					if (connInfo.SupportsSharePointAuth())
					{
						return;
					}
					try
					{
						this.SetWindowsIdentity();
						return;
					}
					catch (Exception innerException)
					{
						throw new InvalidOperationException("", innerException);
					}
				}
				if (connInfo.SupportsSharePointAuth())
				{
					this.SetWindowsPrincipal();
				}
			}
		}

		internal static IDisposable Resolve(ConnectionInfo connInfo)
		{
			return new IdentityResolver(connInfo);
		}

		public void Dispose()
		{
			//if (this.ctx != null)
			//{
			//	this.ctx.Undo();
			//	this.ctx = null;
			//}
			if (this.originalPrincipal != null)
			{
				Thread.CurrentPrincipal = this.originalPrincipal;
				this.originalPrincipal = null;
			}
		}

		private void SetWindowsIdentity()
		{
			XmlaClient.TraceVerbose("Recovering windows identity from SharePoint", new object[0]);
			WindowsIdentity windowsIdentityFromCurrentPrincipal = XmlaClient.SPProxy.GetWindowsIdentityFromCurrentPrincipal();
			this.originalPrincipal = Thread.CurrentPrincipal;
			//this.ctx = windowsIdentityFromCurrentPrincipal.Impersonate();
			Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			XmlaClient.TraceVerbose("Set thread identity to user '{0}'", new object[]
			{
				windowsIdentityFromCurrentPrincipal.Name
			});
		}

		private void SetWindowsPrincipal()
		{
			WindowsIdentity windowsIdentity = null;
			try
			{
				//using (WindowsIdentity.Impersonate(IntPtr.Zero))
				//{
				//	windowsIdentity = XmlaClient.SPProxy.GetWindowsIdentityFromCurrentPrincipal();
				//}
			}
			catch
			{
			}
			if (windowsIdentity == null || !windowsIdentity.User.Equals(WindowsIdentity.GetCurrent().User))
			{
				XmlaClient.TraceVerbose("Setting windows principal on the thread to match the windows user {0}", new object[]
				{
					WindowsIdentity.GetCurrent().Name
				});
				this.originalPrincipal = Thread.CurrentPrincipal;
				Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				return;
			}
			XmlaClient.TraceVerbose("SharePoint principal matches windows user so leaving SharePoint principal. User {0}", new object[]
			{
				WindowsIdentity.GetCurrent().Name
			});
		}
	}
}
