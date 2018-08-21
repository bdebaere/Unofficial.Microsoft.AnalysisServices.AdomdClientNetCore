using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class OlapInfoCube
	{
		private DataRow cubeInfo;

		public string CubeName
		{
			get
			{
				return this.cubeInfo["CubeName"].ToString();
			}
		}

		public DateTime LastDataUpdate
		{
			get
			{
				if (this.cubeInfo["LastDataUpdate"] is DBNull)
				{
					throw new NotSupportedException(SR.NotSupportedByProvider);
				}
				return (DateTime)this.cubeInfo["LastDataUpdate"];
			}
		}

		public DateTime LastSchemaUpdate
		{
			get
			{
				if (this.cubeInfo["LastSchemaUpdate"] is DBNull)
				{
					throw new NotSupportedException(SR.NotSupportedByProvider);
				}
				return (DateTime)this.cubeInfo["LastSchemaUpdate"];
			}
		}

		internal OlapInfoCube(DataRow cubeInfo)
		{
			this.cubeInfo = cubeInfo;
		}
	}
}
