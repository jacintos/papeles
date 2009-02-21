/* -*- coding: utf-8 -*- */
/* main.cs
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

using System;
using Gtk;

namespace Papeles
{
  public class PapelesApp
  {
    // static UIManager CreateUIManager()
    // {
    //   UIManager uim = new UIManager();
    //   ActionGroup actionGroup = new ActionGroup("Actions");
    //   ActionEntry[] menuActions = new ActionEntry[]{
    //     new ActionEntry("File", "", "_File", "", "", null),
    //     new ActionEntry("FileImport", Stock.Add, "_Import", "", "", new EventHandler(OnFileImportActivated)),
    //     new ActionEntry("FileQuit", Stock.Quit, "_Quit", "<control>Q", "", new EventHandler(OnFileQuitActivated)),
    //     new ActionEntry("Help", "", "_Help", "", "", null),
    //     new ActionEntry("HelpAbout", Stock.About, "_About", "", "", new EventHandler(OnHelpAboutActivated)),
    //   };
    //   ActionEntry[] mainToolbarActions = new ActionEntry[]{
    //     new ActionEntry("MainToolbarPrint", Stock.Print, "Print", "<control>P", "Print", null),
    //     new ActionEntry("MainToolbarPageUp", Stock.GoUp, "", "", "Previous page", null),
    //     new ActionEntry("MainToolbarPageDown", Stock.GoDown, "", "", "Next page", null)
    //   };
    //   ToolItem scaleItem = new ToolItem();
    //   Scale scale = new HScale(1.0, 5.0, 0.1);
    //   Gtk.Action pageScaleAction = new Gtk.Action("MainToolbarPageScale", "", "Scale", "");

    //   // Seems we can't use custom widgets for a toolitem created in the UI description.
    //   scale.DrawValue = false;
    //   scale.SetSizeRequest(100, -1);
    //   scaleItem.Add(scale);
    //   pageScaleAction.ConnectProxy(scaleItem);

    //   actionGroup.Add(pageScaleAction);
    //   actionGroup.Add(menuActions);
    //   actionGroup.Add(mainToolbarActions);

    //   uim.InsertActionGroup(actionGroup, 0);
    //   uim.AddUiFromFile("uidesc.xml");
    //   return uim;
    // }

    static ScrolledWindow CreateLibraryView(ListStore store)
    {
      ScrolledWindow sw = new ScrolledWindow();
      TreeView tv = new TreeView(store);
      TreeViewColumn authorColumn  = new TreeViewColumn("Author",  new CellRendererText(), "text", 0);
      TreeViewColumn titleColumn   = new TreeViewColumn("Title",   new CellRendererText(), "text", 1);
      TreeViewColumn journalColumn = new TreeViewColumn("Journal", new CellRendererText(), "text", 2);
      TreeViewColumn yearColumn    = new TreeViewColumn("Year",    new CellRendererText(), "text", 3);

      authorColumn.SortColumnId = 0;
      titleColumn.SortColumnId = 1;
      journalColumn.SortColumnId = 2;
      yearColumn.SortColumnId = 3;

      authorColumn.Expand  = true;
      titleColumn.Expand   = true;
      journalColumn.Expand = true;
      //yearColumn.Expand    = true;

      authorColumn.Resizable  = true;
      titleColumn.Resizable   = true;
      journalColumn.Resizable = true;
      yearColumn.Resizable    = true;

      tv.HeadersVisible = true;
      tv.RulesHint = true;
      tv.EnableGridLines = TreeViewGridLines.Vertical;
      tv.AppendColumn(authorColumn);
      tv.AppendColumn(titleColumn);
      tv.AppendColumn(journalColumn);
      tv.AppendColumn(yearColumn);

      sw.Add(tv);
      return sw;
    }

    static ScrolledWindow CreatePreview(IDocument doc)
    {
      Box box = new VBox(true, 0);
      ScrolledWindow scwin = new ScrolledWindow();
      Viewport vp = new Viewport();

      Gdk.Color white = new Gdk.Color(0xFF, 0xFF, 0xFF);
      for (int i = 0; i < doc.NPages; i++) {
        RenderedDocument page = new RenderedDocument(new RenderContext(i, 0, 1.0), doc);

        page.ModifyBg(StateType.Normal, white);
        box.Add(page);
      }

      vp.Add(box);
      scwin.Add(vp);
      return scwin;
    }

    public static void Main(string[] args)
    {
      new PapelesApp();

      // string filePath, title;
      // if (args.Length >= 1)
      //   filePath = args[0];
      // else
      //   filePath = "/home/jacinto/Documents/papers/inference-secco08.pdf";

      // IDocument doc = new PdfDocument("file://" + filePath, "");
      // DocumentInfo info = doc.Info;

      // if (info.Title == null || info.Title == "")
      //   title = System.IO.Path.GetFileName(filePath);
      // else
      //   title = info.Title;

      // Window myWin = new Window(title);
      // myWin.DeleteEvent += OnDelete;

      // // double pageHeight, pageWidth;
      // // page.GetSize(out pageWidth, out pageHeight);
      // myWin.SetDefaultSize(640, 480);

      // UIManager uim = CreateUIManager();
      // myWin.AddAccelGroup(uim.AccelGroup);

      // Box winBox = new VBox(false, 0);

      // MenuBar menu = (MenuBar)uim.GetWidget("/MenuBar");

      // Paned paned = new VPaned();

      // ListStore docStore = new ListStore(typeof(string), typeof(string),
      //                                    typeof(string), typeof(string));
      // docStore.AppendValues("Jacinto Shy", "Tetrahydrobiopterin",
      //                       "J Phys Chem B", "2006");
      // docStore.AppendValues("Jacinto Shy", "Nascent HDL",
      //                       "Nat Struct Mol Biol", "2007");

      // ScrolledWindow library = CreateLibraryView(docStore);
      // ScrolledWindow preview = CreatePreview(doc);

      // paned.Add1(library);
      // paned.Add2(preview);

      // Toolbar mainToolbar = (Toolbar)uim.GetWidget("/MainToolbar");

      // /*
      // Table table = new Table(2, 2, false);
      // table.Attach(myLabel, 0, 1, 0, 1, AttachOptions.Fill | AttachOptions.Expand,
      //              AttachOptions.Fill | AttachOptions.Expand, 0, 0);
      // table.Attach(myButton, 0, 2, 1, 2, AttachOptions.Fill | AttachOptions.Expand,
      //              AttachOptions.Fill | AttachOptions.Expand, 0, 0);
      // */

      // winBox.PackStart(menu, false, false, 0);
      // winBox.PackStart(paned, true, true, 0);
      // winBox.PackStart(mainToolbar, false, false, 0);

      // myWin.Add(winBox);
      // myWin.ShowAll();
    }

    public PapelesApp()
    {
      Application.Init();

      new MainWindow();

      Application.Run();
    }
  }
}
