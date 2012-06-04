using System;

using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Codons;
using MonoDevelop.Projects;

using MonoDevelop.AspNet;

namespace AspNetEdit2.Integration
{
	public class AspNetEdit2DisplayBinding : IAttachableDisplayBinding
	{
		public bool CanAttachTo (IViewContent content)
		{
			
			if (AspNetAppProject.DetermineWebSubtype (content.IsUntitled? content.UntitledName : content.ContentName) == WebSubtype.Html)
				return true;
			
			return false;
		}
		
		public IAttachableViewContent CreateViewContent (IViewContent viewContent)
		{
			return new AspNetEdit2ViewContent (viewContent);
		}
	}
}

