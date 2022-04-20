using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OperationSystem
{
    public static class LexicalAnalysis
    {
        public static List<List<string>> codeElements = new List<List<string>>(3);
        static string line, pattern = @"\s+", patternForKoma = @"\s*,\s*";

        static string[] dataRegisters8bit = new string[] { "AL", "AH", "BL", "BH", "CL", "CH", "DL", "DH" };
        static string[] dataRegisters16bit = new string[] { "AX", "BX", "CX", "DX" };
        static string[] segmentRegisters = { "DS", "SS", "ES" };
        static string[] keywords = { "db", "dw", "add", "sub", "pop" };

        static List<Token> tokens = new List<Token>();

        static List<string> dbVariables = new List<string>();
        static List<string> dwVariables = new List<string>();

        public static void GenerateListOfcodeElements()
        {
            List<String> sentences = SourceCode.assemblerCode;
            codeElements.Clear();

            for (int i = 0; i < sentences.Count; i++)
            {
                line = getCleanLine(sentences[i]);

                List<string> lineOfcodeElements = line.Split(' ').ToList<string>();
                codeElements.Add(lineOfcodeElements);
            }
        }

        public static void GenerateListOfTokens()
        {
            for (int i = 0; i < codeElements.Count; i++)
            {
                while (codeElements[i].Count < 5)
                {
                    codeElements[i].Add("");
                }
            }
            tokens.Clear();
            dbVariables.Clear();
            dwVariables.Clear();

            bool alwaysCorrect = true;

            for (int i = 0; i < codeElements.Count; i++)
            {
                if (alwaysCorrect) {
                    if (codeElements[i][1].ToLower() == "db" || codeElements[i][1].ToLower() == "dw")
                    {
                        alwaysCorrect = AnalyzeDeclareVariable(codeElements[i], i);
                    }
                    else if (codeElements[i][0].ToLower() == "pop")
                    {
                        alwaysCorrect = AnalyzePopCommand(codeElements[i], i);
                    }
                    else if (codeElements[i][0].ToLower() == "sub")
                    {
                        alwaysCorrect = AnalyzeSubCommand(codeElements[i], i);
                    }
                    else if (codeElements[i][0].ToLower() == "add")
                    {
                        alwaysCorrect = AnalyzeAddCommand(codeElements[i], i);
                    }
                    else if (codeElements[i][0].ToLower() == "")
                    {
                        alwaysCorrect = true;
                    }
                    else
                    {
                        ErrorChecker.PrintErrorUncorrectCommand(i, SourceCode.GetStringOfCode(i));
                        return;
                    }
                } else
                {
                    return;
                }
            }
        }

        public static void ShowTokensInDataGridView(DataGridView dgvTokens)
        {
            dgvTokens.Rows.Clear();

            for (int i = 0; i < tokens.Count; i++)
            {
                dgvTokens.Rows.Add();

                if (!(i != 0 && tokens[i - 1].numStr == tokens[i].numStr)) {
                    dgvTokens[0, i].Value = tokens[i].numStr;
                }
                dgvTokens[1, i].Value = tokens[i].numToken;
                dgvTokens[2, i].Value = tokens[i].specifierToken;
            }
        }

        static bool AnalyzeDeclareVariable(List<string> command, int index)
        {
            Token tokenVariable;
            Token tokenDataType;
            Token tokenDataValue;

            command[0] = command[0].ToUpper();
            command[1] = command[1].ToUpper();
            command[2] = command[2].ToUpper();

            if (IsVariable(command[0]) && !(dwVariables.Any(s => s == command[0]) || dbVariables.Any(s => s == command[0])) && !keywords.Contains(command[0].ToLower()))
            {
                tokenVariable = new Token(index + 1, 12, command[0]);
            } else
            {
                ErrorChecker.PrintErrorUncorrectDeclareVariable(index, command[0]);
                return false;
            }

            if (command[1].ToLower() == "dw")
            {
                tokenDataType = new Token(index + 1, 3, "");

            }
            else if (command[1].ToLower() == "db")
            {
                tokenDataType = new Token(index + 1, 2, "");
            }
            else
            {
                ErrorChecker.PrintErrorUncorrectDeclareVariable(index, command[1]);
                return false;
            }

            if (IsNumber(command[2]) == 1 && command[1].ToLower() == "db")
            {
                tokenDataValue = new Token(index + 1, 11, command[2]);
                dbVariables.Add(command[0]);
            }
            else if ((IsNumber(command[2]) == 2 || IsNumber(command[2]) == 1) && command[1].ToLower() == "dw")
            {
                tokenDataValue = new Token(index + 1, 11, command[2]);
                dwVariables.Add(command[0]);
            }
            else
            {
                ErrorChecker.PrintErrorUncorrectDeclareVariable(index, command[2]);
                return false;
            }

            if (!String.IsNullOrEmpty(command[3]))
            {
                ErrorChecker.PrintErrorUncorrectDeclareVariable(index, command[3]);
                return false;
            }

            tokens.Add(tokenVariable);
            tokens.Add(tokenDataType);
            tokens.Add(tokenDataValue);

            return true;
        }

        static bool AnalyzePopCommand(List<string> command, int index)
        {
            Token tokenPop;
            Token tokenDataType;
            command[1] = command[1].ToUpper();

            tokenPop = new Token(index + 1, 6, "");

            if (dataRegisters16bit.Contains(command[1]))
            {
                tokenDataType = new Token(index + 1, 8, command[1]);
            }
            else if (segmentRegisters.Contains(command[1]))
            {
                tokenDataType = new Token(index + 1, 9, command[1]);
            }
            else if (dwVariables.Contains(command[1]))
            {
                tokenDataType = new Token(index + 1, 12, command[1]);
            }
            else
            {
                ErrorChecker.PrintErrorUncorrectPopCommand(index, command[1]);
                return false;
            }

            if (!String.IsNullOrEmpty(command[2]))
            {
                ErrorChecker.PrintErrorUncorrectPopCommand(index, command[2]);
                return false;
            }

            tokens.Add(tokenPop);
            tokens.Add(tokenDataType);

            return true;
        }

        static bool AnalyzeSubCommand(List<string> command, int index)
        {
            Token tokenSub;
            Token tokenOp1;
            Token tokenComma;
            Token tokenOp2;

            command[0] = command[0].ToUpper();
            command[1] = command[1].ToUpper();
            command[2] = command[2].ToUpper();
            command[3] = command[3].ToUpper();

            tokenSub = new Token(index + 1, 5, "");

            if (dataRegisters16bit.Contains(command[1]))
            {
                tokenOp1 = new Token(index + 1, 8, command[1]);
            }
            else if (dataRegisters8bit.Contains(command[1]))
            {
                tokenOp1 = new Token(index + 1, 7, command[1]);
            }
            else if (dwVariables.Contains(command[1]) || dbVariables.Contains(command[1]))
            {
                tokenOp1 = new Token(index + 1, 12, command[1]);
            }
            else
            {
                ErrorChecker.PrintErrorUncorrectSubCommand(index, command[1]);
                return false;
            }

            if (command[2] == ",")
            {
                tokenComma = new Token(index + 1, 1, "");
            }
            else
            {
                ErrorChecker.PrintErrorUncorrectSubCommand(index, command[2]);
                return false;
            }

            if (dataRegisters16bit.Contains(command[3]))
            {
                tokenOp2 = new Token(index + 1, 8, command[3]);
            }
            else if (dataRegisters8bit.Contains(command[3]))
            {
                tokenOp2 = new Token(index + 1, 3, command[3]);
            }
            else if (dwVariables.Contains(command[3]) || dbVariables.Contains(command[3]))
            {
                tokenOp2 = new Token(index + 1, 12, command[3]);
            }
            else if (IsNumber(command[3]) != 0)
            {
                tokenOp2 = new Token(index + 1, 11, command[3]);
            }
            else
            {
                ErrorChecker.PrintErrorUncorrectSubCommand(index, command[3]);
                return false;
            }

            if (!String.IsNullOrEmpty(command[4]))
            {
                ErrorChecker.PrintErrorUncorrectSubCommand(index, command[4]);
                return false;
            }

            if (!CheckPossibilityOperation(command[1].ToUpper(), command[3]))
            {
                ErrorChecker.PrintErrorUncorrectOperation(index, command[1], command[3]);
                return false;
            }

            tokens.Add(tokenSub);
            tokens.Add(tokenOp1);
            tokens.Add(tokenComma);
            tokens.Add(tokenOp2);

            return true;
        }

        static bool AnalyzeAddCommand(List<string> command, int index)
        {
            Token tokenAdd;
            Token tokenOp1;
            Token tokenComma;
            Token tokenOp2;

            command[0] = command[0].ToUpper();
            command[1] = command[1].ToUpper();
            command[2] = command[2].ToUpper();
            command[3] = command[3].ToUpper();

            tokenAdd = new Token(index + 1, 4, "");

            if (dataRegisters16bit.Contains(command[1]))
            {
                tokenOp1 = new Token(index + 1, 8, command[1]);
            }
            else if (dataRegisters8bit.Contains(command[1]))
            {
                tokenOp1 = new Token(index + 1, 7, command[1]);
            }
            else if (dwVariables.Contains(command[1]) || dbVariables.Contains(command[1]))
            {
                tokenOp1 = new Token(index + 1, 12, command[1]);
            }
            else
            {
                ErrorChecker.PrintErrorUncorrectAddCommand(index, command[1]);
                return false;
            }

            if (command[2] == ",")
            {
                tokenComma = new Token(index + 1, 1, "");
            }
            else
            {
                ErrorChecker.PrintErrorUncorrectAddCommand(index, command[2]);
                return false;
            }

            if (dataRegisters16bit.Contains(command[3]))
            {
                tokenOp2 = new Token(index + 1, 8, command[3]);
            }
            else if (dataRegisters8bit.Contains(command[3]))
            {
                tokenOp2 = new Token(index + 1, 3, command[3]);
            }
            else if (dwVariables.Contains(command[3]) || dbVariables.Contains(command[3]))
            {
                tokenOp2 = new Token(index + 1, 12, command[3]);
            }
            else if (IsNumber(command[3]) != 0)
            {
                tokenOp2 = new Token(index + 1, 11, command[3]);
            }
            else
            {
                ErrorChecker.PrintErrorUncorrectAddCommand(index, command[3]);
                return false;
            }

            if (!String.IsNullOrEmpty(command[4]))
            {
                ErrorChecker.PrintErrorUncorrectAddCommand(index, command[4]);
                return false;
            }

            if (!CheckPossibilityOperation(command[1], command[3]))
            {
                ErrorChecker.PrintErrorUncorrectOperation(index, command[1], command[3]);
                return false;
            }

            tokens.Add(tokenAdd);
            tokens.Add(tokenOp1);
            tokens.Add(tokenComma);
            tokens.Add(tokenOp2);

            return true;
        }

        public static bool IsVariable(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[0];
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                    return false;
            }

            return true;
        }

        public static int IsNumber(string line)
        {
            string[] hexNumberSymbols = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
            bool isNormalNum = true;
            int num = 0;

            for (int i = 0; i < line.Length - 1; i++)
            {
                isNormalNum = hexNumberSymbols.Contains(line[i].ToString().ToLower()) ? true : false;
            }

            if (line[line.Length - 1].ToString().ToLower() == "h" && isNormalNum && Char.IsNumber(line[0]))
            {
                line = line.Remove(line.Length - 1);
                num = Convert.ToInt32(line, 16);
            }
            else if (line.All(s => Char.IsNumber(s)))
            {
                num = Convert.ToInt32(line);
            }
            else
            {
                return 0;
            }

            if (num >= 0 && num < 256)
            {
                return 1;
            }
            else if (num < 65356)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        static bool CheckPossibilityOperation(string op1, string op2)
        {
            int numOp1 = 0, numOp2 = 0;

            if (dataRegisters8bit.Any(r => r == op1) || IsNumber(op1) == 1 || dbVariables.Any(r => r == op1))
            {
                numOp1 = 1;
            }
            else if (dataRegisters16bit.Any(r => r == op1) || dwVariables.Any(r => r == op1))
            {
                numOp1 = 2;
            }

            if (dataRegisters8bit.Any(r => r == op2) || IsNumber(op2) == 1 || dbVariables.Any(r => r == op2))
            {
                numOp2 = 1;
            }
            else if (dataRegisters16bit.Any(r => r == op2) || IsNumber(op2) == 1 || IsNumber(op2) == 2 || dwVariables.Any(r => r == op2))
            {
                numOp2 = 2;
            }

            return numOp1 == numOp2 || IsNumber(op2) == 1 ? true : false;
        }

        public static void SaveCompilerCodeToFile()
        {
            using (StreamWriter writer = new StreamWriter("compiler_code.txt", false))
            {
                for (int i = 0; i < tokens.Count; i++)
                {
                    writer.Write(tokens[i].numStr + "\t");
                    writer.Write(tokens[i].numToken + "\t");
                    writer.Write(tokens[i].specifierToken);
                    writer.Write("\n");
                }
            }
        }

        #region
        // выдаёт строку удобную для лексического анализа
        public static string getCleanLine(string line)
        {
            Regex regex = new Regex(pattern);

            line = line.Trim();
            line = regex.Replace(line, " ");

            regex = new Regex(patternForKoma);
            line = regex.Replace(line, " , ");

            return line;
        }

        // выдаёт строку токенов
        public static void GetSentence(int index, RichTextBox textBox)
        {
            for (int i = 0; i < codeElements[index].Count; i++)
            {
                textBox.Text += codeElements[index][i];
                textBox.Text += "\n";
            }
        }

        // показывает все токены
        public static void ShowAllcodeElements(RichTextBox textBox)
        {
            for (int i = 0; i < codeElements.Count; i++)
            {
                for (int j = 0; j < codeElements[i].Count; j++)
                {
                    textBox.Text += codeElements[i][j];
                    textBox.Text += "  ";
                }

                textBox.Text += "\n";
            }
        }

        #endregion
    }
}