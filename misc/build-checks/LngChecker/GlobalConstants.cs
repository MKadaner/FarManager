using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace LngChecker
{
	internal static class GlobalConstants
	{
		public enum Operation { Validate, WriteBackCorrected };

		public static readonly ImmutableList<(string file, string language, string comment)> Languages = new[]
		{
			( "FarEng.lng.m4", "English", "English" ), // Must be the first
			( "FarBel.lng.m4", "Belarusian", "Belarusian (Беларуская)" ),
			( "FarCze.lng.m4", "Czech", "Czech (Čeština)" ),
			( "FarGer.lng.m4", "German", "German (Deutsch)" ),
			( "FarHun.lng.m4", "Hungarian", "Hungarian (Magyar)" ),
			( "FarIta.lng.m4", "Italian", "Italian (Italiano)" ),
			( "FarLit.lng.m4", "Lithuanian", "Lithuanian (Lietuvių)" ),
			( "FarPol.lng.m4", "Polish", "Polish (Polski)" ),
			( "FarRus.lng.m4", "Russian", "Russian (Русский)" ),
			( "FarSky.lng.m4", "Slovak", "Slovak (Slovenčina)" ),
			( "FarSpa.lng.m4", "Spanish", "Spanish (Español)" ),
			( "FarUkr.lng.m4", "Ukrainian", "Ukrainian (Українська)" ),
		}.ToImmutableList();

		public static readonly string FarDir = Path.GetFullPath(Path.Combine(GeneratedConstants.FarManagerRootDir, "far"));
		public static readonly string LangHppPath = Path.Combine(FarDir, "lang.hpp");

		public static readonly Regex LangHppLabelPattern = new(@"^(?<label>M[\w\d]+),$", _RegexOptions);

		public static readonly string[] LngFileHeaderFormat =
		{
			@"m4_include(`farversion.m4')m4_dnl",
			@".Language={0},{1}",
			@"",
			@"// Version: M4_MACRO_GET(FULLVERSION)",
			@"",
		};

		public static readonly Regex LngFileHeaderLanguagePattern = new(@"^\.Language=(?<language>\w+),(?<comment>[\p{L}\s()]+)$", _RegexOptions);
		public static readonly Regex LngFileLabelValuePattern = new(@"^(?<label>M[\w\d]+)=(?<value>"".*"")$", _RegexOptions);

		public const string NeedTranslation = "// need translation:";

		private const RegexOptions _RegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
	}
}
