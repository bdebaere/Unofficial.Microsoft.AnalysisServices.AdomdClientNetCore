using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal static class CertUtils
	{
		public static X509Certificate2 LoadCertificateByThumbprint(string thumbprint, bool requirePrivateKey)
		{
			StoreName[] array = new StoreName[]
			{
				StoreName.My,
				StoreName.Root
			};
			StoreLocation[] array2 = new StoreLocation[]
			{
				StoreLocation.CurrentUser,
				StoreLocation.LocalMachine
			};
			StoreName[] array3 = array;
			for (int i = 0; i < array3.Length; i++)
			{
				StoreName storeName = array3[i];
				StoreLocation[] array4 = array2;
				for (int j = 0; j < array4.Length; j++)
				{
					StoreLocation storeLocation = array4[j];
					X509Certificate2 x509Certificate = CertUtils.TryLoadCertificate(thumbprint, requirePrivateKey, storeName, storeLocation);
					if (x509Certificate != null)
					{
						return x509Certificate;
					}
				}
			}
			return null;
		}

		private static X509Certificate2 TryLoadCertificate(string thumbprint, bool requirePrivateKey, StoreName storeName, StoreLocation storeLocation)
		{
			X509Store x509Store = null;
			X509Certificate2 result;
			try
			{
				x509Store = new X509Store(storeName, storeLocation);
				x509Store.Open(OpenFlags.MaxAllowed);
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
				if (x509Certificate2Collection.Count == 0)
				{
					result = null;
				}
				else
				{
					X509Certificate2 x509Certificate = x509Certificate2Collection[0];
					if (requirePrivateKey && !x509Certificate.HasPrivateKey)
					{
						result = null;
					}
					else
					{
						result = x509Certificate;
					}
				}
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
			return result;
		}
	}
}
