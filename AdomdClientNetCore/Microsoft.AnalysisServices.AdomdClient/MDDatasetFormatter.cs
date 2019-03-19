using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
    internal class MDDatasetFormatter : ResultsetFormatter
    {
        private class DataSetFormatterAxis : IDSFDataSet, ICollection, IEnumerable
        {
            private ArrayList tables;

            private Dictionary<string, DataTable> hash;

            private string name;

            public string DataSetName
            {
                get
                {
                    return this.name;
                }
            }

            int ICollection.Count
            {
                get
                {
                    return this.tables.Count;
                }
            }

            DataTable IDSFDataSet.this[int index]
            {
                get
                {
                    return (DataTable)this.tables[index];
                }
            }

            DataTable IDSFDataSet.this[string index]
            {
                get
                {
                    return this.hash[index];
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this.tables.SyncRoot;
                }
            }

            internal DataSetFormatterAxis(string datasetName)
            {
                this.name = datasetName;
                this.tables = new ArrayList();
                this.hash = new Dictionary<string, DataTable>(StringComparer.OrdinalIgnoreCase);
            }

            internal int AddTable(DataTable table)
            {
                if (this.hash.ContainsKey(table.TableName))
                {
                    throw new AdomdUnknownResponseException(SR.DatasetResponse_HierarchyWithSameNameOnSameAxis(table.TableName, this.name), "");
                }
                this.tables.Add(table);
                this.hash[table.TableName] = table;
                return this.tables.Count - 1;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.tables.GetEnumerator();
            }

            bool IDSFDataSet.Contains(string tableName)
            {
                return this.hash.ContainsKey(tableName);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                this.tables.CopyTo(array, index);
            }
        }

        private class DataSetFormatterAxisCollection : IDSFAxisCollection, ICollection, IEnumerable
        {
            private ArrayList collection;

            public IDSFDataSet this[int index]
            {
                get
                {
                    return (IDSFDataSet)this.collection[index];
                }
            }

            int ICollection.Count
            {
                get
                {
                    return this.collection.Count;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this.collection.SyncRoot;
                }
            }

            internal DataSetFormatterAxisCollection()
            {
                this.collection = new ArrayList();
            }

            internal int Add(IDSFDataSet set)
            {
                return this.collection.Add(set);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                this.collection.CopyTo(array, index);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.collection.GetEnumerator();
            }
        }

        private const string dataSetNamespace = "urn:schemas-microsoft-com:xml-analysis:mddataset";

        private const string xsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

        private const string ddlNamespace = "http://schemas.microsoft.com/analysisservices/2003/engine";

        private const string olapInfoElement = "OlapInfo";

        private const string axesInfoElement = "AxesInfo";

        private const string axisInfoElement = "AxisInfo";

        private const string hierarchyInfoElement = "HierarchyInfo";

        private const string elementCellInfo = "CellInfo";

        private const string slicerAxisName = "SlicerAxis";

        private const string axesElement = "Axes";

        private const string axisElement = "Axis";

        private const string tuplesElement = "Tuples";

        private const string tupleElement = "Tuple";

        private const string memberElement = "Member";

        private const string cellDataElement = "CellData";

        private const string cellElement = "Cell";

        internal const string cubeInfoElement = "CubeInfo";

        internal const string cubeElement = "Cube";

        internal const string cubeNameElement = "CubeName";

        internal const string lastDataUpdateElement = "LastDataUpdate";

        internal const string lastSchemaUpdateElement = "LastSchemaUpdate";

        private const string nameAtribute = "name";

        private const string hierarchyAttribute = "Hierarchy";

        internal const string cellOrdinalAttribute = "CellOrdinal";

        internal const string valueProperty = "Value";

        internal const string frmValueProperty = "FmtValue";

        internal const string uniqueNameProperty = "UName";

        internal const string levelNameProperty = "LName";

        internal const string levelNumberProperty = "LNum";

        internal const string captionProperty = "Caption";

        internal const string parentProperty = "Parent";

        internal const string descriptionProperty = "Description";

        internal const string displayInfoProperty = "DisplayInfo";

        internal const string MemberPropertiesIndexMapProp = "MemberProperties";

        private MDDatasetFormatter.DataSetFormatterAxis filterAxis;

        private MDDatasetFormatter.DataSetFormatterAxisCollection axesList;

        private DataTable cellTable;

        private DataTable cubesInfos;

        internal IDSFAxisCollection AxesList
        {
            get
            {
                return this.axesList;
            }
        }

        internal DataTable CellTable
        {
            get
            {
                return this.cellTable;
            }
        }

        internal IDSFDataSet FilterAxis
        {
            get
            {
                return this.filterAxis;
            }
        }

        internal DataTable CubesInfos
        {
            get
            {
                return this.cubesInfos;
            }
        }

        internal string CubeName
        {
            get
            {
                if (this.cubesInfos == null || this.cubesInfos.Rows.Count == 0)
                {
                    return null;
                }
                return this.cubesInfos.Rows[0]["CubeName"] as string;
            }
        }

        internal MDDatasetFormatter()
        {
            this.axesList = new MDDatasetFormatter.DataSetFormatterAxisCollection();
        }

        public void ReadMDDataset(XmlReader reader)
        {
            if (reader.IsStartElement("schema", "http://www.w3.org/2001/XMLSchema"))
            {
                this.ReadXSD(reader);
            }
            if (reader.IsStartElement("OlapInfo", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
            {
                this.ReadOlapInfo(reader);
            }
            this.ReadMembers(reader);
            this.ReadCells(reader);
        }

        private void ReadMembers(XmlReader reader)
        {
            if (!reader.IsStartElement("Axes", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
            {
                if (reader.IsEmptyElement)
                {
                    FormattersHelpers.CheckException(reader);
                }
                return;
            }
            if (reader.IsEmptyElement)
            {
                FormattersHelpers.CheckException(reader);
                reader.Skip();
                return;
            }
            this.BeginLoadAxisTables();
            reader.ReadStartElement("Axes", "urn:schemas-microsoft-com:xml-analysis:mddataset");
            int num = 0;
            while (reader.IsStartElement("Axis", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
            {
                string attribute = reader.GetAttribute("name");
                reader.ReadStartElement();
                IDSFDataSet iDSFDataSet;
                if (attribute != "SlicerAxis")
                {
                    iDSFDataSet = this.axesList[num++];
                }
                else
                {
                    iDSFDataSet = this.filterAxis;
                }
                reader.MoveToContent();
                if (!reader.IsEmptyElement)
                {
                    reader.ReadStartElement("Tuples", "urn:schemas-microsoft-com:xml-analysis:mddataset");
                    while (reader.IsStartElement("Tuple", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
                    {
                        reader.ReadStartElement();
                        while (reader.IsStartElement("Member", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
                        {
                            DataTable dataTable = iDSFDataSet[reader.GetAttribute("Hierarchy")];
                            reader.ReadStartElement();
                            DataRow dataRow = dataTable.NewRow();
                            DataColumnCollection columns = dataTable.Columns;
                            while (reader.IsStartElement())
                            {
                                FormattersHelpers.CheckException(reader);
                                DataColumn dataColumn = columns[reader.Name];
                                if (dataColumn == null)
                                {
                                    throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Unexpected element {0}", new object[]
                                    {
                                        reader.Name
                                    }));
                                }
                                if (!FormattersHelpers.IsNullContentElement(reader))
                                {
                                    Type elementType = FormattersHelpers.GetElementType(reader, "http://www.w3.org/2001/XMLSchema-instance", dataColumn);
                                    dataRow[dataColumn] = FormattersHelpers.ReadDataSetProperty(reader, elementType);
                                }
                                else
                                {
                                    reader.Skip();
                                }
                            }
                            dataTable.Rows.Add(dataRow);
                            reader.ReadEndElement();
                        }
                        FormattersHelpers.CheckException(reader);
                        reader.ReadEndElement();
                    }
                    FormattersHelpers.CheckException(reader);
                    reader.ReadEndElement();
                }
                else
                {
                    FormattersHelpers.CheckException(reader);
                    reader.ReadStartElement();
                }
                reader.ReadEndElement();
            }
            FormattersHelpers.CheckException(reader);
            reader.ReadEndElement();
            this.EndLoadAxisTables();
        }

        private void BeginLoadAxisTables()
        {
            foreach (IDSFDataSet iDSFDataSet in ((IEnumerable)this.axesList))
            {
                foreach (DataTable dataTable in iDSFDataSet)
                {
                    dataTable.BeginLoadData();
                }
            }
            if (this.filterAxis != null)
            {
                foreach (DataTable dataTable2 in ((IEnumerable)this.filterAxis))
                {
                    dataTable2.BeginLoadData();
                }
            }
        }

        private void EndLoadAxisTables()
        {
            foreach (IDSFDataSet iDSFDataSet in ((IEnumerable)this.axesList))
            {
                foreach (DataTable dataTable in iDSFDataSet)
                {
                    dataTable.EndLoadData();
                }
            }
            if (this.filterAxis != null)
            {
                foreach (DataTable dataTable2 in ((IEnumerable)this.filterAxis))
                {
                    dataTable2.EndLoadData();
                }
            }
        }

        private void ReadXSD(XmlReader reader)
        {
            reader.Skip();
        }

        private void ReadOlapInfo(XmlReader reader)
        {
            reader.ReadStartElement();
            this.ReadCubeInfo(reader);
            this.ReadAxesInfo(reader);
            this.ReadCellInfo(reader);
            reader.ReadEndElement();
        }

        private void ReadAxesInfo(XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.IsEmptyElement)
            {
                FormattersHelpers.CheckException(reader);
                if (reader.IsStartElement("AxesInfo", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
                {
                    reader.Skip();
                }
                return;
            }
            reader.ReadStartElement("AxesInfo", "urn:schemas-microsoft-com:xml-analysis:mddataset");
            while (reader.IsStartElement("AxisInfo", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
            {
                MDDatasetFormatter.DataSetFormatterAxis dataSetFormatterAxis = new MDDatasetFormatter.DataSetFormatterAxis(reader.GetAttribute("name"));
                if (!reader.IsEmptyElement)
                {
                    reader.ReadStartElement();
                    while (reader.IsStartElement("HierarchyInfo", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
                    {
                        DataTable dataTable = new DataTable(reader.GetAttribute("name"));
                        dataTable.Locale = CultureInfo.InvariantCulture;
                        dataSetFormatterAxis.AddTable(dataTable);
                        Collection<int> collection = new Collection<int>();
                        dataTable.ExtendedProperties["MemberProperties"] = collection;
                        reader.ReadStartElement();
                        DataColumnCollection columns = dataTable.Columns;
                        while (reader.IsStartElement())
                        {
                            FormattersHelpers.CheckException(reader);
                            try
                            {
                                DataColumn dataColumn = new DataColumn(reader.Name, typeof(object));
                                dataColumn.Namespace = reader.NamespaceURI;
                                FormattersHelpers.SetColumnType(dataColumn, FormattersHelpers.GetElementType(reader));
                                dataColumn.Caption = reader.GetAttribute("name");
                                columns.Add(dataColumn);
                                collection.Add(columns.Count - 1);
                            }
                            catch (DuplicateNameException)
                            {
                                collection.Add(columns.IndexOf(reader.Name));
                            }
                            reader.Skip();
                        }
                        reader.ReadEndElement();
                    }
                    FormattersHelpers.CheckException(reader);
                    reader.ReadEndElement();
                }
                else
                {
                    FormattersHelpers.CheckException(reader);
                    reader.Skip();
                }
                if ("SlicerAxis" == dataSetFormatterAxis.DataSetName)
                {
                    this.filterAxis = dataSetFormatterAxis;
                }
                else
                {
                    this.axesList.Add(dataSetFormatterAxis);
                }
            }
            FormattersHelpers.CheckException(reader);
            reader.ReadEndElement();
        }

        private void ReadCellInfo(XmlReader reader)
        {
            this.cellTable = new DataTable();
            this.cellTable.Locale = CultureInfo.InvariantCulture;
            Collection<int> collection = new Collection<int>();
            this.cellTable.ExtendedProperties["MemberProperties"] = collection;
            DataColumn dataColumn = new DataColumn("CellOrdinal", typeof(int));
            dataColumn.Caption = "CellOrdinal";
            dataColumn.Namespace = "urn:schemas-microsoft-com:xml-analysis:mddataset";
            this.cellTable.Columns.Add(dataColumn);
            collection.Add(this.cellTable.Columns.Count - 1);
            reader.MoveToContent();
            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement("CellInfo", "urn:schemas-microsoft-com:xml-analysis:mddataset");
                while (reader.IsStartElement())
                {
                    FormattersHelpers.CheckException(reader);
                    if (reader.Name != "CellOrdinal")
                    {
                        goto IL_136;
                    }
                    if (reader.NamespaceURI != "urn:schemas-microsoft-com:xml-analysis:mddataset")
                    {
                        goto Block_4;
                    }
                    IL_1BF:
                    reader.Skip();
                    continue;
                    Block_4:
                    IL_136:
                    try
                    {
                        dataColumn = new DataColumn(reader.Name, typeof(object));
                        dataColumn.Namespace = reader.NamespaceURI;
                        FormattersHelpers.SetColumnType(dataColumn, FormattersHelpers.GetElementType(reader));
                        dataColumn.Caption = reader.GetAttribute("name");
                        this.cellTable.Columns.Add(dataColumn);
                        collection.Add(this.cellTable.Columns.Count - 1);
                    }
                    catch (DuplicateNameException)
                    {
                        collection.Add(this.cellTable.Columns.IndexOf(reader.Name));
                    }
                    goto IL_1BF;
                }
                reader.ReadEndElement();
                return;
            }
            FormattersHelpers.CheckException(reader);
            if (reader.IsStartElement("CellInfo", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
            {
                reader.Skip();
                return;
            }
            throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected element {0}:{1}, got {2}", new object[]
            {
                "urn:schemas-microsoft-com:xml-analysis:mddataset",
                "CellInfo",
                reader.Name
            }));
        }

        private void ReadCubeInfo(XmlReader reader)
        {
            if (!reader.IsStartElement("CubeInfo", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
            {
                FormattersHelpers.CheckException(reader);
                return;
            }
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return;
            }
            reader.ReadStartElement();
            this.CreateCubesInfosTable();
            this.cubesInfos.BeginLoadData();
            while (reader.IsStartElement("Cube", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
            {
                reader.ReadStartElement();
                DataRow dataRow = this.cubesInfos.NewRow();
                if (reader.IsStartElement("CubeName", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
                {
                    dataRow["CubeName"] = FormattersHelpers.ReadDataSetProperty(reader, typeof(string));
                }
                if (reader.IsStartElement("LastDataUpdate", "http://schemas.microsoft.com/analysisservices/2003/engine"))
                {
                    dataRow["LastDataUpdate"] = MDDatasetFormatter.GetDateTimeIfValid(reader);
                }
                if (reader.IsStartElement("LastSchemaUpdate", "http://schemas.microsoft.com/analysisservices/2003/engine"))
                {
                    dataRow["LastSchemaUpdate"] = MDDatasetFormatter.GetDateTimeIfValid(reader);
                }
                this.cubesInfos.Rows.Add(dataRow);
                FormattersHelpers.CheckException(reader);
                reader.ReadEndElement();
            }
            this.cubesInfos.EndLoadData();
            FormattersHelpers.CheckException(reader);
            reader.ReadEndElement();
        }

        private void ReadCells(XmlReader reader)
        {
            if (!reader.IsStartElement("CellData", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
            {
                if (reader.IsEmptyElement)
                {
                    FormattersHelpers.CheckException(reader);
                }
                return;
            }
            if (reader.IsEmptyElement)
            {
                FormattersHelpers.CheckException(reader);
                reader.Skip();
                return;
            }
            this.cellTable.BeginLoadData();
            DataColumn column = this.cellTable.Columns["CellOrdinal"];
            reader.ReadStartElement("CellData", "urn:schemas-microsoft-com:xml-analysis:mddataset");
            while (reader.IsStartElement("Cell", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
            {
                DataRow dataRow = this.cellTable.NewRow();
                string attribute = reader.GetAttribute("CellOrdinal");
                dataRow[column] = XmlConvert.ToInt32(attribute);
                if (!reader.IsEmptyElement)
                {
                    reader.ReadStartElement();
                    while (reader.IsStartElement())
                    {
                        FormattersHelpers.CheckException(reader);
                        DataColumn dataColumn = this.cellTable.Columns[reader.Name];
                        if (dataColumn == null)
                        {
                            throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Unexpected element {0}", new object[]
                            {
                                reader.Name
                            }));
                        }
                        if (!FormattersHelpers.IsNullContentElement(reader))
                        {
                            Type elementType = FormattersHelpers.GetElementType(reader, "http://www.w3.org/2001/XMLSchema-instance", dataColumn);
                            dataRow[dataColumn] = FormattersHelpers.ReadDataSetProperty(reader, elementType);
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                    reader.ReadEndElement();
                }
                else
                {
                    FormattersHelpers.CheckException(reader);
                    reader.ReadStartElement();
                }
                this.cellTable.Rows.Add(dataRow);
            }
            FormattersHelpers.CheckException(reader);
            reader.ReadEndElement();
            this.cellTable.EndLoadData();
        }

        private void CreateCubesInfosTable()
        {
            this.cubesInfos = new DataTable();
            this.cubesInfos.Locale = CultureInfo.InvariantCulture;
            this.cubesInfos.Columns.Add("CubeName", typeof(string));
            this.cubesInfos.Columns.Add("LastSchemaUpdate", typeof(DateTime));
            this.cubesInfos.Columns.Add("LastDataUpdate", typeof(DateTime));
            this.cubesInfos.Columns[1].DateTimeMode = DataSetDateTime.Local;
            this.cubesInfos.Columns[2].DateTimeMode = DataSetDateTime.Local;
        }

        private static object GetDateTimeIfValid(XmlReader reader)
        {
            object result = DBNull.Value;
            try
            {
                result = ((DateTime)FormattersHelpers.ReadDataSetProperty(reader, typeof(DateTime))).ToLocalTime();
            }
            catch (ArgumentNullException)
            {
            }
            catch (FormatException)
            {
            }
            return result;
        }
    }
}
