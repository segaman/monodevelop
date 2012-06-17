//
//	AspNEtEdit2DisplayBinding.cs
//
//	Authors:
//		Petar Dodev <petar.dodev@gmail.com>
//	
//		Copyright 2012 Petar Dodev
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

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

