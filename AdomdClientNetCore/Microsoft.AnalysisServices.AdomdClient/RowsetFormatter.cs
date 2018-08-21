using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class RowsetFormatter : ResultsetFormatter
	{
		internal class DataSetCreator
		{
			private const string MainTableName = "rowsetTable";

			private DataSet dataset;

			private bool setColumnTypes;

			private Dictionary<string, bool> columnsToConvertTimeFor;

			public DataSet DataSet
			{
				get
				{
					return this.dataset;
				}
			}

			public DataSetCreator(DataSet dataset, bool setColumnTypes, Dictionary<string, bool> columnsToConvertTimeFor)
			{
				if (dataset == null)
				{
					dataset = new DataSet();
					dataset.Locale = CultureInfo.InvariantCulture;
				}
				this.dataset = dataset;
				if (!this.dataset.Tables.Contains("rowsetTable"))
				{
					DataTable dataTable = new DataTable("rowsetTable");
					dataTable.Locale = CultureInfo.InvariantCulture;
					this.dataset.Tables.Add(dataTable);
				}
				this.setColumnTypes = setColumnTypes;
				this.columnsToConvertTimeFor = columnsToConvertTimeFor;
			}

			public object ConstructDataSetColumnDelegate(int ordinal, string name, string theNamespace, string caption, Type type, bool isNested, object parent, string strColumnXsdType)
			{
				object result = parent;
				DataTable dataTable;
				if (parent == null)
				{
					dataTable = this.dataset.Tables["rowsetTable"];
				}
				else
				{
					dataTable = (parent as DataTable);
				}
				if (isNested)
				{
					DataColumn dataColumn = new DataColumn(name, typeof(int));
					dataColumn.Caption = name;
					dataColumn.AutoIncrement = true;
					FormattersHelpers.SetColumnNamespace(dataColumn, theNamespace);
					dataTable.Columns.Add(dataColumn);
					string text = dataTable.TableName + name;
					DataTable dataTable2;
					if (!this.dataset.Tables.Contains(text))
					{
						dataTable2 = new DataTable(text);
						dataTable2.Locale = CultureInfo.InvariantCulture;
						this.dataset.Tables.Add(dataTable2);
					}
					else
					{
						dataTable2 = this.dataset.Tables[text];
					}
					DataColumn dataColumn2 = new DataColumn(text, typeof(int));
					dataColumn2.Caption = text;
					dataTable2.Columns.Add(dataColumn2);
					this.dataset.Relations.Add(text, dataColumn, dataColumn2, false);
					result = dataTable2;
				}
				else
				{
					RowsetFormatter.AddColumn(dataTable, name, theNamespace, caption, type, this.setColumnTypes, strColumnXsdType, this.columnsToConvertTimeFor);
				}
				return result;
			}
		}

		private class DataTableCreator
		{
			private DataTable datatable;

			private bool setColumnTypes;

			private Dictionary<string, bool> columnsToConvertTimeFor;

			public DataTableCreator(DataTable datatable, bool setColumnTypes, Dictionary<string, bool> columnsToConvertTimeFor)
			{
				if (datatable == null)
				{
					datatable = new DataTable("rowsetTable");
					datatable.Locale = CultureInfo.InvariantCulture;
				}
				this.datatable = datatable;
				this.setColumnTypes = setColumnTypes;
				this.columnsToConvertTimeFor = columnsToConvertTimeFor;
			}

			public object ConstructDataTableColumnDelegate(int ordinal, string name, string theNamespace, string caption, Type type, bool isNested, object parent, string strColumnXsdType)
			{
				object result = parent;
				if (parent == null)
				{
					if (isNested)
					{
						DataColumn dataColumn = new DataColumn(name, typeof(int));
						dataColumn.Caption = name;
						dataColumn.AutoIncrement = true;
						FormattersHelpers.SetColumnNamespace(dataColumn, theNamespace);
						this.datatable.Columns.Add(dataColumn);
						result = this.datatable;
					}
					else
					{
						RowsetFormatter.AddColumn(this.datatable, name, theNamespace, caption, type, this.setColumnTypes, strColumnXsdType, this.columnsToConvertTimeFor);
					}
				}
				return result;
			}
		}

		internal const string rowsetTable = "rowsetTable";

		private DataSet rowsetDataset;

		internal DataTable MainRowsetTable
		{
			get
			{
				if (this.rowsetDataset != null && this.rowsetDataset.Tables.Contains("rowsetTable"))
				{
					return this.rowsetDataset.Tables["rowsetTable"];
				}
				return null;
			}
		}

		internal DataSet RowsetDataset
		{
			get
			{
				return this.rowsetDataset;
			}
		}

		public void ReadRowset(XmlReader reader, InlineErrorHandlingType inlineErrorHandling, DataTable inTable, bool doCreate, bool schemaOnly, Dictionary<string, bool> columnsToConvertTimeFor)
		{
			if (doCreate)
			{
				this.CreateDataset(reader, inlineErrorHandling == InlineErrorHandlingType.Throw || inlineErrorHandling == InlineErrorHandlingType.StoreInErrorsCollection, inTable, columnsToConvertTimeFor);
			}
			if (!schemaOnly)
			{
				RowsetFormatter.PopulateDataset(reader, inlineErrorHandling, (inTable == null) ? this.rowsetDataset.Tables["rowsetTable"] : inTable);
			}
		}

		internal static void PopulateDataset(XmlReader reader, string rowElementName, string rowNamespace, InlineErrorHandlingType inlineErrorHandling, DataTable inTable)
		{
			DataSet dataSet = inTable.DataSet;
			if (dataSet != null)
			{
			    IEnumerator enumerator = dataSet.Tables.GetEnumerator();
				{
					while (enumerator.MoveNext())
					{
						DataTable dataTable = (DataTable)enumerator.Current;
						dataTable.BeginLoadData();
					}
					goto IL_50;
				}
			}
			inTable.BeginLoadData();
			IL_50:
			RowsetFormatter.PopulateMultiRow(reader, rowElementName, rowNamespace, inTable, inlineErrorHandling, -1);
			if (dataSet != null)
			{
				IEnumerator enumerator2 = dataSet.Tables.GetEnumerator();
				{
					while (enumerator2.MoveNext())
					{
						DataTable dataTable2 = (DataTable)enumerator2.Current;
						dataTable2.EndLoadData();
					}
					return;
				}
			}
			inTable.EndLoadData();
		}

		private void CreateDataset(XmlReader reader, bool setColumnTypes, DataTable inTable, Dictionary<string, bool> columnsToConvertTimeFor)
		{
			if (inTable != null)
			{
				RowsetFormatter.DataTableCreator @object = new RowsetFormatter.DataTableCreator(inTable, setColumnTypes, columnsToConvertTimeFor);
				FormattersHelpers.LoadSchema(reader, new FormattersHelpers.ColumnDefinitionDelegate(@object.ConstructDataTableColumnDelegate));
				return;
			}
			RowsetFormatter.DataSetCreator dataSetCreator = new RowsetFormatter.DataSetCreator(this.rowsetDataset, setColumnTypes, columnsToConvertTimeFor);
			FormattersHelpers.LoadSchema(reader, new FormattersHelpers.ColumnDefinitionDelegate(dataSetCreator.ConstructDataSetColumnDelegate));
			this.rowsetDataset = dataSetCreator.DataSet;
		}

		private static void PopulateDataset(XmlReader reader, InlineErrorHandlingType inlineErrorHandling, DataTable inTable)
		{
			RowsetFormatter.PopulateMultiRow(reader, FormattersHelpers.RowElement, FormattersHelpers.RowElementNamespace, inTable, inlineErrorHandling, -1);
		}

		private static void PopulateMultiRow(XmlReader reader, string elementName, string elementNamespace, DataTable datatable, InlineErrorHandlingType inlineErrorHandling, int parentRowIndex)
		{
			DataColumn parentColumn = null;
			if (parentRowIndex >= 0 && datatable.Columns.Contains(datatable.TableName))
			{
				parentColumn = datatable.Columns[datatable.TableName];
			}
			while (reader.IsStartElement(elementName, elementNamespace))
			{
				if (reader.IsEmptyElement)
				{
					reader.ReadStartElement();
					DataRow row = RowsetFormatter.CreateNewRow(datatable, parentColumn, parentRowIndex);
					datatable.Rows.Add(row);
				}
				else
				{
					reader.ReadStartElement();
					RowsetFormatter.PopulateRow(reader, datatable, inlineErrorHandling, parentRowIndex, parentColumn);
					reader.ReadEndElement();
				}
			}
			FormattersHelpers.CheckException(reader);
		}

		private static DataRow CreateNewRow(DataTable datatable, DataColumn parentColumn, int parentRowIndex)
		{
			DataRow dataRow = datatable.NewRow();
			if (parentColumn != null)
			{
				dataRow[parentColumn] = parentRowIndex;
			}
			return dataRow;
		}

		private static void PopulateRow(XmlReader reader, DataTable datatable, InlineErrorHandlingType inlineErrorHandling, int parentRowIndex, DataColumn parentColumn)
		{
			DataRow dataRow = RowsetFormatter.CreateNewRow(datatable, parentColumn, parentRowIndex);
			while (reader.IsStartElement())
			{
				FormattersHelpers.CheckException(reader);
				if (!FormattersHelpers.IsNullContentElement(reader))
				{
					string localName = reader.LocalName;
					DataColumn dataColumn = datatable.Columns[localName];
					if (dataColumn == null)
					{
						throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Unknown column {0}", new object[]
						{
							localName
						}));
					}
					string columnNamespace = FormattersHelpers.GetColumnNamespace(dataColumn);
					if (reader.NamespaceURI == columnNamespace)
					{
						if (RowsetFormatter.IsNestedColumn(dataColumn))
						{
							DataTable dataTable = RowsetFormatter.FindNestedTable(dataColumn);
							if (dataTable != null)
							{
								RowsetFormatter.PopulateMultiRow(reader, localName, columnNamespace, dataTable, inlineErrorHandling, (int)dataRow[dataColumn]);
							}
							else
							{
								reader.Skip();
							}
						}
						else
						{
							Type elementType = FormattersHelpers.GetElementType(reader, "http://www.w3.org/2001/XMLSchema-instance", dataColumn);
							bool throwOnInlineError = inlineErrorHandling == InlineErrorHandlingType.Throw;
							object obj = FormattersHelpers.ReadRowsetProperty(reader, localName, columnNamespace, elementType, throwOnInlineError, elementType.IsArray && FormattersHelpers.GetColumnXsdTypeName(dataColumn) != "base64Binary", FormattersHelpers.GetConvertToLocalTime(dataColumn));
							if (inlineErrorHandling == InlineErrorHandlingType.StoreInErrorsCollection)
							{
								XmlaError xmlaError = obj as XmlaError;
								if (xmlaError == null)
								{
									dataRow[localName] = obj;
								}
								else
								{
									dataRow.SetColumnError(localName, xmlaError.Description);
								}
							}
							else
							{
								dataRow[localName] = obj;
							}
						}
					}
					else
					{
						reader.Skip();
					}
				}
				else
				{
					reader.Skip();
				}
			}
			datatable.Rows.Add(dataRow);
		}

		private static DataTable FindNestedTable(DataColumn column)
		{
			string name = column.Table.TableName + column.ColumnName;
			if (!column.Table.ChildRelations.Contains(name))
			{
				return null;
			}
			return column.Table.ChildRelations[name].ChildTable;
		}

		private static bool IsNestedColumn(DataColumn column)
		{
			return column.AutoIncrement;
		}

		private static void AddColumn(DataTable table, string colName, string colNamespace, string caption, Type colType, bool setColumnTypes, string strColumnXsdType, Dictionary<string, bool> columnsToConvertTimeFor)
		{
			DataColumn dataColumn;
			if (setColumnTypes)
			{
				dataColumn = new DataColumn(colName, colType);
			}
			else
			{
				dataColumn = new DataColumn(colName, typeof(object));
			}
			FormattersHelpers.SetColumnType(dataColumn, colType);
			FormattersHelpers.SetColumnNamespace(dataColumn, colNamespace);
			FormattersHelpers.SetColumnXsdTypeName(dataColumn, strColumnXsdType);
			if (columnsToConvertTimeFor != null && columnsToConvertTimeFor.ContainsKey(colName))
			{
				FormattersHelpers.SetConvertToLocalTime(dataColumn, true);
				if (setColumnTypes)
				{
					dataColumn.DateTimeMode = DataSetDateTime.Local;
				}
			}
			dataColumn.Caption = caption;
			try
			{
				table.Columns.Add(dataColumn);
			}
			catch (DuplicateNameException)
			{
			}
		}
	}
}
