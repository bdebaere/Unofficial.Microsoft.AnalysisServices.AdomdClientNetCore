using System;
using System.Collections;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class AdomdClient : XmlaClient
	{
		internal AdomdClient()
		{
		}

		internal override void WriteProperties(IDictionary connectionProperties, IDictionary commandProperties)
		{
			this.writer.WriteStartElement("Properties");
			this.writer.WriteStartElement("PropertyList");
			if (connectionProperties != null && connectionProperties.Count > 0)
			{
				foreach (DictionaryEntry propertyEntry in connectionProperties)
				{
					if (propertyEntry.Value != null && (commandProperties == null || commandProperties.Count <= 0 || (propertyEntry.Key is IXmlaPropertyKey && !commandProperties.Contains(propertyEntry.Key)) || (!(propertyEntry.Key is IXmlaPropertyKey) && !commandProperties.Contains(new XmlaPropertyKey((string)propertyEntry.Key, null)))))
					{
						this.WriteVersionSafeXmlaProperty(propertyEntry);
					}
				}
			}
			if (commandProperties != null && commandProperties.Count > 0)
			{
				foreach (DictionaryEntry propertyEntry2 in commandProperties)
				{
					if (propertyEntry2.Value != null)
					{
						this.WriteVersionSafeXmlaProperty(propertyEntry2);
					}
				}
			}
			this.writer.WriteEndElement();
			this.writer.WriteEndElement();
		}

		internal override void WriteXmlaProperty(DictionaryEntry entry)
		{
			if (entry.Key is IXmlaPropertyKey)
			{
				IXmlaPropertyKey xmlaPropertyKey = (IXmlaPropertyKey)entry.Key;
				if (entry.Value != null && xmlaPropertyKey.Name != null && xmlaPropertyKey.Name.Length > 0)
				{
					string localName = XmlConvert.EncodeLocalName(xmlaPropertyKey.Name);
					if (xmlaPropertyKey.Namespace == null || xmlaPropertyKey.Namespace.Length == 0)
					{
						this.writer.WriteElementString(localName, FormattersHelpers.ConvertToXml(entry.Value));
						return;
					}
					this.writer.WriteElementString(localName, xmlaPropertyKey.Namespace, FormattersHelpers.ConvertToXml(entry.Value));
					return;
				}
			}
			else
			{
				this.writer.WriteElementString((string)entry.Key, FormattersHelpers.ConvertToXml(entry.Value));
			}
		}

		private void WriteVersionSafeXmlaProperty(DictionaryEntry propertyEntry)
		{
			if (propertyEntry.Key is IXmlaPropertyKey)
			{
				IXmlaPropertyKey xmlaPropertyKey = (IXmlaPropertyKey)propertyEntry.Key;
				if (this.IsVersionSafeProperty(xmlaPropertyKey.Name))
				{
					this.WriteXmlaProperty(propertyEntry);
					return;
				}
			}
			else if (this.IsVersionSafeProperty((string)propertyEntry.Key))
			{
				this.WriteXmlaProperty(propertyEntry);
			}
		}

		private bool IsVersionSafeProperty(string propertyName)
		{
			if (propertyName.Equals("DbpropMsmdCurrentActivityID", StringComparison.OrdinalIgnoreCase))
			{
				if (base.SupportsCurrentActivityID)
				{
					return true;
				}
			}
			else if (propertyName.Equals("DbpropMsmdRequestID", StringComparison.OrdinalIgnoreCase))
			{
				if (base.SupportsActivityIDAndRequestID)
				{
					return true;
				}
			}
			else if (propertyName.Equals("DbpropMsmdActivityID", StringComparison.OrdinalIgnoreCase))
			{
				if (base.SupportsActivityIDAndRequestID)
				{
					return true;
				}
			}
			else
			{
				if (!propertyName.Equals("DbPropmsmdRequestMemoryLimit", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if (!base.ConnectionInfo.IsOnPremFromCloudAccess)
				{
					return true;
				}
			}
			return false;
		}
	}
}
