//
//	INode.cs - Defines an interface for managing the nodes 
// 	in the Main DOM tree representation of the edited document
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
	public interface INode
	{
		// AspNetEdit's internal ID for the node
		int AneId { get;}
		
		// A list of the elements children
		List<INode> Children { get; }
		
		// Reference to the element's parent
		INode Parent { get; }
		
		// Method, which will define how to serialize
		// the class of elements to HTML for the visual editor
		//
		// TODO: figure out if the method should be split in three methods
		//   for the openning tag, the innerHTML and the closing tag
		// OR return a string[]....
		// for now each class implementing the interface should
		// call the method for his children, so it's kind of a hidden recursion,
		// which might become a performance issue in verry deep tree structures
		string ToHtml();
	}
}

