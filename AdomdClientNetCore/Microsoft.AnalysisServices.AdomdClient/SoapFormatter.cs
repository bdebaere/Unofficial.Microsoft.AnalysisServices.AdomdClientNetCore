using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class SoapFormatter
	{
		private XmlaClient client;

		public SoapFormatter(XmlaClient theClient)
		{
			this.client = theClient;
		}

		public ResultsetFormatter ReadResponse(XmlReader reader)
		{
			return this.ReadResponse(reader, InlineErrorHandlingType.StoreInCell);
		}

		public ResultsetFormatter ReadResponse(XmlReader reader, InlineErrorHandlingType inlineErrorHandling)
		{
			ResultsetFormatter result;
			try
			{
				ResultsetFormatter resultsetFormatter = null;
				if (XmlaClient.IsExecuteResponseS(reader))
				{
					resultsetFormatter = SoapFormatter.ReadExecuteResponsePrivate(reader, inlineErrorHandling);
				}
				else if (XmlaClient.IsDiscoverResponseS(reader))
				{
					resultsetFormatter = SoapFormatter.ReadDiscoverResponsePrivate(reader, inlineErrorHandling, null, false, null);
				}
				else
				{
					if (!XmlaClient.IsEmptyResultS(reader))
					{
						throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected execute or discover response, or empty result, got {0}", new object[]
						{
							reader.Name
						}));
					}
					XmlaClient.ReadEmptyRootS(reader);
				}
				result = resultsetFormatter;
			}
			catch (XmlException innerException)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
			catch (IOException innerException2)
			{
				if (this.client != null)
				{
					this.client.Disconnect(false);
				}
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch (AdomdUnknownResponseException)
			{
				throw;
			}
			catch (XmlaException)
			{
				throw;
			}
			catch
			{
				if (this.client != null)
				{
					this.client.Disconnect(false);
				}
				throw;
			}
			finally
			{
				this.EndReceival(reader);
			}
			return result;
		}

		public ResultsetFormatter ReadDiscoverResponse(XmlReader reader, InlineErrorHandlingType inlineErrorHandling, DataTable inTable, Dictionary<string, bool> columnsToConvertTimeFor)
		{
			return this.ReadDiscoverResponse(reader, inlineErrorHandling, inTable, false, columnsToConvertTimeFor);
		}

		public ResultsetFormatter ReadDiscoverResponse(XmlReader reader, InlineErrorHandlingType inlineErrorHandling, DataTable inTable, bool schemaOnly, Dictionary<string, bool> columnsToConvertTimeFor)
		{
			ResultsetFormatter result;
			try
			{
				ResultsetFormatter resultsetFormatter = SoapFormatter.ReadDiscoverResponsePrivate(reader, inlineErrorHandling, inTable, schemaOnly, columnsToConvertTimeFor);
				result = resultsetFormatter;
			}
			catch (XmlException innerException)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
			catch (IOException innerException2)
			{
				if (this.client != null)
				{
					this.client.Disconnect(false);
				}
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch (AdomdUnknownResponseException)
			{
				throw;
			}
			catch (XmlaException)
			{
				throw;
			}
			catch
			{
				if (this.client != null)
				{
					this.client.Disconnect(false);
				}
				throw;
			}
			finally
			{
				this.EndReceival(reader);
			}
			return result;
		}

		internal static MDDatasetFormatter ReadDataSetResponse(XmlReader reader)
		{
			if (!XmlaClient.IsDatasetResponseS(reader))
			{
				throw new InvalidOperationException(XmlaSR.SoapFormatter_ResponseIsNotDataset);
			}
			return SoapFormatter.ReadDataSetResponsePrivate(reader);
		}

		private void EndReceival(XmlReader reader)
		{
			if (!(reader is XmlaReader) || (reader is XmlaReader && !((XmlaReader)reader).IsReaderDetached))
			{
				if (this.client != null && this.client.IsConnected)
				{
					this.client.EndReceival();
					return;
				}
			}
			else
			{
				reader.Close();
			}
		}

		private static ResultsetFormatter ReadExecuteResponsePrivate(XmlReader reader, InlineErrorHandlingType inlineErrorHandling)
		{
			XmlaClient.StartExecuteResponseS(reader);
			if (XmlaClient.IsDatasetResponseS(reader))
			{
				return SoapFormatter.ReadDataSetResponsePrivate(reader);
			}
			if (XmlaClient.IsRowsetResponseS(reader))
			{
				return SoapFormatter.ReadRowsetResponsePrivate(reader, inlineErrorHandling, null, false, null);
			}
			if (!XmlaClient.IsEmptyResultS(reader))
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format("Expected dataset, rowset, or empty result, got {0}", reader.Name));
			}
			XmlaClient.ReadEmptyRootS(reader);
			XmlaClient.EndExecuteResponseS(reader);
			return null;
		}

		private static ResultsetFormatter ReadDiscoverResponsePrivate(XmlReader reader, InlineErrorHandlingType inlineErrorHandling, DataTable inTable, bool schemaOnly, Dictionary<string, bool> columnsToConvertTimeFor)
		{
			XmlaClient.StartDiscoverResponseS(reader);
			if (XmlaClient.IsRowsetResponseS(reader))
			{
				return SoapFormatter.ReadRowsetResponsePrivate(reader, inlineErrorHandling, inTable, schemaOnly, columnsToConvertTimeFor);
			}
			if (!XmlaClient.IsEmptyResultS(reader))
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format("Expected rowset or empty result, got {0}", reader.Name));
			}
			XmlaClient.ReadEmptyRootS(reader);
			XmlaClient.EndDiscoverResponseS(reader);
			return null;
		}

		private static ResultsetFormatter ReadRowsetResponsePrivate(XmlReader reader, InlineErrorHandlingType inlineErrorHandling, DataTable inTable, bool schemaOnly, Dictionary<string, bool> columnsToConvertTimeFor)
		{
			RowsetFormatter rowsetFormatter = new RowsetFormatter();
			if (reader.IsEmptyElement)
			{
				reader.Skip();
			}
			else
			{
				XmlaClient.StartRowsetResponseS(reader);
				rowsetFormatter.ReadRowset(reader, inlineErrorHandling, inTable, inTable == null || inTable.Columns.Count <= 1, schemaOnly, columnsToConvertTimeFor);
				XmlaClient.EndRowsetResponseS(reader);
			}
			return rowsetFormatter;
		}

		private static MDDatasetFormatter ReadDataSetResponsePrivate(XmlReader reader)
		{
			MDDatasetFormatter mDDatasetFormatter = new MDDatasetFormatter();
			if (reader.IsEmptyElement)
			{
				reader.Skip();
			}
			else
			{
				XmlaClient.StartDatasetResponseS(reader);
				mDDatasetFormatter.ReadMDDataset(reader);
				XmlaClient.EndDatasetResponseS(reader);
			}
			return mDDatasetFormatter;
		}
	}
}
