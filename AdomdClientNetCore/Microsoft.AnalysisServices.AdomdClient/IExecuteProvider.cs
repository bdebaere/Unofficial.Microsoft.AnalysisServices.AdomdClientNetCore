using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IExecuteProvider
	{
		MDDatasetFormatter ExecuteMultidimensional(ICommandContentProvider contentProvider, AdomdPropertyCollection commandProperties, IDataParameterCollection parameters);

		XmlaReader ExecuteTabular(CommandBehavior behavior, ICommandContentProvider contentProvider, AdomdPropertyCollection commandProperties, IDataParameterCollection parameters);

		void ExecuteAny(ICommandContentProvider contentProvider, AdomdPropertyCollection commandProperties, IDataParameterCollection parameters);

		XmlaReader Execute(ICommandContentProvider contentProvider, AdomdPropertyCollection commandProperties, IDataParameterCollection parameters);

		void Prepare(ICommandContentProvider contentProvider, AdomdPropertyCollection commandProperties, IDataParameterCollection parameters);
	}
}
