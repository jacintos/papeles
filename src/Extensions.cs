
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
	}
}
