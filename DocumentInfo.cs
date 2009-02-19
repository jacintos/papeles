/* -*- coding: utf-8 -*- */
/* DocumentInfo.cs
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
  public class DocumentInfo
  {
    public string Title { get; set; }
    public string Format { get; set; }
    public string Author { get; set; }
    public string Subject { get; set; }
    public string Keywords { get; set; }
    public string Creator { get; set; }
    public string Producer { get; set; }
    public int CreationDate { get; set; }
    public int ModificationDate { get; set; }
    public string Linearized { get; set; }
    public string Metadata { get; set; }
  }
}
