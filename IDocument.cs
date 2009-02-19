
namespace Papeles
{
  public interface IDocument
  {
    int NPages { get; }
    void Load(string uri);
    void GetPageSize(int pageIndex, out int width, out int height);
    void Render(RenderContext rc, Gdk.Drawable drawable);
  }
}
