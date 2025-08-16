namespace BlazorBlog.Services;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public class SlugService : ISlugService
{
    public string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var normalized = input.Trim().ToLowerInvariant();

        // Remove diacritics
        normalized = RemoveDiacritics(normalized);

        // Replace non-alphanumeric with hyphens
        normalized = Regex.Replace(normalized, "[^a-z0-9]+", "-");

        // Trim hyphens
        normalized = normalized.Trim('-');

        // Collapse multiple hyphens
        normalized = Regex.Replace(normalized, "-+", "-");

        return normalized;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
