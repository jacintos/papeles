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
		static Dictionary<string, DbType> properties_lookup;
		static Dictionary<string, DbType> columns_lookup;
		static Dictionary<string, string> column_property_map;

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
				Database.Execute (command, this, properties_lookup);
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
				Database.Execute (command, this, properties_lookup);
			}
		}

		public void Load ()
		{
			string query = String.Format ("SELECT * FROM papers WHERE id = {0}", ID);
			DbDataReader reader = Database.Query (query);

			if (reader == null) {
				Console.WriteLine (String.Format ("Unable to find paper with id {0}", ID));
				return;
			}
			reader.Read ();
			Database.SetProperties (this, reader, columns_lookup, column_property_map);
			reader.Close ();
		}

		public void Delete ()
		{
			Database.Execute (String.Format ("DELETE FROM papers WHERE id = {0}", ID));
		}

		public Paper ()
		{
		}

		public Paper (int id)
		{
			ID = id;
		}

		public Paper (string filename)
		{
			FilePath = filename;
		}

		public static void Init ()
		{
			CreatePropertiesLookupTable ();
			CreateColumnsLookupTable ();
			CreateColumnPropertyMap ();
		}

		public static void CreateTable ()
		{
			Database.Execute (
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
)");
		}

		public static List<Paper> All ()
		{
			string query = "SELECT authors, title, journal, year, file_path, id FROM papers";
			DbDataReader reader = Database.Query (query);
			List<Paper> papers = new List<Paper> ();

			if (reader == null) {
				Console.WriteLine ("No papers in the database");
				return papers;
			}

			while (reader.Read ()) {
				Paper paper = new Paper ();

				Database.SetProperties (paper, reader, columns_lookup, column_property_map);
				papers.Add (paper);
			}
			reader.Close ();
			return papers;
		}

		public static Paper FindById (int id)
		{
			string query = String.Format (
"SELECT authors, title, journal, year, rating, flagged FROM papers WHERE id = {0}", id);
			DbDataReader reader = Database.Query (query);

			if (reader == null) {
				Console.WriteLine (String.Format ("Unable to find paper with id {0}", id));
				return null;
			}

			Paper paper = new Paper ();
			Database.SetProperties (paper, reader, columns_lookup, column_property_map);
			return paper;
		}

		static void CreatePropertiesLookupTable ()
		{
			properties_lookup = new Dictionary<string, DbType> ();
			properties_lookup.Add ("Authors",    DbType.String);
			properties_lookup.Add ("Uri",        DbType.String);
			properties_lookup.Add ("Doi",        DbType.String);
			properties_lookup.Add ("Title",      DbType.String);
			properties_lookup.Add ("Journal",    DbType.String);
			properties_lookup.Add ("Volume",     DbType.String);
			properties_lookup.Add ("Number",     DbType.String);
			properties_lookup.Add ("Month",      DbType.String);
			properties_lookup.Add ("Pages",      DbType.String);
			properties_lookup.Add ("Year",       DbType.String);
			properties_lookup.Add ("Abstract",   DbType.String);
			properties_lookup.Add ("Notes",      DbType.String);
			properties_lookup.Add ("CiteKey",    DbType.String);
			properties_lookup.Add ("Keywords",   DbType.String);
			properties_lookup.Add ("FilePath",   DbType.String);
			properties_lookup.Add ("Rating",     DbType.Int32);
			properties_lookup.Add ("Flagged",    DbType.Boolean); // FIXME: this will likely cause trouble
			properties_lookup.Add ("ImportedAt", DbType.DateTime);
			properties_lookup.Add ("ReadAt",     DbType.DateTime);
			properties_lookup.Add ("ID",         DbType.Int32);
		}

		static void CreateColumnsLookupTable ()
		{
			columns_lookup = new Dictionary<string, DbType> ();
			columns_lookup.Add ("authors",     DbType.String);
			columns_lookup.Add ("uri",         DbType.String);
			columns_lookup.Add ("doi",         DbType.String);
			columns_lookup.Add ("title",       DbType.String);
			columns_lookup.Add ("journal",     DbType.String);
			columns_lookup.Add ("volume",      DbType.String);
			columns_lookup.Add ("number",      DbType.String);
			columns_lookup.Add ("month",       DbType.String);
			columns_lookup.Add ("pages",       DbType.String);
			columns_lookup.Add ("year",        DbType.String);
			columns_lookup.Add ("abstract",    DbType.String);
			columns_lookup.Add ("notes",       DbType.String);
			columns_lookup.Add ("cite_key",    DbType.String);
			columns_lookup.Add ("keywords",    DbType.String);
			columns_lookup.Add ("file_path",   DbType.String);
			columns_lookup.Add ("rating",      DbType.Int32);
			columns_lookup.Add ("flagged",     DbType.Boolean);
			columns_lookup.Add ("imported_at", DbType.DateTime);
			columns_lookup.Add ("read_at",     DbType.DateTime);
			columns_lookup.Add ("id",          DbType.Int32);
		}

		static void CreateColumnPropertyMap ()
		{
			// FIXME: generate automatically 
			column_property_map = new Dictionary<string, string> ();
			column_property_map.Add ("authors",     "Authors");
			column_property_map.Add ("uri",         "Uri");
			column_property_map.Add ("doi",         "Doi");
			column_property_map.Add ("title",       "Title");
			column_property_map.Add ("journal",     "Journal");
			column_property_map.Add ("volume",      "Volume");
			column_property_map.Add ("number",      "Number");
			column_property_map.Add ("month",       "Month");
			column_property_map.Add ("pages",       "Pages");
			column_property_map.Add ("year",        "Year");
			column_property_map.Add ("abstract",    "Abstract");
			column_property_map.Add ("notes",       "Notes");
			column_property_map.Add ("cite_key",    "CiteKey");
			column_property_map.Add ("keywords",    "Keywords");
			column_property_map.Add ("file_path",   "FilePath");
			column_property_map.Add ("rating",      "Rating");
			column_property_map.Add ("flagged",     "Flagged");
			column_property_map.Add ("imported_at", "ImportedAt");
			column_property_map.Add ("read_at",     "ReadAt");
			column_property_map.Add ("id",          "ID");
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
