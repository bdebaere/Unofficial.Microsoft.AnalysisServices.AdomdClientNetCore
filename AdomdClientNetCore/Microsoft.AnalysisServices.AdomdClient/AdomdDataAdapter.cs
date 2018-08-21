using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Designer(typeof(AdomdDataAdapterDesigner))]
	public sealed class AdomdDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, IDataReaderConsumer
	{
		private AdomdDataReader dataReader;

		private bool isAffectedObjects;

		private string AffectedObjectsDatabase;

		private int AffectedObjectsBaseVersion;

		private int AffectedObjectsCurrentVersion;

		public new AdomdCommand SelectCommand
		{
			get
			{
				return (AdomdCommand)((IDbDataAdapter)this).SelectCommand;
			}
			set
			{
				((IDbDataAdapter)this).SelectCommand = value;
			}
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				return null;
			}
			set
			{
				if (value != null)
				{
					throw new NotSupportedException();
				}
			}
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				return null;
			}
			set
			{
				if (value != null)
				{
					throw new NotSupportedException();
				}
			}
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				return null;
			}
			set
			{
				if (value != null)
				{
					throw new NotSupportedException();
				}
			}
		}

		public AdomdDataAdapter()
		{
		}

		public AdomdDataAdapter(AdomdCommand selectCommand)
		{
			this.SelectCommand = selectCommand;
		}

		public AdomdDataAdapter(string selectCommandText, AdomdConnection selectConnection) : this(new AdomdCommand(selectCommandText, selectConnection))
		{
		}

		public AdomdDataAdapter(string selectCommandText, string selectConnectionString) : this(selectCommandText, new AdomdConnection(selectConnectionString))
		{
		}

		protected override int Fill(DataTable dataTable, IDbCommand command, CommandBehavior behavior)
		{
			AdomdCommand adomdCommand = command as AdomdCommand;
			if (adomdCommand != null)
			{
				adomdCommand.DataReaderConsumer = this;
			}
			int result = base.Fill(dataTable, command, behavior);
			this.AdjustDataTableName(dataTable);
			return result;
		}

		protected override int Fill(DataTable[] dataTables, int startRecord, int maxRecords, IDbCommand command, CommandBehavior behavior)
		{
			AdomdCommand adomdCommand = command as AdomdCommand;
			if (adomdCommand != null)
			{
				adomdCommand.DataReaderConsumer = this;
			}
			int result = base.Fill(dataTables, startRecord, maxRecords, command, behavior);
			this.AdjustDataTableNames(dataTables);
			return result;
		}

		protected override int Fill(DataSet dataSet, int startRecord, int maxRecords, string srcTable, IDbCommand command, CommandBehavior behavior)
		{
			AdomdCommand adomdCommand = command as AdomdCommand;
			if (adomdCommand != null)
			{
				adomdCommand.DataReaderConsumer = this;
			}
			int result = base.Fill(dataSet, startRecord, maxRecords, srcTable, command, behavior);
			this.AdjustDataSet(dataSet);
			return result;
		}

		public override int Update(DataSet dataSet)
		{
			throw new NotSupportedException();
		}

		protected override int Update(DataRow[] dataRows, DataTableMapping tableMapping)
		{
			throw new NotSupportedException();
		}

		protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			throw new NotSupportedException();
		}

		protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			throw new NotSupportedException();
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			throw new NotSupportedException();
		}

		protected override void OnRowUpdated(RowUpdatedEventArgs value)
		{
			throw new NotSupportedException();
		}

		private void AdjustDataTableName(DataTable dataTable)
		{
			if (this.dataReader != null)
			{
				this.AdjustDataTableNames(new DataTable[]
				{
					dataTable
				});
			}
		}

		private void AdjustDataTableNames(DataTable[] dataTables)
		{
			XmlaDataReader xmlaDataReader = (this.dataReader != null) ? this.dataReader.XmlaDataReader : null;
			if (xmlaDataReader != null && xmlaDataReader.RowsetNames != null && xmlaDataReader.RowsetNames.Count == dataTables.Length)
			{
				for (int i = 0; i < dataTables.Length; i++)
				{
					if (!string.IsNullOrEmpty(xmlaDataReader.RowsetNames[i]))
					{
						dataTables[i].TableName = xmlaDataReader.RowsetNames[i];
					}
				}
			}
		}

		private void AdjustDataSet(DataSet dataSet)
		{
			XmlaDataReader xmlaDataReader = (this.dataReader != null) ? this.dataReader.XmlaDataReader : null;
			if (xmlaDataReader != null)
			{
				DataTable[] array = new DataTable[dataSet.Tables.Count];
				dataSet.Tables.CopyTo(array, 0);
				this.AdjustDataTableNames(array);
			}
			if (this.isAffectedObjects)
			{
				dataSet.DataSetName = "AffectedObjects";
				dataSet.ExtendedProperties["BaseVersion"] = this.AffectedObjectsBaseVersion;
				dataSet.ExtendedProperties["CurrentVersion"] = this.AffectedObjectsCurrentVersion;
				dataSet.ExtendedProperties["name"] = this.AffectedObjectsDatabase;
			}
		}

		void IDataReaderConsumer.SetDataReader(AdomdDataReader reader)
		{
			this.dataReader = reader;
			AdomdAffectedObjectsReader adomdAffectedObjectsReader = reader as AdomdAffectedObjectsReader;
			if (adomdAffectedObjectsReader != null)
			{
				this.isAffectedObjects = true;
				this.AffectedObjectsDatabase = adomdAffectedObjectsReader.Database;
				this.AffectedObjectsBaseVersion = adomdAffectedObjectsReader.BaseVersion;
				this.AffectedObjectsCurrentVersion = adomdAffectedObjectsReader.CurrentVersion;
			}
		}
	}
}
