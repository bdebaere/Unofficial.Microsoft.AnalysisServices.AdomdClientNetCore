using System;
using System.ComponentModel;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class LocalCubeStream : ClearTextXmlaStream
	{
		private static byte[] bufferForSkip = new byte[1024];

		private string cubeFile;

		private IntPtr hLocalServer;

		private IntPtr hLocalRequest;

		private MsmdlocalWrapper msmdlocalWraper;

		public LocalCubeStream(string cubeFile, MsmdlocalWrapper.OpenFlags settings, int timeout, string password, string serverName)
		{
			try
			{
				this.cubeFile = cubeFile;
				this.msmdlocalWraper = MsmdlocalWrapper.LocalWrapper;
				this.hLocalServer = this.msmdlocalWraper.MSMDOpenLocal(cubeFile, settings, password, serverName);
			}
			catch (Win32Exception innerException)
			{
				this.msmdlocalWraper = null;
				this.hLocalServer = IntPtr.Zero;
				throw new XmlaStreamException(XmlaSR.LocalCube_FileNotOpened(cubeFile), innerException);
			}
			catch
			{
				this.msmdlocalWraper = null;
				this.hLocalServer = IntPtr.Zero;
				throw;
			}
		}

		public override void Dispose()
		{
			this.Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this.msmdlocalWraper != null)
				{
					if (this.hLocalRequest != IntPtr.Zero)
					{
						try
						{
							this.msmdlocalWraper.MSMDCloseHandle(this.hLocalRequest);
						}
						catch (Win32Exception)
						{
						}
						this.hLocalRequest = IntPtr.Zero;
					}
					if (this.hLocalServer != IntPtr.Zero)
					{
						try
						{
							this.msmdlocalWraper.MSMDCloseHandle(this.hLocalServer);
						}
						catch (Win32Exception)
						{
						}
						this.hLocalServer = IntPtr.Zero;
					}
					this.msmdlocalWraper = null;
				}
				this.disposed = true;
				if (disposing)
				{
					GC.SuppressFinalize(this);
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		~LocalCubeStream()
		{
			this.Dispose(false);
		}

		public override void Close()
		{
		}

		public override void WriteEndOfMessage()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				if (this.hLocalRequest != IntPtr.Zero)
				{
					this.msmdlocalWraper.MSMDReceiveResponse(this.hLocalRequest);
				}
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (size + offset > buffer.Length)
			{
				throw new ArgumentException(XmlaSR.InvalidArgument, "buffer");
			}
			int result;
			try
			{
				if (this.hLocalRequest == IntPtr.Zero)
				{
					result = 0;
				}
				else
				{
					int num = 0;
					this.msmdlocalWraper.MSMDReadDataEx(this.hLocalRequest, buffer, offset, size, out num);
					if (num == 0)
					{
						this.ResetRequest();
					}
					result = num;
				}
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			return result;
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (size + offset > buffer.Length)
			{
				throw new ArgumentException(XmlaSR.InvalidArgument, "buffer");
			}
			try
			{
				if (this.hLocalRequest == IntPtr.Zero)
				{
					this.hLocalRequest = this.msmdlocalWraper.MSMDOpenRequest(this.hLocalServer, MsmdlocalWrapper.MSMDLOCAL_REQUEST_ENCODING.MSMDLOCAL_REQUEST_DEFAULT, 0u);
					this.msmdlocalWraper.MSMDSendRequest(this.hLocalRequest);
				}
				int num;
				for (int i = 0; i < size; i += num)
				{
					num = 0;
					this.msmdlocalWraper.MSMDWriteDataEx(this.hLocalRequest, buffer, offset + i, size, out num);
				}
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override void Skip()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				if (!(this.hLocalRequest == IntPtr.Zero))
				{
					while (this.Read(LocalCubeStream.bufferForSkip, 0, LocalCubeStream.bufferForSkip.Length) > 0)
					{
					}
					this.ResetRequest();
				}
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override void Flush()
		{
		}

		private void ResetRequest()
		{
			if (this.hLocalRequest != IntPtr.Zero)
			{
				this.msmdlocalWraper.MSMDCloseHandle(this.hLocalRequest);
				this.hLocalRequest = IntPtr.Zero;
			}
		}
	}
}
