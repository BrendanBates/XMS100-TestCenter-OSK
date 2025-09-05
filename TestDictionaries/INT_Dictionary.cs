using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCenter.TestDictionaries
{
    class INT_Dictionary
    {
        public Dictionary<int, Dictionary<string, Action>> intDictionary = new Dictionary<int, Dictionary<string, Action>>();
        public INT_Dictionary(){
        }
    }
}
