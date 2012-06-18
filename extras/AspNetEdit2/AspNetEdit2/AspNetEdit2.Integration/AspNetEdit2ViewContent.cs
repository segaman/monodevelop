//
//	AspNEtEdit2ViewContent.cs - Adds a secondary ViewContet for ASP.NET files
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
using System.IO;

using Gtk;

//using WebKit;

using MonoDevelop.Ide.Gui;
using MonoDevelop.DesignerSupport;
using MonoDevelop.AspNet;
using MonoDevelop.AspNet.Parser;
using MonoDevelop.AspNet.Parser.Dom;
using MonoDevelop.AspNet.Parser.Internal;
using MonoDevelop.SourceEditor;

using AspNetEdit2.Architecture;

namespace AspNetEdit2.Integration
{
	public class AspNetEdit2ViewContent : AbstractAttachableViewContent
	{
		Frame designerFrame;
		IViewContent viewContent;
//		WebView view;
		MainDomTree domTree; // for testing purposes the Main DOM tree is contained here
							// will be moved whe a DesignerHost is implemented
		
		public AspNetEdit2ViewContent (IViewContent viewContent)
		{
			this.viewContent = viewContent;
			
			designerFrame = new Frame ();
			designerFrame.CanFocus = true;
			designerFrame.Shadow = ShadowType.Out;
			designerFrame.BorderWidth = 1;
			
			domTree = new MainDomTree ();
		}
		
		public override Gtk.Widget Control {
			get { return designerFrame; }
		}
		
		public override string TabPageLabel {
			get { return "Browser Preview"; }
		}
		
//		private bool viewDisposed;
		
		public override void Selected ()
		{
//			this.view = new WebView ();
//			designerFrame.Add (view);
//			designerFrame.ShowAll ();
			
			SourceEditorView tempView = viewContent as SourceEditorView;
			if (tempView != null) {
				
				domTree.BuildTree (null, tempView.Text);
				
				domTree.DisplayEditor (designerFrame);
				
//				view.LoadString (tempView.Text, null, null, null);
//				view.Show();
//				viewDisposed = false;

			}
			
		}
		
		public override void Deselected ()
		{
//			designerFrame.Remove(view);
//			view.Dispose();
//			viewDisposed = true;
		}
		
		public override void Dispose ()
		{
//			designerFrame.Remove(view);
//			if (!viewDisposed)
//				this.view.Dispose ();
			
			designerFrame.Dispose ();
			domTree.CleanUp ();
		}
	}
}

