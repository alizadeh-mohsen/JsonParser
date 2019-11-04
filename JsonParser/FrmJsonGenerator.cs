using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace JsonParser
{
    public partial class FrmJsonGenerator : Form
    {
        FileStream fileStream;
        string[] CategoryArray;
        readonly StringBuilder json = new StringBuilder();
        int rootId;
        int firstChildId;
        int SecondChildId;
        private string previousSymbol;
        private string SymbolOrder = "!@#$";

        public FrmJsonGenerator()
        {
            InitializeComponent();
        }

        private bool Process()
        {
            json.AppendLine(MakeRoot());
            int i = 1;
            while (i < CategoryArray.Length)
            {
                CloseObjects(i);
                var item = CategoryArray[i].Trim();
                if (string.IsNullOrEmpty(item))
                {
                    i++;
                    continue;
                }
                if (item.StartsWith("@"))
                {
                    ++rootId;
                    json.AppendLine(MakeChild(rootId, CategoryArray[i]));
                    previousSymbol = "@";
                    i++;
                }
                else if (item.StartsWith("#"))
                {
                    ++firstChildId;
                    json.AppendLine(MakeChild(firstChildId, CategoryArray[i]));
                    previousSymbol = "#";
                    i++;
                }
                else if (item.StartsWith("$"))
                {
                    ++SecondChildId;
                    i = MakeLeaf(SecondChildId, i);
                    previousSymbol = "$";
                }
                else
                {
                    textBox1.Text = $"invalid item at line {i} >> {CategoryArray[i]}";
                    return false;
                }
            }

            json.AppendLine("]}]}]}");
            return true;
        }

        private void CloseObjects(int i)
        {
            var currentSymbol = CategoryArray[i].Substring(0, 1);
            var symbolBack = SymbolOrder.IndexOf(currentSymbol, StringComparison.Ordinal) <
            SymbolOrder.IndexOf(previousSymbol, StringComparison.Ordinal);

            var symbolDiff = Math.Abs(SymbolOrder.IndexOf(currentSymbol, StringComparison.Ordinal) -
            SymbolOrder.IndexOf(previousSymbol, StringComparison.Ordinal));
            if (symbolBack && symbolDiff == 1)
            {
                json.Append("},");
            }

            if (symbolBack && symbolDiff == 2)
            {
                json.Append("}]},");
            }
        }

        private string MakeRoot()
        {
            previousSymbol = "!";
            var name = CategoryArray[0];
            var nameTrim = name.TrimStart('!');
            StringBuilder sb = new StringBuilder("{");
            sb.AppendFormat($"\"name\": \"{nameTrim.Trim()}\",");
            //sb.Append("\"isActive\": true,");
            //sb.AppendLine("\"parentId\": null,");
            sb.AppendLine("\"children\": [");
            return sb.ToString();
        }

        private string MakeChild(int parentId, string name)
        {
            var nameTrim = name.TrimStart('@', '#');
            StringBuilder sb = new StringBuilder("{");
            sb.AppendFormat($"\"name\": \"{nameTrim.Trim()}\",");
            //sb.Append("\"isActive\": true,");
            //sb.AppendFormat($"\"parentId\": {parentId},");
            sb.AppendLine("\"children\": [");

            return sb.ToString();
        }

        private int MakeLeaf(int parentId, int i)
        {
            while (i < CategoryArray.Length)
            {
                var item = CategoryArray[i];
                if (string.IsNullOrEmpty(item))
                {
                    i++;
                    continue;
                }
                if (item.Trim().StartsWith("$"))
                {
                    ++parentId;
                    var nameTrim = item.TrimStart(new char[] { '$', '#' });

                    json.Append("{");
                    json.AppendFormat($"\"name\": \"{nameTrim.Trim()}\",");
                    //json.Append("\"isActive\": true,");
                    //json.AppendFormat("\"parentId\": {0},", parentId);

                    json.Append("\"isLeaf\": true},");

                    i++;
                }
                else
                {
                    json.Length--;
                    json.AppendLine("]");
                    break;
                }
            }
            {
                json.Length--;
            }
            return i;
        }


        #region FileOperation
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stStatus.Text = "";
            Clipboard.SetText(json.ToString());
            stStatus.Text = "Copied!";
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                stStatus.Text = "";
                json.Length = 0;
                textBox1.Text = "";

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "D:\\OjWorld\\Shop Category\\Gmarket\\Category";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileStream = openFileDialog.OpenFile() as FileStream;
                    }
                    ReadFile();
                    if (ValidateJson())
                    {
                        if (Process())
                        {
                            textBox1.Text = json.ToString();
                            Clipboard.SetText(json.ToString());
                            stStatus.Text = fileStream.Name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
            }
            finally
            {
                fileStream?.Close();
            }
        }

        private void ReadFile()
        {
            List<String> tempList = new List<string>();
            //var file = @"D:\Category.txt";
            var tempArray = File.ReadAllLines(fileStream.Name);

            int counter = 0;
            foreach (string t in tempArray)
            {
                if (string.IsNullOrEmpty(t.TrimStart())) continue;
                tempList.Add(t.TrimStart());
                counter++;
            }
            CategoryArray = tempList.ToArray();
        }
        #endregion

        private bool ValidateJson()
        {
            for (int i = 0; i < CategoryArray.Length - 1; i++)
            {
                if (CategoryArray[i].StartsWith("#") && CategoryArray[i + 1].StartsWith("#"))
                {
                    textBox1.Text = $"invalid item at line {i} >> {CategoryArray[i]}";
                    return false;
                }
            }
            return true;
        }
    }
}


