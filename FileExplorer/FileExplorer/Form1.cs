using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
namespace FileExplorer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadDrives(); // Tải danh sách ổ đĩa vào TreeView khi form khởi động


            // Thiết lập các cột cho ListView
            listView1.Columns.Add("Name", 200);       // Cột Name
            listView1.Columns.Add("Type", 100);       // Cột Type (File hoặc Folder)
            listView1.Columns.Add("LastModify", 150); // Cột LastModify (Thời gian chỉnh sửa cuối)
        }

        // Tải các ổ đĩa vào TreeView
        private void LoadDrives()
        {
            TreeNode rootNode;
            foreach (var drive in Directory.GetLogicalDrives())
            {
                rootNode = new TreeNode(drive)
                {
                    Tag = drive
                };
                treeView1.Nodes.Add(rootNode);
                LoadDirectories(rootNode); // Tải các thư mục con của ổ đĩa
            }
        }

        // Tải các thư mục con
        private void LoadDirectories(TreeNode node)
        {
            try
            {
                var dirs = Directory.GetDirectories(node.Tag.ToString());
                foreach (var dir in dirs)
                {
                    TreeNode directoryNode = new TreeNode(Path.GetFileName(dir))
                    {
                        Tag = dir
                    };
                    node.Nodes.Add(directoryNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Khi một thư mục được chọn trong TreeView
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string path = e.Node.Tag.ToString();
            LoadFilesAndDirectories(path);
        }

        // Tải các file và thư mục con vào ListView
        private void LoadFilesAndDirectories(string path)
        {
            listView1.Items.Clear();

            try
            {
                // Tải các thư mục con
                var dirs = Directory.GetDirectories(path);
                foreach (var dir in dirs)
                {
                    ListViewItem item = new ListViewItem(Path.GetFileName(dir)); // Cột Name
                    item.SubItems.Add("Folder"); // Cột Type
                    item.SubItems.Add(Directory.GetLastWriteTime(dir).ToString()); // Cột LastModify
                    item.Tag = dir;
                    listView1.Items.Add(item);
                }

                // Tải các file trong thư mục
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    ListViewItem item = new ListViewItem(Path.GetFileName(file)); // Cột Name
                    item.SubItems.Add("File"); // Cột Type
                    item.SubItems.Add(File.GetLastWriteTime(file).ToString()); // Cột LastModify
                    item.Tag = file;
                    listView1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Khi một file được chọn trong ListView (có thể xử lý thêm hành động tại đây)
        // Khi một file được chọn trong ListView
        // Khi một file được chọn trong ListView
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                string selectedPath = selectedItem.Tag.ToString();

                // Kiểm tra nếu là file .txt
                if (Path.GetExtension(selectedPath).ToLower() == ".txt")
                {
                    try
                    {
                        // Mở Notepad và đọc file .txt
                        Process.Start("notepad.exe", selectedPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể mở Notepad: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // Kiểm tra nếu là file .png (hoặc xử lý các loại file khác)
                else if (Path.GetExtension(selectedPath).ToLower() == ".png")
                {
                    try
                    {
                        // Hiển thị ảnh .png trong PictureBox
                        PictureBox pictureBox = new PictureBox();
                        pictureBox.Image = Image.FromFile(selectedPath);
                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox.Dock = DockStyle.Fill;

                        Form imageForm = new Form();
                        imageForm.Text = "Hình ảnh: " + Path.GetFileName(selectedPath);
                        imageForm.Size = new Size(800, 600);
                        imageForm.Controls.Add(pictureBox);
                        imageForm.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể mở file ảnh .png: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}