/* -*- coding: utf-8 -*- */
/* Library.cs
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

using System.Collections.Generic;
using System.Linq;

namespace Papeles
{
	// TODO: callback on paper added to library or removed from library
	public static class Library
	{
		static List<Paper> papers = new List<Paper> ();

		public delegate void PaperAddedHandler (Paper paper);
		public delegate void PaperRemovedHandler (Paper paper);

		public static PaperAddedHandler PaperAdded;
		public static PaperRemovedHandler PaperRemoved;

		/// <summary>
		/// Get the number of papers in the library.
		/// </summary>
		public static int Count {
			get { return papers.Count; }
		}

		public static List<Paper> Papers {
			get { return papers; }
		}

		/// <summary>
		/// Load the library from the database. Database should obviously be
		/// loaded first.
		/// </summary>
		public static void Load ()
		{
			// Test if database is open here because this happens at initialization,
			// so it's kind of subtle.
			if (!Database.Open)
				return; // FIXME: this should be an exception

			papers = Paper.All ();
		}

		public static void Add (string filename)
		{
			Paper paper = new Paper (filename);

			papers.Add (paper);
			if (PaperAdded != null)
				PaperAdded (paper);
		}

		public static void Add (Paper paper)
		{
			papers.Add (paper);
			if (PaperAdded != null)
				PaperAdded (paper);
		}

		delegate bool SavePaper (Paper p);

		public static void Remove (int id)
		{
			Paper paper = null;
			SavePaper save = p => { paper = p; return true; };

			papers.RemoveAll (p => p.ID == id && save (p));
			if (paper != null && PaperRemoved != null)
				PaperRemoved (paper);
		}

		public static bool Contains (int id)
		{
			return papers.Find ((paper => paper.ID == id)) == null;
		}
	}
}
