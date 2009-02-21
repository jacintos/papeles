
all:
	gmcs -pkg:gtk-sharp-2.0 -pkg:glade-sharp-2.0 -r:Mono.Cairo -r:poppler-sharp.dll \
	 -resource:papeles.glade \
	 -out:papeles.exe main.cs MainWindow.cs RenderContext.cs PdfDocument.cs \
	 IDocument.cs RenderedDocument.cs DocumentInfo.cs
