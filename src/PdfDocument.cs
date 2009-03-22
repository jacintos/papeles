/* -*- coding: utf-8 -*- */
/* PdfDocument.cs
 * Copyright (c) 2009 Jacinto Shy, Jr.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

using Poppler;
using System;

namespace Papeles
{
	public class PdfDocument : IDocument
	{
		Document document;
		DocumentInfo doc_info;

		public int NPages {
			get { return document.NPages; }
		}

		// TODO: Use XMP metadata instead
		public DocumentInfo Info {
			get {
				if (doc_info == null) {
					doc_info = new DocumentInfo ();
					doc_info.Title = document.Title;
					doc_info.Format = document.Format;
					doc_info.Author = document.Author;
					doc_info.Subject = document.Subject;
					doc_info.Keywords = document.Keywords;
					doc_info.Creator = document.Creator;
					doc_info.Producer = document.Producer;
					doc_info.CreationDate = document.CreationDate;
					doc_info.ModificationDate = document.ModificationDate;
					doc_info.Linearized = document.Linearized;
					doc_info.Metadata = document.Metadata;
				}
				return doc_info;
			}
		}

		public string Password { get; set; }

		public PdfDocument ()
		{
			Password = "";
		}

		public PdfDocument (string uri, string password)
		{
			Password = password;
			Load (uri);
		}

		public void Load (string uri)
		{
			document = Document.NewFromFile (uri, Password);
		}

		// public Page GetPage(int index)
		// {
		//   if (document != null && index < document.NPages)
		//     return document.GetPage(index);
		//   else
		//     return null;
		// }

		public void GetPageSize (int pageIndex, out int width, out int height)
		{
			Page page;
			double pageWidth, pageHeight;

			if (document != null && pageIndex < document.NPages) {
				page = document.GetPage (pageIndex);
				page.GetSize (out pageWidth, out pageHeight);
				width = (int) pageWidth;
				height = (int) pageHeight;
			} else {
				height = width = 0; // FIXME: throw an exception
			}
		}

		public void Render (int pageIndex, RenderContext rc, Gdk.Drawable drawable)
		{
			Page page;
			Cairo.Context context;
			double width, height;

			if (document != null && pageIndex < document.NPages) {
				page = document.GetPage (pageIndex);
			} else {
				Console.WriteLine ("Bad page index");
				return;
			}

			page.GetSize (out width, out height);
			width *= rc.Scale;
			height *= rc.Scale;

			context = Gdk.CairoHelper.Create (drawable);
			// context.Rectangle(0.0, 0.0, width, height);
			context.Scale(rc.Scale, rc.Scale);
			page.Render (context);

			// Garbage collection not currently supported in Mono.Cairo
			(context as IDisposable).Dispose ();
		}
	}
}
