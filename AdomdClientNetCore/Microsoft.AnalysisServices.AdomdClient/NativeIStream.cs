using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class NativeIStream : Stream
	{
		private IStream nativeIStream;

		private IntPtr varAddress = IntPtr.Zero;

		public override long Length
		{
			get
			{
				System.Runtime.InteropServices.ComTypes.STATSTG sTATSTG;
				this.nativeIStream.Stat(out sTATSTG, 1);
				return sTATSTG.cbSize;
			}
		}

		public override long Position
		{
			get
			{
				return this.Seek(0L, SeekOrigin.Current);
			}
			set
			{
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public NativeIStream(IStream nativeStream)
		{
			if (nativeStream == null)
			{
				throw new ArgumentNullException("nativeStream");
			}
			this.nativeIStream = nativeStream;
			this.varAddress = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(ulong)));
		}

		~NativeIStream()
		{
			this.Close();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (offset != 0)
			{
				throw new NotSupportedException(XmlaSR.IXMLAInterop_OnlyZeroOffsetIsSupported);
			}
			this.nativeIStream.Read(buffer, count, this.varAddress);
			int result = (int)Marshal.ReadInt64(this.varAddress);
			GC.KeepAlive(this);
			return result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (offset != 0)
			{
				throw new NotSupportedException(XmlaSR.IXMLAInterop_OnlyZeroOffsetIsSupported);
			}
			this.nativeIStream.Write(buffer, count, IntPtr.Zero);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.nativeIStream.Seek(offset, (int)origin, this.varAddress);
			long result = Marshal.ReadInt64(this.varAddress);
			GC.KeepAlive(this);
			return result;
		}

		public override void SetLength(long value)
		{
			this.nativeIStream.SetSize(value);
		}

		public override void Close()
		{
			if (this.nativeIStream != null)
			{
				this.nativeIStream.Commit(0);
				Marshal.ReleaseComObject(this.nativeIStream);
				this.nativeIStream = null;
				GC.SuppressFinalize(this);
				if (this.varAddress != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(this.varAddress);
					this.varAddress = IntPtr.Zero;
				}
			}
			GC.KeepAlive(this);
		}

		public override void Flush()
		{
			this.nativeIStream.Commit(0);
		}
	}
}
