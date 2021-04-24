using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace OneSchedule
{
    internal static class Util
    {
        private static readonly Regex WhitespaceRegex = new Regex(@"\s");

        public static string BuildCommandLine(IEnumerable<string> arguments, bool force = false)
        {
            var commandLine = new StringBuilder();
            foreach (var argument in arguments)
            {
                ArgvQuote(commandLine, argument, force);
                commandLine.Append(' ');
            }

            return commandLine.ToString().TrimEnd();
        }

        private static void ArgvQuote(StringBuilder commandLine, string argument, bool force = false)
        {
            // https://web.archive.org/web/20110426111000/https://blogs.msdn.com/b/twistylittlepassagesallalike/archive/2011/04/23/everyone-quotes-arguments-the-wrong-way.aspx

            Debug.Assert(argument != null, nameof(argument) + " != null");

            //
            // Unless we're told otherwise, don't quote unless we actually
            // need to do so --- hopefully avoid problems if programs won't
            // parse quotes properly
            //
            if (!force && !string.IsNullOrEmpty(argument) && !WhitespaceRegex.IsMatch(argument))
            {
                commandLine.Append(argument);
                return;
            }

            commandLine.Append('"');

            for (var index = 0;; ++index)
            {
                var numberBackslashes = 0;

                while (index < argument.Length && argument[index] == '\\')
                {
                    ++index;
                    ++numberBackslashes;
                }

                if (index == argument.Length)
                {
                    //
                    // Escape all backslashes, but let the terminating
                    // double quotation mark we add below be interpreted
                    // as a metacharacter.
                    //
                    commandLine.Append('\\', numberBackslashes * 2);
                    break;
                }
                else if (argument[index] == '"')
                {
                    //
                    // Escape all backslashes and the following
                    // double quotation mark.
                    //
                    commandLine.Append('\\', numberBackslashes * 2 + 1);
                    commandLine.Append(argument[index]);
                }
                else
                {
                    //
                    // Backslashes aren't special here.
                    //
                    commandLine.Append('\\', numberBackslashes);
                    commandLine.Append(argument[index]);
                }
            }

            commandLine.Append('"');
        }
    }
}
