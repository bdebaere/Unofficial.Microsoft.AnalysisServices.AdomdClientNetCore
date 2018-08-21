using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class TransportCapabilities
	{
		public const int TotalSize = 4;

		private const int numberOptions = 5;

		private BitArray m_Data;

		public bool ContentTypeNegotiated
		{
			get
			{
				return this.m_Data[0];
			}
			set
			{
				this.m_Data[0] = value;
			}
		}

		public bool RequestBinary
		{
			get
			{
				return this.m_Data[1];
			}
		}

		public bool RequestCompression
		{
			get
			{
				return this.m_Data[2];
			}
		}

		public bool ResponseBinary
		{
			set
			{
				this.m_Data[3] = value;
			}
		}

		public bool ResponseCompression
		{
			set
			{
				this.m_Data[4] = value;
			}
		}

		public DataType RequestType
		{
			get
			{
				if (!this.ContentTypeNegotiated)
				{
					return DataType.TextXml;
				}
				if (this.RequestBinary && this.RequestCompression)
				{
					return DataType.CompressedBinaryXml;
				}
				if (this.RequestBinary)
				{
					return DataType.BinaryXml;
				}
				if (this.RequestCompression)
				{
					return DataType.CompressedXml;
				}
				return DataType.TextXml;
			}
		}

		public TransportCapabilities()
		{
			this.m_Data = new BitArray(32, false);
		}

		private TransportCapabilities(BitArray bitArray)
		{
			this.m_Data = new BitArray(bitArray);
		}

		public TransportCapabilities Clone()
		{
			return new TransportCapabilities(this.m_Data);
		}

		public void FromBytes(byte[] bytes)
		{
			int i = 0;
			int num = 0;
			int num2 = 0;
			while (i < 32)
			{
				this.m_Data[i] = (0 != ((int)bytes[num] & 1 << num2));
				if (num2 == 7)
				{
					num++;
					num2 = 0;
				}
				else
				{
					num2++;
				}
				i++;
			}
		}

		public byte[] GetBytes()
		{
			byte[] array = new byte[4];
			Array.Clear(array, 0, 4);
			int i = 0;
			int num = 0;
			int num2 = 0;
			while (i < 32)
			{
				byte[] expr_1E_cp_0 = array;
				int expr_1E_cp_1 = num;
				expr_1E_cp_0[expr_1E_cp_1] |= (byte)(this.m_Data[i] ? (1 << num2) : 0);
				if (num2 == 7)
				{
					num++;
					num2 = 0;
				}
				else
				{
					num2++;
				}
				i++;
			}
			return array;
		}

		public void FromString(string optionsStr)
		{
			if (string.IsNullOrEmpty(optionsStr))
			{
				this.m_Data.SetAll(false);
				return;
			}
			string[] array = optionsStr.Split(new char[]
			{
				','
			});
			if (array.Length == 0)
			{
				this.m_Data.SetAll(false);
				return;
			}
			if (array.Length < 5)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "");
			}
			for (int i = 0; i < array.Length; i++)
			{
				int num;
				try
				{
					num = int.Parse(array[i], CultureInfo.InvariantCulture);
				}
				catch (Exception e)
				{
					throw new AdomdUnknownResponseException(e);
				}
				if (num == 0)
				{
					this.m_Data[i] = false;
				}
				else
				{
					if (num != 1)
					{
						throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "");
					}
					this.m_Data[i] = true;
				}
			}
		}

		public string GetString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_Data[0] ? "1" : "0");
			for (int i = 1; i < 5; i++)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(this.m_Data[i] ? "1" : "0");
			}
			return stringBuilder.ToString();
		}
	}
}
