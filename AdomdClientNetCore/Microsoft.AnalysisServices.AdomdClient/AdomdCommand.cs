using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AdomdCommand : Component, IDbCommand, IDisposable, ICloneable, ICommandContentProvider
	{
		private const string timeoutPropName = "Timeout";

		private const string ActivityIDPropertyName = "DbpropMsmdActivityID";

		private CommandType commandType = CommandType.Text;

		private string commandText;

		private Stream commandStream;

		private int timeOut;

		private AdomdConnection connection;

		private AdomdParameterCollection parameters;

		private AdomdPropertyCollection commandProperties;

		private AdomdTransaction transaction;

		private Guid activityID = Guid.Empty;

		[Browsable(false)]
		public Stream CommandStream
		{
			get
			{
				return this.commandStream;
			}
			set
			{
				if (value != null && !value.CanRead)
				{
					throw new ArgumentException(SR.Command_CommandStreamDoesNotSupportReadingFrom);
				}
				this.commandStream = value;
			}
		}

		public string CommandText
		{
			get
			{
				return this.commandText;
			}
			set
			{
				this.commandText = value;
			}
		}

		public Guid ActivityID
		{
			get
			{
				return this.activityID;
			}
			set
			{
				this.activityID = value;
				this.AddCommandProperty("DbpropMsmdActivityID", this.activityID);
			}
		}

		public int CommandTimeout
		{
			get
			{
				return this.timeOut;
			}
			set
			{
				this.timeOut = value;
				if (this.timeOut < 0)
				{
					throw new ArgumentException(SR.Command_InvalidTimeout(this.timeOut.ToString(CultureInfo.CurrentCulture)));
				}
				this.AddCommandProperty("Timeout", this.timeOut);
			}
		}

		[Browsable(false)]
		public CommandType CommandType
		{
			get
			{
				return this.commandType;
			}
			set
			{
				this.commandType = value;
			}
		}

		public AdomdConnection Connection
		{
			get
			{
				return this.connection;
			}
			set
			{
				this.connection = value;
			}
		}

		[Browsable(false)]
		public AdomdParameterCollection Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					this.parameters = new AdomdParameterCollection(this);
				}
				return this.parameters;
			}
		}

		[Browsable(false)]
		public AdomdPropertyCollection Properties
		{
			get
			{
				if (this.commandProperties == null)
				{
					this.commandProperties = new AdomdPropertyCollection();
				}
				return this.commandProperties;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public UpdateRowSource UpdatedRowSource
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal IDataReaderConsumer DataReaderConsumer
		{
			get;
			set;
		}

		IDbConnection IDbCommand.Connection
		{
			get
			{
				return this.Connection;
			}
			set
			{
				this.Connection = (AdomdConnection)value;
			}
		}

		IDataParameterCollection IDbCommand.Parameters
		{
			get
			{
				return this.Parameters;
			}
		}

		IDbTransaction IDbCommand.Transaction
		{
			get
			{
				return this.transaction;
			}
			set
			{
				if (value == null)
				{
					this.transaction = null;
					return;
				}
				if (!(value is AdomdTransaction))
				{
					throw new ArgumentException(SR.Command_OnlyAdomdTransactionObjectIsSupported, "value");
				}
				AdomdTransaction adomdTransaction = value as AdomdTransaction;
				if (adomdTransaction.IsCompleted)
				{
					throw new InvalidOperationException(SR.Command_OnlyActiveTransactionCanBeAssigned);
				}
				if (adomdTransaction.Connection != this.connection)
				{
					throw new InvalidOperationException(SR.Command_OnlyTransactionAssociatedWithTheSameConnectionCanBeAssigned);
				}
				this.transaction = adomdTransaction;
			}
		}

		private IDataParameterCollection PrivateParameters
		{
			get
			{
				return this.parameters;
			}
		}

		string ICommandContentProvider.CommandText
		{
			get
			{
				return this.CommandText;
			}
		}

		Stream ICommandContentProvider.CommandStream
		{
			get
			{
				return this.CommandStream;
			}
		}

		bool ICommandContentProvider.IsContentMdx
		{
			get
			{
				return this.CommandText != null && AdomdCommand.IsMdx(this.CommandText);
			}
		}

		public AdomdCommand()
		{
			this.commandStream = null;
			this.commandText = null;
			this.timeOut = 0;
			this.connection = null;
			this.parameters = null;
			this.commandProperties = null;
			this.transaction = null;
		}

		public AdomdCommand(string commandText) : this()
		{
			this.commandText = commandText;
		}

		public AdomdCommand(string commandText, AdomdConnection connection) : this(commandText)
		{
			this.connection = connection;
		}

		private AdomdCommand(AdomdCommand originalCommand)
		{
			this.Connection = originalCommand.Connection;
			this.CommandText = originalCommand.CommandText;
			this.CommandStream = originalCommand.CommandStream;
			this.CommandTimeout = originalCommand.CommandTimeout;
			this.CommandType = originalCommand.CommandType;
			if (originalCommand.Parameters.Count > 0)
			{
				AdomdParameterCollection adomdParameterCollection = this.Parameters;
				foreach (AdomdParameter adomdParameter in ((IEnumerable)originalCommand.Parameters))
				{
					adomdParameterCollection.Add(adomdParameter.Clone());
				}
			}
			if (originalCommand.Properties.Count > 0)
			{
				AdomdPropertyCollection properties = this.Properties;
				AdomdPropertyCollection.Enumerator enumerator2 = originalCommand.Properties.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					AdomdProperty current = enumerator2.Current;
					properties.Add(new AdomdProperty(current.Name, current.Namespace, current.Value));
				}
			}
		}

		public void Cancel()
		{
			if (this.connection != null && this.connection.State == ConnectionState.Open)
			{
				AdomdConnection.CancelCommand(this.connection);
			}
		}

		public AdomdParameter CreateParameter()
		{
			return new AdomdParameter();
		}

		public int ExecuteNonQuery()
		{
			this.CheckCanExecute();
			this.connection.IExecuteProvider.ExecuteAny(this, this.Properties, this.PrivateParameters);
			this.Connection.OpenedReader = null;
			this.connection.MarkCacheNeedsCheckForValidness();
			return 1;
		}

		public AdomdDataReader ExecuteReader()
		{
			return this.ExecuteReader(CommandBehavior.Default);
		}

		public AdomdDataReader ExecuteReader(CommandBehavior behavior)
		{
			if ((behavior & CommandBehavior.SingleRow) == CommandBehavior.SingleRow)
			{
				throw new NotSupportedException();
			}
			this.CheckCanExecute();
			XmlReader xmlReader = this.connection.IExecuteProvider.ExecuteTabular(behavior, this, this.Properties, this.PrivateParameters);
			AdomdDataReader adomdDataReader = AdomdDataReader.CreateInstance(xmlReader, behavior, this.Connection);
			if (this.DataReaderConsumer != null)
			{
				this.DataReaderConsumer.SetDataReader(adomdDataReader);
			}
			this.Connection.OpenedReader = adomdDataReader;
			return adomdDataReader;
		}

		public object ExecuteScalar()
		{
			throw new NotSupportedException();
		}

		public void Prepare()
		{
			this.CheckCanExecute();
			this.connection.IExecuteProvider.Prepare(this, this.Properties, this.PrivateParameters);
			this.Connection.OpenedReader = null;
		}

		public CellSet ExecuteCellSet()
		{
			this.CheckCanExecute();
			MDDatasetFormatter formatter = this.connection.IExecuteProvider.ExecuteMultidimensional(this, this.Properties, this.PrivateParameters);
			this.Connection.OpenedReader = null;
			return new CellSet(this.connection, formatter);
		}

		public object Execute()
		{
			this.CheckCanExecute();
			XmlaReader xmlaReader = null;
			xmlaReader = this.connection.IExecuteProvider.Execute(this, this.Properties, this.PrivateParameters);
			this.Connection.OpenedReader = null;
			if (xmlaReader == null)
			{
				this.connection.MarkCacheNeedsCheckForValidness();
				return null;
			}
			object result;
			try
			{
				object obj = null;
				if (XmlaClient.IsExecuteResponseS(xmlaReader))
				{
					XmlaClient.StartExecuteResponseS(xmlaReader);
					if (XmlaClient.IsDatasetResponseS(xmlaReader))
					{
						MDDatasetFormatter mDDatasetFormatter = SoapFormatter.ReadDataSetResponse(xmlaReader);
						if (mDDatasetFormatter != null)
						{
							obj = new CellSet(this.connection, mDDatasetFormatter);
						}
					}
					else if (XmlaClient.IsRowsetResponseS(xmlaReader))
					{
						obj = AdomdDataReader.CreateInstance(xmlaReader, CommandBehavior.Default, this.connection);
					}
					else if (XmlaClient.IsEmptyResultS(xmlaReader))
					{
						this.connection.MarkCacheNeedsCheckForValidness();
						XmlaClient.ReadEmptyRootS(xmlaReader);
					}
					else
					{
						if (!XmlaClient.IsMultipleResult(xmlaReader) && !XmlaClient.IsAffectedObjects(xmlaReader))
						{
							this.connection.MarkCacheNeedsCheckForValidness();
							throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected dataset, rowset, empty or multiple results, got {0}", new object[]
							{
								xmlaReader.Name
							}));
						}
						this.connection.MarkCacheNeedsCheckForValidness();
						XmlaClient.ReadMultipleResults(xmlaReader);
					}
				}
				if (!(obj is AdomdDataReader))
				{
					xmlaReader.Close();
				}
				else
				{
					this.Connection.OpenedReader = obj;
				}
				result = obj;
			}
			catch (AdomdUnknownResponseException)
			{
				if (xmlaReader != null)
				{
					xmlaReader.Close();
				}
				throw;
			}
			catch (AdomdConnectionException)
			{
				throw;
			}
			catch (XmlException innerException)
			{
				if (xmlaReader != null)
				{
					xmlaReader.Close();
				}
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
			catch (IOException innerException2)
			{
				if (this.connection != null)
				{
					this.connection.Close(false);
				}
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch (XmlaException innerException3)
			{
				if (xmlaReader != null)
				{
					xmlaReader.Close();
				}
				throw new AdomdErrorResponseException(innerException3);
			}
			catch
			{
				if (this.connection != null)
				{
					this.connection.Close(false);
				}
				throw;
			}
			return result;
		}

		public XmlReader ExecuteXmlReader()
		{
			this.CheckCanExecute();
			XmlaReader xmlaReader = this.connection.IExecuteProvider.Execute(this, this.Properties, this.PrivateParameters);
			this.Connection.OpenedReader = null;
			if (xmlaReader == null)
			{
				return null;
			}
			XmlReader result;
			try
			{
				XmlaClient.ReadUptoRoot(xmlaReader);
				if (!XmlaClient.IsRowsetResponseS(xmlaReader) && !XmlaClient.IsDatasetResponseS(xmlaReader))
				{
					this.connection.MarkCacheNeedsCheckForValidness();
				}
				xmlaReader.MaskEndOfStream = true;
				xmlaReader.SkipElements = false;
				this.Connection.OpenedReader = xmlaReader;
				result = xmlaReader;
			}
			catch (AdomdUnknownResponseException)
			{
				if (xmlaReader != null)
				{
					xmlaReader.Close();
				}
				throw;
			}
			catch (AdomdConnectionException)
			{
				throw;
			}
			catch (XmlException innerException)
			{
				if (xmlaReader != null)
				{
					xmlaReader.Close();
				}
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
			catch (IOException innerException2)
			{
				if (this.connection != null)
				{
					this.connection.Close(false);
				}
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch (XmlaException innerException3)
			{
				if (xmlaReader != null)
				{
					xmlaReader.Close();
				}
				throw new AdomdErrorResponseException(innerException3);
			}
			catch
			{
				if (this.connection != null)
				{
					this.connection.Close(false);
				}
				throw;
			}
			return result;
		}

		public AdomdCommand Clone()
		{
			return new AdomdCommand(this);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		IDbDataParameter IDbCommand.CreateParameter()
		{
			return this.CreateParameter();
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			return this.ExecuteReader();
		}

		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			return this.ExecuteReader(behavior);
		}

		private void AddCommandProperty(string propKey, object propValue)
		{
			AdomdProperty adomdProperty = new AdomdProperty(propKey, propValue);
			if (this.commandProperties == null)
			{
				this.commandProperties = new AdomdPropertyCollection();
			}
			else
			{
				int num = this.commandProperties.InternalCollection.IndexOf(adomdProperty);
				if (num != -1)
				{
					this.commandProperties.InternalCollection.RemoveAt(num);
				}
			}
			this.commandProperties.Add(adomdProperty);
		}

		private void CheckCanExecute()
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException(SR.Command_ConnectionIsNotSet);
			}
			AdomdUtils.CheckConnectionOpened(this.connection);
			if (this.CommandText == null && this.CommandStream == null)
			{
				throw new InvalidOperationException(SR.Command_CommandTextCommandStreamNotSet);
			}
			if (this.CommandStream != null && this.CommandText != null)
			{
				throw new InvalidOperationException(SR.Command_CommandTextCommandStreamBothSet);
			}
		}

		private static bool IsMdx(string statement)
		{
			statement = statement.Trim();
			return statement.Length == 0 || statement[0] != '<';
		}
	}
}
