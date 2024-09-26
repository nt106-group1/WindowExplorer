using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FileExplorer
{
    public partial class SpNotepad : Form
    {
        string filePath = "";
        public SpNotepad()
        {
            InitializeComponent();
        }
        // Constructor filepath
        public SpNotepad(string filePath) : this()
        {
            this.filePath = filePath;
            OpenFile(filePath);
        }

        // Openfile from path
        private void OpenFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    richTextBox1.Text = sr.ReadToEnd(); // Read and show 
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*", ValidateNames = true, Multiselect = false })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        filePath = ofd.FileName;
                        Task<string> text = sr.ReadToEndAsync();
                        richTextBox1.Text = text.Result;
                    }
                }
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*", ValidateNames = true })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName))
                        {
                            sw.WriteLineAsync(richTextBox1.Text);
                        }
                    }
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLineAsync(richTextBox1.Text);
                }
            }

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*", ValidateNames = true })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        sw.WriteLineAsync(richTextBox1.Text);
                    }
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            copyToolStripMenuItem1.Enabled = richTextBox1.SelectedText.Length > 0;
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }
    }
}
