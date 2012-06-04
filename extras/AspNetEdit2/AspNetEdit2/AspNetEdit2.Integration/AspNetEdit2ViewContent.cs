using System;

using Gtk;

using WebKit;

using MonoDevelop.Ide.Gui;
using MonoDevelop.DesignerSupport;
using MonoDevelop.AspNet;
using MonoDevelop.AspNet.Parser;
using MonoDevelop.AspNet.Parser.Dom;
using MonoDevelop.SourceEditor;
using Mono.TextEditor;

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
				
				view.LoadString (tempView.Text, null, null, null);
				view.Show();
				viewDisposed = false;

			}
			
		}
		
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

