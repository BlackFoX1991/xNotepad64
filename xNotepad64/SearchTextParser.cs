using System.Globalization;
using System.Text;

namespace xNotepad64
{
    public static class SearchTextParser
    {
        public static string Unescape(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var builder = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                char current = text[i];
                if (current != '\\')
                {
                    builder.Append(current);
                    continue;
                }

                if (i == text.Length - 1)
                {
                    throw new FormatException("Der Suchbegriff endet mit einem unvollstaendigen Escape.");
                }

                i++;
                char escape = text[i];

                switch (escape)
                {
                    case '\\':
                        builder.Append('\\');
                        break;
                    case '\'':
                        builder.Append('\'');
                        break;
                    case '"':
                        builder.Append('"');
                        break;
                    case '0':
                        builder.Append('\0');
                        break;
                    case 'a':
                        builder.Append('\a');
                        break;
                    case 'b':
                        builder.Append('\b');
                        break;
                    case 'f':
                        builder.Append('\f');
                        break;
                    case 'n':
                        builder.Append('\n');
                        break;
                    case 'r':
                        builder.Append('\r');
                        break;
                    case 't':
                        builder.Append('\t');
                        break;
                    case 'v':
                        builder.Append('\v');
                        break;
                    case 'u':
                        builder.Append(ParseUnicodeEscape(text, ref i, 4, "u"));
                        break;
                    case 'x':
                        builder.Append(ParseVariableHexEscape(text, ref i));
                        break;
                    default:
                        throw new FormatException($"Unbekannte Escape-Sequenz \\{escape}.");
                }
            }

            return builder.ToString();
        }

        private static char ParseUnicodeEscape(string text, ref int index, int hexDigits, string prefix)
        {
            if (index + hexDigits >= text.Length)
            {
                throw new FormatException($"Escape-Sequenz \\{prefix} erwartet {hexDigits} Hex-Ziffern.");
            }

            string hex = text.Substring(index + 1, hexDigits);
            if (!ushort.TryParse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out ushort value))
            {
                throw new FormatException($"Escape-Sequenz \\{prefix}{hex} ist ungueltig.");
            }

            index += hexDigits;
            return (char)value;
        }

        private static char ParseVariableHexEscape(string text, ref int index)
        {
            int start = index + 1;
            int count = 0;

            while (start + count < text.Length && count < 4 && IsHexDigit(text[start + count]))
            {
                count++;
            }

            if (count == 0)
            {
                throw new FormatException("Escape-Sequenz \\x erwartet mindestens eine Hex-Ziffer.");
            }

            string hex = text.Substring(start, count);
            if (!ushort.TryParse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out ushort value))
            {
                throw new FormatException($"Escape-Sequenz \\x{hex} ist ungueltig.");
            }

            index += count;
            return (char)value;
        }

        private static bool IsHexDigit(char value)
        {
            return value is >= '0' and <= '9'
                or >= 'a' and <= 'f'
                or >= 'A' and <= 'F';
        }
    }
}
