//
// BackgroundProgressMonitor.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.IO;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Core.Gui.Components;
using MonoDevelop.Core.Gui.ProgressMonitoring;
using Gtk;

namespace MonoDevelop.Ide.Gui
{
	internal class BackgroundProgressMonitor: BaseProgressMonitor
	{
		string title;
		IStatusIcon icon;
		
		public BackgroundProgressMonitor (string title, string iconName)
		{
			this.title = title;
			Gdk.Pixbuf img = Services.Resources.GetBitmap (iconName, Gtk.IconSize.Menu);
			icon = IdeApp.Workbench.StatusBar.ShowStatusIcon (img);
			if (icon == null)
				Runtime.LoggingService.Error ("Icon '" + iconName + "' not found.");
		}
		
		protected override void OnProgressChanged ()
		{
			if (icon != null) {
				if (UnknownWork)
					icon.ToolTip = string.Format ("{0}\n{1}", title, CurrentTask);
				else
					icon.ToolTip = string.Format ("{0} ({1}%)\n{2}", title, (int)(GlobalWork * 100), CurrentTask);
			}
		}
		
		public override void Dispose()
		{
			base.Dispose ();
			if (icon != null)
				icon.Dispose ();
		}
	}
}
