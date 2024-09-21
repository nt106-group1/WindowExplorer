using System;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadTreeView();
        }

        private void LoadTreeView()
        {
            treeView1.Nodes.Add("Máy tính");
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                TreeNode node = new TreeNode(drive.Name);
                node.Tag = drive.RootDirectory.FullName;
                treeView1.Nodes[0].Nodes.Add(node);
            }
            treeView1.ExpandAll();
        }

 
        private void LoadListView(string path)
        {
            listView1.Items.Clear();
            listView1.Columns.Clear();

            listView1.Columns.Add("Tên", 200);
            listView1.Columns.Add("Kích thước", 100);
            listView1.Columns.Add("Ngày sửa đổi", 150);

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                ListViewItem item = new ListViewItem(file.Name);
                item.SubItems.Add(file.Length.ToString());
                item.SubItems.Add(file.LastWriteTime.ToString());
                listView1.Items.Add(item);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string selectedPath = e.Node.Tag.ToString();
            LoadListView(selectedPath);
        }
    }
   
}
