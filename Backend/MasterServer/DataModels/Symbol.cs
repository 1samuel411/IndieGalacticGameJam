using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterServerProj
{
    [Serializable]
    public class Symbol
    {

        public static char[] characters = { '☋', '☄', '≈', '☆', '☥', '♡', '☽', '☾', '☼', '☙' };

        public int value;
        public string unicodeValue;
    }
}
