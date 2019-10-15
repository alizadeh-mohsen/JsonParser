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
        StreamReader reader;
        public FrmJsonGenerator()
        {
            InitializeComponent();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                    // filePath = openFileDialog.FileName;
                    fileStream = openFileDialog.OpenFile();
                }
                var json = ReadFile();
                WriteFile(json);
                label1.Text = "Done";
            }
        }

        private void WriteFile(string jsonString)
        {
            string path = @"D:\Output.josn";
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(jsonString);
            }
        }

        private string ReadFile()
        {
            StringBuilder json = new StringBuilder("[");
            int rootId = 0;
            int firstChildId = 0;
            int SecondChildId = 0;
            string currentIcon = "";
            string oldIcon = "!";
            string line;

            using (reader = new StreamReader(fileStream))
            {
                json.AppendLine(MakeRoot(reader.ReadLine()));

                while ((line = reader.ReadLine()) != null)
                {
                    var item = line.Trim();
                    if (item.StartsWith("@") || item.StartsWith("#"))
                    {
                        ++rootId;
                        json.AppendLine(MakeChild(rootId, item));
                    }
                    if (item.StartsWith("$"))
                    {
                        ++SecondChildId;
                        json.AppendLine(MakeLeaf(SecondChildId, item));
                        json.AppendLine("}]");

                    }
                }
                json.AppendLine("}]");
            }
            return json.ToString();
        }

        private string MakeRoot(string name)
        {
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
            var nameTrim = name.TrimStart('@');
            StringBuilder sb = new StringBuilder("{");
            sb.AppendFormat("\"name\": \"{0}\",", nameTrim);
            //sb.Append("\"isActive\": true");
            //sb.AppendFormat("\"parent\": {0},", parentId);
            //sb.AppendLine("\"isRoot\": false,");
            //sb.AppendFormat("\"searchIndex\": \"{0}", nameTrim);
            sb.AppendLine("\"children\": [");
            return sb.ToString();
        }
        private string MakeLeaf(int parentId, string item)
        {
            var nameTrim = item.TrimStart('#');
            StringBuilder sb = new StringBuilder("{");
            sb.AppendFormat("\"name\": \"{0}\",", nameTrim);
            //sb.Append("\"isActive\": true");
            //sb.AppendFormat("\"parent\": {0},", parentId);
            //sb.AppendLine("\"isRoot\": false,");
            //sb.AppendFormat("\"searchIndex\": \"{0}", nameTrim);
            sb.AppendLine("\"isLeaf\": true},");
            string line;
            do
            {
                line = reader.ReadLine();
                if (line.Trim().StartsWith("$"))
                {
                    ++parentId;
                    sb.AppendLine(MakeLeaf(++parentId, line));
                }
            } while (line != null || line.StartsWith("$"));

            return sb.ToString();

        }
    }
}


