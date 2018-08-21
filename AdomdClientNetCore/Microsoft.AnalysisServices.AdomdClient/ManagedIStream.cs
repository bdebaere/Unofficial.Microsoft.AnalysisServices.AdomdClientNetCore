using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[ClassInterface(ClassInterfaceType.None)]
	internal class ManagedIStream : IStream, IDisposable
	{
		private Stream managedStream;

		public ManagedIStream(Stream managedStream)
		{
			if (managedStream == null)
			{
				throw new ArgumentNullException("managedStream");
			}
			this.managedStream = managedStream;
		}

		~ManagedIStream()
		{
			this.Close();
		}

		public virtual void Dispose()
		{
			this.Close();
		}

		public void Read(byte[] pv, int cb, IntPtr pcbRead)
		{
			if (pcbRead == IntPtr.Zero)
			{
				this.managedStream.Read(pv, 0, cb);
				return;
			}
			Marshal.WriteInt32(pcbRead, this.managedStream.Read(pv, 0, cb));
		}

		public void Write(byte[] pv, int cb, IntPtr pcbWritten)
		{
			if (pcbWritten == IntPtr.Zero)
			{
				this.managedStream.Write(pv, 0, cb);
				return;
			}
			long position = this.managedStream.Position;
			this.managedStream.Write(pv, 0, cb);
			Marshal.WriteInt32(pcbWritten, (int)(this.managedStream.Position - position));
		}

		public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
		{
			if (plibNewPosition == IntPtr.Zero)
			{
				this.managedStream.Seek(dlibMove, (SeekOrigin)dwOrigin);
				return;
			}
			Marshal.WriteInt64(plibNewPosition, this.managedStream.Seek(dlibMove, (SeekOrigin)dwOrigin));
		}

		public void SetSize(long libNewSize)
		{
			this.managedStream.SetLength(libNewSize);
		}

		public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
		{
			byte[] buffer = new byte[cb];
			int num = 0;
			while ((long)num < cb)
			{
				int num2 = this.managedStream.Read(buffer, 0, (int)(cb - (long)num));
				if (num2 == 0)
				{
					break;
				}
				this.managedStream.Write(buffer, 0, num2);
				num += num2;
			}
			if (pcbRead != IntPtr.Zero)
			{
				Marshal.WriteInt64(pcbRead, (long)num);
			}
			if (pcbWritten != IntPtr.Zero)
			{
				Marshal.WriteInt64(pcbWritten, (long)num);
			}
		}

		public void Commit(int grfCommitFlags)
		{
			this.managedStream.Flush();
		}

		public void Revert()
		{
			throw new NotSupportedException(XmlaSR.IXMLAInterop_StreamDoesNotSupportReverting);
		}

		public void LockRegion(long libOffset, long cb, int dwLockType)
		{
			throw new NotSupportedException(XmlaSR.IXMLAInterop_StreamDoesNotSupportLocking);
		}

		public void UnlockRegion(long libOffset, long cb, int dwLockType)
		{
			throw new NotSupportedException(XmlaSR.IXMLAInterop_StreamDoesNotSupportUnlocking);
		}

		public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
		{
			pstatstg = default(System.Runtime.InteropServices.ComTypes.STATSTG);
			pstatstg.type = 2;
			pstatstg.cbSize = this.managedStream.Length;
			pstatstg.grfMode = 2;
			pstatstg.grfLocksSupported = 2;
		}

		public void Clone(out IStream ppstm)
		{
			throw new NotSupportedException(XmlaSR.IXMLAInterop_StreamCannotBeCloned);
		}

		public void Close()
		{
			if (this.managedStream != null)
			{
				this.managedStream.Close();
				this.managedStream = null;
				GC.SuppressFinalize(this);
			}
		}
	}
}
