/* -*- coding: utf-8 -*- */
/* MainWindow.cs
 * Copyright (c) 2009 Jacinto Shy, Jr.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

using Banshee.Base;
using Commons.Collections;
using Gtk;
using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using System;
using System.IO;
using System.Reflection;
using WebKit;

namespace Papeles
{
	enum Column {
		//Flag,
		Authors,
		Title,
		Journal,
		Year,
		//Rating,
		ID
	}

	enum PaperPropertiesColumn {
		Pixbuf,
		FileName,
		FilePath
	}

	class MainWindow
	{
		[Glade.Widget] Window main_window;
		[Glade.Widget] Viewport document_viewport;
		[Glade.Widget] Toolbar main_toolbar;
		[Glade.Widget] Toolbar document_toolbar;
		[Glade.Widget] TreeView document_treeview;
		[Glade.Widget] HScale toolbar_scale_page;
		[Glade.Widget] ScrolledWindow paper_properties_window;
		[Glade.Widget] Alignment paper_properties_frame_inner;
		[Glade.Widget] Statusbar statusbar;

		Menu library_context_menu;
		ListStore library_store;
		ListStore paper_properties_icon_store;
		WebView paper_properties_web_view;
		IconView paper_properties_icon_view;
		RenderContext render_context;
		VelocityEngine template_engine;
		string config_dir;
		string data_dir;
		string documents_dir;

		void CreateDocumentTreeViewContextMenu ()
		{
			ImageMenuItem edit = new ImageMenuItem ("_Edit Document Information");
			ImageMenuItem remove = new ImageMenuItem ("_Remove from Library");
			ImageMenuItem delete = new ImageMenuItem ("_Delete from Drive");

			edit.Image   = new Image (Stock.Edit, IconSize.Menu);
			remove.Image = new Image (Stock.Remove, IconSize.Menu);
			delete.Image = new Image (Stock.Delete, IconSize.Menu);

			edit.Activated   += OnEditDocumentInformation;
			remove.Activated += OnEditRemoveFromLibrary;
			delete.Activated += OnEditDeleteFromDrive;

			library_context_menu = new Menu ();
			library_context_menu.Add (edit);
			library_context_menu.Add (new SeparatorMenuItem ());
			library_context_menu.Add (remove);
			library_context_menu.Add (delete);
		}

		void CreateLibraryStore ()
		{
			library_store = new ListStore (typeof(string), typeof(string), typeof(string), typeof(string),
						       typeof(string));

			foreach (Paper paper in Library.Papers)
				AddPaperToLibraryStore (paper);
		}

		/// <summary>
		/// Populate headers in treeview for library.
		/// </summary>
		void CreateLibraryView ()
		{
			// TreeViewColumn flagColumn    = new TreeViewColumn ("Flag",    new CellRendererText (),
			// 												   "text",    Column.Flag);
			TreeViewColumn authorsColumn = new TreeViewColumn ("Authors", new CellRendererText (),
									   "text",    Column.Authors);
			TreeViewColumn titleColumn   = new TreeViewColumn ("Title",   new CellRendererText (),
									   "text",    Column.Title);
			TreeViewColumn journalColumn = new TreeViewColumn ("Journal", new CellRendererText (),
									   "text",    Column.Journal);
			TreeViewColumn yearColumn    = new TreeViewColumn ("Year",    new CellRendererText (),
									   "text",    Column.Year);
			TreeViewColumn idColumn      = new TreeViewColumn ("ID",      new CellRendererText (),
									   "text",    Column.ID);
			// TreeViewColumn ratingColumn  = new TreeViewColumn ("Rating",  new CellRendererText (),
			// 												   "text",    Column.Rating);

			idColumn.Visible = false;

			authorsColumn.SortColumnId = (int) Column.Authors;
			titleColumn.SortColumnId   = (int) Column.Title;
			journalColumn.SortColumnId = (int) Column.Journal;
			yearColumn.SortColumnId    = (int) Column.Year;
			// ratingColumn.SortColumnId  = (int) Column.Rating;

			authorsColumn.Expand = true;
			titleColumn.Expand   = true;
			journalColumn.Expand = true;
			yearColumn.Expand    = true;
			// ratingColumn.Expand  = true;

			authorsColumn.Resizable = true;
			titleColumn.Resizable   = true;
			journalColumn.Resizable = true;
			yearColumn.Resizable    = true;

			// document_treeview.AppendColumn (flagColumn);
			document_treeview.AppendColumn (authorsColumn);
			document_treeview.AppendColumn (titleColumn);
			document_treeview.AppendColumn (journalColumn);
			document_treeview.AppendColumn (yearColumn);
			// document_treeview.AppendColumn (ratingColumn);

			document_treeview.Selection.Mode = SelectionMode.Multiple;
			document_treeview.Selection.Changed += OnLibrarySelectedPaperChanged;
			document_treeview.Model = library_store;
		}

		void CreateTemplateEngine ()
		{
			ExtendedProperties props = new ExtendedProperties ();

			props.AddProperty ("resource.loader", "assembly");
			props.AddProperty ("assembly.resource.loader.class", "NVelocity.Runtime.Resource.Loader.AssemblyResourceLoader, NVelocity");
			props.AddProperty ("assembly.resource.loader.assembly", Assembly.GetExecutingAssembly ().GetName ().Name);

			template_engine = new VelocityEngine ();
			template_engine.Init (props);
		}

		void CreatePaperPropertiesView ()
		{
			paper_properties_icon_store = new ListStore (typeof (Gdk.Pixbuf), typeof (string), typeof (string));
			paper_properties_icon_view = new IconView (paper_properties_icon_store);

			paper_properties_icon_view.PixbufColumn = (int) PaperPropertiesColumn.Pixbuf;
			paper_properties_icon_view.TextColumn   = (int) PaperPropertiesColumn.FileName;
			paper_properties_icon_view.MarkupColumn = (int) PaperPropertiesColumn.FileName;
			paper_properties_icon_view.ItemWidth = 185;
			paper_properties_icon_view.Orientation = Orientation.Horizontal;
			paper_properties_icon_view.ItemActivated += OnPropertiesIconActivated;
			paper_properties_frame_inner.Add (paper_properties_icon_view);

			paper_properties_web_view = new WebView ();
			paper_properties_window.Add (paper_properties_web_view);
		}

		void DisplayDocument (string filePath)
		{
			IDocument doc = new PdfDocument ("file://" + filePath, "");
			Box box = new VBox (true, 0);
			Gdk.Color white = new Gdk.Color (0xFF, 0xFF, 0xFF);

			render_context = new RenderContext (0, 1.0);
			for (int i = 0; i < doc.NPages; i++) {
				RenderedDocument page = new RenderedDocument (i, render_context, doc);

				page.ModifyBg (StateType.Normal, white); // FIXME: probably don't want this
				box.Add (page);
				page.Show ();
			}
			document_viewport.Foreach (document_viewport.Remove);
			document_viewport.Add (box);
			box.Show ();
		}

		void QuitApplication ()
		{
			Application.Quit ();
		}

		void ShowDocumentTreeViewContextMenu ()
		{
			library_context_menu.Popup ();
			library_context_menu.ShowAll ();
		}

		public void ShowPaperInformation (Paper paper)
		{
			Template template = template_engine.GetTemplate ("paperinfo.vm");
			VelocityContext context = new VelocityContext ();
			StringWriter writer = new StringWriter ();

			context.Put ("paper", paper);
			template.Merge (context, writer);
			paper_properties_web_view.LoadHtmlString (writer.GetStringBuilder ().ToString (), "");

			// FIXME: get icon based on mime-type
			Gdk.Pixbuf icon = IconTheme.Default.LoadIcon ("gnome-fs-regular", 32, (IconLookupFlags) 0);
			string fileName = Path.GetFileName (paper.FilePath);
			string fileSize = "2.8 MB";
			string label = String.Format ("<b>{0}</b>\n{1}", fileName.Truncate (18, "..."), fileSize);

			paper_properties_icon_store.Clear ();
			paper_properties_icon_store.AppendValues (icon, label, paper.FilePath);
			paper_properties_icon_view.TooltipText = fileName;
		}

		public MainWindow ()
		{
			Glade.XML gxml = new Glade.XML (null, "papeles.glade", "main_window", null);

			gxml.Autoconnect (this);

			config_dir    = XdgBaseDirectorySpec.GetUserDirectory ("XDG_CONFIG_HOME", ".config");
			data_dir      = XdgBaseDirectorySpec.GetUserDirectory ("XDG_DATA_HOME", ".local/share");
			documents_dir = XdgBaseDirectorySpec.GetUserDirectory ("XDG_DOCUMENTS_DIR", "Documents");

			config_dir = Path.Combine (config_dir, "papeles");
			data_dir   = Path.Combine (data_dir, "papeles");

			if (!Directory.Exists (config_dir))
				Directory.CreateDirectory (config_dir);
			if (!Directory.Exists (data_dir))
				Directory.CreateDirectory (data_dir);

			Database.Load (Path.Combine (data_dir, "papeles.db3"));
			Library.Load ();

			Library.PaperAdded   += AddPaperToLibraryStore;
			Library.PaperRemoved += RemovePaperFromLibraryStore;
      
			CreateDocumentTreeViewContextMenu ();
			CreateLibraryStore ();
			CreateLibraryView ();
			CreateTemplateEngine ();
			CreatePaperPropertiesView ();

			statusbar.Push (1, String.Format ("{0} papers", Library.Count));

			main_toolbar.IconSize     = IconSize.SmallToolbar;
			document_toolbar.IconSize = IconSize.SmallToolbar;

			main_window.ShowAll ();
		}

		// Event handlers

		public void OnDelete (object obj, DeleteEventArgs args)
		{
			QuitApplication ();
		}

		public void OnFileImportDocument (object obj, EventArgs args)
		{
			FileChooserDialog dialog = new FileChooserDialog ("Import", null, FileChooserAction.Open,
									  "Cancel", ResponseType.Cancel,
									  "Import", ResponseType.Accept);
			FileFilter filter = new FileFilter ();

			filter.Name = "Documents";
			filter.AddMimeType ("application/pdf");
			filter.AddPattern ("*.pdf");
			// filter.AddMimeType ("application/postscript");
			// filter.AddPattern ("*.ps");
			dialog.AddFilter (filter);

			if (dialog.Run () == (int) ResponseType.Accept)
				Library.Add (dialog.Filename);
			dialog.Destroy ();
		}

		public void OnFilePrint (object obj, EventArgs args)
		{
		}

		public void OnFileQuit (object obj, EventArgs args)
		{
			QuitApplication ();
		}

		public void OnEditSelectAll (object obj, EventArgs args)
		{
		}

		public void OnEditDocumentInformation (object obj, EventArgs args)
		{
			TreeSelection selection = document_treeview.Selection;

			if (selection.CountSelectedRows () == 1) {
				TreePath path = selection.GetSelectedRows ()[0];
				TreeIter iter;
				int id;

				library_store.GetIter (out iter, path);
				id = Convert.ToInt32(library_store.GetValue (iter, (int) Column.ID) as string);

				new EditPaperInformationDialog (Library.GetPaper (id));
			}
		}

		public void OnEditRemoveFromLibrary (object obj, EventArgs args)
		{
			Console.WriteLine("Remove from library");
		}

		public void OnEditDeleteFromDrive (object obj, EventArgs args)
		{
			Console.WriteLine("Delete from drive");
		}

		public void OnEditPreferences (object obj, EventArgs args)
		{
		}

		public void OnHelpAbout (object obj, EventArgs args)
		{
			AboutDialog dialog = new AboutDialog ();

			dialog.ProgramName = "Papeles";
			dialog.Version = "0.1";
			dialog.Copyright = "Copyright \u00a9 2009 Jacinto Shy, Jr.";
			dialog.Run (); // TODO: don't block
			dialog.Destroy ();
		}

		public void OnPreviousPage (object obj, EventArgs args)
		{
		}

		public void OnNextPage (object obj, EventArgs args)
		{
		}

		public void OnZoomOut (object obj, EventArgs args)
		{
			Adjustment adj = toolbar_scale_page.Adjustment;

			if (toolbar_scale_page.Value - adj.PageIncrement >= adj.Lower)
				toolbar_scale_page.Value -= adj.PageIncrement;
		}

		public void OnZoomIn (object obj, EventArgs args)
		{
			Adjustment adj = toolbar_scale_page.Adjustment;

			if (toolbar_scale_page.Value + adj.PageIncrement <= adj.Upper)
				toolbar_scale_page.Value += adj.PageIncrement;
		}

		public void OnScalePageValueChanged (object obj, EventArgs args)
		{
			render_context.Scale = toolbar_scale_page.Value;
			document_viewport.QueueDraw ();
		}

		public void FormatZoomScaleValue (object obj, FormatValueArgs args)
		{
			args.RetVal = String.Format ("{0}%", Math.Ceiling (args.Value * 100));
		}

		[GLib.ConnectBefore]
		public void OnDocumentTreeViewButtonPress (object obj, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3)
				ShowDocumentTreeViewContextMenu ();
		}

		[GLib.ConnectBefore]
		public void OnDocumentTreeViewPopupMenu (object obj, PopupMenuArgs args)
		{
			ShowDocumentTreeViewContextMenu ();
		}

		// Non-GTK widget callbacks

		public void OnLibrarySelectedPaperChanged (object obj, EventArgs args)
		{
			TreeSelection selection = obj as TreeSelection;

			if (selection.CountSelectedRows () == 1) {
				TreePath path = selection.GetSelectedRows ()[0];
				TreeIter iter;
				Paper paper;
				int id;

				library_store.GetIter (out iter, path);
				id = Convert.ToInt32(library_store.GetValue (iter, (int) Column.ID) as string);
				paper = Library.GetPaper (id);
				ShowPaperInformation (paper);
				DisplayDocument (paper.FilePath);
			}
		}

		public void OnPropertiesIconActivated (object obj, ItemActivatedArgs args)
		{
			TreeIter iter;
			string path;

			paper_properties_icon_store.GetIter (out iter, args.Path);
			path = paper_properties_icon_store.GetValue (iter, (int) PaperPropertiesColumn.FilePath) as string;

			// FIXME: xdg-open on Linux only
			System.Diagnostics.Process.Start ("xdg-open", String.Format ("\"{0}\"", path));
		}

		public void AddPaperToLibraryStore (Paper paper)
		{
			library_store.AppendValues (paper.Authors, paper.Title, paper.Journal, paper.Year, Convert.ToString(paper.ID));
			statusbar.Push (1, String.Format ("{0} papers", Library.Count));
		}

		public void RemovePaperFromLibraryStore (Paper paper)
		{
			statusbar.Push (1, String.Format ("{0} papers", Library.Count));
		}
	}
}
