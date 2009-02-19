
all:
	gmcs -pkg:gtk-sharp-2.0 -r:Mono.Cairo -r:poppler-sharp.dll \
	 -out:papeles.exe main.cs RenderContext.cs PdfDocument.cs \
	 IDocument.cs RenderedDocument.cs
