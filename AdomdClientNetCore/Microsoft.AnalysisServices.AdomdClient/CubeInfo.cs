using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class CubeInfo
	{
		private MDDatasetFormatter formatter;

		private OlapInfoCubeCollection theCubes;

		public OlapInfoCubeCollection Cubes
		{
			get
			{
				if (this.theCubes == null)
				{
					this.theCubes = new OlapInfoCubeCollection(this.formatter);
				}
				return this.theCubes;
			}
		}

		internal CubeInfo(MDDatasetFormatter formatter)
		{
			this.formatter = formatter;
			this.theCubes = null;
		}
	}
}
