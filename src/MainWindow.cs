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
using Banshee.Widgets;
using Commons.Collections;
using FSpot.Utils;
using Gtk;
using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WebKit;

namespace Papeles
{
	enum LibraryPaperColumn {
		//Flag,
		Authors,
		Title,
		Journal,
		Year,
		//Rating,
		Object
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
			library_store = new ListStore (typeof (string), typeof (string), typeof (string), typeof (string), typeof (Paper));

			foreach (Paper paper in Library.Papers)
				library_store.AppendValues (null, null, null, null, paper);

			statusbar.Push (1, String.Format ("{0} papers", Library.Count));
		}

		/// <summary>
		/// Populate headers in treeview for library.
		/// </summary>
		void CreateLibraryView ()
		{
			CellRendererText authorsRenderer =  new CellRendererText ();
			CellRendererText titleRenderer   =  new CellRendererText ();
			CellRendererText journalRenderer =  new CellRendererText ();
			CellRendererText yearRenderer    =  new CellRendererText ();

			authorsRenderer.Editable = true;
			titleRenderer.Editable   = true;
			journalRenderer.Editable = true;
			yearRenderer.Editable    = true;

			authorsRenderer.Edited += OnLibraryPaperAuthorCellEdited;
			titleRenderer.Edited   += OnLibraryPaperTitleCellEdited;
			journalRenderer.Edited += OnLibraryPaperJournalCellEdited;
			yearRenderer.Edited    += OnLibraryPaperYearCellEdited;

			// TreeViewColumn flagColumn    = new TreeViewColumn ("Flag",    new CellRendererText (),
			// 												   "text",    Column.Flag);
			TreeViewColumn authorsColumn = new TreeViewColumn ("Authors", authorsRenderer, "text", (int) LibraryPaperColumn.Authors);
			TreeViewColumn titleColumn   = new TreeViewColumn ("Title",   titleRenderer,   "text", (int) LibraryPaperColumn.Title);
			TreeViewColumn journalColumn = new TreeViewColumn ("Journal", journalRenderer, "text", (int) LibraryPaperColumn.Journal);
			TreeViewColumn yearColumn    = new TreeViewColumn ("Year",    yearRenderer,    "text", (int) LibraryPaperColumn.Year);
			// TreeViewColumn ratingColumn  = new TreeViewColumn ("Rating",  new CellRendererText (),
			// 												   "text",    Column.Rating);

			authorsColumn.SetCellDataFunc (authorsRenderer, new TreeCellDataFunc (ShowLibraryPaperCellAuthors));
			titleColumn.SetCellDataFunc (titleRenderer,     new TreeCellDataFunc (ShowLibraryPaperCellTitle));
			journalColumn.SetCellDataFunc (journalRenderer, new TreeCellDataFunc (ShowLibraryPaperCellJournal));
			yearColumn.SetCellDataFunc (yearRenderer,       new TreeCellDataFunc (ShowLibraryPaperCellYear));

			library_store.SetSortFunc ((int) LibraryPaperColumn.Authors, SortLibraryPaperCellAuthors);
			library_store.SetSortFunc ((int) LibraryPaperColumn.Title,   SortLibraryPaperCellTitle);
			library_store.SetSortFunc ((int) LibraryPaperColumn.Journal, SortLibraryPaperCellJournal);
			library_store.SetSortFunc ((int) LibraryPaperColumn.Year,    SortLibraryPaperCellYear);

			authorsColumn.SortColumnId = (int) LibraryPaperColumn.Authors;
			titleColumn.SortColumnId   = (int) LibraryPaperColumn.Title;
			journalColumn.SortColumnId = (int) LibraryPaperColumn.Journal;
			yearColumn.SortColumnId    = (int) LibraryPaperColumn.Year;
			// ratingColumn.SortColumnId  = (int) Column.Rating;

			authorsColumn.Expand = false;
			titleColumn.Expand   = false;
			journalColumn.Expand = false;
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

		#region Library TreeView Functions

		void ShowLibraryPaperCellAuthors (TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			Paper paper = library_store.GetValue (iter, (int) LibraryPaperColumn.Object) as Paper;

			if (paper == null) {
				Log.Warning ("Attempted to render authors cell for missing paper");
				return;
			}
			(cell as CellRendererText).Markup = paper.Authors != null ? paper.Authors : "";
		}

		void ShowLibraryPaperCellTitle (TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			Paper paper = library_store.GetValue (iter, (int) LibraryPaperColumn.Object) as Paper;

			if (paper == null) {
				Log.Warning ("Attempted to render title cell for missing paper");
				return;
			}
			(cell as CellRendererText).Markup = paper.Title != null ? paper.Title : "";
		}

		void ShowLibraryPaperCellJournal (TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			Paper paper = library_store.GetValue (iter, (int) LibraryPaperColumn.Object) as Paper;

			if (paper == null) {
				Log.Warning ("Attempted to render journal cell for missing paper");
				return;
			}
			(cell as CellRendererText).Markup = paper.Journal != null ? paper.Journal : "";
		}

		void ShowLibraryPaperCellYear (TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			Paper paper = library_store.GetValue (iter, (int) LibraryPaperColumn.Object) as Paper;

			if (paper == null) {
				Log.Warning ("Attempted to render year cell for missing paper");
				return;
			}
			(cell as CellRendererText).Markup = paper.Year != null ? paper.Year : "";
		}

		int SortLibraryPaperCellAuthors (TreeModel model, TreeIter iterA, TreeIter iterB)
		{
			Paper paperA = library_store.GetValue (iterA, (int) LibraryPaperColumn.Object) as Paper;
			Paper paperB = library_store.GetValue (iterB, (int) LibraryPaperColumn.Object) as Paper;

			return String.Compare (paperA.Authors, paperB.Authors);
		}

		int SortLibraryPaperCellTitle (TreeModel model, TreeIter iterA, TreeIter iterB)
		{
			Paper paperA = library_store.GetValue (iterA, (int) LibraryPaperColumn.Object) as Paper;
			Paper paperB = library_store.GetValue (iterB, (int) LibraryPaperColumn.Object) as Paper;

			return String.Compare (paperA.Title, paperB.Title);
		}

		int SortLibraryPaperCellJournal (TreeModel model, TreeIter iterA, TreeIter iterB)
		{
			Paper paperA = library_store.GetValue (iterA, (int) LibraryPaperColumn.Object) as Paper;
			Paper paperB = library_store.GetValue (iterB, (int) LibraryPaperColumn.Object) as Paper;

			return String.Compare (paperA.Journal, paperB.Journal);
		}

		int SortLibraryPaperCellYear (TreeModel model, TreeIter iterA, TreeIter iterB)
		{
			Paper paperA = library_store.GetValue (iterA, (int) LibraryPaperColumn.Object) as Paper;
			Paper paperB = library_store.GetValue (iterB, (int) LibraryPaperColumn.Object) as Paper;

			return String.Compare (paperA.Year, paperB.Year);
		}

		#endregion

		void ShowPaperInformation (Paper paper)
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

		/// <summary>
		/// Return the first selected paper.
		/// </summary>
		Paper GetSelectedPaper ()
		{
			TreeSelection selection = document_treeview.Selection;

			if (selection.CountSelectedRows () > 0) {
				TreePath path = selection.GetSelectedRows ()[0];
				TreeIter iter;

				library_store.GetIter (out iter, path);
				return library_store.GetValue (iter, (int) LibraryPaperColumn.Object) as Paper;
			}
			return null;
		}

		/// <summary>
		/// Return list of currently selected papers.
		/// </summary>
		List<Paper> GetSelectedPapers ()
		{
			List<Paper> selected = new List<Paper> ();
			TreeSelection selection = document_treeview.Selection;

			if (selection.CountSelectedRows () > 0) {
				TreePath[] paths = selection.GetSelectedRows ();

				foreach (TreePath path in paths) {
					TreeIter iter;

					library_store.GetIter (out iter, path);
					selected.Add (library_store.GetValue (iter, (int) LibraryPaperColumn.Object) as Paper);
				}
			}
			return selected;
		}

		delegate void ProcessPaper (Paper paper);

		void RemoveSelectedItems (ProcessPaper callback)
		{
			TreeSelection selection = document_treeview.Selection;

			if (selection.CountSelectedRows () > 0) {
				TreePath[] paths = selection.GetSelectedRows ();

				foreach (TreePath path in paths) {
					TreeIter iter;

					library_store.GetIter (out iter, path);
					if (callback != null)
						callback (library_store.GetValue (iter, (int) LibraryPaperColumn.Object) as Paper);
					library_store.Remove (ref iter);
				}
			}
			statusbar.Push (1, String.Format ("{0} papers", Library.Count));
		}

		void SetLibraryPaperCell (LibraryPaperColumn column, string rawPath, string value)
		{
			TreePath path = new TreePath (rawPath);
			TreeIter iter;

			library_store.GetIter (out iter, path);
			// library_store.SetValue (iter, (int) column, value);

			Paper paper = library_store.GetValue (iter, (int) LibraryPaperColumn.Object) as Paper;

			switch (column) {
			case LibraryPaperColumn.Authors:
				paper.Authors = value;
				break;
			case LibraryPaperColumn.Title:
				paper.Title = value;
				break;
			case LibraryPaperColumn.Journal:
				paper.Journal = value;
				break;
			case LibraryPaperColumn.Year:
				paper.Year = value;
				break;
			default:
				break;
			}
			paper.Save ();
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

			Library.PaperAdded += AddPaperToLibraryStore;
      
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

		#region Glade-connected Event Handlers

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
			document_treeview.Selection.SelectAll ();
		}

		public void OnEditSelectNone (object obj, EventArgs args)
		{
			document_treeview.Selection.UnselectAll ();
		}

		public void OnEditDocumentInformation (object obj, EventArgs args)
		{
			Paper paper = GetSelectedPaper ();

			if (paper != null)
				new EditPaperInformationDialog (paper);
			else
				Log.Warning ("Tried to edit paper information but no paper was selected");
		}

		public void OnEditRemoveFromLibrary (object obj, EventArgs args)
		{
			// FIXME: abstract this
			// FIXME: test if more than one is selected, and updated text accordingly
			string header = "Remove this item?";
			string message = "Are you sure you want to remove the selected item from your library?";
			HigMessageDialog dialog = new HigMessageDialog (main_window, DialogFlags.DestroyWithParent, MessageType.Warning,
															ButtonsType.None, header, message);

			dialog.AddButton ("gtk-cancel", ResponseType.No, true);
			dialog.AddButton ("gtk-remove", ResponseType.Yes, false);

			try {
				if (ResponseType.Yes == (ResponseType) dialog.Run ())
					RemoveSelectedItems (paper => Library.Remove (paper));
			} finally {
				dialog.Destroy ();
			}
		}

		public void OnEditDeleteFromDrive (object obj, EventArgs args)
		{
			// FIXME: abstract this
			// FIXME: test if more than one is selected, and updated text accordingly
			string header = "Delete this item?";
			string message = "Are you sure you want to permanently delete the selected item? If deleted, it will be permanently lost.";
			HigMessageDialog dialog = new HigMessageDialog (main_window, DialogFlags.DestroyWithParent, MessageType.Warning,
															ButtonsType.None, header, message);

			dialog.AddButton ("gtk-cancel", ResponseType.No, true);
			dialog.AddButton ("gtk-delete", ResponseType.Yes, false);

			try {
				if (ResponseType.Yes == (ResponseType) dialog.Run ())
					RemoveSelectedItems (paper => Library.Delete (paper));
			} finally {
				dialog.Destroy ();
			}
		}

		public void OnEditPreferences (object obj, EventArgs args)
		{
		}

		public void OnHelpAbout (object obj, EventArgs args)
		{
			AboutDialog dialog = new AboutDialog ();

			dialog.ProgramName = "Papeles";
			dialog.Version = "0.1";
			dialog.Copyright = "Copyright \u00a9 2009 Jacinto Shy, Jr.\nCopyright \u00a9 2005-2008 Novell, Inc.";
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

		#endregion

		#region Internal Event Handlers

		void OnLibrarySelectedPaperChanged (object obj, EventArgs args)
		{
			Paper paper = GetSelectedPaper ();

			if (paper != null) {
				ShowPaperInformation (paper);
				DisplayDocument (paper.FilePath);
			}
		}

		void OnPropertiesIconActivated (object obj, ItemActivatedArgs args)
		{
			TreeIter iter;
			string path;

			paper_properties_icon_store.GetIter (out iter, args.Path);
			path = paper_properties_icon_store.GetValue (iter, (int) PaperPropertiesColumn.FilePath) as string;

			// FIXME: xdg-open on Linux only
			System.Diagnostics.Process.Start ("xdg-open", String.Format ("\"{0}\"", path));
		}

		void OnLibraryPaperAuthorCellEdited (object obj, EditedArgs args)
		{
			SetLibraryPaperCell (LibraryPaperColumn.Authors, args.Path, args.NewText);
		}

		void OnLibraryPaperTitleCellEdited (object obj, EditedArgs args)
		{
			SetLibraryPaperCell (LibraryPaperColumn.Title, args.Path, args.NewText);
		}

		void OnLibraryPaperJournalCellEdited (object obj, EditedArgs args)
		{
			SetLibraryPaperCell (LibraryPaperColumn.Journal, args.Path, args.NewText);
		}

		void OnLibraryPaperYearCellEdited (object obj, EditedArgs args)
		{
			SetLibraryPaperCell (LibraryPaperColumn.Year, args.Path, args.NewText);
		}

		void AddPaperToLibraryStore (Paper paper)
		{
			library_store.AppendValues (null, null, null, null, paper);
			statusbar.Push (1, String.Format ("{0} papers", Library.Count));
		}

		#endregion
	}
}
