using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JsonParser
{
    public partial class FrmJsonGenerator : Form
    {
        Stream fileStream;
        string filePath = @"D:\category.txt";
        StreamReader reader;
        string[] CategoryArray;
        StringBuilder json = new StringBuilder("[");
        int rootId;
        int firstChildId;
        int SecondChildId;
        private string previousSymbol;
        private string closing = "]";
        private string SymbolOrder = "!@#$";

        public FrmJsonGenerator()
        {
            InitializeComponent();
        }
      
        private void Process()
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
                if (item.StartsWith("#"))
                {
                    ++firstChildId;
                    json.AppendLine(MakeChild(firstChildId, CategoryArray[i]));
                    previousSymbol = "#";
                    i++;
                }
                if (item.StartsWith("$"))
                {
                    ++SecondChildId;
                    i = MakeLeaf(SecondChildId, i);
                    previousSymbol = "$";
                }
            }

            CloseJson();
        }

        private void CloseJson()
        {
            json.AppendLine("]");
            foreach (var item in CategoryArray)
            {
                if (item.TrimStart().StartsWith("@"))
                {
                    json.AppendLine("}]");
                }
            }
            json.AppendLine("}]}");
        }

        private void CloseObjects(int i)
        {
            
            string currentSymbol = CategoryArray[i].Substring(0, 1);
            if (SymbolOrder.IndexOf(currentSymbol, StringComparison.Ordinal) -
                SymbolOrder.IndexOf(previousSymbol, StringComparison.Ordinal)==1)
            {
                json.Append("},");
            }

            if (SymbolOrder.IndexOf(currentSymbol, StringComparison.Ordinal) -
                SymbolOrder.IndexOf(previousSymbol, StringComparison.Ordinal)==2)
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
            sb.AppendFormat("\"name\": \"{0}\",", nameTrim);

            //sb.Append("\"isActive\": true");
            //sb.AppendLine("\"parent\": null,");
            //sb.AppendLine("\"isRoot\": true,");
            //sb.AppendFormat("\"searchIndex\": \"{0}", nameTrim);
            sb.AppendLine("\"children\": [");
            return sb.ToString();
        }

        private string MakeChild(int parentId, string name)
        {
            var nameTrim = name.TrimStart(new char[] { '@', '#' });
            StringBuilder sb = new StringBuilder("{");
            sb.AppendFormat("\"name\": \"{0}\",", nameTrim);
            //sb.Append("\"isActive\": true");
            //sb.AppendFormat("\"parent\": {0},", parentId);
            //sb.AppendLine("\"isRoot\": false,");
            //sb.AppendFormat("\"searchIndex\": \"{0}", nameTrim);
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
                    var nameTrim = item.TrimStart('#');
                    json.Append("{");
                    json.AppendFormat("\"name\": \"{0}\"", nameTrim);
                    //sb.Append("\"isActive\": true");
                    //sb.AppendFormat("\"parent\": {0},", parentId);
                    //sb.AppendLine("\"isRoot\": false,");
                    //sb.AppendFormat("\"searchIndex\": \"{0}", nameTrim);
                    //json.Append("\"isLeaf\": true},");
                    json.Append("},");
                    i++;
                }
                else
                {
                    AddClsoing();
                    break;
                }
            }
            {
                json.Length--;
            }
            return i;
        }
        private void AddClsoing()
        {
            json.Length--;
            json.AppendLine(closing);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(json.ToString());
        }

        #region FileOperation

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            json.Length = 0;
            textBox1.Text = "";
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "D:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    fileStream = openFileDialog.OpenFile();
                }
                ReadFile();
                Process();
                WriteFile(json.ToString());
                textBox1.Text = json.ToString();
            }
        }

        private void ReadFile()
        {
            string[] tempArray;
            List<String> tempList = new List<string>();
            using (reader = new StreamReader(fileStream))
            {
                tempArray = File.ReadAllLines(filePath);

            }
            int counter = 0;
            foreach (string t in tempArray)
            {
                if (string.IsNullOrEmpty(t.TrimStart())) continue;
                tempList.Add(t.TrimStart());
                counter++;
            }
            CategoryArray = tempList.ToArray();
        }

        private void WriteFile(string jsonString)
        {
            string path = @"D:\Output.josn";
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(jsonString);
            }
        }

        #endregion
    }
}


