using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem
{
    public struct ObjectCode
    {
        public int numStr;
        public int address;
        public string objCode;
        public string str;

        public ObjectCode(int numStr, int address, string objCode, string str)
        {
            this.numStr = numStr;
            this.address = address;
            this.objCode = objCode;
            this.str = str;
        }
    }
}
