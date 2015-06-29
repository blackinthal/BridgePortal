using System;

namespace Domain.Extensions
{
    public static class StringExtensions
    {
        public static string FormatImportErrorMessage(this String message, int line)
        {
            return string.Format("Eroare la linia {0}: {1}",line, message);
        }
    }
}
