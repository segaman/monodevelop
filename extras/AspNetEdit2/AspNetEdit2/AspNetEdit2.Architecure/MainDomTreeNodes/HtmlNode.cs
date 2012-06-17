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
using System.Collections.Generic;

namespace AspNetEdit2.Architecture
{
	public class HtmlNode : INode
	{
		public HtmlNode ()
		{
			
		}
		
		#region INode implementation
		
		public int AneId {
			get {
				throw new System.NotImplementedException ();
			}
		}
		
		public string ToHtml ()
		{
			throw new System.NotImplementedException ();
		}

		public List<INode> Children {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public INode Parent {
			get {
				throw new System.NotImplementedException ();
			}
		}
		#endregion
	}
}

