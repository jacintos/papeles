
using Poppler;

namespace Papeles
{
  public class PdfDocument : IDocument
  {
    Document document;
    string password;

    public int NPages
    {
      get {
        return document.NPages;
      }
    }

    public string Password
    {
      get {
        return password;
      }
      set {
        password = value;
      }
    }

    public PdfDocument()
    {
      password = "";
    }

    public PdfDocument(string uri, string password)
    {
      this.password = password;
      this.Load(uri);
    }

    public void Load(string uri)
    {
      document = Document.NewFromFile(uri, password);
    }

    // public Page GetPage(int index)
    // {
    //   if (document != null && index < document.NPages)
    //     return document.GetPage(index);
    //   else
    //     return null;
    // }

    public void GetPageSize(int pageIndex, out int width, out int height)
    {
      Page page;
      double pageWidth, pageHeight;

      if (document != null && pageIndex < document.NPages) {
        page = document.GetPage(pageIndex);
        page.GetSize(out pageWidth, out pageHeight);
        width = (int)pageWidth;
        height = (int)pageHeight;
      } else {
        height = width = 0; // FIXME: throw an exception
      }
    }

    public void Render(RenderContext rc, Gdk.Drawable drawable)
    {
      Page page;
      Cairo.Context context;
      double width, height;

      if (document != null && rc.pageIndex < document.NPages) {
        page = document.GetPage(rc.pageIndex);
      } else {
        System.Console.WriteLine("Bad page index");
        return;
      }

      page.GetSize(out width, out height);
      width *= rc.scale;
      height *= rc.scale;

      context = Gdk.CairoHelper.Create(drawable);
      // context.Rectangle(0.0, 0.0, width, height);
      // context.Clip();
      // context.Scale(rc.scale, rc.scale);
      page.Render(context);

      // Garbage collection not currently supported in Mono.Cairo
      ((System.IDisposable)context).Dispose();
    }
  }
}
