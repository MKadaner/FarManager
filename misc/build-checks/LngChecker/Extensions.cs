using System.Collections.Immutable;

namespace LngChecker
{
	internal static class Extensions
	{
		public static IEnumerable<string> UnIndent(this IEnumerable<string> lines)
		{
			foreach (var line in lines)
			{
				if (line.Length == 0)
					yield return line;
				else if (line[0] == '\t')
					yield return line[1..];
				else
					throw new ProcessingException($"Malformed line (indentation): \"{line}\"");
			}
		}

		public static IEnumerable<string> Indent(this IEnumerable<string> lines)
		{
			return lines.Select(l => l.Length == 0 ? l : $"\t{l}");
		}

		public static IEnumerable<(ImmutableList<string> comments, string label)> ToLangHppLabels(
			this IEnumerable<string> lines)
		{
			var comments = new List<string>();

			foreach (var line in lines)
			{
				if (line.Length == 0 || line.StartsWith("// ", StringComparison.OrdinalIgnoreCase))
				{
					comments.Add(line);
					continue;
				}

				var match = GlobalConstants.LangHppLabelPattern.Match(line);
				if (match.Success)
				{
					yield return (comments.ToImmutableList(), match.Groups["label"].Value);
					comments = new List<string>();
					continue;
				}

				throw new ProcessingException($"Malformed line (comment): \"{line}\"");
			}
		}

		public static IEnumerable<(string label, bool needTranslation, string value)> ToLngLabels(
			this IEnumerable<string> lines)
		{
			var needTranslation = false;

			foreach (var line in lines)
			{
				if (line == GlobalConstants.NeedTranslation)
				{
					needTranslation = true;
					continue;
				}

				var match = GlobalConstants.LngFileLabelValuePattern.Match(line);
				if (match.Success)
				{
					yield return (match.Groups["label"].Value, needTranslation, match.Groups["value"].Value);
					needTranslation = false;
				}
			}
		}
	}
}
