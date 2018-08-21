using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XpressMethodsWrapper : LibraryHandle
	{
		private class Lock
		{
		}

		private delegate IntPtr CompressInitDelegate([In] int maxInputSize, [In] int compressionLevel);

		private delegate int CompressDelegate([In] IntPtr compressHandle, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] input, [In] int inputOffset, [In] int inputSize, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] output, [In] int outputOffset, [In] int outputSize);

		private delegate void CompressCloseDelegate([In] IntPtr compressHandle);

		private delegate IntPtr DecompressInitDelegate();

		private delegate int DecompressDelegate([In] IntPtr decompressHandle, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] input, [In] int inputSize, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] output, [In] int outputSize, [In] int bytesToDecompress);

		private delegate void DecompressCloseDelegate([In] IntPtr decompressHandle);

		internal const int MaxBlock = 65536;

		private XpressMethodsWrapper.CompressInitDelegate compressInitDelegate;

		private XpressMethodsWrapper.CompressDelegate compressDelegate;

		private XpressMethodsWrapper.CompressCloseDelegate compressCloseDelegate;

		private XpressMethodsWrapper.DecompressInitDelegate decompressInitDelegate;

		private XpressMethodsWrapper.DecompressDelegate decompressDelegate;

		private XpressMethodsWrapper.DecompressCloseDelegate decompressCloseDelegate;

		private static XpressMethodsWrapper xpressMethodsWrapper;

		private static XpressMethodsWrapper.Lock LockForCreatingWrapper;

		private static readonly string XpressPath;

		internal static readonly bool XpressAvailable;

		internal static XpressMethodsWrapper XpressWrapper
		{
			get
			{
				XpressMethodsWrapper result;
				lock (XpressMethodsWrapper.LockForCreatingWrapper)
				{
					if (XpressMethodsWrapper.xpressMethodsWrapper == null || XpressMethodsWrapper.xpressMethodsWrapper.IsInvalid)
					{
						XpressMethodsWrapper.xpressMethodsWrapper = XpressMethodsWrapper.LoadLibrary(XpressMethodsWrapper.XpressPath);
						XpressMethodsWrapper.xpressMethodsWrapper.SetDelegates();
					}
					result = XpressMethodsWrapper.xpressMethodsWrapper;
				}
				return result;
			}
		}

		static XpressMethodsWrapper()
		{
			XpressMethodsWrapper.xpressMethodsWrapper = null;
			XpressMethodsWrapper.LockForCreatingWrapper = new XpressMethodsWrapper.Lock();
			XpressMethodsWrapper.XpressPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Microsoft SQL Server\\130\\shared\\msasxpress.dll";
			XpressMethodsWrapper.XpressAvailable = File.Exists(XpressMethodsWrapper.XpressPath);
		}

		private XpressMethodsWrapper()
		{
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern XpressMethodsWrapper LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] [In] string fileName);

		private void SetDelegates()
		{
			if (XpressMethodsWrapper.xpressMethodsWrapper.IsInvalid)
			{
				throw new Win32Exception(Marshal.GetHRForLastWin32Error());
			}
			try
			{
				this.compressInitDelegate = (XpressMethodsWrapper.CompressInitDelegate)XpressMethodsWrapper.xpressMethodsWrapper.GetDelegate("CompressInit", typeof(XpressMethodsWrapper.CompressInitDelegate));
				this.compressDelegate = (XpressMethodsWrapper.CompressDelegate)XpressMethodsWrapper.xpressMethodsWrapper.GetDelegate("Compress", typeof(XpressMethodsWrapper.CompressDelegate));
				this.compressCloseDelegate = (XpressMethodsWrapper.CompressCloseDelegate)XpressMethodsWrapper.xpressMethodsWrapper.GetDelegate("CompressClose", typeof(XpressMethodsWrapper.CompressCloseDelegate));
				this.decompressInitDelegate = (XpressMethodsWrapper.DecompressInitDelegate)XpressMethodsWrapper.xpressMethodsWrapper.GetDelegate("DecompressInit", typeof(XpressMethodsWrapper.DecompressInitDelegate));
				this.decompressDelegate = (XpressMethodsWrapper.DecompressDelegate)XpressMethodsWrapper.xpressMethodsWrapper.GetDelegate("Decompress", typeof(XpressMethodsWrapper.DecompressDelegate));
				this.decompressCloseDelegate = (XpressMethodsWrapper.DecompressCloseDelegate)XpressMethodsWrapper.xpressMethodsWrapper.GetDelegate("DecompressClose", typeof(XpressMethodsWrapper.DecompressCloseDelegate));
			}
			catch
			{
				XpressMethodsWrapper.xpressMethodsWrapper.Close();
				XpressMethodsWrapper.xpressMethodsWrapper.SetHandleAsInvalid();
				throw;
			}
		}

		internal IntPtr CompressInit(int maxInputSize, int compressionLevel)
		{
			return base.CheckEmptyHandle(this.compressInitDelegate(maxInputSize, compressionLevel));
		}

		internal int Compress(IntPtr compressHandle, byte[] input, int inputOffset, int inputSize, byte[] output, int outputOffset, int outputSize)
		{
			return this.compressDelegate(compressHandle, input, inputOffset, inputSize, output, outputOffset, outputSize);
		}

		internal void CompressClose(IntPtr compressHandle)
		{
			this.compressCloseDelegate(compressHandle);
		}

		internal IntPtr DecompressInit()
		{
			return base.CheckEmptyHandle(this.decompressInitDelegate());
		}

		internal int Decompress(IntPtr decompressHandle, byte[] input, int inputSize, byte[] output, int outputSize, int bytesToDecompress)
		{
			return this.decompressDelegate(decompressHandle, input, inputSize, output, outputSize, bytesToDecompress);
		}

		internal void DecompressClose(IntPtr decompressHandle)
		{
			this.decompressCloseDelegate(decompressHandle);
		}
	}
}
