
using System;
using Gtk;

namespace Papeles
{
  class RenderedDocument : DrawingArea
  {
    RenderContext rc;
    IDocument doc;

    public RenderedDocument(RenderContext rc, IDocument doc)
    {
      int height, width;

      this.rc = rc;
      this.doc = doc;
      doc.GetPageSize(rc.pageIndex, out width, out height);
      this.SetSizeRequest(width, height);
    }

    protected override bool OnExposeEvent(Gdk.EventExpose args)
    {
      doc.Render(rc, args.Window);
      return true;
    }
  }

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

      Window myWin = new Window("Papeles");
      myWin.DeleteEvent += delete_event;

      IDocument doc = new PdfDocument("file:///home/jacinto/Documents/papers/inference-secco08.pdf", "");

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
