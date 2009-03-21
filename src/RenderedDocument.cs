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
		RenderContext rc;
		IDocument doc;

		public RenderedDocument (RenderContext rc, IDocument doc)
		{
			int height, width;

			this.rc = rc;
			this.doc = doc;
			doc.GetPageSize (rc.pageIndex, out width, out height);
			this.SetSizeRequest (width, height);
		}

		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			doc.Render (rc, args.Window);
			return true;
		}
	}
}