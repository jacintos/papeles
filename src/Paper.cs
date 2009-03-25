/* -*- coding: utf-8 -*- */
/* Paper.cs
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Mono.Data.Sqlite;

namespace Papeles
{
    /// <summary>
    /// Schema for a paper in the library.
    /// </summary>
    public class Paper
    {
		static Dictionary<string, DbType> lookup;

        public string   Authors { get; set; }
        public string   Uri { get; set; }
        public string   Doi { get; set; }
        public string   Title { get; set; }
        public string   Journal { get; set; }
        public string   Volume { get; set; }
        public string   Number { get; set; }
        public string   Month { get; set; } 
        public string   Pages { get; set; }
        public string   Year { get; set; }
        public string   Abstract { get; set; }
        public string   Notes { get; set; }
        public string   CiteKey { get; set; }
        public string   Keywords { get; set; }
        public string   FilePath { get; set; }
        public int      Rating { get; set; }
        public bool     Flagged { get; set; }
        public DateTime ImportedAt { get; set; }
        public DateTime ReadAt { get; set;} 
        public int      ID { get; set; }

		public void Save ()
		{
			if (ID == 0) {
				string command =
@"INSERT INTO papers (
    authors, uri, doi, title, journal, volume, number, month, pages, year, abstract,
    notes, cite_key, keywords, file_path, rating, flagged, imported_at, read_at
) VALUES (
    @Authors, @Uri, @Doi, @Title, @Journal, @Volume, @Number, @Month, @Pages, @Year, @Abstract,
    @Notes, @CiteKey, @Keywords, @FilePath, @Rating, @Flagged, @ImportedAt, @ReadAt
)";
				Database.Execute (command, this, lookup);
				ID = Convert.ToInt32 (Database.ExecuteScalar ("SELECT last_insert_rowid()"));
			} else {
				string command =
@"UPDATE papers SET
    authors = @Authors, uri = @Uri, doi = @Doi, title = @Title, journal = @Journal,
    volume = @Volume, number = @Number, month = @Month, pages = @Pages, year = @Year,
    abstract = @Abstract, notes = @Notes, cite_key = @CiteKey, keywords = @Keywords,
    file_path = @FilePath, rating = @Rating, flagged = @Flagged,
    imported_at = @ImportedAt, read_at = @ReadAt
WHERE ID = @ID";
				Database.Execute (command, this, lookup);
			}
		}

		public void Load ()
		{
			string query = String.Format ("SELECT * FROM papers WHERE id = {0}", ID);
			IDataReader reader = Database.Query (query);

			if (reader == null) {
				Console.WriteLine (String.Format ("Unable to find paper with id {0}", ID));
				return;
			}
			reader.Read ();
			Database.SetProperties (this, reader, lookup);
			reader.Close ();
		}

		public void Delete ()
		{
			Database.Execute (String.Format ("DELETE FROM papers WHERE id = {0}", ID));
		}

		public Paper ()
		{
			if (lookup == null)
				CreateLookupTable ();
		}

		public Paper (int id)
		{
			ID = id;
			if (lookup == null)
				CreateLookupTable ();
		}

		public Paper (string filename)
		{
			FilePath = filename;
			if (lookup == null)
				CreateLookupTable ();
		}

		public static void CreateTable(IDbCommand cmd)
		{
			cmd.CommandText =
@"CREATE TABLE IF NOT EXISTS papers (
    authors TEXT,
    uri TEXT,
    doi TEXT,
    title TEXT,
    journal TEXT,
    volume TEXT,
    number TEXT,
    month TEXT,
    pages TEXT,
    year TEXT,
    abstract TEXT,
    notes TEXT,
    cite_key TEXT,
    keywords TEXT,
    file_path TEXT,
    rating INTEGER,
    flagged INTEGER,
    imported_at DATETIME,
    read_at DATETIME,
    id INTEGER PRIMARY KEY
)";
			cmd.ExecuteNonQuery ();
		}

		public static List<Paper> All ()
		{
			string query = "SELECT authors, title, journal, year, rating, flagged FROM papers";
			IDataReader reader = Database.Query (query);
			List<Paper> papers = new List<Paper> ();

			if (reader == null) {
				Console.WriteLine ("No papers in the database");
				return papers;
			}

			while (reader.Read ()) {
				Paper paper = new Paper ();

				Database.SetProperties (paper, reader, lookup);
				reader.Close ();
				papers.Add (paper);
			}
			return papers;
		}

		public static Paper FindById (int id)
		{
			string query = String.Format (
"SELECT authors, title, journal, year, rating, flagged FROM papers WHERE id = {0}", id);
			IDataReader reader = Database.Query (query);

			if (reader == null) {
				Console.WriteLine (String.Format ("Unable to find paper with id {0}", id));
				return null;
			}

			Paper paper = new Paper ();
			Database.SetProperties (paper, reader, lookup);
			return paper;
		}

		static void CreateLookupTable ()
		{
			lookup = new Dictionary<string, DbType> ();
			lookup.Add ("Authors",    DbType.String);
			lookup.Add ("Uri",        DbType.String);
			lookup.Add ("Doi",        DbType.String);
			lookup.Add ("Title",      DbType.String);
			lookup.Add ("Journal",    DbType.String);
			lookup.Add ("Volume",     DbType.String);
			lookup.Add ("Number",     DbType.String);
			lookup.Add ("Month",      DbType.String);
			lookup.Add ("Pages",      DbType.String);
			lookup.Add ("Year",       DbType.String);
			lookup.Add ("Abstract",   DbType.String);
			lookup.Add ("Notes",      DbType.String);
			lookup.Add ("CiteKey",    DbType.String);
			lookup.Add ("Keywords",   DbType.String);
			lookup.Add ("FilePath",   DbType.String);
			lookup.Add ("Rating",     DbType.Int32);
			lookup.Add ("Flagged",    DbType.Int32);
			lookup.Add ("ImportedAt", DbType.DateTime);
			lookup.Add ("ReadAt",     DbType.DateTime);
			lookup.Add ("ID",         DbType.Int32);
		}
    }

	class PaperComparer : IEqualityComparer<Paper>
	{
		public bool Equals (Paper a, Paper b)
		{
			return a.ID == b.ID;
		}

		public int GetHashCode (Paper paper)
		{
			if (Object.ReferenceEquals (paper, null))
				return 0;
			return paper.ID.GetHashCode ();
		}
	}
}
