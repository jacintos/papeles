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
		DocumentInfo docInfo;

		public int NPages {
			get { return document.NPages; }
		}

		// TODO: Use XMP metadata instead
		public DocumentInfo Info {
			get {
				if (docInfo == null) {
					docInfo = new DocumentInfo ();
					docInfo.Title = document.Title;
					docInfo.Format = document.Format;
					docInfo.Author = document.Author;
					docInfo.Subject = document.Subject;
					docInfo.Keywords = document.Keywords;
					docInfo.Creator = document.Creator;
					docInfo.Producer = document.Producer;
					docInfo.CreationDate = document.CreationDate;
					docInfo.ModificationDate = document.ModificationDate;
					docInfo.Linearized = document.Linearized;
					docInfo.Metadata = document.Metadata;
				}
				return docInfo;
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
