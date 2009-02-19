
using System;
using Gtk;

namespace Papeles
{
  class RenderedDocument : DrawingArea
  {
    RenderContext rc;
    PdfDocument doc;

    public RenderedDocument(RenderContext rc, PdfDocument doc)
    {
      int height, width;

      this.rc = rc;
      this.doc = doc;
      doc.GetPageSize(rc.pageIndex, out width, out height);
      this.Size(width, height);
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

      PdfDocument doc = new PdfDocument("file:///home/jacinto/Documents/papers/inference-secco08.pdf", "");

      // double pageHeight, pageWidth;
      // page.GetSize(out pageWidth, out pageHeight);
      myWin.SetDefaultSize(640, 480);

      Box box = new VBox(true, 0);
      ScrolledWindow scwin = new ScrolledWindow();
      Viewport vp = new Viewport();

      RenderedDocument p1 = new RenderedDocument(new RenderContext(0, 0, 1.0), doc);
      RenderedDocument p2 = new RenderedDocument(new RenderContext(1, 0, 1.0), doc);
      p1.ModifyBg(StateType.Normal, new Gdk.Color(0xFF, 0xFF, 0xFF));
      p2.ModifyBg(StateType.Normal, new Gdk.Color(0xFF, 0xFF, 0xFF));
      vp.Add(p1);
      //box.Add(p2);

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
