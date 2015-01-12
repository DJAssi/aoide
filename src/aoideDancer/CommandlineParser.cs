using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace aoideDancer
{
    class CommandlineParser
    {

        public static Dictionary<String, String> getDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] rows = Environment.GetCommandLineArgs();
            Regex r1 = new Regex("^(/|--?)([A-Za-z0-9]+)\\s*=\\s*(.*)$");
            foreach (string row in rows) {
                Match m = r1.Match(row);
                if (m.Length > 0) {
                    dict.Add(m.Groups[2].Value,m.Groups[3].Value);
                } 
            }
            return dict;
        }


    }
}
