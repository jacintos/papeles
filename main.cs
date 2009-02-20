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
  public class PapelesMain
  {
    static void delete_event(object obj, DeleteEventArgs args)
    {
      Application.Quit();
    }

    static void button_callback(object obj, EventArgs args)
    {
      Console.WriteLine("Button was pressed");
    }

    static Button CreateButton(String text)
    {
      Label label = new Label();
      Button button = new Button();

      label.Text = text;
      button.Add(label);
      return button;
    }

    static ScrolledWindow CreateLibraryView(ListStore store)
    {
      ScrolledWindow sw = new ScrolledWindow();
      TreeView tv = new TreeView(store);
      TreeViewColumn authorColumn  = new TreeViewColumn("Author",  new CellRendererText(), "text", 0);
      TreeViewColumn titleColumn   = new TreeViewColumn("Title",   new CellRendererText(), "text", 1);
      TreeViewColumn journalColumn = new TreeViewColumn("Journal", new CellRendererText(), "text", 2);
      TreeViewColumn yearColumn    = new TreeViewColumn("Year",    new CellRendererText(), "text", 3);

      authorColumn.Clickable  = true;
      titleColumn.Clickable   = true;
      journalColumn.Clickable = true;
      yearColumn.Clickable    = true;

      authorColumn.Expand  = true;
      titleColumn.Expand   = true;
      journalColumn.Expand = true;
      //yearColumn.Expand    = true;

      authorColumn.Resizable  = true;
      titleColumn.Resizable   = true;
      journalColumn.Resizable = true;
      yearColumn.Resizable    = true;

      authorColumn.SortIndicator  = true;
      // titleColumn.SortIndicator   = true;
      // journalColumn.SortIndicator = true;
      // yearColumn.SortIndicator    = true;

      tv.HeadersVisible = true;
      // tv.EnableGridLines = TreeViewGridLines.Vertical;
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
      string filePath, title;

      Application.Init();

      if (args.Length >= 1)
        filePath = args[0];
      else
        filePath = "/home/jacinto/Documents/papers/inference-secco08.pdf";

      IDocument doc = new PdfDocument("file://" + filePath, "");
      DocumentInfo info = doc.Info;

      if (info.Title == null || info.Title == "")
        title = System.IO.Path.GetFileName(filePath);
      else
        title = info.Title;

      Window myWin = new Window(title);
      myWin.DeleteEvent += delete_event;


      // double pageHeight, pageWidth;
      // page.GetSize(out pageWidth, out pageHeight);
      myWin.SetDefaultSize(640, 480);

      Paned paned = new VPaned();

      ListStore docStore = new ListStore(typeof(string), typeof(string),
                                         typeof(string), typeof(string));
      docStore.AppendValues("Jacinto Shy", "Tetrahydrobiopterin",
                            "J Phys Chem B", "2006");
      docStore.AppendValues("Jacinto Shy", "Nascent HDL",
                            "Nat Struct Mol Biol", "2007");

      ScrolledWindow library = CreateLibraryView(docStore);
      ScrolledWindow preview = CreatePreview(doc);

      paned.Add1(library);
      paned.Add2(preview);
      /*
      Label myLabel = new Label();
      myLabel.Text = "Hello, world";

      Button myButton = CreateButton("Press Me");
      myButton.Clicked += button_callback;
      /*
      /*
      VBox myBox = new VBox(false, 0);
      myBox.PackStart(myLabel, false, false, 0);
      myBox.PackStart(myButton, false, false, 0);
      */
      /*
      Table table = new Table(2, 2, false);
      table.Attach(myLabel, 0, 1, 0, 1, AttachOptions.Fill | AttachOptions.Expand,
                   AttachOptions.Fill | AttachOptions.Expand, 0, 0);
      table.Attach(myButton, 0, 2, 1, 2, AttachOptions.Fill | AttachOptions.Expand,
                   AttachOptions.Fill | AttachOptions.Expand, 0, 0);
      */
      myWin.Add(paned);
      myWin.ShowAll();

      Application.Run();
    }
  }
}
