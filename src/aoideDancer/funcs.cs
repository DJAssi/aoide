using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoideDancer
{
    class funcs
    {

        public static readonly Dictionary<string, Dance> dances = new Dictionary<string, Dance>() { {"BA",new Dance("Bachata",4,0,100)},{"BL",new Dance("Blues",4,14,27)},{"BO",new Dance("Boogie",4,35,60)},{ "CC", new Dance("ChaChaCha",4,25,40) }, {"DF",new Dance("Discofox",4,25,49)},{"FO",new Dance("Foxtrott",4,35,60)},{"FS",new Dance("Freestyle",4,25,49)},{"IF",new Dance("Italofox",4,25,49)},{"JI",new Dance("Jive",4,30,55)},{"LW",new Dance("Langsamer Walzer",3,25,35)},{"ME",new Dance("Merengue",4,0,100)},{"QS",new Dance("Quickstep",4,30,59)},{"RU",new Dance("Rumba",4,20,35)},{"SS",new Dance("Salsa",4,33,65)},{"SA",new Dance("Samba",4,35,65)},{"SF",new Dance("Slowfox",4,25,35)},{"TA",new Dance("Tango",2,40,70)},{"TO",new Dance("Tango-Argentino",4,35,65)},{"WW",new Dance("Wiener Walzer",3,40,65)} };

        public static string format_time(float value) {
            int a1 = (int)Math.Floor(value/60);
            value -= a1*60;
            int a2 = (int)Math.Floor(value);
            return a1+":"+(a2<10?"0":"")+a2;
        }

        public static string dancename(string code)
        {
            if (!dances.ContainsKey(code))
            {
                if (code == "--") return "";
                if (code.Length == 2 && code.ToUpper() == code) return code;
                return null;
            }
            return dances[code].Name;
        }

        public static string dancecode(string name)
        {
            if (name == "") return null;
            if (name == "--") return null;
            foreach (KeyValuePair<string, Dance> a in dances) if (a.Value.Name == name) return a.Key;
            return null;
        }



    }

    public class Dance {
        public String Name;
        public int QTakt = 4;
        public int TpMmin;
        public int TpMmax;

        public Dance(String name2, int QTakte2, int TpMmin2, int TpMmax2)
        {
            this.Name = name2;
            this.QTakt = QTakte2;
            this.TpMmin = TpMmin2;
            this.TpMmax = TpMmax2;
        }
    }
}
