using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AdomdTransaction : IDbTransaction, IDisposable
	{
		private const string mdxBeginTransaction = "BEGIN TRANSACTION";

		private const string mdxCommitTransaction = "COMMIT TRANSACTION";

		private const string mdxRollbackTransaction = "ROLLBACK TRANSACTION";

		private AdomdConnection connection;

		private bool complete = true;

		internal bool IsCompleted
		{
			get
			{
				return this.complete;
			}
		}

		public AdomdConnection Connection
		{
			get
			{
				this.CheckDisposed();
				return this.connection;
			}
		}

		public IsolationLevel IsolationLevel
		{
			get
			{
				this.CheckDisposed();
				return IsolationLevel.ReadCommitted;
			}
		}

		IDbConnection IDbTransaction.Connection
		{
			get
			{
				return this.Connection;
			}
		}

		internal AdomdTransaction(AdomdConnection connection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			this.connection = connection;
			this.ExecuteMdx("BEGIN TRANSACTION");
			this.complete = false;
		}

		~AdomdTransaction()
		{
		}

		public void Rollback()
		{
			this.CheckDisposed();
			if (!this.complete)
			{
				this.ExecuteMdx("ROLLBACK TRANSACTION");
				this.complete = true;
				return;
			}
			throw new InvalidOperationException(SR.TransactionAlreadyComplete);
		}

		public void Commit()
		{
			this.CheckDisposed();
			if (!this.complete)
			{
				this.ExecuteMdx("COMMIT TRANSACTION");
				this.complete = true;
				return;
			}
			throw new InvalidOperationException(SR.TransactionAlreadyComplete);
		}

		public void Dispose()
		{
			try
			{
				if (!this.complete && this.connection != null)
				{
					this.Rollback();
				}
				this.connection = null;
			}
			catch (AdomdException)
			{
			}
		}

		private void ExecuteMdx(string mdx)
		{
			AdomdCommand adomdCommand = new AdomdCommand(mdx, this.connection);
			adomdCommand.ExecuteNonQuery();
		}

		private void CheckDisposed()
		{
			if (this.connection == null)
			{
				throw new ObjectDisposedException("AdomdTransaction");
			}
		}
	}
}
