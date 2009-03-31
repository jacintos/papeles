/* -*- coding: utf-8 -*- */
/* Extensions.cs
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

namespace Papeles
{
	public static class Extensions
	{
		public static string Truncate (this string s, int maxLength, string replacement)
		{
			if (s.Length <= maxLength - replacement.Length)
				return s;

			// Split string in two
			int sHalfLength = s.Length >> 1;
			// These two statements help for understanding
			// string first = s.Substring (0, sHalfLength);
			// string last = s.Substring (sHalfLength, s.Length - sHalfLength);

			// Figure out how much needs to be dropped from each half
			int dropAmount = s.Length - maxLength + replacement.Length;
			int dropAmountHalf = dropAmount >> 1;
			int dropSecondAmount = dropAmount - dropAmountHalf;

			// Now actually do it
			string first = s.Substring (0, sHalfLength - dropAmountHalf);
			string last = s.Substring(sHalfLength + dropSecondAmount, s.Length - sHalfLength - dropSecondAmount);
			return first + replacement + last;
		}

		public static string ToFileSize (this long l)
		{
			return String.Format (new FileSizeFormatProvider (), "{0:fs}", l);
		}
	}
}
