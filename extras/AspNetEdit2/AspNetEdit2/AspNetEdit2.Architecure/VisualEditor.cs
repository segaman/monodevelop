//
//	VisualEditor.cs - Container of the designer surface
//					
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

using WebKit;

using Gtk;

namespace AspNetEdit2.Architecture
{
	public class VisualEditor
	{
		Frame designerFrame;
		WebView view;
		
		public VisualEditor ()
		{
			designerFrame = null;
			view = null;
		}
		
		public void SetFrame (Frame frame)
		{
			designerFrame = frame;
		}
		
		public void LoadString (string content, string mimeType, string encoding, string baseUrl)
		{
			if (designerFrame != null) {
				if (view != null)
					view.Dispose ();
				
				view = new WebView ();
				
				designerFrame.Add (view);
				designerFrame.ShowAll ();
				view.LoadString (content, mimeType, encoding, baseUrl);
				view.Show ();
			}
		}
		
		public void DisposeView ()
		{
			view.Dispose ();
			view = null;
		}
	}
}

