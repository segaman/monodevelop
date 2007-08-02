using System;
using System.Collections;
using System.IO;
using System.Web.Services.Discovery;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;
using MonoDevelop.WebReferences.Dialogs;

namespace MonoDevelop.WebReferences.Commands
{
	/// <summary>Defines the properties and methods for the WebReferenceCommandHandler class.</summary>
	public class WebReferenceCommandHandler : NodeCommandHandler
	{
		/// <summary>Execute the command for adding a new web reference to a project.</summary>
		[CommandHandler (MonoDevelop.WebReferences.WebReferenceCommands.Add)]
		public void NewWebReference()
		{
			// Get the project and project folder
			Project project = CurrentNode.GetParentDataItem (typeof(Project), true) as Project;
			DotNetProject dotProject = (DotNetProject) project;
			
			// Check and switch the runtime environment for the current project
			if (dotProject.ClrVersion == ClrVersion.Net_1_1)
			{
				string question = "The current runtime environment for your project is set to version 1.0.";
				question += "Web Service is not supported in this version.";
				question += "Do you want switch the runtime environment for this project version 2.0 ?";
				
				if (IdeApp.Services.MessageService.AskQuestion(question))
					dotProject.ClrVersion = ClrVersion.Net_2_0;					
				else
					return;
			}
			
			WebReferenceDialog dialog = new WebReferenceDialog(Library.GetWebReferencePath(project));
			dialog.NamespacePrefix = project.Name;
			
			int response = dialog.Run();
			dialog.Destroy();
			if (response == (int)Gtk.ResponseType.Ok)
			{
				try
				{
					CodeGenerator gen = new CodeGenerator(project, dialog.SelectedService);
					
					// Create the base directory if it does not exists
					string basePath = dialog.ReferencePath;
					if (!Directory.Exists(basePath))
						Directory.CreateDirectory(basePath);
					
					// Generate the wsdl, disco and map files
					string mapSpec = gen.CreateMapFile(basePath, "Reference.map");
					ProjectFile mapFile = new ProjectFile(mapSpec);
					mapFile.BuildAction = BuildAction.Nothing;
					mapFile.Subtype = Subtype.Code;
					project.ProjectFiles.Add(mapFile);
			
					// Generate the proxy class
					string proxySpec = gen.CreateProxyFile(basePath, dialog.Namespace + "." + dialog.ReferenceName, "Reference");
					ProjectFile proxyFile = new ProjectFile(proxySpec);
					proxyFile.BuildAction = BuildAction.Compile;
					proxyFile.Subtype = Subtype.Code;
					project.ProjectFiles.Add(proxyFile);
					
					// Add a reference System.Web.Services to the project if it does not exists
					string refName = "System.Web.Services, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
					ProjectReference gacRef = new ProjectReference(ReferenceType.Gac, refName);
					if (!project.ProjectReferences.Contains(gacRef))
						project.ProjectReferences.Add(gacRef);

					IdeApp.ProjectOperations.SaveProject(project);
				}
				catch(Exception exception)
				{
					IdeApp.Services.MessageService.ShowError(exception);
				}
			}
		}
		
		/// <summary>Execute the command for updating a web reference in a project.</summary>
		[CommandHandler (MonoDevelop.WebReferences.WebReferenceCommands.Update)]
		public void Update()
		{
			WebReferenceItem item = (WebReferenceItem) CurrentNode.DataItem;
			item.Update();
			IdeApp.Workbench.StatusBar.SetMessage("Updated Web Reference " + item.Name);
		}
		
		/// <summary>Execute the command for updating all web reference in a project.</summary>
		[CommandHandler (MonoDevelop.WebReferences.WebReferenceCommands.UpdateAll)]
		public void UpdateAll()
		{
			Project project = ((WebReferenceFolder) CurrentNode.DataItem).Project;
			WebReferenceItemCollection items = new WebReferenceItemCollection (project);
			for (int index = 0; index < items.AllKeys.Length; index ++)
			{
				items[items.AllKeys[index]].Update();
				IdeApp.Workbench.StatusBar.SetMessage("Updated Web Reference " + items.AllKeys[index]);
			}
			IdeApp.Workbench.StatusBar.SetMessage("Updated all Web References");
		}
		
		/// <summary>Execute the command for removing a web reference from a project.</summary>
		[CommandHandler (MonoDevelop.WebReferences.WebReferenceCommands.Delete)]
		public void Delete()
		{
			WebReferenceItem item = (WebReferenceItem) CurrentNode.DataItem;
			Project project = item.ProxyFile.Project;
			item.Delete();
			IdeApp.ProjectOperations.SaveProject(project);
			IdeApp.Workbench.StatusBar.SetMessage("Deleted Web Reference " + item.Name);
		}
		
		/// <summary>Execute the command for removing all web references from a project.</summary>
		[CommandHandler (MonoDevelop.WebReferences.WebReferenceCommands.DeleteAll)]
		public void DeleteAll()
		{
			Project project = ((WebReferenceFolder) CurrentNode.DataItem).Project;
			WebReferenceItemCollection items = new WebReferenceItemCollection (project);
			for (int index = 0; index < items.AllKeys.Length; index ++)
			{
				items[items.AllKeys[index]].Delete();
				IdeApp.Workbench.StatusBar.SetMessage("Deleted Web Reference " + items.AllKeys[index]);
			}
			IdeApp.ProjectOperations.SaveProject(project);
			IdeApp.Workbench.StatusBar.SetMessage("Deleted all Web References");
		}
	}	
}
