using System.Text;

namespace PluginInterface
{
    public static class PluginTools
    {
        public static string FormatStringWithClassFields(string sample, object sourceClass)
        {
            var result = new StringBuilder();
            var openBracketPosition = sample.IndexOf('{');

            if (openBracketPosition < 0)
            {
                return sample;
            }

            if (openBracketPosition > 0)
            {
                result.Append(sample.Substring(0, openBracketPosition));
            }

            while (openBracketPosition >= 0)
            {
                var closeBracketPosition = sample.IndexOf('}', openBracketPosition + 1);
                if (closeBracketPosition < 0)
                {
                    result.Append(sample.Substring(openBracketPosition));

                    return result.ToString();
                }

                var propertyName = sample.Substring(openBracketPosition + 1, closeBracketPosition - openBracketPosition - 1);

                object propertyValue = null;
                try
                {
                    propertyValue = sourceClass.GetType()?.GetField(propertyName)?.GetValue(sourceClass);

                    if (propertyValue == null)
                    {
                        propertyValue = sourceClass.GetType()?.GetProperty(propertyName)?.GetValue(sourceClass);
                    }
                }
                catch
                {

                }

                if (propertyValue != null)
                {
                    result.Append(propertyValue.ToString());
                }
                else
                {
                    result.Append("{" + propertyName + "}");
                }

                openBracketPosition = sample.IndexOf('{', closeBracketPosition + 1);

                if (openBracketPosition > 0)
                {
                    result.Append(sample.Substring(closeBracketPosition + 1, openBracketPosition - closeBracketPosition - 1));
                }
                else
                {
                    result.Append(sample.Substring(closeBracketPosition + 1));
                }
            }

            return result.ToString();
        }
    }
}
