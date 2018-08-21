using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[XmlRoot(ElementName = "root")]
	public class CacheContent
	{
		public string DataSource
		{
			get;
			set;
		}

		public string CacheDataAsString
		{
			get;
			set;
		}

		internal CacheContent()
		{
		}

		internal CacheContent(string dataSource, byte[] protectedData)
		{
			this.DataSource = dataSource;
			this.CacheDataAsString = BitConverter.ToString(protectedData).Replace("-", string.Empty);
		}

		internal void Serialize(string cacheFilePath)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(CacheContent));
			TextWriter textWriter = new StreamWriter(cacheFilePath);
			xmlSerializer.Serialize(textWriter, this);
			textWriter.Close();
		}

		internal void Deserialize(string cacheFilePath)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(CacheContent));
			FileStream fileStream = new FileStream(cacheFilePath, FileMode.Open);
			CacheContent cacheContent = (CacheContent)xmlSerializer.Deserialize(fileStream);
			this.DataSource = cacheContent.DataSource;
			this.CacheDataAsString = cacheContent.CacheDataAsString;
			fileStream.Close();
		}

		internal byte[] GetProtectedData()
		{
			if (string.IsNullOrEmpty(this.CacheDataAsString))
			{
				return null;
			}
			return Array.ConvertAll<string, byte>((from x in Regex.Split(this.CacheDataAsString, "(?<=\\G.{2})")
			where x != string.Empty
			select x).ToArray<string>(), (string s) => Convert.ToByte(s, 16));
		}
	}
}
