/* -*- coding: utf-8 -*- */
/* Database.cs
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

using System.Data;
using Mono.Data.Sqlite;

namespace Papeles
{
  class Database
  {
    IDbConnection conn;
    IDbCommand cmd;

    public Database()
    {
      conn = new SqliteConnection("Data Source=test.db3;version=3;") as IDbConnection;
      conn.Open();

      cmd = conn.CreateCommand();

      cmd.CommandText =
@"CREATE TABLE IF NOT EXISTS test (
    uri TEXT,
    authors TEXT,
    title TEXT,
    journal_name TEXT,
    journal_volume TEXT,
    journal_number TEXT,
    journal_pages TEXT,
    year TEXT,
    rating INTEGER,
    flagged INTEGER,
    date_added TEXT,
    date_last_read TEXT,
    doi TEXT,
    id INTEGER PRIMARY KEY)";

      cmd.ExecuteNonQuery();
    }
  }
}
