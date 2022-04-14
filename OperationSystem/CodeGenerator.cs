using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperationSystem
{
    public static class CodeGenerator
    {
        static List<String> assemblerCode = SourceCode.assemblerCode;
        static List<List<string>> codeElements = LexicalAnalysis.codeElements;
        static List<ObjectCode> objectCodes = new List<ObjectCode>();

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

        static int currentAddress, num = 0;

        static string CreateCorrectAddress(int address)
        {
            string strAddress = Convert.ToString(address, 16).ToUpper();

            while (strAddress.Length < 4) {
                strAddress = "0" + strAddress;
            }

            return strAddress;
        }

        public static void GenerateObjectCode()
        {
            /*for (int i = 0; i < codeElements.Count; i++) {
                objectCodes.Add(new ObjectCode(i + 1, i * 2, (i + i).ToString(), Math.Pow(i, 2).ToString()));
            }*/
            objectCodes.Clear();
            currentAddress = 0;

            for (int i = 0; i < assemblerCode.Count; i++)
            {
                if (codeElements[i][1].ToLower() == "db" || codeElements[i][1].ToLower() == "dw")
                {
                    currentAddress += GenerateObjectCodeForVariable(i);
                }
                else if (codeElements[i][0].ToLower() == "pop")
                {
                    currentAddress += GenerateObjectCodeForPopCommand(i);
                }
                else if (codeElements[i][0].ToLower() == "sub")
                {
                    //currentAddress += GenerateObjectCodeForSubCommand(i);
                    currentAddress += Gen(i);
                }
                else if (codeElements[i][0].ToLower() == "add")
                {
                    //currentAddress += GenerateObjectCodeForAddCommand(i);
                    currentAddress += Gen(i);
                } else
                {
                    currentAddress += Gen(i);
                }
            }

            //objectCodes.RemoveAll(sentence => sentence.str == "49");
            objectCodes.RemoveAll(sentence => int.TryParse(sentence.str, out num));
        }

        static int Gen(int i)
        {
            objectCodes.Add(new ObjectCode(i + 1, currentAddress, (i + i).ToString(), Math.Pow(i, 2).ToString()));


            return 0;
        }

        static int GenerateObjectCodeForVariable(int i)
        {
            string number = codeElements[i][2].ToUpper();

            if (number[number.Length - 1] == 'H') number = number.Remove(number.Length - 1);
            else number = Convert.ToString(Convert.ToInt32(number), 16);

            if (codeElements[i][1].ToLower() == "db")
            {
                objectCodes.Add(new ObjectCode(i + 1, currentAddress, number, assemblerCode[i]));
                return 1;
            }
            else if (codeElements[i][1].ToLower() == "dw")
            {
                while (number.Length < 4) {
                    number = "0" + number;
                }
                objectCodes.Add(new ObjectCode(i + 1, currentAddress, number, assemblerCode[i]));
                return 2;
            }

            return 0;
        }

        static int GenerateObjectCodeForPopCommand(int i)
        {
            string sg, reg, mod, rm, objectCode_part1, objectCode_part2, objectCode_part3;
            int memory = 0;

            foreach (var register in segmentRegisters)
            {
                if (register.Key == codeElements[i][1])
                {
                    sg = register.Value;
                    objectCode_part1 = FromBinaryToHex("000" + sg + "111");
                    objectCodes.Add(new ObjectCode(i + 1, currentAddress, objectCode_part1, assemblerCode[i]));
                    return 1;
                }
            }

            foreach (var register in dataRegisters16bit)
            {
                if (register.Key == codeElements[i][1])
                {
                    reg = register.Value;
                    objectCode_part1 = FromBinaryToHex("01011" + reg);
                    objectCodes.Add(new ObjectCode(i + 1, currentAddress, objectCode_part1, assemblerCode[i]));
                    return 1;
                }
            }

            for (int j = 0; j < i; j++)
            {
                if (objectCodes[j].str.ToUpper().IndexOf(codeElements[i][1].ToUpper()) > -1)
                {
                    objectCode_part1 = FromBinaryToHex("10001111");
                    objectCode_part2 = FromBinaryToHex("00000110");
                    objectCode_part3 = CreateCorrectAddress(objectCodes[j].address);
                    objectCodes.Add(new ObjectCode(i + 1, currentAddress, objectCode_part1 +  " " + objectCode_part2 + " " + objectCode_part3, assemblerCode[i]));
                    return 4;
                }
            }
            return 0;
        }

        static int GenerateObjectCodeForSubCommand(int i)
        {
            

            return 2;
        }

        static int GenerateObjectCodeForAddCommand(int i)
        {
            

            return 3;
        }

        static string FromBinaryToHex(string Number)
        {
            int DecimalNumber = 0;
            for (int i = 0; i < Number.Length; i++)
            {
                if (Number[Number.Length - i - 1] == '0') continue;
                DecimalNumber += (int)Math.Pow(2, i);
            }

            Number = Convert.ToString(DecimalNumber, 16).ToUpper();

            while (Number.Length < 2)
            {
                Number = "0" + Number;
            }

            return Number;
        }

        public static void ShowObjectCodeInDataGridView(DataGridView dgvObjectCode)
        {
            dgvObjectCode.Rows.Clear();
            GenerateObjectCode();
            int num;

            for (int i = 0; i < objectCodes.Count; i++)
            {
                dgvObjectCode.Rows.Add();

                dgvObjectCode[0, i].Value = objectCodes[i].numStr;
                dgvObjectCode[1, i].Value = CreateCorrectAddress(objectCodes[i].address);
                dgvObjectCode[2, i].Value = objectCodes[i].objCode;
                dgvObjectCode[3, i].Value = LexicalAnalysis.getCleanLine(objectCodes[i].str);
            }
        }
    }
}
