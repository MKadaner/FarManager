using System.Collections.Immutable;
using System.Text;

namespace LngChecker
{
	internal class LngFile
	{
		public LngFile((string file, string language, string comment) descriptor)
		{
			FileName = descriptor.file;
			_language = descriptor.language;
			_comment = descriptor.comment;
			_allLines = File.ReadAllLines(FilePath).ToImmutableList();
			Labels = _allLines
				.Skip(GlobalConstants.LngFileHeaderFormat.Length)
				.ToLngLabels()
				.ToImmutableDictionary(l => l.label, l => (l.needTranslation, l.value));
		}

		public bool Validate(IEnumerable<(ImmutableList<string> comments, string label)> masterLabels)
		{
			return _allLines.SequenceEqual(GenerateLines(masterLabels));
		}

		public int GetNeedTranslationCount() => Labels.Count(l => l.Value.needTranslation);

		public (IEnumerable<string> onlyInMaster, IEnumerable<string> onlyInLng) GetDiff(IEnumerable<string> masterLabels)
		{
			return (masterLabels.Except(Labels.Keys).OrderBy(l => l), Labels.Keys.Except(masterLabels).OrderBy(l => l));
		}

		public void WriteBackCorrected(
			IEnumerable<(ImmutableList<string> comments, string label)> masterLabels,
			ImmutableDictionary<string, (bool needTranslation, string value)>? masterValues = default)
		{
			File.WriteAllLines(FilePath, GenerateLines(masterLabels, masterValues), Encoding.UTF8);
		}

		private IEnumerable<string> GenerateLines(
			IEnumerable<(ImmutableList<string> comments, string label)> masterLabels,
			ImmutableDictionary<string, (bool needTranslation, string value)>? masterValues = default)
		{
			return GlobalConstants.LngFileHeaderFormat
				.Select(l => string.Format(l, _language, _comment))
				.Concat(masterLabels.SelectMany(l => l.comments.Concat(GetLabelStrings(l.label, masterValues))));
		}

		private IEnumerable<string> GetLabelStrings(
			string label,
			ImmutableDictionary<string, (bool needTranslation, string value)>? masterValues)
		{
			if (Labels.TryGetValue(label, out var lngValue))
			{
				return GetValueStrings(label, lngValue.needTranslation, lngValue.value);
			}

			if (masterValues?.TryGetValue(label, out var masterValue) ?? false)
			{
				return GetValueStrings(label, true, masterValue.value);
			}

			return GetValueStrings(label, true, $"\"{label}\"");
		}

		private static IEnumerable<string> GetValueStrings(string label, bool needTranslation, string value)
		{
			if (needTranslation) yield return GlobalConstants.NeedTranslation;
			yield return $"{label}={value}";
		}

		public string FileName { get; }
		public string FilePath => Path.Combine(GlobalConstants.FarDir, FileName);
		public ImmutableDictionary<string, (bool needTranslation, string value)> Labels { get; }

		private readonly string _language;
		private readonly string _comment;
		private readonly ImmutableList<string> _allLines;
	}
}
