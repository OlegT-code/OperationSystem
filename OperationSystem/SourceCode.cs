using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OperationSystem
{
    public class SourceCode
    {
        public static List<String> assemblerCode = new List<string>();

        // загружает код из файла в List
        public static void UploadSourceCodeFromFileToList(string filename)
        {
            assemblerCode.Clear();

            string line;

            using (StreamReader reader = new StreamReader(filename))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    assemblerCode.Add(line);
                }
            }
            SaveLastFileCode(filename);

            return;
        }

        // показывает исходный код
        public static void ShowSourceCode(RichTextBox textBox)
        {
            textBox.Text = String.Empty;

            for (int i = 0; i < assemblerCode.Count(); i++)
            {
                textBox.Text += assemblerCode[i];

                if (i != assemblerCode.Count - 1)
                    textBox.Text += '\n';
            }
        }

        // сохраняет последний открытый файл
        public static void SaveLastFileCode(string filename)
        {
            using (StreamWriter writer = new StreamWriter("filepath.txt", false))
            {
                writer.WriteLine(filename);
            }
        }

        // загружает последний открытый файл
        public static string LoadLastFileCode()
        {
            string text;
            using (StreamReader reader = new StreamReader("filepath.txt"))
            {
                text = reader.ReadLine();
            }

            return text;
        }

        // сохраняет текущий код в TextBox
        public static void SaveCurrentSourceCode(RichTextBox textBox)
        {     
            assemblerCode.Clear();

            foreach (string str in textBox.Lines)
            {
                assemblerCode.Add(str);
            }

            using (StreamWriter writer = new StreamWriter(LoadLastFileCode(), false))
            {
                for (int i = 0; i < assemblerCode.Count; i++)
                {
                    writer.WriteLine(assemblerCode[i]);
                }
            }
        }

        // выдаёт текущий assemblerCode
        public static List<string> getCurrentAssemblCode()
        {
            return assemblerCode;
        }

        public static string GetStringOfCode(int index)
        {
            return assemblerCode[index];
        }
    }
}