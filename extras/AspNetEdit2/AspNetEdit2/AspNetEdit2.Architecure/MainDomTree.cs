//
//	MainDomTree.cs - Stores and manages a canonical DOM tree
//					representation of the edited document.
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
using System.Collections.Generic;

using MonoDevelop.AspNet;
using MonoDevelop.AspNet.Parser;
using MonoDevelop.AspNet.Parser.Dom;

namespace AspNetEdit2.Architecture
{
	public class MainDomTree
	{
		// List of the root level elements
		// each INode contains a list of its own
		// resembling the tree structure of the DOM
		//List<INode> domTree;
		
		// contains the document as parsed by the  AspNetParser
		// will be used when serializing to the sourceEditor view is implemented...
		//AspNetParsedDocument parsedDocument;
		
		// Contains the root node. It has the ability to parse a ASP.NET documentt
		// and build a DOM tree. Also it can serialize itself to HTML for displaying in the
		// webkit's designer surface
		RootNode rootNode;
		// TODO: drop the tree when the view is being disposed
		
		VisualEditor vEditor;
		
		public MainDomTree ()
		{
			rootNode = new RootNode ();
			vEditor = new VisualEditor ();
		}
		
		
		public void BuildTree (string fileName, string document)
		{
			try {
				rootNode.ParseDocument (fileName, document);
			} catch (Exception e) {
				// TODO: error handling mechanism
			}
		}
		
		public void DisplayEditor (Gtk.Frame designerFrame)
		{
			vEditor.SetFrame (designerFrame);
			
			vEditor.LoadString (rootNode.ToHtml (), null, null, null);
		}
		
		public void CleanUp ()
		{
			vEditor.DisposeView ();
		}
	}
}

