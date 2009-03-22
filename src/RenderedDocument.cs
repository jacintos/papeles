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
		bool initialized;
		int page_index;
		RenderContext rc;
		IDocument doc;

		public RenderedDocument (int pageIndex, RenderContext rc, IDocument doc)
		{
			this.page_index  = pageIndex;
			this.rc          = rc;
			this.doc         = doc;
			this.initialized = false;
		}

		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			int width, height;

			doc.GetPageSize (page_index, out width, out height);
			SetSizeRequest ((int) (rc.Scale * width), (int) (rc.Scale * height));

			// When container is first shown, the expose-event signall will be sent to all elements
			if (initialized)
				doc.Render (page_index, rc, args.Window);
			else
				initialized = true;
			return true;
		}
	}
}