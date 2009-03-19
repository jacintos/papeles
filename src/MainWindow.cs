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

using Gtk;
using Glade;
using System;

namespace Papeles
{
    enum Column {
        Authors = 0,
        Title = 1,
        Journal = 2,
        Year = 3
    }

    class MainWindow
    {
        [Widget] Window main_window;
        [Widget] Viewport document_viewport;
        [Widget] Toolbar main_toolbar;
        [Widget] TreeView document_treeview;
        [Widget] HScale toolbar_scale_page;
        [Widget] Statusbar statusbar;

        public void OnDelete(object obj, DeleteEventArgs args)
        {
            Application.Quit();
        }

        public void OnFileImportActivated(object obj, EventArgs args)
        {
            FileChooserDialog dialog = new FileChooserDialog("Import", null, FileChooserAction.Open,
                                                             "Cancel", ResponseType.Cancel,
                                                             "Import", ResponseType.Accept);
            FileFilter filter = new FileFilter();

            filter.Name = "PDF and PostScript documents";
            filter.AddMimeType("application/pdf");
            filter.AddPattern("*.pdf");
            filter.AddMimeType("application/postscript");
            filter.AddPattern("*.ps");
            dialog.AddFilter(filter);

            if (dialog.Run() == (int)ResponseType.Accept) {
                Console.WriteLine("Import paper");
            }
            dialog.Destroy();
        }

        public void OnFileQuitActivated(object obj, EventArgs args)
        {
            Application.Quit();
        }

        public void OnHelpAboutActivated(object obj, EventArgs args)
        {
            AboutDialog dialog = new AboutDialog();

            dialog.ProgramName = "Papeles";
            dialog.Version = "0.1";
            dialog.Copyright = "Copyright \u00a9 2009 Jacinto Shy, Jr.";
            dialog.Run(); // TODO: don't block
            dialog.Destroy();
        }

        public void OnPrintClicked(object obj, EventArgs args)
        {
        }

        public void OnPrevPageClicked(object obj, EventArgs args)
        {
        }

        public void OnNextPageClicked(object obj, EventArgs args)
        {
        }

        public void OnZoomOutClicked(object obj, EventArgs args)
        {
            Adjustment adj = toolbar_scale_page.Adjustment;

            if (toolbar_scale_page.Value - adj.PageIncrement >= adj.Lower)
                toolbar_scale_page.Value -= adj.PageIncrement;
        }

        public void OnZoomInClicked(object obj, EventArgs args)
        {
            Adjustment adj = toolbar_scale_page.Adjustment;

            if (toolbar_scale_page.Value + adj.PageIncrement <= adj.Upper)
                toolbar_scale_page.Value += adj.PageIncrement;
        }

        public void OnScalePageValueChanged(object obj, EventArgs args)
        {
            // toolbar_scale_page.Value
        }

        /// <summary>
        /// Populate headers in treeview for library.
        /// </summary>
        void CreateLibraryView(ListStore store)
        {
            TreeViewColumn authorsColumn = new TreeViewColumn("Authors", new CellRendererText(),
                                                              "text",    Column.Authors);
            TreeViewColumn titleColumn   = new TreeViewColumn("Title",   new CellRendererText(),
                                                              "text",    Column.Title);
            TreeViewColumn journalColumn = new TreeViewColumn("Journal", new CellRendererText(),
                                                              "text",    Column.Journal);
            TreeViewColumn yearColumn    = new TreeViewColumn("Year",    new CellRendererText(),
                                                              "text",    Column.Year);

            authorsColumn.SortColumnId = (int) Column.Authors;
            titleColumn.SortColumnId   = (int) Column.Title;
            journalColumn.SortColumnId = (int) Column.Journal;
            yearColumn.SortColumnId    = (int) Column.Year;

            authorsColumn.Expand = true;
            titleColumn.Expand   = true;
            journalColumn.Expand = true;
            yearColumn.Expand    = true;

            authorsColumn.Resizable = true;
            titleColumn.Resizable   = true;
            journalColumn.Resizable = true;
            yearColumn.Resizable    = true;

            document_treeview.AppendColumn(authorsColumn);
            document_treeview.AppendColumn(titleColumn);
            document_treeview.AppendColumn(journalColumn);
            document_treeview.AppendColumn(yearColumn);
            document_treeview.Model = store;
        }

        void DisplayDocument(string filePath)
        {
            IDocument doc = new PdfDocument("file://" + filePath, "");
            DocumentInfo info = doc.Info;

            if (info.Title == null || info.Title == "")
                main_window.Title = System.IO.Path.GetFileName(filePath);
            else
                main_window.Title = info.Title;

            Box box = new VBox(true, 0);

            Gdk.Color white = new Gdk.Color(0xFF, 0xFF, 0xFF);
            for (int i = 0; i < doc.NPages; i++) {
                RenderedDocument page = new RenderedDocument(new RenderContext(i, 0, 1.0), doc);

                page.ModifyBg(StateType.Normal, white);
                box.Add(page);
            }

            document_viewport.Add(box);
        }

        public MainWindow()
        {
            Glade.XML gxml = new Glade.XML(null, "papeles.glade", "main_window", null);

            gxml.Autoconnect(this);

            Database db = new Database();
      
            ListStore docStore = new ListStore(typeof(string), typeof(string),
                                               typeof(string), typeof(string));
            /*docStore.AppendValues("Jacinto Shy", "Tetrahydrobiopterin",
                                  "J Phys Chem B", "2006");
            docStore.AppendValues("Jacinto Shy", "Nascent HDL",
            "Nat Struct Mol Biol", "2007");
            this.CreateLibraryView(docStore);*/

            //this.DisplayDocument("/home/jacinto/Documents/papers/inference-secco08.pdf");

            uint totalPapers = 2;
            statusbar.Push(1, String.Format("{0} papers", totalPapers));

            main_toolbar.IconSize = IconSize.SmallToolbar;

            main_window.ShowAll();
        }
    }
}
