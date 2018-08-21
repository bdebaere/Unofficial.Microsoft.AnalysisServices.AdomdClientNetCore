using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MsmdlocalWrapper : LibraryHandle
	{
		private class Lock
		{
		}

		[Flags]
		public enum OpenFlags : uint
		{
			OpenExisting = 1u,
			OpenOrCreate = 2u,
			CreateAlways = 4u,
			UseImbi = 16u
		}

		public enum MSMDLOCAL_REQUEST_ENCODING
		{
			MSMDLOCAL_REQUEST_DEFAULT,
			MSMDLOCAL_REQUEST_UTF16,
			MSMDLOCAL_REQUEST_UTF8,
			MSMDLOCAL_REQUEST_US_ASCII
		}

		private delegate IntPtr MSMDOpenLocalDelegate([MarshalAs(UnmanagedType.LPWStr)] [In] string pszPathToFile, [MarshalAs(UnmanagedType.U4)] [In] uint mskSettings, [MarshalAs(UnmanagedType.LPWStr)] [In] string pszPassword, [MarshalAs(UnmanagedType.LPWStr)] [In] string pszServerName);

		private delegate bool MSMDCloseHandleDelegate([In] IntPtr hLocal);

		private delegate IntPtr MSMDOpenRequestDelegate([In] IntPtr hLocal, [MarshalAs(UnmanagedType.I4)] [In] int encoding, [MarshalAs(UnmanagedType.U4)] [In] uint cTimeout);

		private delegate bool MSMDSendRequestDelegate([In] IntPtr hLocal, [In] bool binaryXml);

		private delegate bool MSMDWriteDataExDelegate([In] IntPtr hLocal, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] byte[] buffer, [MarshalAs(UnmanagedType.U4)] [In] int iOffset, [MarshalAs(UnmanagedType.U4)] [In] int bytesAvailable, [MarshalAs(UnmanagedType.U4)] out int bytesWritten);

		private delegate bool MSMDReceiveResponseDelegate([In] IntPtr hLocal);

		private delegate bool MSMDReadDataExDelegate([In] IntPtr hLocal, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] [Out] byte[] buffer, [MarshalAs(UnmanagedType.U4)] [In] int iOffset, [MarshalAs(UnmanagedType.U4)] [In] int bytes, [MarshalAs(UnmanagedType.U4)] out int bytesRead);

		private delegate int MSMDCanUnloadNowDelegate();

		private const int S_OK = 0;

		private const int S_FALSE = 1;

		private MsmdlocalWrapper.MSMDOpenLocalDelegate msmdOpenLocalDelegate;

		private MsmdlocalWrapper.MSMDCloseHandleDelegate msmdCloseHandleDelegate;

		private MsmdlocalWrapper.MSMDOpenRequestDelegate msmdOpenRequestDelegate;

		private MsmdlocalWrapper.MSMDSendRequestDelegate msmdSendRequestDelegate;

		private MsmdlocalWrapper.MSMDWriteDataExDelegate msmdWriteDataExDelegate;

		private MsmdlocalWrapper.MSMDReceiveResponseDelegate msmdReceiveResponseDelegate;

		private MsmdlocalWrapper.MSMDReadDataExDelegate msmdReadDataExDelegate;

		private MsmdlocalWrapper.MSMDCanUnloadNowDelegate msmdCanUnloadNowDelegate;

		private static MsmdlocalWrapper msmdlocalWrapper = null;

		private static MsmdlocalWrapper.Lock LockForCreatingWrapper = new MsmdlocalWrapper.Lock();

		public static MsmdlocalWrapper LocalWrapper
		{
			get
			{
				MsmdlocalWrapper result;
				lock (MsmdlocalWrapper.LockForCreatingWrapper)
				{
					if (MsmdlocalWrapper.msmdlocalWrapper == null || MsmdlocalWrapper.msmdlocalWrapper.IsInvalid)
					{
						string text = LocalExcelVar.MSMDLOCAL_PATH;
						if (!File.Exists(text))
						{
							text = LocalExcelVar.MSMDLOCAL_FALLBACK_PATH;
						}
						MsmdlocalWrapper.msmdlocalWrapper = MsmdlocalWrapper.LoadLibrary(text);
						MsmdlocalWrapper.msmdlocalWrapper.SetDelegates();
					}
					result = MsmdlocalWrapper.msmdlocalWrapper;
				}
				return result;
			}
		}

		private MsmdlocalWrapper()
		{
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern MsmdlocalWrapper LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] [In] string fileName);

		private void SetDelegates()
		{
			if (MsmdlocalWrapper.msmdlocalWrapper.IsInvalid)
			{
				throw new Win32Exception(Marshal.GetHRForLastWin32Error());
			}
			try
			{
				this.msmdOpenLocalDelegate = (MsmdlocalWrapper.MSMDOpenLocalDelegate)MsmdlocalWrapper.msmdlocalWrapper.GetDelegate("MSMDOpenLocal", typeof(MsmdlocalWrapper.MSMDOpenLocalDelegate));
				this.msmdCloseHandleDelegate = (MsmdlocalWrapper.MSMDCloseHandleDelegate)MsmdlocalWrapper.msmdlocalWrapper.GetDelegate("MSMDCloseHandle", typeof(MsmdlocalWrapper.MSMDCloseHandleDelegate));
				this.msmdOpenRequestDelegate = (MsmdlocalWrapper.MSMDOpenRequestDelegate)MsmdlocalWrapper.msmdlocalWrapper.GetDelegate("MSMDOpenRequest", typeof(MsmdlocalWrapper.MSMDOpenRequestDelegate));
				this.msmdSendRequestDelegate = (MsmdlocalWrapper.MSMDSendRequestDelegate)MsmdlocalWrapper.msmdlocalWrapper.GetDelegate("MSMDSendRequest", typeof(MsmdlocalWrapper.MSMDSendRequestDelegate));
				this.msmdWriteDataExDelegate = (MsmdlocalWrapper.MSMDWriteDataExDelegate)MsmdlocalWrapper.msmdlocalWrapper.GetDelegate("MSMDWriteDataEx", typeof(MsmdlocalWrapper.MSMDWriteDataExDelegate));
				this.msmdReceiveResponseDelegate = (MsmdlocalWrapper.MSMDReceiveResponseDelegate)MsmdlocalWrapper.msmdlocalWrapper.GetDelegate("MSMDReceiveResponse", typeof(MsmdlocalWrapper.MSMDReceiveResponseDelegate));
				this.msmdReadDataExDelegate = (MsmdlocalWrapper.MSMDReadDataExDelegate)MsmdlocalWrapper.msmdlocalWrapper.GetDelegate("MSMDReadDataEx", typeof(MsmdlocalWrapper.MSMDReadDataExDelegate));
				this.msmdCanUnloadNowDelegate = (MsmdlocalWrapper.MSMDCanUnloadNowDelegate)MsmdlocalWrapper.msmdlocalWrapper.GetDelegate("MSMDCanUnloadNow", typeof(MsmdlocalWrapper.MSMDCanUnloadNowDelegate));
			}
			catch
			{
				MsmdlocalWrapper.msmdlocalWrapper.Close();
				MsmdlocalWrapper.msmdlocalWrapper.SetHandleAsInvalid();
				throw;
			}
		}

		public IntPtr MSMDOpenLocal(string pszPathToFile, MsmdlocalWrapper.OpenFlags mskSettings, string pszPassword, string serverName)
		{
			return base.CheckEmptyHandle(this.msmdOpenLocalDelegate(pszPathToFile, (uint)mskSettings, pszPassword, serverName));
		}

		public void MSMDCloseHandle(IntPtr hLocal)
		{
			this.CheckFalse(this.msmdCloseHandleDelegate(hLocal));
		}

		public IntPtr MSMDOpenRequest(IntPtr hLocal, MsmdlocalWrapper.MSMDLOCAL_REQUEST_ENCODING encoding, uint cTimeout)
		{
			return base.CheckEmptyHandle(this.msmdOpenRequestDelegate(hLocal, (int)encoding, cTimeout));
		}

		public void MSMDSendRequest(IntPtr hLocal)
		{
			this.CheckFalse(this.msmdSendRequestDelegate(hLocal, false));
		}

		public void MSMDWriteDataEx(IntPtr hLocal, byte[] buffer, int offset, int bytesAvailable, out int bytesWritten)
		{
			this.CheckFalse(this.msmdWriteDataExDelegate(hLocal, buffer, offset, bytesAvailable, out bytesWritten));
		}

		public void MSMDReceiveResponse(IntPtr hLocal)
		{
			this.CheckFalse(this.msmdReceiveResponseDelegate(hLocal));
		}

		public void MSMDReadDataEx(IntPtr hLocal, byte[] buffer, int offset, int bytes, out int bytesRead)
		{
			this.CheckFalse(this.msmdReadDataExDelegate(hLocal, buffer, offset, bytes, out bytesRead));
		}

		private void CheckFalse(bool result)
		{
			if (!result)
			{
				base.ThrowOnError();
			}
		}
	}
}
