using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XmlaDataReader : IDataReader, IDisposable, IDataRecord
	{
		private class SchemaColumnName
		{
			public const string ColumnName = "ColumnName";

			public const string ColumnOrdinal = "ColumnOrdinal";

			public const string ColumnSize = "ColumnSize";

			public const string NumericPrecision = "NumericPrecision";

			public const string NumericScale = "NumericScale";

			public const string DataType = "DataType";

			public const string ProviderType = "ProviderType";

			public const string IsLong = "IsLong";

			public const string AllowDBNull = "AllowDBNull";

			public const string IsReadOnly = "IsReadOnly";

			public const string IsRowVersion = "IsRowVersion";

			public const string IsUnique = "IsUnique";

			public const string IsKeyColumn = "IsKeyColumn";

			public const string IsAutoIncrement = "IsAutoIncrement";

			public const string BaseSchemaName = "BaseSchemaName";

			public const string BaseCatalogName = "BaseCatalogName";

			public const string BaseTableName = "BaseTableName";

			public const string BaseColumnName = "BaseColumnName";

			private SchemaColumnName()
			{
			}
		}

		public struct Enumerator : IEnumerator
		{
			private DbEnumerator enumerator;

			public IDataRecord Current
			{
				get
				{
					return (IDataRecord)this.enumerator.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(XmlaDataReader dataReader)
			{
				bool closeReader = (CommandBehavior.CloseConnection & dataReader.commandBehavior) != CommandBehavior.Default;
				this.enumerator = new DbEnumerator(dataReader, closeReader);
			}

			public bool MoveNext()
			{
				return this.enumerator.MoveNext();
			}

			public void Reset()
			{
				this.enumerator.Reset();
			}
		}

		private const string rootTag = "root";

		private const int schemaTableColumnCount = 18;

		private IXmlaDataReaderOwner owner;

		private XmlReader xmlReader;

		private CommandBehavior commandBehavior;

		private DataTable schemaTable;

		private bool sequentialAccess;

		private int columnCount;

		private int currentColumn;

		private bool dataReady;

		private Hashtable columnNameLookup;

		private Hashtable columnXmlNameLookup;

		private int depth;

		private bool isClosed;

		private bool isMultipleResult;

		private bool emptyResult;

		private List<string> rowsetNames;

		private XmlaDataReader[] nestedDataReaders;

		private XmlaDataReader parentReader;

		private DataTable dtStore;

		private int currentRow;

		private int currentParentRow;

		private string rowElement;

		private string rowNamespace;

		private int readersXmlDepth;

		private static string[] schemaTableColumnNames = new string[]
		{
			"ColumnName",
			"ColumnOrdinal",
			"ColumnSize",
			"NumericPrecision",
			"NumericScale",
			"DataType",
			"ProviderType",
			"IsLong",
			"AllowDBNull",
			"IsReadOnly",
			"IsRowVersion",
			"IsUnique",
			"IsKeyColumn",
			"IsAutoIncrement",
			"BaseSchemaName",
			"BaseCatalogName",
			"BaseTableName",
			"BaseColumnName"
		};

		private static Type[] schemaTableColumnTypes = new Type[]
		{
			typeof(string),
			typeof(int),
			typeof(int),
			typeof(int),
			typeof(int),
			typeof(Type),
			typeof(object),
			typeof(bool),
			typeof(bool),
			typeof(bool),
			typeof(bool),
			typeof(bool),
			typeof(bool),
			typeof(bool),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string)
		};

		internal bool IsAffectedObjects
		{
			get;
			private set;
		}

		internal string RowsetName
		{
			get;
			private set;
		}

		internal List<string> RowsetNames
		{
			get
			{
				return this.rowsetNames;
			}
		}

		internal XmlReader XmlReader
		{
			get
			{
				return this.xmlReader;
			}
		}

		internal CommandBehavior CommandBehavior
		{
			get
			{
				return this.commandBehavior;
			}
		}

		internal XmlaResultCollection Results
		{
			get;
			private set;
		}

		internal Dictionary<XName, string> TopLevelAttributes
		{
			get;
			private set;
		}

		public int Depth
		{
			get
			{
				return this.depth;
			}
		}

		public int FieldCount
		{
			get
			{
				return this.columnCount;
			}
		}

		public bool IsClosed
		{
			get
			{
				return this.isClosed;
			}
		}

		public int RecordsAffected
		{
			get
			{
				return -1;
			}
		}

		private XmlaResult CurrentResult
		{
			get
			{
				if (this.parentReader != null)
				{
					return this.parentReader.CurrentResult;
				}
				return this.Results[this.Results.Count - 1];
			}
		}

		public object this[int index]
		{
			get
			{
				return this.GetValue(index);
			}
		}

		public object this[string columnName]
		{
			get
			{
				int ordinal = this.GetOrdinal(columnName);
				return this.GetValue(ordinal);
			}
		}

		internal int ParentId
		{
			get
			{
				if (this.parentReader == null)
				{
					return -1;
				}
				return this.FieldCount;
			}
		}

		internal XmlaDataReader(XmlReader xmlReader, CommandBehavior commandBehavior = CommandBehavior.Default) : this(xmlReader, commandBehavior, false, null)
		{
		}

		internal XmlaDataReader(XmlReader xmlReader, CommandBehavior commandBehavior, bool isXmlReaderAtRoot, IXmlaDataReaderOwner owner)
		{
			this.columnNameLookup = new Hashtable();
			this.columnXmlNameLookup = new Hashtable();
			this.rowsetNames = new List<string>();
			this.rowElement = FormattersHelpers.RowElement;
			this.rowNamespace = FormattersHelpers.RowElementNamespace;
			this.readersXmlDepth = -1;
			//base..ctor();
			try
			{
				this.InternalInitialize(xmlReader, commandBehavior, owner);
				if (!isXmlReaderAtRoot)
				{
					XmlaClient.ReadUptoRoot(xmlReader);
				}
				this.IsAffectedObjects = XmlaClient.IsAffectedObjectsResponseS(xmlReader);
				this.isMultipleResult = (this.IsAffectedObjects || XmlaClient.IsMultipleResultResponseS(xmlReader));
				this.Results = new XmlaResultCollection();
				this.CollectTopLevelAttributes();
				if (XmlaClient.IsRowsetResponseS(xmlReader))
				{
					this.RowsetName = xmlReader.GetAttribute("name");
					this.rowsetNames.Add(this.RowsetName);
					this.EnsureResultForNewRowset();
					XmlaClient.StartRowsetResponseS(xmlReader);
					this.LoadResponseSchema();
				}
				else if (XmlaClient.IsMultipleResultResponseS(xmlReader) || XmlaClient.IsAffectedObjectsResponseS(xmlReader))
				{
					XmlaClient.StartElementS(xmlReader, this.IsAffectedObjects ? "AffectedObjects" : "results", "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults");
					if (XmlaClient.IsRootElementS(xmlReader))
					{
						this.InternalNextResult(true);
					}
					else
					{
						if (!this.IsAffectedObjects)
						{
							throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "Expected at least one root element");
						}
						this.StartEmptyAffectedObjects();
					}
				}
				else
				{
					if (XmlaClient.IsDatasetResponseS(xmlReader))
					{
						throw new AdomdUnknownResponseException(XmlaSR.Resultset_IsNotRowset, string.Format(CultureInfo.InvariantCulture, "Got {0}:{1}", new object[]
						{
							"urn:schemas-microsoft-com:xml-analysis:mddataset",
							"root"
						}));
					}
					if (!XmlaClient.IsEmptyResultS(xmlReader))
					{
						throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected {0}:{1}, got {2}", new object[]
						{
							"urn:schemas-microsoft-com:xml-analysis:empty",
							"root",
							xmlReader.Name
						}));
					}
					XmlaClient.ReadEmptyRootS(xmlReader);
					throw new AdomdUnknownResponseException(XmlaSR.Resultset_IsNotRowset, string.Format(CultureInfo.InvariantCulture, "Unexpected node {0}", new object[]
					{
						xmlReader.Name
					}));
				}
			}
			catch (AdomdUnknownResponseException)
			{
				xmlReader.Close();
				throw;
			}
			catch (AdomdConnectionException)
			{
				throw;
			}
			catch (XmlException innerException)
			{
				xmlReader.Close();
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
			catch (IOException innerException2)
			{
				if (this.owner != null)
				{
					owner.CloseConnection(false);
				}
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch (XmlaException innerException3)
			{
				xmlReader.Close();
				throw new AdomdErrorResponseException(innerException3);
			}
			catch
			{
				if (this.owner != null)
				{
					owner.CloseConnection(false);
				}
				throw;
			}
		}

		private XmlaDataReader(XmlaDataReader parentReader)
		{
			this.columnNameLookup = new Hashtable();
			this.columnXmlNameLookup = new Hashtable();
			this.rowsetNames = new List<string>();
			this.rowElement = FormattersHelpers.RowElement;
			this.rowNamespace = FormattersHelpers.RowElementNamespace;
			this.readersXmlDepth = -1;
			//base..ctor();
			this.parentReader = parentReader;
			this.InternalInitialize(parentReader.xmlReader, parentReader.commandBehavior, parentReader.owner);
		}

		private XmlaDataReader(XmlaDataReader parentReader, XmlaDataReader nested)
		{
			this.columnNameLookup = new Hashtable();
			this.columnXmlNameLookup = new Hashtable();
			this.rowsetNames = new List<string>();
			this.rowElement = FormattersHelpers.RowElement;
			this.rowNamespace = FormattersHelpers.RowElementNamespace;
			this.readersXmlDepth = -1;
			//base..ctor();
			this.xmlReader = nested.xmlReader;
			this.commandBehavior = (nested.commandBehavior & ~CommandBehavior.SequentialAccess);
			this.owner = nested.owner;
			this.schemaTable = nested.schemaTable;
			this.sequentialAccess = false;
			this.columnCount = nested.columnCount;
			this.currentColumn = 0;
			this.dataReady = false;
			this.columnNameLookup = nested.columnNameLookup;
			this.columnXmlNameLookup = nested.columnXmlNameLookup;
			this.depth = nested.depth;
			this.isClosed = false;
			this.parentReader = nested.parentReader;
			this.dtStore = nested.dtStore;
			this.currentRow = -1;
			this.currentParentRow = nested.currentParentRow;
			this.rowElement = nested.rowElement;
			this.rowNamespace = nested.rowNamespace;
			this.nestedDataReaders = new XmlaDataReader[nested.nestedDataReaders.Length];
			for (int i = 0; i < nested.nestedDataReaders.Length; i++)
			{
				if (nested.nestedDataReaders[i] != null)
				{
					this.nestedDataReaders[i] = new XmlaDataReader(nested, nested.nestedDataReaders[i]);
				}
				else
				{
					this.nestedDataReaders[i] = null;
				}
			}
		}

		private void CollectTopLevelAttributes()
		{
			int attributeCount = this.xmlReader.AttributeCount;
			for (int i = 0; i < attributeCount; i++)
			{
				this.xmlReader.MoveToNextAttribute();
				if (this.TopLevelAttributes == null)
				{
					this.TopLevelAttributes = new Dictionary<XName, string>();
				}
				XName key;
				if (!string.IsNullOrEmpty(this.xmlReader.NamespaceURI))
				{
					XNamespace ns = this.xmlReader.NamespaceURI;
					key = ns + this.xmlReader.LocalName;
				}
				else
				{
					key = this.xmlReader.LocalName;
				}
				this.TopLevelAttributes.Add(key, this.xmlReader.Value);
			}
		}

		public void Dispose()
		{
			this.Close();
		}

		public void Close()
		{
			this.ResetCurrentRowCache();
			this.dataReady = false;
			if (this.Depth == 0 && this.xmlReader != null)
			{
				this.xmlReader.Close();
				if (this.xmlReader != null)
				{
					((IDisposable)this.xmlReader).Dispose();
				}
				this.xmlReader = null;
			}
			this.isClosed = true;
		}

		public DataTable GetSchemaTable()
		{
			return this.schemaTable;
		}

		private void StartEmptyAffectedObjects()
		{
			this.EnsureResultForNewRowset();
			this.CheckForMessages();
			this.isMultipleResult = false;
			this.emptyResult = true;
		}

		private void SkipRootContents()
		{
			while (true)
			{
				XmlNodeType nodeType = this.xmlReader.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType == XmlNodeType.EndElement)
					{
						if ((this.xmlReader.LocalName == "results" || this.xmlReader.LocalName == "AffectedObjects") && this.xmlReader.NamespaceURI == "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults")
						{
							return;
						}
					}
				}
				else
				{
					if (this.xmlReader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:empty") || this.xmlReader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:rowset"))
					{
						break;
					}
					if (this.CheckForMessages())
					{
						continue;
					}
				}
				if (!this.xmlReader.Read())
				{
					return;
				}
			}
		}

		private void EnsureResultForNewRowset()
		{
			if (this.parentReader != null)
			{
				return;
			}
			if (!this.IsAffectedObjects || this.Results.Count == 0)
			{
				this.Results.Add(new XmlaResult());
			}
		}

		private bool CheckForMessages()
		{
			XmlaMessageCollection xmlaMessageCollection = null;
			if (!XmlaClient.CheckForMessages(this.xmlReader, ref xmlaMessageCollection))
			{
				return false;
			}
			XmlaResult currentResult = this.CurrentResult;
			foreach (XmlaMessage item in ((IEnumerable)xmlaMessageCollection))
			{
				currentResult.Messages.Add(item);
			}
			return true;
		}

		private bool InternalNextResult(bool first)
		{
			if (!first)
			{
				this.InitResultData();
				this.SkipRootContents();
			}
			if (XmlaClient.IsEmptyResultS(this.xmlReader))
			{
				if (this.IsAffectedObjects)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "Got an empty result inside AffectedObjects");
				}
				this.RowsetName = null;
				this.xmlReader.ReadStartElement("root", "urn:schemas-microsoft-com:xml-analysis:empty");
				this.GenerateSchemaForEmptyResult();
				this.emptyResult = true;
				this.EnsureResultForNewRowset();
				return true;
			}
			else
			{
				if (this.xmlReader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:rowset"))
				{
					this.RowsetName = this.xmlReader.GetAttribute("name");
					this.rowsetNames.Add(this.RowsetName);
					XmlaClient.StartRowsetResponseS(this.xmlReader);
					this.LoadResponseSchema();
					this.dataReady = true;
					this.emptyResult = false;
					this.EnsureResultForNewRowset();
					return true;
				}
				if (first)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "Expected at least one root element");
				}
				this.CheckForMessages();
				XmlaClient.EndElementS(this.xmlReader, this.IsAffectedObjects ? "AffectedObjects" : "results", "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults");
				this.isMultipleResult = false;
				return false;
			}
		}

		public bool NextResult()
		{
			return this.isMultipleResult && this.InternalNextResult(false);
		}

		public bool Read()
		{
			if (this.parentReader == null || this.sequentialAccess)
			{
				this.currentRow = 0;
				this.ResetCurrentRowCache();
				return this.InternalRead();
			}
			this.currentRow++;
			this.dataReady = (this.currentRow < this.dtStore.Rows.Count);
			if (this.dataReady)
			{
				int num = (int)this.dtStore.Rows[this.currentRow][this.ParentId];
				this.dataReady = (num == this.parentReader.currentRow);
			}
			return this.dataReady;
		}

		public IDataReader GetData(int ordinal)
		{
			return (IDataReader)this.GetValue(ordinal);
		}

		public bool GetBoolean(int ordinal)
		{
			return (bool)this.InternalGetValue(ordinal);
		}

		public byte GetByte(int ordinal)
		{
			return (byte)this.InternalGetValue(ordinal);
		}

		public long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			throw new NotSupportedException();
		}

		public char GetChar(int ordinal)
		{
			return (char)this.InternalGetValue(ordinal);
		}

		public long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			string text = (string)this.InternalGetValue(ordinal);
			char[] array = text.ToCharArray();
			int num = array.Length;
			if (dataIndex > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("dataIndex", XmlaSR.DataReader_IndexOutOfRange);
			}
			int num2 = (int)dataIndex;
			if (buffer == null)
			{
				return (long)num;
			}
			if (num2 < 0 || num2 >= num)
			{
				return 0L;
			}
			if (num2 < num)
			{
				if (num2 + length > num)
				{
					num -= num2;
				}
				else
				{
					num = length;
				}
			}
			Array.Copy(array, num2, buffer, bufferIndex, num);
			return (long)num;
		}

		public string GetDataTypeName(int index)
		{
			return this.GetFieldType(index).Name;
		}

		public DateTime GetDateTime(int ordinal)
		{
			return (DateTime)Convert.ChangeType(this.InternalGetValue(ordinal), typeof(DateTime), CultureInfo.InvariantCulture);
		}

		public decimal GetDecimal(int ordinal)
		{
			return (decimal)Convert.ChangeType(this.InternalGetValue(ordinal), typeof(decimal), CultureInfo.InvariantCulture);
		}

		public double GetDouble(int ordinal)
		{
			return (double)Convert.ChangeType(this.InternalGetValue(ordinal), typeof(double), CultureInfo.InvariantCulture);
		}

		public Type GetFieldType(int ordinal)
		{
			return (Type)this.schemaTable.Rows[ordinal]["DataType"];
		}

		public float GetFloat(int ordinal)
		{
			return (float)Convert.ChangeType(this.InternalGetValue(ordinal), typeof(float), CultureInfo.InvariantCulture);
		}

		public Guid GetGuid(int ordinal)
		{
			return (Guid)Convert.ChangeType(this.InternalGetValue(ordinal), typeof(Guid), CultureInfo.InvariantCulture);
		}

		public short GetInt16(int ordinal)
		{
			return (short)Convert.ChangeType(this.InternalGetValue(ordinal), typeof(short), CultureInfo.InvariantCulture);
		}

		public int GetInt32(int ordinal)
		{
			return (int)Convert.ChangeType(this.InternalGetValue(ordinal), typeof(int), CultureInfo.InvariantCulture);
		}

		public long GetInt64(int ordinal)
		{
			return (long)Convert.ChangeType(this.InternalGetValue(ordinal), typeof(long), CultureInfo.InvariantCulture);
		}

		public string GetName(int ordinal)
		{
			return (string)this.schemaTable.Rows[ordinal]["ColumnName"];
		}

		public int GetOrdinal(string name)
		{
			object obj = this.columnNameLookup[name];
			if (obj == null)
			{
				throw new ArgumentException(XmlaSR.InvalidArgument);
			}
			return (int)obj;
		}

		public string GetString(int ordinal)
		{
			return this.InternalGetValue(ordinal).ToString();
		}

		public TimeSpan GetTimeSpan(int ordinal)
		{
			return (TimeSpan)Convert.ChangeType(this.InternalGetValue(ordinal), typeof(TimeSpan), CultureInfo.InvariantCulture);
		}

		public object GetValue(int ordinal)
		{
			object obj = this.InternalGetValue(ordinal);
			bool flag = FormattersHelpers.GetColumnXsdTypeName(this.dtStore.Columns[ordinal]) == "xmlDocument";
			if (flag)
			{
				ColumnXmlReader columnXmlReader = obj as ColumnXmlReader;
				if (columnXmlReader.IsDataSet)
				{
					obj = columnXmlReader.Dataset;
				}
				else
				{
					obj = columnXmlReader;
				}
			}
			return obj;
		}

		public XmlaDataReader GetDataReader(int ordinal)
		{
			object obj = this.InternalGetValue(ordinal);
			return (XmlaDataReader)obj;
		}

		public int GetValues(object[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = Math.Min(values.Length, this.FieldCount);
			for (int i = 0; i < num; i++)
			{
				if (this.sequentialAccess && this.nestedDataReaders[i] != null)
				{
					this.currentColumn = i + 1;
					XmlaDataReader xmlaDataReader = new XmlaDataReader(this, this.nestedDataReaders[i]);
					xmlaDataReader.currentParentRow = this.dtStore.Rows.Count;
					while (xmlaDataReader.InternalRead())
					{
					}
					values[i] = xmlaDataReader;
				}
				else
				{
					values[i] = this.GetValue(i);
					if (this.sequentialAccess && values[i] is ColumnXmlReader)
					{
						values[i] = new ColumnXmlReader(values[i].ToString());
					}
				}
			}
			return num;
		}

		public bool IsDBNull(int ordinal)
		{
			object value = this.GetValue(ordinal);
			return value == null || Convert.IsDBNull(value);
		}

		public XmlaDataReader.Enumerator GetEnumerator()
		{
			return new XmlaDataReader.Enumerator(this);
		}

		private void InitResultData()
		{
			this.nestedDataReaders = null;
			this.schemaTable = this.CreateSchemaTable();
			this.currentRow = -1;
		}

		private void InternalInitialize(XmlReader xmlReader, CommandBehavior commandBehavior, IXmlaDataReaderOwner owner)
		{
			this.xmlReader = xmlReader;
			this.commandBehavior = commandBehavior;
			this.owner = owner;
			this.sequentialAccess = ((commandBehavior & CommandBehavior.SequentialAccess) != CommandBehavior.Default);
			this.InitResultData();
			this.depth = 0;
			this.isClosed = false;
			if (this.parentReader != null)
			{
				this.depth = this.parentReader.Depth + 1;
			}
		}

		private int GetOrdinalFromXmlName(string xmlName)
		{
			object obj = this.columnXmlNameLookup[xmlName];
			if (obj == null)
			{
				return -1;
			}
			return (int)obj;
		}

		private int GetRowXmlValues(object[] xmlValues)
		{
			int fieldCount = this.FieldCount;
			for (int i = 0; i < fieldCount; i++)
			{
				xmlValues[i] = this.SequentialReadXmlValue(i);
			}
			return fieldCount;
		}

		private object SequentialReadXmlValue(int ordinal)
		{
			if (ordinal < this.currentColumn)
			{
				throw new ArgumentException(XmlaSR.NonSequentialColumnAccessError, "ordinal");
			}
			if (ordinal >= this.FieldCount)
			{
				throw new ArgumentOutOfRangeException("ordinal");
			}
			object result;
			try
			{
				this.currentColumn = ordinal + 1;
				object obj = null;
				while (this.xmlReader.IsStartElement() && this.xmlReader.Depth == this.readersXmlDepth)
				{
					int ordinalFromXmlName = this.GetOrdinalFromXmlName(this.xmlReader.LocalName);
					if (ordinalFromXmlName == -1)
					{
						FormattersHelpers.CheckException(this.xmlReader);
						throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Unexpected element {0}", new object[]
						{
							this.xmlReader.Name
						}));
					}
					bool flag = false;
					bool skipElements = false;
					if (FormattersHelpers.GetColumnXsdTypeName(this.dtStore.Columns[ordinalFromXmlName]) == "xmlDocument")
					{
						flag = true;
						skipElements = ((XmlaReader)this.xmlReader).SkipElements;
						((XmlaReader)this.xmlReader).SkipElements = false;
					}
					try
					{
						FormattersHelpers.CheckException(this.xmlReader);
					}
					finally
					{
						if (flag)
						{
							((XmlaReader)this.xmlReader).SkipElements = skipElements;
						}
					}
					if (ordinalFromXmlName == ordinal)
					{
						if (!FormattersHelpers.IsNullContentElement(this.xmlReader))
						{
							obj = this.ReadColumnValue(ordinal);
							break;
						}
						this.xmlReader.Skip();
						break;
					}
					else
					{
						if (ordinalFromXmlName >= ordinal)
						{
							break;
						}
						string name = this.xmlReader.Name;
						while (this.xmlReader.IsStartElement(name))
						{
							this.xmlReader.Skip();
						}
					}
				}
				result = obj;
			}
			catch (AdomdUnknownResponseException)
			{
				if (this.xmlReader != null)
				{
					this.xmlReader.Close();
				}
				throw;
			}
			catch (AdomdConnectionException)
			{
				throw;
			}
			catch (XmlException innerException)
			{
				if (this.xmlReader != null)
				{
					this.xmlReader.Close();
				}
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
			catch (IOException innerException2)
			{
				if (this.owner != null)
				{
					this.owner.CloseConnection(false);
				}
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch (XmlaException innerException3)
			{
				if (this.xmlReader != null)
				{
					this.xmlReader.Close();
				}
				throw new AdomdErrorResponseException(innerException3);
			}
			catch
			{
				if (this.owner != null)
				{
					this.owner.CloseConnection(false);
				}
				throw;
			}
			return result;
		}

		private object ReadColumnValue(int ordinal)
		{
		    //Type type = FormattersHelpers.GetElementType(this.xmlReader, "http://www.w3.org/2001/XMLSchema-instance", null);
		    Type type = FormattersHelpers.GetElementType(this.xmlReader, "http://www.w3.org/2001/XMLSchema-instance", typeof(object));
            if (type == null)
			{
				type = this.GetFieldType(ordinal);
			}
			XmlaDataReader xmlaDataReader = this.nestedDataReaders[ordinal];
			object obj;
			if (xmlaDataReader == null)
			{
				string columnXsdTypeName = FormattersHelpers.GetColumnXsdTypeName(this.dtStore.Columns[ordinal]);
				bool flag = columnXsdTypeName == "xmlDocument";
				bool isArray = type.IsArray && columnXsdTypeName != "base64Binary";
				if (!this.sequentialAccess)
				{
					obj = FormattersHelpers.ReadRowsetProperty(this.xmlReader, this.xmlReader.LocalName, this.xmlReader.LookupNamespace(this.xmlReader.Prefix), type, false, isArray, false);
					if (flag)
					{
						string strIn = obj as string;
						ColumnXmlReader columnXmlReader = new ColumnXmlReader(strIn);
						obj = columnXmlReader;
					}
				}
				else if (flag)
				{
					ColumnXmlReader columnXmlReader2 = new ColumnXmlReader(this.xmlReader, this.xmlReader.LocalName, this.xmlReader.NamespaceURI);
					obj = columnXmlReader2;
				}
				else
				{
					obj = FormattersHelpers.ReadRowsetProperty(this.xmlReader, this.xmlReader.LocalName, this.xmlReader.NamespaceURI, type, false, isArray, false);
				}
			}
			else if (this.sequentialAccess)
			{
				obj = xmlaDataReader;
			}
			else
			{
				bool flag2 = xmlaDataReader.IsClosed;
				if (flag2)
				{
					xmlaDataReader.ReOpen();
				}
				int count = xmlaDataReader.dtStore.Rows.Count;
				xmlaDataReader.currentParentRow = this.dtStore.Rows.Count;
				while (xmlaDataReader.InternalRead())
				{
				}
				obj = count;
			}
			return obj;
		}

		private void ThrowIfInlineError(object columnValue)
		{
			if (columnValue is XmlaError)
			{
				throw new AdomdErrorResponseException((XmlaError)columnValue);
			}
		}

		private bool BeginNewRow()
		{
			bool result;
			try
			{
				if (this.xmlReader.IsStartElement(this.rowElement, this.rowNamespace))
				{
					this.readersXmlDepth = this.xmlReader.Depth + 1;
					this.xmlReader.ReadStartElement();
					this.currentColumn = 0;
					result = true;
				}
				else
				{
					FormattersHelpers.CheckException(this.xmlReader);
					result = false;
				}
			}
			catch (AdomdUnknownResponseException)
			{
				if (this.xmlReader != null)
				{
					this.xmlReader.Close();
				}
				throw;
			}
			catch (AdomdConnectionException)
			{
				throw;
			}
			catch (XmlException innerException)
			{
				if (this.xmlReader != null)
				{
					this.xmlReader.Close();
				}
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
			catch (IOException innerException2)
			{
				if (this.owner != null)
				{
					this.owner.CloseConnection(false);
				}
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch (XmlaException innerException3)
			{
				if (this.xmlReader != null)
				{
					this.xmlReader.Close();
				}
				throw new AdomdErrorResponseException(innerException3);
			}
			catch
			{
				if (this.owner != null)
				{
					this.owner.CloseConnection(false);
				}
				throw;
			}
			return result;
		}

		private void CompletePreviousRow()
		{
			try
			{
				if (!this.xmlReader.IsStartElement(this.rowElement, this.rowNamespace))
				{
					while (this.xmlReader.IsStartElement() && this.xmlReader.Depth == this.readersXmlDepth)
					{
						if (this.xmlReader.IsEmptyElement)
						{
							FormattersHelpers.CheckException(this.xmlReader);
						}
						int ordinalFromXmlName = this.GetOrdinalFromXmlName(this.xmlReader.LocalName);
						if (ordinalFromXmlName == -1)
						{
							throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Unexpected element {0}", new object[]
							{
								this.xmlReader.Name
							}));
						}
						this.xmlReader.Skip();
					}
					this.currentColumn = this.FieldCount;
					if (this.xmlReader.MoveToContent() == XmlNodeType.EndElement && this.xmlReader.LocalName == this.rowElement && this.xmlReader.NamespaceURI == this.rowNamespace)
					{
						this.xmlReader.ReadEndElement();
					}
				}
			}
			catch (AdomdUnknownResponseException)
			{
				if (this.xmlReader != null)
				{
					this.xmlReader.Close();
				}
				throw;
			}
			catch (AdomdConnectionException)
			{
				throw;
			}
			catch (XmlException innerException)
			{
				if (this.xmlReader != null)
				{
					this.xmlReader.Close();
				}
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
			catch (IOException innerException2)
			{
				if (this.owner != null)
				{
					this.owner.CloseConnection(false);
				}
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch (XmlaException innerException3)
			{
				if (this.xmlReader != null)
				{
					this.xmlReader.Close();
				}
				throw new AdomdErrorResponseException(innerException3);
			}
			catch
			{
				if (this.owner != null)
				{
					this.owner.CloseConnection(false);
				}
				throw;
			}
		}

		private DataTable CreateSchemaTable()
		{
			DataTable dataTable = new DataTable();
			dataTable.Locale = CultureInfo.InvariantCulture;
			for (int i = 0; i < 18; i++)
			{
				dataTable.Columns.Add(XmlaDataReader.schemaTableColumnNames[i], XmlaDataReader.schemaTableColumnTypes[i]);
			}
			return dataTable;
		}

		private void GenerateSchemaForEmptyResult()
		{
			this.columnCount = 0;
			this.dtStore = new DataTable();
			this.dtStore.Locale = CultureInfo.InvariantCulture;
		}

		private void LoadResponseSchema()
		{
			FormattersHelpers.ColumnDefinitionDelegate definitionDelegate = new FormattersHelpers.ColumnDefinitionDelegate(this.ColumnDef);
			FormattersHelpers.LoadSchema(this.xmlReader, definitionDelegate, true);
		}

		private object ColumnDef(int ordinal, string name, string colNamespace, string caption, Type type, bool isNested, object parent, string strColumnXsdType)
		{
			if (ordinal == -1)
			{
				this.columnCount = (int)parent;
				this.dtStore = new DataTable();
				this.dtStore.Locale = CultureInfo.InvariantCulture;
				if (this.nestedDataReaders == null)
				{
					this.nestedDataReaders = new XmlaDataReader[this.columnCount];
				}
				return null;
			}
			if (ordinal == -2)
			{
				if (this.parentReader != null)
				{
					this.dtStore.Columns.Add(string.Empty, this.currentRow.GetType());
				}
				return null;
			}
			Type type2 = type;
			FormattersHelpers.ColumnDefinitionDelegate result = null;
			if (strColumnXsdType == "xmlDocument")
			{
				type2 = typeof(object);
			}
			if (isNested)
			{
				type2 = typeof(XmlaDataReader);
				this.nestedDataReaders[ordinal] = new XmlaDataReader(this);
				this.nestedDataReaders[ordinal].rowElement = name;
				this.nestedDataReaders[ordinal].rowNamespace = colNamespace;
				result = new FormattersHelpers.ColumnDefinitionDelegate(this.nestedDataReaders[ordinal].ColumnDef);
			}
			DataRow dataRow = this.schemaTable.NewRow();
			this.columnXmlNameLookup[name] = ordinal;
			this.columnNameLookup[caption] = ordinal;
			dataRow["ColumnName"] = caption;
			dataRow["ColumnOrdinal"] = ordinal;
			dataRow["ColumnSize"] = 0;
			if (type2 == typeof(decimal))
			{
				dataRow["NumericPrecision"] = 19;
				dataRow["NumericScale"] = 4;
			}
			else
			{
				dataRow["NumericPrecision"] = 0;
				dataRow["NumericScale"] = 0;
			}
			dataRow["DataType"] = type2;
			dataRow["ProviderType"] = type2;
			dataRow["IsLong"] = false;
			dataRow["AllowDBNull"] = true;
			dataRow["IsReadOnly"] = true;
			dataRow["IsRowVersion"] = false;
			dataRow["IsUnique"] = false;
			dataRow["IsKeyColumn"] = false;
			dataRow["IsAutoIncrement"] = false;
			dataRow["BaseSchemaName"] = null;
			dataRow["BaseCatalogName"] = null;
			dataRow["BaseTableName"] = null;
			dataRow["BaseColumnName"] = null;
			this.schemaTable.Rows.Add(dataRow);
			if (isNested)
			{
				this.dtStore.Columns.Add(name, this.currentRow.GetType());
			}
			else
			{
				DataColumn column = this.dtStore.Columns.Add(name, typeof(object));
				FormattersHelpers.SetColumnXsdTypeName(column, strColumnXsdType);
			}
			return result;
		}

		private bool InternalRead()
		{
			if (this.isClosed)
			{
				throw new InvalidOperationException(XmlaSR.DataReaderClosedError);
			}
			if (this.xmlReader == null || this.xmlReader.ReadState == ReadState.Closed)
			{
				throw new InvalidOperationException();
			}
			if (this.emptyResult)
			{
				XmlaClient.CheckForException(this.xmlReader, null, true);
				return false;
			}
			if (this.dataReady)
			{
				this.CompletePreviousRow();
			}
			this.dataReady = this.BeginNewRow();
			if (this.dataReady)
			{
				if (!this.sequentialAccess)
				{
					int num = this.FieldCount;
					if (this.parentReader != null)
					{
						num++;
					}
					object[] array = new object[num];
					this.GetRowXmlValues(array);
					if (this.parentReader != null)
					{
						array[this.ParentId] = this.currentParentRow;
					}
					this.dtStore.Rows.Add(array);
				}
			}
			else if (this.Depth == 0)
			{
				XmlaClient.EndRowsetResponseS(this.xmlReader);
			}
			return this.dataReady;
		}

		private void ResetCurrentRowCache()
		{
			try
			{
				if (this.dtStore != null)
				{
					this.dtStore.Rows.Clear();
				}
				if (this.nestedDataReaders != null)
				{
					for (int i = 0; i < this.FieldCount; i++)
					{
						if (this.nestedDataReaders[i] != null)
						{
							this.nestedDataReaders[i].ResetCurrentRowCache();
						}
					}
				}
			}
			catch (NullReferenceException)
			{
			}
		}

		private void ReOpen()
		{
			if (this.IsClosed)
			{
				this.isClosed = false;
			}
		}

		private object InternalGetValue(int ordinal)
		{
			if (this.isClosed)
			{
				throw new InvalidOperationException(XmlaSR.DataReaderClosedError);
			}
			if (!this.dataReady)
			{
				throw new InvalidOperationException(XmlaSR.DataReaderInvalidRowError);
			}
			if (this.xmlReader == null || this.xmlReader.ReadState == ReadState.Closed)
			{
				throw new InvalidOperationException();
			}
			if (ordinal < 0 || ordinal >= this.FieldCount)
			{
				throw new ArgumentOutOfRangeException("ordinal");
			}
			object obj;
			if (this.sequentialAccess)
			{
				obj = this.SequentialReadXmlValue(ordinal);
			}
			else
			{
				DataRow dataRow = this.dtStore.Rows[this.currentRow];
				if (this.nestedDataReaders[ordinal] == null)
				{
					obj = null;
					if (!(dataRow[ordinal] is DBNull))
					{
						obj = dataRow[ordinal];
					}
				}
				else
				{
					int num = 0;
					if (!(dataRow[ordinal] is DBNull))
					{
						num = (int)dataRow[ordinal];
					}
					XmlaDataReader xmlaDataReader = this.nestedDataReaders[ordinal];
					xmlaDataReader.currentRow = num - 1;
					obj = xmlaDataReader;
				}
			}
			this.ThrowIfInlineError(obj);
			if (obj is XmlaDataReader)
			{
				XmlaDataReader xmlaDataReader2 = obj as XmlaDataReader;
				if (xmlaDataReader2.IsClosed)
				{
					xmlaDataReader2.ReOpen();
				}
			}
			return obj;
		}
	}
}
