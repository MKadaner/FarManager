using System.Collections.Immutable;
using System.Diagnostics;

namespace LngChecker
{
	public class Program
	{
		public static int Main(string[] parameters)
		{
			try
			{
				return ParseParameters(parameters)(new LangHpp(GlobalConstants.LangHppPath));
			}
			catch (ProcessingException e)
			{
				Console.WriteLine(e.Message);
				return 200;
			}
		}

		private static int Validate(LangHpp langHpp)
		{
			var result = 0;
			var masterLng = true;

			foreach (var lngFile in GlobalConstants.Languages.Select(descriptor => new LngFile(descriptor)))
			{
				var needTranslation = lngFile.GetNeedTranslationCount();
				Console.WriteLine($"{lngFile.FileName}: {needTranslation,4} labels need translation.");
				if (masterLng)
				{
					masterLng = false;
					if (needTranslation > 0)
					{
						Console.WriteLine($@"
  All labels in the master language file {lngFile.FileName} must have values.
");
						result++;
					}
				}

				if (lngFile.Validate(langHpp.Labels))
					continue;

				result++;

				Action<string, IImmutableList<string>> printLabels = (string side, IImmutableList<string> labels) =>
				{
					if (labels.Count == 0) return;
					Console.WriteLine($"  Labels only in {side}: {string.Join(", ", labels)}");
				};

				var (onlyInMaster, onlyInLng) = lngFile.GetDiff(langHpp.Labels.Select(l => l.label));

				Console.WriteLine($@"
{lngFile.FileName} does not match master label list.");
				printLabels("master label list", onlyInMaster.ToImmutableList());
				printLabels(lngFile.FileName, onlyInLng.ToImmutableList());

				Console.WriteLine($@"  Run the checker with ""{GlobalConstants.Operation.WriteBackCorrected}"" parameter to see the full diff.
");
			}

			return result;
		}

		private static int WriteBackCorrected(LangHpp langHpp)
		{
			var masterLng = new LngFile(GlobalConstants.Languages[0]);
			if (masterLng.GetNeedTranslationCount() > 0 || !masterLng.Validate(langHpp.Labels))
			{
				Console.WriteLine($"Writing back corrected master language file {masterLng.FileName}.");
				masterLng.WriteBackCorrected(langHpp.Labels);
				Console.WriteLine($"Master language file {masterLng.FileName} needed corrections. Cannot correct other language files.");
				return -1;
			}

			var result = 0;

			foreach (var lngFile in GlobalConstants.Languages.Skip(1).Select(descriptor => new LngFile(descriptor)))
			{
				if (lngFile.Validate(langHpp.Labels))
					continue;

				result--;

				Console.WriteLine($"Writing back corrected language file {lngFile.FileName}.");
				lngFile.WriteBackCorrected(langHpp.Labels, masterLng.Labels);
			}

			return result;
		}

		private delegate int Processor(LangHpp langHpp);

		private static Processor ParseParameters(string[] parameters)
		{
			if (parameters.Length == 1)
			{
				Enum.TryParse<GlobalConstants.Operation>(parameters[0], true, out var operation);
				switch (operation)
				{
					case GlobalConstants.Operation.Validate: return Validate;
					case GlobalConstants.Operation.WriteBackCorrected: return WriteBackCorrected;
				}
			}

			var lngCheckerName = Path.GetFileName(Process.GetCurrentProcess().MainModule?.FileName);
			throw new ProcessingException($@"
Invalid parameters.

{lngCheckerName} validates or tries to correct Far language files.

Synopsys:
    {lngCheckerName} {GlobalConstants.Operation.Validate}
    {lngCheckerName} {GlobalConstants.Operation.WriteBackCorrected}
");
		}
	}

	internal class ProcessingException : Exception
	{
		internal ProcessingException(string message) : base(message)
		{}
	}
}
