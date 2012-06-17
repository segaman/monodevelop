//
//	HtmlNode.cs - 
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
		public TextNode ()
		{
			
		}

		#region INode implementation
		string INode.ToHtml ()
		{
			throw new System.NotImplementedException ();
		}

		int INode.AneId {
			get {
				throw new System.NotImplementedException ();
			}
		}

		System.Collections.Generic.List<INode> INode.Children {
			get {
				throw new System.NotImplementedException ();
			}
		}

		INode INode.Parent {
			get {
				throw new System.NotImplementedException ();
			}
		}
		#endregion
	}
}

