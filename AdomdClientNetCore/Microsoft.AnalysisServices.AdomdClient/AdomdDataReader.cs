using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public class AdomdDataReader : MarshalByRefObject, IDataReader, IDisposable, IDataRecord, IEnumerable, IXmlaDataReaderOwner
	{
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

			internal Enumerator(AdomdDataReader dataReader, CommandBehavior commandBehavior)
			{
				bool closeReader = (CommandBehavior.CloseConnection & commandBehavior) != CommandBehavior.Default;
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

		private AdomdConnection connection;

		private AdomdDataReader[] embeddedReaders;

		internal XmlaDataReader XmlaDataReader
		{
			get;
			private set;
		}

		public string RowsetName
		{
			get
			{
				return this.XmlaDataReader.RowsetName;
			}
		}

		public int Depth
		{
			get
			{
				return this.XmlaDataReader.Depth;
			}
		}

		public int FieldCount
		{
			get
			{
				return this.XmlaDataReader.FieldCount;
			}
		}

		public bool IsClosed
		{
			get
			{
				return this.XmlaDataReader.IsClosed;
			}
		}

		public int RecordsAffected
		{
			get
			{
				return -1;
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
				return this.XmlaDataReader[columnName];
			}
		}

		internal static AdomdDataReader CreateInstance(XmlReader xmlReader, CommandBehavior commandBehavior, AdomdConnection connection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (xmlReader == null)
			{
				throw new ArgumentNullException("xmlReader");
			}
			XmlaClient.ReadUptoRoot(xmlReader);
			if (XmlaClient.IsAffectedObjectsResponseS(xmlReader))
			{
				return new AdomdAffectedObjectsReader(xmlReader, commandBehavior, connection);
			}
			return new AdomdDataReader(xmlReader, commandBehavior, connection, true);
		}

		protected AdomdDataReader(XmlReader xmlReader, CommandBehavior commandBehavior, AdomdConnection connection, bool readerAtRoot)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (xmlReader == null)
			{
				throw new ArgumentNullException("xmlReader");
			}
			this.connection = connection;
			this.XmlaDataReader = new XmlaDataReader(xmlReader, commandBehavior, readerAtRoot, this);
		}

		private AdomdDataReader(XmlaDataReader xmlaDataReader)
		{
			this.XmlaDataReader = xmlaDataReader;
		}

		public void Dispose()
		{
			this.Close();
		}

		public void Close()
		{
			this.XmlaDataReader.Close();
			if (this.connection != null && (this.XmlaDataReader.CommandBehavior & CommandBehavior.CloseConnection) != CommandBehavior.Default)
			{
				this.connection.OpenedReader = null;
				this.connection.Close();
			}
			this.connection = null;
		}

		public DataTable GetSchemaTable()
		{
			return this.XmlaDataReader.GetSchemaTable();
		}

		public bool NextResult()
		{
			return this.XmlaDataReader.NextResult();
		}

		public bool Read()
		{
			return this.XmlaDataReader.Read();
		}

		public IDataReader GetData(int ordinal)
		{
			return this.GetDataReader(ordinal);
		}

		public bool GetBoolean(int ordinal)
		{
			return this.XmlaDataReader.GetBoolean(ordinal);
		}

		public byte GetByte(int ordinal)
		{
			return this.XmlaDataReader.GetByte(ordinal);
		}

		public long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			return this.XmlaDataReader.GetBytes(ordinal, dataIndex, buffer, bufferIndex, length);
		}

		public char GetChar(int ordinal)
		{
			return this.XmlaDataReader.GetChar(ordinal);
		}

		public long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			return this.XmlaDataReader.GetChars(ordinal, dataIndex, buffer, bufferIndex, length);
		}

		public string GetDataTypeName(int index)
		{
			return this.XmlaDataReader.GetDataTypeName(index);
		}

		public DateTime GetDateTime(int ordinal)
		{
			return this.XmlaDataReader.GetDateTime(ordinal);
		}

		public decimal GetDecimal(int ordinal)
		{
			return this.XmlaDataReader.GetDecimal(ordinal);
		}

		public double GetDouble(int ordinal)
		{
			return this.XmlaDataReader.GetDouble(ordinal);
		}

		public Type GetFieldType(int ordinal)
		{
			return this.XmlaDataReader.GetFieldType(ordinal);
		}

		public float GetFloat(int ordinal)
		{
			return this.XmlaDataReader.GetFloat(ordinal);
		}

		public Guid GetGuid(int ordinal)
		{
			return this.XmlaDataReader.GetGuid(ordinal);
		}

		public short GetInt16(int ordinal)
		{
			return this.XmlaDataReader.GetInt16(ordinal);
		}

		public int GetInt32(int ordinal)
		{
			return this.XmlaDataReader.GetInt32(ordinal);
		}

		public long GetInt64(int ordinal)
		{
			return this.XmlaDataReader.GetInt64(ordinal);
		}

		public string GetName(int ordinal)
		{
			return this.XmlaDataReader.GetName(ordinal);
		}

		public int GetOrdinal(string name)
		{
			return this.XmlaDataReader.GetOrdinal(name);
		}

		public string GetString(int ordinal)
		{
			return this.XmlaDataReader.GetString(ordinal);
		}

		public TimeSpan GetTimeSpan(int ordinal)
		{
			return this.XmlaDataReader.GetTimeSpan(ordinal);
		}

		public object GetValue(int ordinal)
		{
			object obj = this.XmlaDataReader.GetValue(ordinal);
			if (obj is XmlaDataReader)
			{
				obj = this.GetDataReader(ordinal);
			}
			return obj;
		}

		public AdomdDataReader GetDataReader(int ordinal)
		{
			if (this.embeddedReaders == null)
			{
				this.embeddedReaders = new AdomdDataReader[this.FieldCount];
			}
			if (this.embeddedReaders[ordinal] == null)
			{
				XmlaDataReader dataReader = this.XmlaDataReader.GetDataReader(ordinal);
				this.embeddedReaders[ordinal] = new AdomdDataReader(dataReader);
			}
			return this.embeddedReaders[ordinal];
		}

		public int GetValues(object[] values)
		{
			return this.XmlaDataReader.GetValues(values);
		}

		public bool IsDBNull(int ordinal)
		{
			return this.XmlaDataReader.IsDBNull(ordinal);
		}

		public AdomdDataReader.Enumerator GetEnumerator()
		{
			return new AdomdDataReader.Enumerator(this, this.XmlaDataReader.CommandBehavior);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void IXmlaDataReaderOwner.CloseConnection(bool endSession)
		{
			if (this.connection != null)
			{
				this.connection.Close(endSession);
				this.connection = null;
			}
		}
	}
}
