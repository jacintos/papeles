
using Gtk;
using Glade;
using System;

namespace Papeles
{
  class MainWindow
  {
    [Widget] Window main_window;
    [Widget] VBox main_vbox;
    [Widget] TreeView document_treeview;
    [Widget] HScale toolbar_scale_page;
    [Widget] Statusbar statusbar;

    public void OnDelete(object obj, DeleteEventArgs args)
    {
      Application.Quit();
    }

    public void OnFileImportActivated(object obj, EventArgs args)
    {
      FileChooserDialog dialog = new FileChooserDialog("Import", null, FileChooserAction.Open,
                                                       "Cancel", ResponseType.Cancel,
                                                       "Import", ResponseType.Accept);
      FileFilter filter = new FileFilter();

      filter.Name = "PDF and PostScript documents";
      filter.AddMimeType("application/pdf");
      filter.AddPattern("*.pdf");
      filter.AddMimeType("application/postscript");
      filter.AddPattern("*.ps");
      dialog.AddFilter(filter);

      if (dialog.Run() == (int)ResponseType.Accept) {
        Console.WriteLine("Import paper");
      }
      dialog.Destroy();
    }

    public void OnFileQuitActivated(object obj, EventArgs args)
    {
      Application.Quit();
    }

    public void OnHelpAboutActivated(object obj, EventArgs args)
    {
      AboutDialog dialog = new AboutDialog();

      dialog.ProgramName = "Papeles";
      dialog.Version = "0.1";
      dialog.Copyright = "Copyright \u00a9 2009 Jacinto Shy, Jr.";
      dialog.Run(); // TODO: don't block
      dialog.Destroy();
    }

    public void OnPrintClicked(object obj, EventArgs args)
    {
    }

    public void OnPrevPageClicked(object obj, EventArgs args)
    {
    }

    public void OnNextPageClicked(object obj, EventArgs args)
    {
    }

    public void OnZoomOutClicked(object obj, EventArgs args)
    {
      Adjustment adj = toolbar_scale_page.Adjustment;

      if (toolbar_scale_page.Value - adj.PageIncrement >= adj.Lower)
        toolbar_scale_page.Value -= adj.PageIncrement;
    }

    public void OnZoomInClicked(object obj, EventArgs args)
    {
      Adjustment adj = toolbar_scale_page.Adjustment;

      if (toolbar_scale_page.Value + adj.PageIncrement <= adj.Upper)
        toolbar_scale_page.Value += adj.PageIncrement;
    }

    public void OnScalePageValueChanged(object obj, EventArgs args)
    {
      // toolbar_scale_page.Value
    }

    public MainWindow()
    {
      Glade.XML gxml = new Glade.XML(null, "papeles.glade", "main_window", null);

      gxml.Autoconnect(this);

      

      main_window.ShowAll();
    }
  }
}
