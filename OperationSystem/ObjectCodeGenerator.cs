using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperationSystem
{
    public static class ObjectCodeGenerator
    {
        static List<String> sentences = SourceCode.assemblerCode;
        static List<int> address = new List<int>(sentences.Count);
        static List<String> objectCode = new List<string>(sentences.Count);
        static List<List<string>> code = LexicalAnalysis.codeElements;
        static List<ObjectCode> objectCodes = new List<ObjectCode>(code.Count);
        
        public static Dictionary<string, string> dataRegisters8bit = new Dictionary<string, string> 
        { 
            ["AL"] = "000", 
            ["AH"] = "100", 
            ["BL"] = "011", 
            ["BH"] = "111", 
            ["CL"] = "001", 
            ["CH"] = "101", 
            ["DL"] = "010", 
            ["DH"] = "110" 
        };

        public static Dictionary<string, string> dataRegisters16bit = new Dictionary<string, string>
        {
            ["AX"] = "000",
            ["BX"] = "011",
            ["CX"] = "001",
            ["DX"] = "010"
        };

        public static Dictionary<string, string> segmentRegisters = new Dictionary<string, string>
        {
            ["ES"] = "00",
            ["CS"] = "01",
            ["SS"] = "10",
            ["DS"] = "11"
        };

        static int currentAddress = 0;

        public static void GenerateObjectCode()
        {
            address.Clear();
            objectCode.Clear();
            sentences.RemoveAll(sentence => String.IsNullOrWhiteSpace(sentence));

            for (int i = 0; i < sentences.Count; i++)
            {
                if (code[i][1].ToLower() == "db" || code[i][1].ToLower() == "dw")
                {
                    currentAddress += GenerateObjectCodeForVariable(i);
                    continue;
                } else if (code[i][0].ToLower() == "pop")
                {
                    currentAddress += GenerateObjectCodeForPopCommand(i);
                    continue;
                } else if (code[i][0].ToLower() == "sub")
                {
                    currentAddress += GenerateObjectCodeForSubCommand(i);
                    continue;
                } else if (code[i][0].ToLower() == "add")
                {
                    currentAddress += GenerateObjectCodeForAddCommand(i);
                    continue;
                }
            }
        }

        static string CreateCorrectAddress(int address)
        {
            string strAddress = Convert.ToString(address, 16).ToUpper();

            while (strAddress.Length < 4) {
                strAddress = "0" + strAddress;
            }

            return strAddress;
        }

        static int GenerateObjectCodeForVariable(int i)
        {
            string number = code[i][2].ToUpper();

            if (number[number.Length - 1] == 'H') number = number.Remove(number.Length - 1);
            else number = Convert.ToString(Convert.ToInt32(number), 16);

            if (code[i][1].ToLower() == "db")
            {
                //address.Add(currentAddress);
                //objectCode.Add(number);
                objectCodes.Add(new ObjectCode(i + 1, currentAddress, number, sentences[i]));
                return 1;
            }
            else if (code[i][1].ToLower() == "dw")
            {
                while (number.Length < 4) {
                    number = "0" + number;
                }

                //address.Add(currentAddress);
                //objectCode.Add(number);
                objectCodes.Add(new ObjectCode(i + 1, currentAddress, number, sentences[i]));
                return 2;
            }

            return 0;
        }

        static int GenerateObjectCodeForPopCommand(int i)
        {
            //address.Add(currentAddress);
            //objectCode.Add("hello");
            string sg = "", reg = "", mod = "", rm = "";
            int memory = 0;

            foreach (var register in segmentRegisters)
            {
                if (register.Key == code[i][1])
                {
                    sg = register.Value;
                    objectCodes.Add(new ObjectCode(i + 1, currentAddress, "000" + sg + "111", sentences[i]));
                    memory = 1;
                    break;
                }
            }

            foreach (var register in dataRegisters16bit)
            {
                if (register.Key == code[i][1])
                {
                    reg = register.Value;
                    objectCodes.Add(new ObjectCode(i + 1, currentAddress, "01011" + reg, sentences[i]));
                    memory = 1;
                    break;
                }
            }

            objectCodes.Add(new ObjectCode(i + 1, currentAddress, "000111", sentences[i]));

            return memory;
        }

        static int GenerateObjectCodeForSubCommand(int i)
        {
            address.Add(currentAddress);
            objectCode.Add("hi");

            return 2;
        }

        static int GenerateObjectCodeForAddCommand(int i)
        {
            address.Add(currentAddress);
            objectCode.Add("bye");

            return 3;
        }

        public static void ShowObjectCodeInDataGridView(DataGridView dgvObjectCode)
        {
            dgvObjectCode.Rows.Clear();
            GenerateObjectCode();

            for (int i = 0; i < objectCodes.Count; i++) {
                dgvObjectCode.Rows.Add();

                dgvObjectCode[0, i].Value = objectCodes[i].numStr;
                dgvObjectCode[1, i].Value = CreateCorrectAddress(objectCodes[i].address);
                dgvObjectCode[2, i].Value = objectCodes[i].objCode;
                dgvObjectCode[3, i].Value = LexicalAnalysis.getCleanLine(objectCodes[i].str);
            }
        }
    }
}