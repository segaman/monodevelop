//
//	HtmlNode.cs - Html tag nodes and their attributes
// 
//
//	Authors:
//		Petar Dodev <petar.dodev@gmail.com>
//	
//		Copyright (c) 2012 Petar Dodev
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
using System.Collections.Generic;

namespace AspNetEdit2.Architecture
{
	public class HtmlNode : IParentNode
	{
		int aneId;
		string tagName;
		List<INode> children;
		IParentNode parent;
		// attributes ?!
		// bool selfclosing ?
		
		// TODO: implement storing attributes
		
		public HtmlNode (int id, string name, IParentNode parentNode)
		{
			aneId = id;
			tagName = name;
			parent = parentNode;
			children = null;
		}

		#region INode implementation
		string INode.ToHtml ()
		{
			string output = string.Empty;
			
			output = "<" + tagName;
			// TODO: serialize attributes
			// TODO: implement the ane_id attribute for recognizing elements from the webview
			// TODO: handle selfclosing tags?!>!>>!>!>?
			output += ">";
			
			if (children != null) {
				foreach (INode child in children)
					output += child.ToHtml ();
			}
			
			return output + "</" + tagName + ">";
		}

		int INode.AneId {
			get {
				return aneId;
			}
		}
		
		public string Name {
			get {
				return tagName;
			}
		}

		public IParentNode Parent {
			get {
				return parent;
			}
		}
		#endregion

		#region IParentNode implementation
		void IParentNode.AddChild (INode child)
		{
			if (children == null) {
				children = new List<INode> ();
			}
			
			children.Add (child);
		}

		List<INode> IParentNode.Children {
			get {
				return children;
			}
		}
		#endregion
	}
}

