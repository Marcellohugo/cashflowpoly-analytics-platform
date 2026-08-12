// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ApiErrorLocalizationCoverageTests.
using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class ApiErrorLocalizationCoverageTests
{
    private static readonly Regex TranslationKeyRegex = new(@"\[""(?<message>[^""\r\n]+)""\]\s*=", RegexOptions.Compiled);
    private static readonly Regex DirectBuildErrorRegex = new(
        @"ApiErrorHelper\.BuildError\s*\(\s*[^,]+,\s*""[^""\r\n]+""\s*,\s*""(?<message>[^""\r\n]+)",
        RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex ServiceBuildErrorRegex = new(
        @"(?<!ApiErrorHelper\.)\bBuildError\s*\(\s*""[^""\r\n]+""\s*,\s*""(?<message>[^""\r\n]+)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    [Fact]
    public void LiteralApiErrors_MustHaveEnglishTranslations()
    {
        var repositoryRoot = ResolveRepositoryRoot();
        var apiRoot = Path.Combine(repositoryRoot, "src", "Cashflowpoly.Api");
        var helperPath = Path.Combine(apiRoot, "Infrastructure", "ApiErrorHelper.cs");
        var translatedMessages = TranslationKeyRegex
            .Matches(File.ReadAllText(helperPath))
            .Select(match => match.Groups["message"].Value)
            .ToHashSet(StringComparer.Ordinal);

        var literalMessages = Directory
            .EnumerateFiles(apiRoot, "*.cs", SearchOption.AllDirectories)
            .SelectMany(path =>
            {
                var content = File.ReadAllText(path);
                return DirectBuildErrorRegex.Matches(content)
                    .Concat(ServiceBuildErrorRegex.Matches(content))
                    .Select(match => match.Groups["message"].Value);
            })
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var missing = literalMessages
            .Where(message => !translatedMessages.Contains(message))
            .OrderBy(message => message, StringComparer.Ordinal)
            .ToList();

        Assert.True(missing.Count == 0, $"Pesan API tanpa terjemahan EN: {string.Join(" | ", missing)}");
    }

    private static string ResolveRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Root repositori tidak ditemukan.");
    }
}
