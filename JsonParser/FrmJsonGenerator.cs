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
              var json=  ReadFile();
                WriteFile(json);
            }
        }

        private void WriteFile(string jsonString)
        {
            string path= @"D:\Output.josn";
            using (StreamWriter writer = new StreamWriter(path)) {
                writer.Write(jsonString);
            }
        }

        private string ReadFile() {
            StringBuilder json = new StringBuilder("[");
            int rootId = 0;
            int firstChildId = 0;
            int SecondChildId = 0;
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim().StartsWith("!"))
                    {
                        json.Append(MakeRoot(line));
                    }
                    else if (line.Trim().StartsWith("@"))
                    {
                        ++rootId;
                        json.Append(MakeChild(rootId, line));
                    }
                    else if (line.Trim().StartsWith("#"))
                    {
                        ++firstChildId;
                        json.Append(MakeChild(firstChildId, line));
                    }
                    else
                    {
                        ++SecondChildId;
                        json.Append(MakeLeaf(SecondChildId, line));
                    }
                    json.Append(line);
                }

                //fileContent = reader.ReadToEnd();
                return json.ToString();
            }
        }

        private string MakeRoot(string name)
        {
            StringBuilder sb = new StringBuilder("{");
            sb.AppendFormat("\"name\": \"{0}\"", name);
            sb.Append("\"isActive\": true");
            sb.Append("\"parent\": null");
            sb.Append("\"isRoot\": true");
            sb.AppendFormat("\"searchIndex\": \"{0}", name);
            sb.Append("\"children\": [");
            return sb.ToString();
        }
        private string MakeChild(int parentId, string name)
        {
            StringBuilder sb = new StringBuilder("{");
            sb.AppendFormat("\"name\": \"{0}\"", name);
            sb.Append("\"isActive\": true");
            sb.AppendFormat("\"parent\": {0}", parentId);
            sb.Append("\"isRoot\": false");
            sb.AppendFormat("\"searchIndex\": \"{0}", name);
            sb.Append("\"children\": [");
            return sb.ToString();
        }
        private string MakeLeaf(int parentId, string name)
        {
            StringBuilder sb = new StringBuilder("{");
            sb.AppendFormat("\"name\": \"{0}\"", name);
            sb.Append("\"isActive\": true");
            sb.AppendFormat("\"parent\": {0}", parentId);
            sb.Append("\"isRoot\": false");
            sb.AppendFormat("\"searchIndex\": \"{0}", name);
            sb.Append("\"children\": []");
            return sb.ToString();
        }
    }
}

/*
sb.AppendFormat("\"name\": \"Appliances\"{0}", Environment.NewLine);
            sb.AppendFormat("\"isActive\": true{0}", Environment.NewLine);
            sb.AppendFormat("\"parent\": true{0}", Environment.NewLine);
            sb.AppendFormat("\"isRoot\": true{ 0}", Environment.NewLine);
            sb.AppendFormat("\"searchIndex\": \"Appliances{0}", Environment.NewLine);
            sb.AppendFormat("\"children\": [{0}", Environment.NewLine);
            */

