using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem
{
    public static class ErrorChecker
    {
        public static bool errorsNotExist = true;
        public static List<string> listOfErrors = new List<string>();
        static int indexColumn = 0, indexRow = 0;

        public static void cleanErrorChecker()
        {
            errorsNotExist = true;
            listOfErrors.Clear();
        }

        // проверяет состоит ли строка из корректных символов
        public static void checkCorrectCharsInText(List<string> listOfTokens)
        {
            for (int i = 0; i < listOfTokens.Count; i++)
            {
                indexRow = i;
                for (int j = 0; j < listOfTokens[i].Length; j++)
                {
                    indexColumn = j;
                    if (!(IsEnglishLetter(listOfTokens[i][j]) || Char.IsDigit(listOfTokens[i][j]) || listOfTokens[i][j] == ',' || listOfTokens[i][j] == ' '))
                    {
                        PrintErrorCorrectSymbol(i, j, listOfTokens[i][j]);
                    }
                }
            }
            return;
        }

        public static void PrintErrorCorrectSymbol(int row, int column, char symbol)
        {
            errorsNotExist = false;
            listOfErrors.Add($"Error! Uncorrect symbol in ({row + 1}:{column + 1}). This symbol is: {symbol}.");
        }

        public static void PrintErrorUncorrectCommand(int row, string uncorrectCommand)
        {
            errorsNotExist = false;
            listOfErrors.Add($"Error! Uncorrect string in {row + 1} line: \"{uncorrectCommand}\".");
        }

        public static void PrintErrorUncorrectDeclareVariable(int row, string uncorrectCommand)
        {
            errorsNotExist = false;
            listOfErrors.Add($"Error! Uncorrect declare veriable in {row + 1} line: \"{uncorrectCommand}\".");
        }

        public static void PrintErrorUncorrectPopCommand(int row, string uncorrectCommand)
        {
            errorsNotExist = false;
            listOfErrors.Add($"Error! Uncorrect pop command in {row + 1} line: \"{uncorrectCommand}\".");
        }

        public static void PrintErrorUncorrectSubCommand(int row, string uncorrectCommand)
        {
            errorsNotExist = false;
            listOfErrors.Add($"Error! Uncorrect sub command in {row + 1} line: \"{uncorrectCommand}\".");
        }

        public static void PrintErrorUncorrectAddCommand(int row, string uncorrectCommand)
        {
            errorsNotExist = false;
            listOfErrors.Add($"Error! Uncorrect add command in {row + 1} line: \"{uncorrectCommand}\".");
        }

        public static void PrintErrorUncorrectOperation(int row, string uncorrectOp1, string uncorrectOp2)
        {
            errorsNotExist = false;
            listOfErrors.Add($"Error! Uncorrect operations in {row + 1} line: \"{uncorrectOp1}\" and \"{uncorrectOp2}\".");
        }

        static bool IsEnglishLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }
    }
}
