
namespace Papeles
{
  class RenderedDocument : Gtk.DrawingArea
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
}