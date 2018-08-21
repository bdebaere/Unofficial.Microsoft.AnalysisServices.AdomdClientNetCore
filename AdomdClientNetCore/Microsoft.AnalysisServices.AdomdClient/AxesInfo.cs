using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AxesInfo
	{
		private MDDatasetFormatter formatter;

		private OlapInfoAxisCollection axes;

		private OlapInfoAxis filterAxis;

		public OlapInfoAxisCollection Axes
		{
			get
			{
				if (this.axes == null)
				{
					this.axes = new OlapInfoAxisCollection(this.formatter);
				}
				return this.axes;
			}
		}

		public OlapInfoAxis FilterAxis
		{
			get
			{
				if (this.filterAxis == null && this.formatter.FilterAxis != null)
				{
					this.filterAxis = new OlapInfoAxis(this.formatter.FilterAxis);
				}
				return this.filterAxis;
			}
		}

		internal AxesInfo(MDDatasetFormatter formatter)
		{
			this.formatter = formatter;
		}
	}
}
