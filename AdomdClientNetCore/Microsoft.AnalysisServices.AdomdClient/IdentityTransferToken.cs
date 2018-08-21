using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class IdentityTransferToken
	{
		public string IdentityProviderName
		{
			get;
			private set;
		}

		public string IdentityName
		{
			get;
			private set;
		}

		public DateTime IssueDate
		{
			get;
			private set;
		}

		public bool AssumeDbAdmin
		{
			get;
			private set;
		}

		public TokenSigner Signer
		{
			get;
			private set;
		}

		public IdentityTransferToken(string identityProviderName, string identityName) : this(identityProviderName, identityName, null)
		{
		}

		public IdentityTransferToken(string identityProviderName, string identityName, TokenSigner signer) : this(identityProviderName, identityName, DateTime.UtcNow, false, signer)
		{
		}

		public IdentityTransferToken(string identityProviderName, string identityName, bool assumeDbAdmin, TokenSigner signer) : this(identityProviderName, identityName, DateTime.UtcNow, assumeDbAdmin, signer)
		{
		}

		public IdentityTransferToken(string identityProviderName, string identityName, DateTime issueDate, bool assumeDbAdmin, TokenSigner signer)
		{
			if (identityProviderName == null)
			{
				throw new ArgumentNullException("identityProviderName");
			}
			if (identityName == null)
			{
				throw new ArgumentNullException("identityName");
			}
			this.IdentityProviderName = identityProviderName;
			this.IdentityName = identityName;
			this.IssueDate = issueDate;
			this.Signer = signer;
			this.AssumeDbAdmin = assumeDbAdmin;
		}

		public string GetTokenXml()
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings
			{
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("IdentityTransferToken");
				xmlWriter.WriteStartElement("IdentityProvider");
				xmlWriter.WriteString(this.IdentityProviderName);
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("Name");
				xmlWriter.WriteString(this.IdentityName);
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("IssueDate");
				xmlWriter.WriteString(XmlConvert.ToString(this.IssueDate, XmlDateTimeSerializationMode.Utc));
				xmlWriter.WriteEndElement();
				if (this.Signer != null)
				{
					byte[] inArray = this.Signer.Sign(this.GetBlobForSigning());
					xmlWriter.WriteStartElement("SigningCertificateThumbprint");
					xmlWriter.WriteString(Convert.ToBase64String(this.Signer.CertificateThumbprint));
					xmlWriter.WriteEndElement();
					xmlWriter.WriteStartElement("DigestMethod");
					xmlWriter.WriteString(this.Signer.DigestMethod);
					xmlWriter.WriteEndElement();
					xmlWriter.WriteStartElement("Signature");
					xmlWriter.WriteString(Convert.ToBase64String(inArray));
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
			}
			return stringBuilder.ToString();
		}

		private byte[] GetBlobForSigning()
		{
			byte[] bytes = Encoding.UTF8.GetBytes("@@");
			byte[] bytes2 = Encoding.UTF8.GetBytes(this.IdentityProviderName);
			byte[] bytes3 = Encoding.UTF8.GetBytes(this.IdentityName);
			byte[] bytes4 = Encoding.UTF8.GetBytes(XmlConvert.ToString(this.IssueDate, XmlDateTimeSerializationMode.Utc));
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(bytes2, 0, bytes2.Length);
				memoryStream.Write(bytes, 0, bytes.Length);
				memoryStream.Write(bytes3, 0, bytes3.Length);
				memoryStream.Write(bytes, 0, bytes.Length);
				memoryStream.Write(bytes4, 0, bytes4.Length);
				memoryStream.Write(bytes, 0, bytes.Length);
				memoryStream.Flush();
				result = memoryStream.ToArray();
			}
			return result;
		}
	}
}
