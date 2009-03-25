/* -*- coding: utf-8 -*- */
/* TestDatabase.cs
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

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System;
using System.Data;
using System.IO;

namespace Papeles
{
	[TestFixture]
	public class TestDatabase
	{
		string database_file;

		[SetUp] public void Init ()
		{
			database_file = "test.db3";
			Database.Load (database_file);
		}

		[TearDown] public void Dispose ()
		{
			Database.Close ();
			File.Delete (database_file);
		}

		[Test] public void TestPaperCrud ()
		{
			IDataReader reader;
			Paper paper = new Paper ();

			paper.Title = "The refined structure of nascent HDL reveals a key functional domain for particle maturation and dysfunction";
			paper.Authors = "Zhiping Wu, Matthew A Wagner, Lemin Zheng, John S Parks, Jacinto M Shy Jr, Jonathan D Smith, Valentin Gogonea  &  Stanley L Hazen";
			paper.Journal = "Nature Structural & Molecular Biology";
			paper.Volume = "14";
			paper.Pages = "861 - 868";
			paper.Year = "2007";
			paper.Save ();

			Assert.That (paper.ID, Is.EqualTo (1));

			reader = Database.Query (String.Format ("SELECT title, authors, journal, volume, pages, year, uri, doi FROM papers WHERE id = {0}", paper.ID));
			Assert.That (reader, Is.Not.Null);

			if (reader != null) {
				reader.Read ();
				Assert.That (reader.GetString (0), Is.EqualTo (paper.Title));
				Assert.That (reader.GetString (1), Is.EqualTo (paper.Authors));
				Assert.That (reader.GetString (2), Is.EqualTo (paper.Journal));
				Assert.That (reader.GetString (3), Is.EqualTo (paper.Volume));
				Assert.That (reader.GetString (4), Is.EqualTo (paper.Pages));
				Assert.That (reader.GetString (5), Is.EqualTo (paper.Year));
				Assert.That (reader.IsDBNull (6), Is.True);
				Assert.That (reader.IsDBNull (7), Is.True);
				reader.Close ();
			}

			paper.Uri = "http://www.nature.com/nsmb/journal/v14/n9/abs/nsmb1284.html";
			paper.Doi = "10.1038/nsmb1284";
			paper.Save ();

			reader = Database.Query (String.Format ("SELECT uri, doi FROM papers WHERE id = {0}", paper.ID));
			Assert.That (reader, Is.Not.Null);

			if (reader != null) {
				reader.Read ();
				Assert.That (reader.GetString (0), Is.EqualTo (paper.Uri));
				Assert.That (reader.GetString (1), Is.EqualTo (paper.Doi));
				reader.Close ();
			}

			paper.Delete ();
			reader = Database.Query (String.Format ("SELECT title FROM papers WHERE id = {0}", paper.ID));
			Assert.That (reader, Is.Null);
		}
	}
}
