using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class DataTypes
	{
		internal const string TextXml = "text/xml";

		internal const string BinaryXml = "application/sx";

		internal const string CompressedXml = "application/xml+xpress";

		internal const string CompressedBinaryXml = "application/sx+xpress";

		private DataTypes()
		{
		}

		public static bool IsSupportedDataType(string dataType)
		{
			return string.Compare(dataType, "text/xml", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(dataType, "application/xml+xpress", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(dataType, "application/sx", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(dataType, "application/sx+xpress", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static DataType GetDataTypeFromString(string dataType)
		{
			if (string.Compare(dataType, "text/xml", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return DataType.TextXml;
			}
			if (string.Compare(dataType, "application/xml+xpress", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return DataType.CompressedXml;
			}
			if (string.Compare(dataType, "application/sx", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return DataType.BinaryXml;
			}
			if (string.Compare(dataType, "application/sx+xpress", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return DataType.CompressedBinaryXml;
			}
			return DataType.Unknown;
		}

		public static string GetDataTypeFromEnum(DataType dataType)
		{
			switch (dataType)
			{
			case DataType.TextXml:
				return "text/xml";
			case DataType.BinaryXml:
				return "application/sx";
			case DataType.CompressedXml:
				return "application/xml+xpress";
			case DataType.CompressedBinaryXml:
				return "application/sx+xpress";
			default:
				throw new ArgumentOutOfRangeException("dataType", XmlaSR.Dime_DataTypeNotSupported(dataType.ToString()));
			}
		}
	}
}
