using System.Collections.Immutable;

namespace LngChecker
{
	internal class LangHpp
	{
		public LangHpp(string path)
		{
			try
			{
				_path = path;
				_labelLines = File.ReadLines(_path)
					.SkipWhile(l => l != "{")
					.Skip(1)
					.TakeWhile(l => l != "};")
					.ToImmutableList();
				Labels = _labelLines
					.UnIndent()
					.ToLangHppLabels()
					.ToImmutableList();

				Validate();
			}
			catch (ProcessingException e)
			{
				throw new ProcessingException($@"
C++ header file {Path.GetFileName(_path)} is malformed.
  {e.Message}
  Cannot continue.");
			}
		}

		private void Validate()
		{
			if (!_labelLines.SequenceEqual(Generate()))
			{
				throw new ProcessingException("Validation failed");
			}
		}

		private IEnumerable<string> Generate() =>
			Labels.SelectMany(l => l.comments.Append($"{l.label},")).Indent();

		public ImmutableList<(ImmutableList<string> comments, string label)> Labels { get; }

		private readonly string _path;
		private readonly ImmutableList<string> _labelLines;
	}
}
