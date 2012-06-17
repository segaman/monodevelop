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

using WebKit;

using MonoDevelop.Ide.Gui;
using MonoDevelop.DesignerSupport;
using MonoDevelop.AspNet;
using MonoDevelop.AspNet.Parser;
using MonoDevelop.AspNet.Parser.Dom;
using MonoDevelop.AspNet.Parser.Internal;
using MonoDevelop.SourceEditor;

namespace AspNetEdit2.Integration
{
	public class AspNetEdit2ViewContent : AbstractAttachableViewContent
	{
		Frame designerFrame;
		IViewContent viewContent;
		WebView view;
		
		public AspNetEdit2ViewContent (IViewContent viewContent)
		{
			this.viewContent = viewContent;
			
			designerFrame = new Frame ();
			designerFrame.CanFocus = true;
			designerFrame.Shadow = ShadowType.Out;
			designerFrame.BorderWidth = 1;
		}
		
		public override Gtk.Widget Control {
			get { return designerFrame; }
		}
		
		public override string TabPageLabel {
			get { return "Browser Preview"; }
		}
		
		private bool viewDisposed;
		
		public override void Selected ()
		{
			this.view = new WebView ();
			designerFrame.Add (view);
			designerFrame.ShowAll ();
			
			SourceEditorView tempView = viewContent as SourceEditorView;
			if (tempView != null) {
				
				// Just trying out how the parser works and what is it's output
				#region Testing_AspNetParser
				/*
				AspParser testParser = null;
				
				using (StringReader strRd = new StringReader (tempView.Text)) {
					testParser = new AspParser (null, strRd);
				}
				
				testParser.TagParsed += new TagParsedHandler (TagParsed);
				
				testParser.Parse ();
				*/
				
				AspNetParser parser = new AspNetParser ();
				
				StringReader reader = new StringReader (tempView.Text);
				
				AspNetParsedDocument parsedDocument = (AspNetParsedDocument)parser.Parse (true, null, reader, null);	
				
				foreach (Node n in parsedDocument.RootNode) {
				
					string something = n.ToString ();
					
				}
				
				#endregion
				
				view.LoadString (tempView.Text, null, null, null);
				view.Show();
				viewDisposed = false;

			}
			
		}
		/*
		void TagParsed (ILocation location, TagType tagtype, string tagid, TagAttributes attributes)
		{
			
		}
		*/
		public override void Deselected ()
		{
			designerFrame.Remove(view);
			view.Dispose();
			viewDisposed = true;
		}
		
		public override void Dispose ()
		{
			designerFrame.Remove(view);
			if (!viewDisposed)
				this.view.Dispose ();
		}
	}
}

