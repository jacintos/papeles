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

    public static void Main()
    {
      Application.Init();

      IDocument doc = new PdfDocument("file:///home/jacinto/Documents/papers/inference-secco08.pdf", "");
      DocumentInfo info = doc.Info;
      string title = info.Title != "" ? info.Title : "Papeles";

      Window myWin = new Window(title);
      myWin.DeleteEvent += delete_event;


      // double pageHeight, pageWidth;
      // page.GetSize(out pageWidth, out pageHeight);
      myWin.SetDefaultSize(640, 480);

      Box box = new VBox(true, 0);
      ScrolledWindow scwin = new ScrolledWindow();
      Viewport vp = new Viewport();

      for (int i = 0; i < doc.NPages; i++) {
        RenderedDocument page = new RenderedDocument(new RenderContext(i, 0, 1.0), doc);

        page.ModifyBg(StateType.Normal, new Gdk.Color(0xFF, 0xFF, 0xFF));
        box.Add(page);
      }

      vp.Add(box);
      scwin.Add(vp);

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
      myWin.Add(scwin);
      myWin.ShowAll();

      Application.Run();
    }
  }
}
