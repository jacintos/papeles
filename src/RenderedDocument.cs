/* -*- coding: utf-8 -*- */
/* RenderedDocument.cs
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

namespace Papeles
{
	class RenderedDocument : Gtk.DrawingArea
	{
		int pageIndex;
		RenderContext rc;
		IDocument doc;

		public RenderedDocument (int pageIndex, RenderContext rc, IDocument doc)
		{
			this.pageIndex = pageIndex;
			this.rc = rc;
			this.doc = doc;
		}

		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			int width, height;

			doc.GetPageSize (pageIndex, out width, out height);
			SetSizeRequest ((int) (rc.Scale * width), (int) (rc.Scale * height));
			doc.Render (pageIndex, rc, args.Window);
			return true;
		}
	}
}