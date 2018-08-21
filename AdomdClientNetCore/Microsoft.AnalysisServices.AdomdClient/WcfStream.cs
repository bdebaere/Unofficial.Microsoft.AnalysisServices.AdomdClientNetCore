using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class WcfStream : TransportCapabilitiesAwareXmlaStream
	{
		public const string DefaultHttpContentType = "text/xml";

		public const string DefaultNegotiationFlags = "1,0,0,0,0";

		private const string contentTypeParam = "content_type";

		private static readonly string recognizedContentTypes = string.Concat(new string[]
		{
			"(",
			"text/xml".Replace("+", "\\+"),
			")|(",
			"application/xml+xpress".Replace("+", "\\+"),
			")|(",
			"application/sx".Replace("+", "\\+"),
			" )|(",
			"application/sx+xpress".Replace("+", "\\+"),
			")"
		});

		private static readonly string contentRegExpr = "^(\\s*)(?<content_type>(" + WcfStream.recognizedContentTypes + "))(\\s*)((;(.*))|)(\\s*)\\z";

		private static readonly Regex contentTypeRegex = new Regex(WcfStream.contentRegExpr, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private List<byte[]> buffers = new List<byte[]>();

		private Stream responseStream;

		private WindowsPrincipal logonWindowsPrincipal;

		private WindowsIdentity logonWindowsIdentity;

		private IDisposable spSite;

		private string loginName;

		private string serverEndpointAddress;

		private string databaseId;

		private string applicationName;

		private bool isFirstRequest = true;

		private bool specificVersion;

		private bool outdatedVersion;

		private string userAgent = "ADOMD.NET";

		private string userAddress = string.Empty;

		private string responseFlags;

		private string responseContentType;

		private string soapAction;

		private void Init()
		{
			if (this.buffers.Count != 0)
			{
				this.buffers = new List<byte[]>();
			}
			if (this.responseStream != null)
			{
				this.responseStream.Dispose();
				this.responseStream = null;
			}
			this.responseFlags = string.Empty;
			this.responseContentType = string.Empty;
		}

		internal WcfStream(string dataSource, string serverEndpointAddress, bool specificVersion, string loginName, string databaseId, DataType desiredRequestType, DataType desiredResponseType, string applicationName) : base(desiredRequestType, desiredResponseType)
		{
			try
			{
				this.Init();
				this.spSite = XmlaClient.CreateSPSite(dataSource);
				this.serverEndpointAddress = serverEndpointAddress;
				this.loginName = loginName;
				this.databaseId = databaseId;
				this.applicationName = applicationName;
				this.specificVersion = specificVersion;
				this.logonWindowsIdentity = WindowsIdentity.GetCurrent();
				this.logonWindowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				XmlaClient.UlsWriterSetCurrentRequestCategoryToRequestProcessing();
			}
			catch (XmlaStreamException e)
			{
				XmlaClient.UlsWriterLogException(e);
				throw;
			}
			catch (Exception ex)
			{
				XmlaClient.UlsWriterLogException(ex);
				throw new XmlaStreamException(ex);
			}
		}

		public override void Close()
		{
		}

		public override void Dispose()
		{
			try
			{
				this.logonWindowsPrincipal = null;
				if (this.logonWindowsIdentity != null)
				{
					this.logonWindowsIdentity.Dispose();
					this.logonWindowsIdentity = null;
				}
				if (this.spSite != null)
				{
					this.spSite.Dispose();
					this.spSite = null;
				}
				if (this.responseStream != null)
				{
					this.responseStream.Dispose();
					this.responseStream = null;
				}
				this.disposed = true;
			}
			finally
			{
				base.Dispose(true);
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
				this.GetResponseStream();
				int num = this.responseStream.Read(buffer, offset, size);
				result = num;
			}
			catch (XmlaStreamException e)
			{
				XmlaClient.UlsWriterLogException(e);
				throw;
			}
			catch (Exception ex)
			{
				XmlaClient.UlsWriterLogException(ex);
				throw new XmlaStreamException(ex);
			}
			return result;
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				byte[] array = new byte[size];
				Array.Copy(buffer, offset, array, 0, size);
				this.buffers.Add(array);
			}
			catch (XmlaStreamException e)
			{
				XmlaClient.UlsWriterLogException(e);
				throw;
			}
			catch (Exception ex)
			{
				XmlaClient.UlsWriterLogException(ex);
				throw new XmlaStreamException(ex);
			}
		}

		public override void WriteEndOfMessage()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
		}

		public override void WriteSoapActionHeader(string action)
		{
			this.soapAction = action;
		}

		public override void Skip()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				this.Init();
			}
			catch (XmlaStreamException e)
			{
				XmlaClient.UlsWriterLogException(e);
				throw;
			}
			catch (Exception ex)
			{
				XmlaClient.UlsWriterLogException(ex);
				throw new XmlaStreamException(ex);
			}
		}

		public override void Flush()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
		}

		public override DataType GetResponseDataType()
		{
			DataType result;
			try
			{
				this.GetResponseStream();
				Match match = WcfStream.contentTypeRegex.Match(this.responseContentType);
				if (!match.Success)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnsupportedDataFormat(this.responseContentType), "");
				}
				DataType dataTypeFromString = DataTypes.GetDataTypeFromString(match.Groups["content_type"].Value);
				if (dataTypeFromString == DataType.Undetermined || dataTypeFromString == DataType.Unknown)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnsupportedDataFormat(this.responseContentType), "");
				}
				result = dataTypeFromString;
			}
			catch (XmlaStreamException e)
			{
				XmlaClient.UlsWriterLogException(e);
				throw;
			}
			catch (Exception ex)
			{
				XmlaClient.UlsWriterLogException(ex);
				throw new XmlaStreamException(ex);
			}
			return result;
		}

		private void GetResponseStreamHelper()
		{
			this.responseFlags = "1,0,0,0,0";
			this.responseContentType = "text/xml";
			try
			{
				long num = 0L;
				foreach (byte[] current in this.buffers)
				{
					num += (long)current.Length;
				}
				byte[] array;
				if (1 == this.buffers.Count)
				{
					array = this.buffers[0];
				}
				else
				{
					array = new byte[num];
					long num2 = 0L;
					foreach (byte[] current2 in this.buffers)
					{
						current2.CopyTo(array, num2);
						num2 += (long)current2.Length;
					}
				}
				this.responseStream = XmlaClient.GetResponseStreamHelper(this.spSite, new MemoryStream(array), this.serverEndpointAddress, this.loginName, this.databaseId, this.specificVersion, this.isFirstRequest, this.userAgent, this.applicationName, this.userAddress, "1,0,0,0,0", "text/xml", ref this.responseFlags, ref this.responseContentType, ref this.outdatedVersion);
				this.isFirstRequest = false;
			}
			catch (XmlaStreamException e)
			{
				XmlaClient.UlsWriterLogException(e);
				throw;
			}
			catch (Exception ex)
			{
				XmlaClient.UlsWriterLogException(ex);
				throw new XmlaStreamException(ex);
			}
			finally
			{
				if (this.buffers.Count != 0)
				{
					this.buffers = new List<byte[]>();
				}
			}
		}

		private void GetResponseStream()
		{
			if (this.outdatedVersion)
			{
				throw new AdomdConnectionException(XmlaSR.Connection_WorkbookIsOutdated);
			}
			if (this.responseStream == null)
			{
				IPrincipal currentPrincipal = Thread.CurrentPrincipal;
				try
				{
					Thread.CurrentPrincipal = this.logonWindowsPrincipal;
					using (WindowsIdentity current = WindowsIdentity.GetCurrent())
					{
						if (this.logonWindowsIdentity.User != current.User)
						{
							//using (this.logonWindowsIdentity.Impersonate())
							//{
							//	this.GetResponseStreamHelper();
							//	goto IL_71;
							//}
						}
						this.GetResponseStreamHelper();
						IL_71:;
					}
					if (this.outdatedVersion)
					{
						throw new AdomdConnectionException(XmlaSR.Connection_WorkbookIsOutdated);
					}
					this.DetermineNegotiatedOptions();
				}
				catch (XmlaStreamException e)
				{
					XmlaClient.UlsWriterLogException(e);
					throw;
				}
				catch (Exception ex)
				{
					XmlaClient.UlsWriterLogException(ex);
					throw new XmlaStreamException(ex);
				}
				finally
				{
					Thread.CurrentPrincipal = currentPrincipal;
				}
			}
		}

		protected override void DetermineNegotiatedOptions()
		{
			if (!base.NegotiatedOptions)
			{
				this.GetResponseStream();
				TransportCapabilities transportCapabilities = new TransportCapabilities();
				if (!string.IsNullOrEmpty(this.responseFlags))
				{
					transportCapabilities.FromString(this.responseFlags);
				}
				transportCapabilities.ContentTypeNegotiated = true;
				base.SetTransportCapabilities(transportCapabilities);
			}
		}
	}
}
