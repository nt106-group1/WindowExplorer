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
            listView1.MouseDoubleClick += new MouseEventHandler(listView1_MouseDoubleClick);
        }
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                string selectedPath = selectedItem.Tag.ToString();

                // Nếu mục được nhấn là một thư mục
                if (Directory.Exists(selectedPath))
                {
                    // Tải các file và thư mục con của thư mục được nhấn vào ListView
                    LoadFilesAndDirectories(selectedPath);
                }
                else if (File.Exists(selectedPath))
                {
                    // Nếu mục được nhấn là một file .txt
                    if (Path.GetExtension(selectedPath).ToLower() == ".txt")
                    {
                        try
                        {
                            Process.Start("notepad.exe", selectedPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Không thể mở Notepad: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
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
        private Stack<string> history = new Stack<string>(); // Lịch sử các thư mục đã mở
        private Stack<string> forwardHistory = new Stack<string>(); // Lịch sử các thư mục quay lại
        private Stack<string> backStack = new Stack<string>(); // Lịch sử quay lại
        private Stack<string> forwardStack = new Stack<string>(); // Lịch sử tiến lên

        // Tải các file và thư mục con vào ListView
        private void LoadFilesAndDirectories(string path)
        {
            /* listView1.Items.Clear();

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
             }*/
            // Lưu đường dẫn hiện tại vào lịch sử
            // Lưu đường dẫn hiện tại vào lịch sử
            if (history.Count == 0 || history.Peek() != path)
            {
                history.Push(path);
                forwardHistory.Clear(); // Xóa lịch sử quay lại mỗi khi mở thư mục mới
            }

            listView1.Items.Clear();

            // Cập nhật Stack cho back
            if (backStack.Count == 0 || backStack.Peek() != path) // Chỉ thêm nếu không phải là thư mục đã mở
            {
                backStack.Push(path);
            }
            forwardStack.Clear(); // Xóa forwardStack khi mở một thư mục mới

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
        private string sourcePath;  // To store the path of the file or folder to be copied
        private string destinationPath;  // To store the destination where the file or folder will be pasted
        private bool isCutAction = false;
        // Helper method to copy a folder and its contents
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }


        private void copyToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                sourcePath = selectedItem.Tag.ToString(); // Set the source path
            }
            else
            {
                MessageBox.Show("Please select a file or folder to copy.");
            }
        }

        private void pasteToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            /*
            if (!string.IsNullOrEmpty(sourcePath))
            {
                // Kiểm tra thư mục đích
                if (listView1.SelectedItems.Count > 0)
                {
                    var selectedItem = listView1.SelectedItems[0];
                    destinationPath = selectedItem.Tag.ToString();
                }
                else if (treeView1.SelectedNode != null)
                {
                    destinationPath = treeView1.SelectedNode.Tag.ToString();
                }

                if (!string.IsNullOrEmpty(destinationPath))
                {
                    // Kiểm tra nếu thư mục đích đã tồn tại
                    if (Directory.Exists(destinationPath) || File.Exists(destinationPath))
                    {
                        // Kiểm tra nếu thư mục nguồn và đích không phải là thư mục con của nhau
                        if (destinationPath.StartsWith(sourcePath, StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("Không thể di chuyển một tệp vào chính nó hoặc vào thư mục con của nó.");
                            return;
                        }

                        // Kiểm tra nếu tệp đã tồn tại trong thư mục đích
                        if (File.Exists(Path.Combine(destinationPath, Path.GetFileName(sourcePath))))
                        {
                            MessageBox.Show("Tệp đã tồn tại trong thư mục đích. Bạn có muốn ghi đè không?");
                            // Nếu cần, có thể thêm logic để ghi đè tệp nếu người dùng đồng ý
                        }

                        // Thực hiện di chuyển hoặc sao chép
                        try
                        {
                            if (File.Exists(sourcePath))
                            {
                                string fileName = Path.GetFileName(sourcePath);
                                string targetFilePath = Path.Combine(destinationPath, fileName);

                                if (isCutAction)
                                {
                                    if (!File.Exists(targetFilePath))
                                    {
                                        File.Move(sourcePath, targetFilePath);
                                        MessageBox.Show("Tệp đã được di chuyển thành công!");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Tệp đã tồn tại trong đích.");
                                    }
                                }
                                else
                                {
                                    File.Copy(sourcePath, targetFilePath, true);
                                    MessageBox.Show("Tệp đã được sao chép thành công!");
                                }
                            }

                            LoadFilesAndDirectories(destinationPath); // Refresh ListView to display the new contents
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi sao chép/cắt: " + ex.Message);
                        }
                        finally
                        {
                            if (isCutAction)
                            {
                                sourcePath = null; // Clear the source path after cut
                                isCutAction = false; // Reset the flag after the cut operation
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn một đích hợp lệ.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng sao chép hoặc cắt một tệp/thư mục trước.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng sao chép hoặc cắt một tệp/thư mục trước.");
            }*/
            if (!string.IsNullOrEmpty(sourcePath))
            {
                // Kiểm tra thư mục đích
                if (listView1.SelectedItems.Count > 0)
                {
                    var selectedItem = listView1.SelectedItems[0];
                    destinationPath = selectedItem.Tag.ToString();
                }
                else if (treeView1.SelectedNode != null)
                {
                    destinationPath = treeView1.SelectedNode.Tag.ToString();
                }

                if (!string.IsNullOrEmpty(destinationPath))
                {
                    // Kiểm tra nếu thư mục đích đã tồn tại
                    if (Directory.Exists(destinationPath) || File.Exists(destinationPath))
                    {
                        // Cập nhật kiểm tra để cho phép cắt và dán trong cùng một ổ đĩa
                        if (destinationPath.StartsWith(sourcePath, StringComparison.OrdinalIgnoreCase) &&
                            !destinationPath.Equals(sourcePath, StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("Không thể di chuyển một tệp vào chính nó hoặc vào thư mục con của nó.");
                            return;
                        }

                        // Kiểm tra nếu tệp đã tồn tại trong thư mục đích
                        if (File.Exists(Path.Combine(destinationPath, Path.GetFileName(sourcePath))))
                        {
                            MessageBox.Show("Tệp đã tồn tại trong thư mục đích. Bạn có muốn ghi đè không?");
                            // Nếu cần, có thể thêm logic để ghi đè tệp nếu người dùng đồng ý
                        }

                        // Thực hiện di chuyển hoặc sao chép
                        try
                        {
                            if (File.Exists(sourcePath))
                            {
                                string fileName = Path.GetFileName(sourcePath);
                                string targetFilePath = Path.Combine(destinationPath, fileName);

                                if (isCutAction)
                                {
                                    if (!File.Exists(targetFilePath))
                                    {
                                        File.Move(sourcePath, targetFilePath);
                                        MessageBox.Show("Tệp đã được di chuyển thành công!");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Tệp đã tồn tại trong đích.");
                                    }
                                }
                                else
                                {
                                    File.Copy(sourcePath, targetFilePath, true);
                                    MessageBox.Show("Tệp đã được sao chép thành công!");
                                }
                            }

                            LoadFilesAndDirectories(destinationPath); // Refresh ListView to display the new contents
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi sao chép/cắt: " + ex.Message);
                        }
                        finally
                        {
                            if (isCutAction)
                            {
                                sourcePath = null; // Clear the source path after cut
                                isCutAction = false; // Reset the flag after the cut operation
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn một đích hợp lệ.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng sao chép hoặc cắt một tệp/thư mục trước.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng sao chép hoặc cắt một tệp/thư mục trước.");
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                sourcePath = selectedItem.Tag.ToString(); // Set the source path
                isCutAction = true; // Set the action as Cut
            }
            else
            {
                MessageBox.Show("Please select a file or folder to cut.");
            }
        }
       

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (history.Count > 1) // Kiểm tra nếu có ít nhất 2 thư mục trong lịch sử
            {
                // Di chuyển thư mục hiện tại sang lịch sử quay lại
                forwardHistory.Push(history.Pop());

                // Lấy thư mục trước đó từ lịch sử
                string previousPath = history.Peek();
                LoadFilesAndDirectories(previousPath); // Tải thư mục trước đó
            }
            else
            {
                MessageBox.Show("Không còn thư mục trước đó.");
            }
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (forwardHistory.Count > 0) // Kiểm tra nếu có thư mục để quay lại
            {
                // Lấy thư mục quay lại từ lịch sử quay lại
                string nextPath = forwardHistory.Pop();
                LoadFilesAndDirectories(nextPath); // Tải thư mục quay lại
            }
            else
            {
                MessageBox.Show("Không còn thư mục để quay lại.");
            }
        }
    }


}