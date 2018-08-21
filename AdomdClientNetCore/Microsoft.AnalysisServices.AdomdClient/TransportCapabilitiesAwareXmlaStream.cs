using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal abstract class TransportCapabilitiesAwareXmlaStream : XmlaStream
	{
		private DataType desiredRequestType;

		private DataType desiredResponseType;

		private TransportCapabilities transportCapabilities;

		private bool finalRequestTypeCalculated;

		private DataType finalRequestType;

		protected bool NegotiatedOptions
		{
			get
			{
				return this.transportCapabilities.ContentTypeNegotiated;
			}
		}

		protected TransportCapabilitiesAwareXmlaStream(DataType desiredRequestType, DataType desiredResponseType)
		{
			this.desiredRequestType = desiredRequestType;
			this.desiredResponseType = desiredResponseType;
			this.InitTransportCapabilities();
		}

		protected TransportCapabilitiesAwareXmlaStream(TransportCapabilitiesAwareXmlaStream originalStream)
		{
			this.desiredRequestType = originalStream.desiredRequestType;
			this.desiredResponseType = originalStream.desiredResponseType;
			this.transportCapabilities = originalStream.transportCapabilities.Clone();
		}

		protected void SetTransportCapabilities(TransportCapabilities capabilities)
		{
			if (capabilities != null)
			{
				this.transportCapabilities = capabilities.Clone();
			}
		}

		internal TransportCapabilities GetTransportCapabilities()
		{
			return this.transportCapabilities.Clone();
		}

		internal string GetTransportCapabilitiesString()
		{
			return this.transportCapabilities.GetString();
		}

		internal void SetRequestDataType(DataType value)
		{
			this.finalRequestTypeCalculated = true;
			this.finalRequestType = value;
		}

		private void InitTransportCapabilities()
		{
			this.transportCapabilities = new TransportCapabilities();
			this.transportCapabilities.ContentTypeNegotiated = false;
			this.transportCapabilities.ResponseBinary = (this.desiredResponseType == DataType.CompressedBinaryXml || this.desiredResponseType == DataType.BinaryXml);
			this.transportCapabilities.ResponseCompression = (this.desiredResponseType == DataType.CompressedBinaryXml || this.desiredResponseType == DataType.CompressedXml);
		}

		protected abstract void DetermineNegotiatedOptions();

		public override DataType GetRequestDataType()
		{
			if (!this.finalRequestTypeCalculated)
			{
				this.finalRequestType = DataType.TextXml;
				switch (this.desiredRequestType)
				{
				case DataType.BinaryXml:
					if (this.transportCapabilities.RequestType == DataType.BinaryXml || this.transportCapabilities.RequestType == DataType.CompressedBinaryXml)
					{
						this.finalRequestType = DataType.BinaryXml;
					}
					break;
				case DataType.CompressedXml:
					if (this.transportCapabilities.RequestType == DataType.CompressedXml || this.transportCapabilities.RequestType == DataType.CompressedBinaryXml)
					{
						this.finalRequestType = DataType.CompressedXml;
					}
					break;
				case DataType.CompressedBinaryXml:
					if (this.transportCapabilities.RequestType == DataType.CompressedBinaryXml)
					{
						this.finalRequestType = DataType.CompressedBinaryXml;
					}
					else if (this.transportCapabilities.RequestType == DataType.CompressedXml)
					{
						this.finalRequestType = DataType.CompressedXml;
					}
					else if (this.transportCapabilities.RequestType == DataType.BinaryXml)
					{
						this.finalRequestType = DataType.BinaryXml;
					}
					break;
				}
				if (this.NegotiatedOptions)
				{
					this.finalRequestTypeCalculated = true;
				}
			}
			return this.finalRequestType;
		}
	}
}
