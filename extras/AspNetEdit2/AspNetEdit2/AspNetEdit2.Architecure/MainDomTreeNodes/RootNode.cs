//
//	RootNode.cs - Represents the root node of a document
//	 and can parse itself
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
using System.IO;
using System.Collections.Generic;

using MonoDevelop.AspNet.Parser;
using MonoDevelop.AspNet.Parser.Dom;
using MonoDevelop.AspNet.Parser.Internal;

namespace AspNetEdit2.Architecture
{
	internal class RootNode : IParentNode
	{
		List<INode> children;
		List<ParseException> errors;
		// TODO: implement error handling
		
		// TODO: unique ane_ids for the editable nodes		
		
		public RootNode ()
		{
			children = new List<INode> ();
			errors = new List<ParseException> ();
		}

		#region INode implementation
		public string ToHtml ()
		{
			string output = string.Empty;
			
			foreach (INode node in children)
				output += node.ToHtml ();
			
			// TODO: remove spans for TextNodes outside the body tag!!!
			
			return output;
		}

		public int AneId {
			get {
				return 0;
			}
		}
		
		public string Name {
			get {
				return string.Empty;
			}
		}

		IParentNode INode.Parent {
			get {
				return null;
			}
		}
		#endregion

		#region IParentNode implementation
		public void AddChild (INode child)
		{
			children.Add (child);
		}

		List<INode> IParentNode.Children {
			get {
				return children;
			}
		}
		#endregion		
		
		
		#region Parser
		static string[] implicitSelfClosing = { "hr", "br", "img" };
		static string[] implicitCloseOnBlock = { "p" };
		static string[] blockLevel = { "p", "div", "hr", "img", "blockquote", "html", "body", "form" };
		
		IParentNode currentNode;
		
		public void ParseDocument (string fileName, string document)
		{
			AspParser parser = null;
			
			using (StringReader reader = new StringReader(document)) {
				parser = new AspParser (fileName, reader);
			}
			
			currentNode = this;
			
			parser.TagParsed += TagParsed;
			parser.TextParsed += TextParsed;
			parser.Error += ParseError;
			
			parser.Parse ();
		}
		
		void TagParsed (ILocation location, TagType tagtype, string tagId, TagAttributes attributes)
		{
			switch (tagtype) {
			case TagType.Close:
				if (currentNode.Name != tagId) {
					errors.Add (new ParseException (
							location,
							"Closing tag '" + tagId + "' does not match opening tag '" + currentNode.Name + "'."
					)
					);
				}
				
				if (currentNode.Parent == null) {
					errors.Add (new ParseException (
						location, 
						"Incorrect structure of the HTML file. Found a closing of type " + tagId + 
						" outside the root element."
					)
					);
				} else {
					// going one level up
					currentNode = currentNode.Parent;
				}
				break;
			case TagType.CodeRender:
				errors.Add (new ParseException (location, "CodeRender TagType not implemented yet: " + location.PlainText));
				break;
			case TagType.CodeRenderExpression:
				errors.Add (new ParseException (
					location,
					"CodeRenderExpression TagType not implemented yet: " + location.PlainText));
				break;
			case TagType.DataBinding:
				errors.Add (new ParseException (location, "DataBinding TagType not implemented yet: " + location.PlainText));
				break;
			case TagType.Directive:
				errors.Add (new ParseException (location, "Directive TagType not implemented yet: " + location.PlainText));
				break;
			case TagType.Include:
				errors.Add (new ParseException (location, "Include TagType not implemented yet: " + location.PlainText));
				break;
			case TagType.SelfClosing:
				currentNode.AddChild (new HtmlNode (0, tagId, currentNode));
				break;
			case TagType.ServerComment:
				errors.Add (new ParseException (location, "ServerComment TagType not implemented yet: " + location.PlainText));
				break;
			case TagType.Tag:				
				//HACK: implicit close on block level in HTML4
				if (Array.IndexOf (implicitCloseOnBlock, currentNode.Name.ToLowerInvariant ()) > -1
					&& Array.IndexOf (blockLevel, tagId.ToLowerInvariant ()) > -1) {
					errors.Add (new ParseException (
						location,
						"Unclosed " + currentNode.Name + " tag. Assuming implicitly closed by block level tag."
					)
					);
					currentNode = currentNode.Parent;
				}
				
				//create and add the new tag
				HtmlNode child = new HtmlNode (0, tagId, currentNode);
				currentNode.AddChild (child);
				
				//HACK: implicitly closing tags in HTML4
				if (Array.IndexOf (implicitSelfClosing, tagId.ToLowerInvariant ()) > -1) {
					errors.Add (new ParseException (location, "Unclosed " + tagId + " tag. Assuming implicitly closed."));
					// mark it as a selfclosing
				} else {
					currentNode = (IParentNode)child;
				}
				break;
			case TagType.Text:
				errors.Add (new ParseException (location, "Text TagType not implemented yet: " + location.PlainText));
				break;
			}
		}
		
		void TextParsed (ILocation location, string text)
		{
			currentNode.AddChild (new TextNode(0, text, currentNode));
		}
		
		void ParseError (ILocation location, string message)
		{
			errors.Add (new ParseException (location, message));
		}
		#endregion
	}
}

