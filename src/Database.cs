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

using FSpot.Utils;
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
		static string data_source;

		public static void Load (string databaseFile)
		{
			bool papersTableExists = false, versionTableExists = false;

			data_source = String.Format ("Data Source={0}", databaseFile);

			SqliteConnection conn = new SqliteConnection (data_source);
			conn.Open ();

			try {
				DataTable tables = (conn as SqliteConnection).GetSchema ("tables");

				foreach (DataRow r in tables.Rows) {
					if ((string) r ["TABLE_NAME"] == "papers")
						papersTableExists = true;
					else if ((string) r ["TABLE_NAME"] == "version")
						versionTableExists = true;
				}
			} catch (NotSupportedException) {
				Log.Warning ("Unable to get database metadata");
			} catch (ArgumentException) {
				Log.Warning ("Unable to get table names from schema");
			}
			conn.Close ();

			if (!papersTableExists)
				Paper.CreateTable ();
			if (!versionTableExists)
				CreateVersionTable ();
		}

		static void CreateVersionTable ()
		{
			Execute ("CREATE TABLE IF NOT EXISTS version ( version TEXT )");
		}

		/// <summary>
		/// Special case of Execute where the ID of the row that was just inserted is returned.
		/// </summary>
		public static object Insert (string command, Object obj, Dictionary<string, DbType> lookup)
		{
			SqliteConnection conn = new SqliteConnection (data_source);
			SqliteCommand cmd;
			object result = null;

			conn.Open ();
			cmd = new SqliteCommand (command, conn);

			try {
				if (obj != null)
					Database.AddParameters (cmd, obj, lookup);
				cmd.ExecuteNonQuery ();

				// Must do this on the same connection, otherwise Sqlite seems to "forget"
				cmd = new SqliteCommand ("SELECT last_insert_rowid()", conn);
				result = cmd.ExecuteScalar ();
			} catch (KeyNotFoundException) {
				Log.Warning ("Missing a parameter somewhere; not executing SQL statement");
			} catch (Exception e) {
				Log.Exception ("Exception occurred while executing query", e);
			} finally {
				cmd.Dispose ();
				conn.Dispose ();
			}
			return result;
		}

		public static void Execute (string command, Object obj, Dictionary<string, DbType> lookup)
		{
			SqliteConnection conn = new SqliteConnection (data_source);
			SqliteCommand cmd;

			conn.Open ();
			cmd = new SqliteCommand (command, conn);

			try {
				if (obj != null)
					Database.AddParameters (cmd, obj, lookup);
				cmd.ExecuteNonQuery ();
			} catch (KeyNotFoundException) {
				Log.Warning ("Missing a parameter somewhere; not executing SQL statement");
			} catch (Exception e) {
				Log.Exception ("Exception occurred while executing query", e);
			} finally {
				cmd.Dispose ();
				conn.Dispose ();
			}
		}

		public static void Execute (string command)
		{
			Execute (command, null, null);
		}

		public static object ExecuteScalar (string command, Object obj, Dictionary<string, DbType> lookup)
		{
			SqliteConnection conn = new SqliteConnection (data_source);
			SqliteCommand cmd;
			object result = null;

			conn.Open ();
			cmd = new SqliteCommand (command, conn);

			try {
				if (obj != null)
					Database.AddParameters (cmd, obj, lookup);
				result = cmd.ExecuteScalar ();
			} catch (KeyNotFoundException) {
				Log.Warning ("Missing a parameter somewhere; not executing SQL statement");
			} catch (Exception e) {
				Log.Exception (e);
			} finally {
				cmd.Dispose ();
				conn.Dispose ();
			}
			return result;
		}

		public static object ExecuteScalar (string command)
		{
			return ExecuteScalar (command, null, null);
		}

		public static DbDataReader Query(string query)
		{
			SqliteConnection conn = new SqliteConnection (data_source);
			SqliteCommand cmd;
			DbDataReader reader;

			conn.Open ();
			cmd = new SqliteCommand (query, conn);

			try {
				reader = cmd.ExecuteReader ();
			} catch (Exception e) {
				Log.Exception ("Exception occurred while executing query", e);
				return null;
			}

			if (!(reader as SqliteDataReader).HasRows)
				return null;
			return reader;
		}

		public static void AddParameters (SqliteCommand cmd, Object obj, Dictionary<string, DbType> lookup)
		{
			foreach (PropertyInfo prop in obj.GetType ().GetProperties ()) {
				SqliteParameter param = new SqliteParameter ();

				param.ParameterName = "@" + prop.Name;
				param.Value = prop.GetValue (obj, null);
				param.DbType = lookup [prop.Name];
				cmd.Parameters.Add (param);
			}
		}

		public static void SetProperties (Object obj, DbDataReader reader, Dictionary<string, DbType> lookup,
						  Dictionary<string, string> property_map)
		{
			Type type = obj.GetType ();

			for (int columnNumber = 0; columnNumber < reader.VisibleFieldCount; columnNumber++) {
				string column = reader.GetName (columnNumber);
				PropertyInfo prop;

				try {
					prop = type.GetProperty (property_map [column]);
				} catch (KeyNotFoundException) {
					Log.WarningFormat ("No mapping to property from column {0}", column);
					continue;
				}
				if (prop == null) {
					Log.WarningFormat ("Unable to find property with name {0}", column);
					continue;
				}
				if (reader.IsDBNull (columnNumber)) {
					prop.SetValue (obj, null, null);
					continue;
				}

				try {
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
					case DbType.Boolean:
						prop.SetValue (obj, reader.GetBoolean (columnNumber), null);
						break;
					default:
						break;
					}
				} catch (KeyNotFoundException) {
					Log.WarningFormat ("Attempted to look up invalid column '{0}'", column);
				}
			}
		}
	}
}
