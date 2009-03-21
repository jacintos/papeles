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
using Gtk;
using System;
using System.IO;

namespace Papeles
{
    enum Column {
        Flag = 0,
        Authors,
        Title,
        Journal,
        Year,
        Rating
    }

    class MainWindow
    {
        [Glade.Widget] Window main_window;
        [Glade.Widget] Viewport document_viewport;
        [Glade.Widget] Toolbar main_toolbar;
        [Glade.Widget] TreeView document_treeview;
        [Glade.Widget] HScale toolbar_scale_page;
        [Glade.Widget] Statusbar statusbar;

		Menu documentTreeViewContextMenu;
		RenderContext renderContext;
		string configDir;
		string dataDir;
		string documentsDir;

        /// <summary>
        /// Populate headers in treeview for library.
        /// </summary>
        void CreateLibraryView (ListStore store)
        {
            TreeViewColumn flagColumn    = new TreeViewColumn ("Flag",    new CellRendererText (),
															   "text",    Column.Flag);
            TreeViewColumn authorsColumn = new TreeViewColumn ("Authors", new CellRendererText (),
															   "text",    Column.Authors);
            TreeViewColumn titleColumn   = new TreeViewColumn ("Title",   new CellRendererText (),
															   "text",    Column.Title);
            TreeViewColumn journalColumn = new TreeViewColumn ("Journal", new CellRendererText (),
															   "text",    Column.Journal);
            TreeViewColumn yearColumn    = new TreeViewColumn ("Year",    new CellRendererText (),
															   "text",    Column.Year);
            TreeViewColumn ratingColumn  = new TreeViewColumn ("Rating",  new CellRendererText (),
															   "text",    Column.Rating);

            authorsColumn.SortColumnId = (int) Column.Authors;
            titleColumn.SortColumnId   = (int) Column.Title;
            journalColumn.SortColumnId = (int) Column.Journal;
            yearColumn.SortColumnId    = (int) Column.Year;
            ratingColumn.SortColumnId  = (int) Column.Rating;

            authorsColumn.Expand = true;
            titleColumn.Expand   = true;
            journalColumn.Expand = true;
            yearColumn.Expand    = true;
            ratingColumn.Expand  = true;

            authorsColumn.Resizable = true;
            titleColumn.Resizable   = true;
            journalColumn.Resizable = true;
            yearColumn.Resizable    = true;

            document_treeview.AppendColumn (flagColumn);
            document_treeview.AppendColumn (authorsColumn);
            document_treeview.AppendColumn (titleColumn);
            document_treeview.AppendColumn (journalColumn);
            document_treeview.AppendColumn (yearColumn);
            document_treeview.AppendColumn (ratingColumn);

            document_treeview.Selection.Mode = SelectionMode.Multiple;
            document_treeview.Model = store;
        }

        void DisplayDocument (string filePath)
        {
            IDocument doc = new PdfDocument ("file://" + filePath, "");
            DocumentInfo info = doc.Info;

			renderContext = new RenderContext(0, 1.0);

            if (info.Title == null || info.Title == "")
                main_window.Title = System.IO.Path.GetFileName (filePath);
            else
                main_window.Title = info.Title;

            Box box = new VBox (true, 0);

            Gdk.Color white = new Gdk.Color (0xFF, 0xFF, 0xFF);
            for (int i = 0; i < doc.NPages; i++) {
                RenderedDocument page = new RenderedDocument (i, renderContext, doc);

                page.ModifyBg (StateType.Normal, white);
                box.Add (page);
            }

            document_viewport.Add (box);
        }

		void QuitApplication ()
        {
            Application.Quit ();
        }

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

			documentTreeViewContextMenu = new Menu ();
			documentTreeViewContextMenu.Add (edit);
			documentTreeViewContextMenu.Add (new SeparatorMenuItem ());
			documentTreeViewContextMenu.Add (remove);
			documentTreeViewContextMenu.Add (delete);
		}

		void ShowDocumentTreeViewContextMenu ()
		{
			documentTreeViewContextMenu.Popup ();
			documentTreeViewContextMenu.ShowAll ();
		}

        public MainWindow ()
        {
            Glade.XML gxml = new Glade.XML (null, "papeles.glade", "main_window", null);

            gxml.Autoconnect (this);

			configDir    = XdgBaseDirectorySpec.GetUserDirectory ("XDG_CONFIG_HOME", ".config");
			dataDir      = XdgBaseDirectorySpec.GetUserDirectory ("XDG_DATA_HOME", ".local/share");
			documentsDir = XdgBaseDirectorySpec.GetUserDirectory ("XDG_DOCUMENTS_DIR", "Documents");

			configDir = Path.Combine (configDir, "papeles");
			dataDir   = Path.Combine (dataDir, "papeles");

			if (!Directory.Exists (configDir))
				Directory.CreateDirectory (configDir);
			if (!Directory.Exists (dataDir))
				Directory.CreateDirectory (dataDir);

            Database db = new Database (Path.Combine (dataDir, "papeles.db3"));
      
            ListStore docStore = new ListStore(typeof(string), typeof(string), typeof(string),
                                               typeof(string), typeof(string), typeof(string));
            // docStore.AppendValues("Jacinto Shy", "Tetrahydrobiopterin",
            //                       "J Phys Chem B", "2006");
            // docStore.AppendValues("Jacinto Shy", "Nascent HDL",
            //                       "Nat Struct Mol Biol", "2007");
            docStore.AppendValues ("false", "Jacinto Shy", "Tetrahydrobiopterin",
								   "J Phys Chem B", "2006", "3");
            docStore.AppendValues ("false", "Jacinto Shy", "Nascent HDL",
								   "Nat Struct Mol Biol", "2007", "0");

			CreateDocumentTreeViewContextMenu ();
            CreateLibraryView (docStore);

            DisplayDocument ("/home/jacinto/Documents/papers/inference-secco08.pdf");

            uint totalPapers = 2;
            statusbar.Push (1, String.Format ("{0} papers", totalPapers));

            main_toolbar.IconSize = IconSize.SmallToolbar;

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

            filter.Name = "PDF and PostScript documents";
            filter.AddMimeType ("application/pdf");
            filter.AddPattern ("*.pdf");
            filter.AddMimeType ("application/postscript");
            filter.AddPattern ("*.ps");
            dialog.AddFilter (filter);

            if (dialog.Run () == (int)ResponseType.Accept) {
                Console.WriteLine ("Import paper");
            }
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
			Console.WriteLine("Edit document information");
        }

        public void OnEditRemoveFromLibrary (object obj, EventArgs args)
        {
			Console.WriteLine("Remove from library");
        }

        public void OnEditDeleteFromDrive (object obj, EventArgs args)
        {
			Console.WriteLine("Delete from drive");
        }

        public void OnEditProperties (object obj, EventArgs args)
        {
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
			renderContext.Scale = toolbar_scale_page.Value;
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
    }
}
