//
//	TextNode.cs - Contains a text node of the structure
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

namespace AspNetEdit2.Architecture
{
	public class TextNode : INode
	{
		int aneId;
		string text;
		IParentNode parent;
		// TODO: check if the whole text is whitespace
		// 		I don't like whitespace in HTML, so there will be no
		//		ws in the editor view >:) mwuhahaha
		
		public TextNode (int id, string content, IParentNode parentNode)
		{
			aneId = id;
			text = content;
			parent = parentNode;
		}

		#region INode implementation
		string INode.ToHtml ()
		{
			// the text is in a span element so that editing it would bring
			// so that editing it would be possible
			// TODO: ane_id attribute
			// TODO: spans with ids only the editable text in the <body> tag
			return /*"<span>"  + */ text /*+ "</span>"*/;
		}

		int INode.AneId {
			get {
				return aneId;
			}
		}
		
		public string Name {
			get {
				return string.Empty;
			}
		}

		IParentNode INode.Parent {
			get {
				return parent;
			}
		}
		#endregion
	}
}

