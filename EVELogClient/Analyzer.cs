using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace EVELogServer
{
    class Analyzer
    {
        public static readonly string REGEX_SYSTEM_BARE = "( )[a-zA-Z0-9]{1,4}-[a-zA-Z0-9]{1,4}";
        public static readonly string REGEX_SYSTEM_FQN = "( )[a-zA-Z0-9]{1,4}-[a-zA-Z0-9]{1,4}( )Solar System";
        public static readonly string REGEX_CHARACTER = "[a-zA-Z0-9-_]{0,24}( )?[a-zA-Z0-9-_]{0,24}(( )( ))?";

        public static void parse(string message)
        {
            Match bare = System.Text.RegularExpressions.Regex.Match(message, REGEX_SYSTEM_BARE);
            Match fqn = System.Text.RegularExpressions.Regex.Match(message, REGEX_SYSTEM_FQN);

            //if we cant pin this to a system, its useless
            if (bare.Success || fqn.Success)
            {
                string system;
                if (fqn.Success)
                {
                    system = fqn.Value;
                }
                else
                {
                    system = bare.Value;
                }

                string next = message.Replace(system, "");

            }
        }
    }
}
