using System.Text;
using System.Text.RegularExpressions;

namespace DALTWNC_QUIZ.Utils
{
    public static class StringUtils
    {
        public static string RemoveAccents(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);

            string regexPattern = @"\p{Mn}";
            text = Regex.Replace(text, regexPattern, string.Empty);

            text = text.Replace("Đ", "D").Replace("đ", "d");

            return text.Normalize(NormalizationForm.FormC);
        }
    }
}