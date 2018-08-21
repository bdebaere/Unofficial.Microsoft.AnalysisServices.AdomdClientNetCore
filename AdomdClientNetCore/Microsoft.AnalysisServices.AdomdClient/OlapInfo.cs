using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class OlapInfo
	{
		private MDDatasetFormatter formatter;

		private CubeInfo theCubeInfo;

		private AxesInfo theAxesInfo;

		private CellInfo theCellInfo;

		public CubeInfo CubeInfo
		{
			get
			{
				if (this.theCubeInfo == null)
				{
					this.theCubeInfo = new CubeInfo(this.formatter);
				}
				return this.theCubeInfo;
			}
		}

		public AxesInfo AxesInfo
		{
			get
			{
				if (this.theAxesInfo == null)
				{
					this.theAxesInfo = new AxesInfo(this.formatter);
				}
				return this.theAxesInfo;
			}
		}

		public CellInfo CellInfo
		{
			get
			{
				if (this.theCellInfo == null)
				{
					this.theCellInfo = new CellInfo(this.formatter);
				}
				return this.theCellInfo;
			}
		}

		internal OlapInfo(MDDatasetFormatter formatter)
		{
			this.formatter = formatter;
		}
	}
}
