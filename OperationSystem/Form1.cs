using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperationSystem
{
    public partial class Form1 : Form
    {
        string filename;

        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SourceCode.UploadSourceCodeFromFileToList(SourceCode.LoadLastFileCode());
            SourceCode.ShowSourceCode(tbSourceCode);
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            filename = openFileDialog1.FileName;

            SourceCode.UploadSourceCodeFromFileToList(filename);
            SourceCode.ShowSourceCode(tbSourceCode);
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SourceCode.SaveCurrentSourceCode(tbSourceCode);
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            tbResult.Clear();
            CompileCode();
        }

        public void CompileCode()
        {
            Compiler.Compile(tbSourceCode, tbResult, dgvTokens);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ErrorChecker.errorsNotExist)
            {
                SourceCode.SaveCurrentSourceCode(tbSourceCode);
            } else
            {
                //MessageBox.Show("В вашем коде есть ошибки!", "OK");
                //e.Cancel = true;
            }
        }

        private void saveFileAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string path = saveFileDialog1.FileName;
            System.IO.File.WriteAllText(path, tbSourceCode.Text);
        }
    }
}