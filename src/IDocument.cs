/* -*- coding: utf-8 -*- */
/* IDocument.cs
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
	public interface IDocument
	{
		int NPages { get; }
		DocumentInfo Info { get; }
		void Load (string uri);
		void GetPageSize (int pageIndex, out int width, out int height);
		void Render (int pageIndex, RenderContext rc, Gdk.Drawable drawable);
	}
}
