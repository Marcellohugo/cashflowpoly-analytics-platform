// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui SourceFileDocumentationTests.
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class SourceFileDocumentationTests
{
    private static readonly HashSet<string> CommentableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cs", ".cshtml", ".js", ".css", ".sql", ".yml", ".yaml", ".ps1", ".csproj", ".conf", ".example", ".http"
    };

    [Fact]
    public void CommentableSourceFiles_MustStartWithFunctionDescription()
    {
        var repositoryRoot = ResolveRepositoryRoot();
        var roots = new[] { "src", "tests", "database", "infra", "scripts", "config" }
            .Select(name => Path.Combine(repositoryRoot, name))
            .Where(Directory.Exists);
        var missing = roots
            .SelectMany(root => Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            .Where(IsCommentableSource)
            .Where(path => File.ReadLines(path).FirstOrDefault() is not { } firstLine ||
                           !firstLine.Contains("Fungsi file:", StringComparison.OrdinalIgnoreCase))
            .Select(path => Path.GetRelativePath(repositoryRoot, path).Replace('\\', '/'))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        Assert.True(missing.Count == 0, $"Source tanpa komentar fungsi: {string.Join(", ", missing)}");
    }

    private static bool IsCommentableSource(string path)
    {
        var normalized = path.Replace('\\', '/');
        if (normalized.Contains("/bin/", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("/obj/", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("/node_modules/", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("/wwwroot/lib/", StringComparison.OrdinalIgnoreCase) ||
            normalized.EndsWith("/wwwroot/css/tailwind.css", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return CommentableExtensions.Contains(Path.GetExtension(path)) ||
               string.Equals(Path.GetFileName(path), "Dockerfile", StringComparison.OrdinalIgnoreCase);
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
