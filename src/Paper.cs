
namespace Papeles
{
  class Paper
  {
    public string Uri { get; set; }
    public string Title { get; set; }
    public string Journal { get; set; }
    public string Volume { get; set; }
    public string Number { get; set; }
    public string Pages { get; set; }
    public string Year { get; set; }
    public int Rating { get; set; }

    public int LocalDbId { get; set; }
  }
}
