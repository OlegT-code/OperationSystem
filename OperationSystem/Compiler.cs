using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperationSystem
{
    public static class Compiler
    {

        public static void Compile(RichTextBox tbSourceCode, RichTextBox tbResult, DataGridView dgvTokens)
        {
            ErrorChecker.cleanErrorChecker();
            SourceCode.SaveCurrentSourceCode(tbSourceCode);
            ErrorChecker.checkCorrectCharsInText(SourceCode.assemblerCode);

            if (ErrorChecker.listOfErrors.Count > 0)
            {
                for (int i = 0; i < ErrorChecker.listOfErrors.Count; i++)
                {
                    tbResult.Text += ErrorChecker.listOfErrors[i];
                    tbResult.Text += "";
                }
                return;
            }

            LexicalAnalysis.GenerateListOfcodeElements();

            LexicalAnalysis.GenerateListOfTokens();

            if (ErrorChecker.listOfErrors.Count > 0)
            {
                dgvTokens.Rows.Clear();
                for (int i = 0; i < ErrorChecker.listOfErrors.Count; i++)
                {
                    tbResult.Text += ErrorChecker.listOfErrors[i];
                    tbResult.Text += "";
                }
                return;
            }

            LexicalAnalysis.ShowTokensInDataGridView(dgvTokens);

            LexicalAnalysis.SaveCompilerCodeToFile();
        }
    }
}
