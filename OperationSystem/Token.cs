using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem
{
    public struct Token
    {
        public int numStr;
        public int numToken;
        public string specifierToken;

        public Token(int numStr, int numToken, string specifierToken)
        {
            this.numStr = numStr;
            this.numToken = numToken;
            this.specifierToken = specifierToken;
        }
     }
}
