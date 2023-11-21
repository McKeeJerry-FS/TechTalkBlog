using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace TechTalkBlog.Helpers
{
    public static class StringHelper
    {
        public static string BlogPostSlug(string? title)
        {
            string? output = RemoveAccents(title);

            //Remove special characters
            output = Regex.Replace(output!, @"[^A-Za-z0-9\s-]", "");
            // Remove additional spaces (more than single spaces)
            output = Regex.Replace(output, @"\s+", " ");
            output = output.Trim();
            // Replace spaces with hyphens
            output = Regex.Replace(output, @"\s", "-");
            // Lowercase
            output = output.ToLower();

            return output;
        }

        private static string? RemoveAccents(string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return title;
            }

            // Convert to Unicode
            title = title.Normalize(NormalizationForm.FormD);
            //Format Unicode in ascii
            char[] chars = title.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            // Convert back return the new title/slug
            return new string(chars).Normalize(NormalizationForm.FormC);

        }
    }
}
