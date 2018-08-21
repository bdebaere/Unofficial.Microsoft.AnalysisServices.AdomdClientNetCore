using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class OnPremFromCloudAccessTranportClient
	{
		internal IMDExternalDataChannel ExternalDataChannel;

		internal OnPremFromCloudAccessTranportClient(int timeoutInSeconds, string configurationProperties)
		{
			using (new ActCtxHelper("Microsoft.DataProxy.NativeClient.dll.manifest"))
			{
				//this.ExternalDataChannel = (IMDExternalDataChannel)new DataProxyTransportChannel();
				this.ExternalDataChannel.Initialize(timeoutInSeconds, configurationProperties);
			}
		}

		internal OnPremFromCloudAccessStream GetTransportStream(string connectionString, DataType desiredRequestType, DataType desiredResponseType)
		{
			object obj;
			this.ExternalDataChannel.OpenConnection(connectionString, null, out obj);
			return new OnPremFromCloudAccessStream(desiredRequestType, desiredResponseType, (IMDExternalConnection)obj);
		}
	}
}
