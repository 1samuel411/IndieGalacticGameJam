using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterServerProj
{
    [Serializable]
    public class Resource
    {

        public string name;
        public List<Symbol> symbol = new List<Symbol>();

    }
}
