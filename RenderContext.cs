
namespace Papeles
{
  public struct RenderContext
  {
    public int pageIndex;
    public int rotation;
    public double scale;

    public RenderContext(int pageIndex, int rotation, double scale)
    {
      this.pageIndex = pageIndex;
      this.rotation = rotation;
      this.scale = scale;
    }
  }
}
