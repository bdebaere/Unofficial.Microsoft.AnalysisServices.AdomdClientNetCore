using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal struct SecurityHandle
	{
		public IntPtr Lower;

		public IntPtr Upper;

		internal bool Initialized
		{
			get
			{
				return this.Lower.ToInt64() != -1L || this.Upper.ToInt64() != -1L;
			}
		}

		internal void Reset()
		{
			this.Lower = new IntPtr(-1);
			this.Upper = new IntPtr(-1);
		}
	}
}
