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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Mono.Data.Sqlite;

namespace Papeles
{
	public static class Database
	{
		static IDbConnection conn;
		static IDbCommand cmd;

		public static bool Open {
			get { return conn.State == ConnectionState.Open; }
		}

		public static bool Closed {
			get { return conn.State == ConnectionState.Closed; }
		}
 
		public static void Load (string databaseFile)
		{
			string dataSource = String.Format ("Data Source={0}", databaseFile);
			bool papersTableExists = false, versionTableExists = false;

			conn = new SqliteConnection (dataSource);
			conn.Open ();
			cmd = conn.CreateCommand ();

			try {
				DataTable tables = (conn as SqliteConnection).GetSchema ("tables");

				foreach (DataRow r in tables.Rows) {
					if ((string) r ["TABLE_NAME"] == "papers")
						papersTableExists = true;
					else if ((string) r ["TABLE_NAME"] == "version")
						versionTableExists = true;
				}
			} catch (NotSupportedException) {
				Console.WriteLine ("Unable to get metadata");
			} catch (ArgumentException) {
				Console.WriteLine ("Unable to get table names from schema");
			}
			if (!papersTableExists)
				Paper.CreateTable (cmd);
			if (!versionTableExists)
				CreateVersionTable ();
		}

		static void CreateVersionTable ()
		{
			cmd.CommandText = "CREATE TABLE IF NOT EXISTS version ( version TEXT )";
			cmd.ExecuteNonQuery ();
		}

		public static void Close ()
		{
			if (!Closed)
				conn.Close ();
		}

		public static void Execute (string command, Object obj, Dictionary<string, DbType> lookup)
		{
			try {
				cmd.CommandText = command;
				if (obj != null)
					Database.AddParameters (obj, lookup);
			} catch (KeyNotFoundException) {
				Console.WriteLine ("Missing a parameter somewhere; not executing SQL statement");
			} finally {
				cmd.ExecuteNonQuery ();
			}
		}

		public static void Execute (string command)
		{
			Execute (command, null, null);
		}

		public static object ExecuteScalar (string command, Object obj, Dictionary<string, DbType> lookup)
		{
			try {
				cmd.CommandText = command;
				if (obj != null)
					Database.AddParameters (obj, lookup);
			} catch (KeyNotFoundException) {
				Console.WriteLine ("Missing a parameter somewhere; not executing SQL statement");
				return null;
			}
			return cmd.ExecuteScalar ();
		}

		public static object ExecuteScalar (string command)
		{
			return ExecuteScalar (command, null, null);
		}

		public static IDataReader Query(string query)
		{
			cmd.CommandText = query;
			IDataReader reader = cmd.ExecuteReader ();

			if (!(reader as SqliteDataReader).HasRows)
				return null;
			return reader;
		}

		public static void AddParameters (Object obj, Dictionary<string, DbType> lookup)
		{
			foreach (PropertyInfo prop in obj.GetType ().GetProperties ()) {
				SqliteParameter param = new SqliteParameter ();

				param.ParameterName = "@" + prop.Name;
				param.Value = prop.GetValue (obj, null);
				param.DbType = lookup [prop.Name];
				cmd.Parameters.Add (param);
			}
		}

		public static void SetProperties (Object obj, IDataReader reader, Dictionary<string, DbType> lookup)
		{
			Type type = obj.GetType ();
			String[] columns = new string [reader.FieldCount];
			int columnNumber = 0;

			reader.GetValues (columns);
			foreach (string column in columns) {
				PropertyInfo prop = type.GetProperty (column);

				if (reader.IsDBNull (columnNumber)) {
					prop.SetValue (obj, null, null);
					continue;
				}

				switch (lookup [column]) {
				case DbType.String:
					prop.SetValue (obj, reader.GetString (columnNumber), null);
					break;
				case DbType.Int32:
					prop.SetValue (obj, reader.GetInt32 (columnNumber), null);
					break;
				case DbType.DateTime:
					prop.SetValue (obj, reader.GetDateTime (columnNumber), null);
					break;
				default:
					break;
				}
				columnNumber++;
			}
		}
	}
}
