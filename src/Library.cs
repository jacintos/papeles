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

using FSpot.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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
		public static PaperRemovedHandler PaperDeleted;

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
		/// Load the library from the database.
		/// </summary>
		public static void Load ()
		{
			Paper.Init ();
			papers = Paper.All ();
		}

		public static void Add (string filename)
		{
			Paper paper = new Paper (filename);

			// FIXME Check for metadata
			paper.Title = "Unknown";
			paper.Authors = "Unknown";
			paper.Journal = "Unknown";
			paper.Year = "Unknown";
			paper.Save ();

			papers.Add (paper);
			Log.DebugFormat ("Added new paper (ID = {0}) at '{1}' to database", paper.ID, paper.FilePath);

			if (PaperAdded != null)
				PaperAdded (paper);
		}

		public static void Add (Paper paper)
		{
			papers.Add (paper);
			if (PaperAdded != null)
				PaperAdded (paper);
		}

		public static void Remove (int id)
		{
			Paper paper = GetPaper (id);
			Remove (paper);
		}

		public static void Remove (Paper paper)
		{
			// FIXME: raise exception if paper is null
			papers.Remove (paper);
			try {
				// FIXME: ensure event handlers do not alter the paper
				if (PaperRemoved != null)
					PaperRemoved (paper);
			} finally {
				paper.Delete ();
			}
		}

		public static void Delete (int id)
		{
			Paper paper = GetPaper (id);
			Delete (paper);
		}

		public static void Delete (Paper paper)
		{
			// FIXME: raise exception if paper is null
			papers.Remove (paper);
			try {
				// FIXME: ensure event handlers do not alter the paper
				if (PaperDeleted != null)
					PaperDeleted (paper);
			} finally {
				File.Delete (paper.FilePath);
				paper.Delete ();
			}
		}

		public static bool Contains (int id)
		{
			return papers.Find ((paper => paper.ID == id)) == null;
		}

		public static Paper GetPaper (int id)
		{
			return papers.Find ((paper => paper.ID == id));
		}

	}
}
