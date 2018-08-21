using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal static class SqlBinaryReaderCreator
	{
		private static MethodInfo createSqlReaderMethodInfo;

		static SqlBinaryReaderCreator()
		{
			SqlBinaryReaderCreator.createSqlReaderMethodInfo = typeof(XmlReader).GetMethod("CreateSqlReader", BindingFlags.Static | BindingFlags.NonPublic);
		}

		internal static XmlReader CreateReader(Stream stream)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			object[] array = new object[3];
			array[0] = stream;
			array[1] = xmlReaderSettings;
			object[] parameters = array;
			return (XmlReader)SqlBinaryReaderCreator.createSqlReaderMethodInfo.Invoke(null, parameters);
		}
	}
}
