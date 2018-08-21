using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningParameter
	{
		private string name;

		private string paramValue;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Value
		{
			get
			{
				return this.paramValue;
			}
		}

		internal MiningParameter(string name, string paramValue)
		{
			this.name = name;
			this.paramValue = paramValue;
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}
