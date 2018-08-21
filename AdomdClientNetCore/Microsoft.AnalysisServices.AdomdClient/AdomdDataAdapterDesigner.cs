using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class AdomdDataAdapterDesigner : IDesigner, IDisposable, IDesignerFilter
	{
		private IComponent theComponent;

		private static readonly string[] propertiesToHide = new string[]
		{
			"AcceptChangesDuringFill",
			"AcceptChangesDuringUpdate",
			"TableMappings",
			"UpdateBatchSize",
			"ContinueUpdateOnError"
		};

		IComponent IDesigner.Component
		{
			get
			{
				return this.theComponent;
			}
		}

		DesignerVerbCollection IDesigner.Verbs
		{
			get
			{
				return null;
			}
		}

		internal AdomdDataAdapterDesigner()
		{
			this.theComponent = null;
		}

		void IDesigner.DoDefaultAction()
		{
		}

		void IDesigner.Initialize(IComponent component)
		{
			this.theComponent = component;
		}

		void IDesignerFilter.PostFilterAttributes(IDictionary attributes)
		{
		}

		void IDesignerFilter.PostFilterEvents(IDictionary events)
		{
		}

		void IDesignerFilter.PostFilterProperties(IDictionary properties)
		{
			string[] array = AdomdDataAdapterDesigner.propertiesToHide;
			for (int i = 0; i < array.Length; i++)
			{
				string key = array[i];
				if (properties.Contains(key))
				{
					properties.Remove(key);
				}
			}
		}

		void IDesignerFilter.PreFilterAttributes(IDictionary attributes)
		{
		}

		void IDesignerFilter.PreFilterEvents(IDictionary events)
		{
		}

		void IDesignerFilter.PreFilterProperties(IDictionary properties)
		{
		}

		void IDisposable.Dispose()
		{
			this.theComponent = null;
		}
	}
}
