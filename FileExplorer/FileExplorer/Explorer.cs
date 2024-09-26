using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;

namespace FileExplorer
{
    public partial class Explorer : Form
    {
        public Explorer()
        {
            InitializeComponent();
            LoadDrives(); // Load the list of drives into the TreeView when the form starts

            // Set up columns for the ListView
            listView1.Columns.Add("Name", 200);       // Name column
            listView1.Columns.Add("Type", 100);       // Type column (File or Folder)
            listView1.Columns.Add("LastModify", 150); // LastModify column (Last modified time)
            listView1.MouseDoubleClick += new MouseEventHandler(listView1_MouseDoubleClick);
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                string selectedPath = selectedItem.Tag.ToString();

                // If the selected item is a directory
                if (Directory.Exists(selectedPath))
                {
                    // Load the files and subdirectories of the selected directory into the ListView
                    LoadFilesAndDirectories(selectedPath);
                }
                else if (File.Exists(selectedPath))
                {
                    string extension = Path.GetExtension(selectedPath).ToLower();

                    // Open image files with PictureViewer
                    if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp" || extension == ".gif")
                    {
                        try
                        {
                            PictureViewer pictureViewer = new PictureViewer(selectedPath);
                            pictureViewer.Show(); // Display PictureViewer with the image path
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Cannot open image file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        // Open other files with SpNotepad
                        try
                        {
                            SpNotepad spNotepad = new SpNotepad(selectedPath);
                            spNotepad.Show(); // Display SpNotepad with the file path
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Cannot open file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        // Load the drives into the TreeView
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
                LoadDirectories(rootNode); // Load the subdirectories of the drive
            }
        }

        // Load subdirectories
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // When a directory is selected in the TreeView
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string path = e.Node.Tag.ToString();
            LoadFilesAndDirectories(path);
        }

        private Stack<string> history = new Stack<string>(); // History of opened directories
        private Stack<string> forwardHistory = new Stack<string>(); // History of directories to go forward
        private Stack<string> backStack = new Stack<string>(); // Back history stack
        private Stack<string> forwardStack = new Stack<string>(); // Forward history stack

        // Load files and subdirectories into ListView
        private void LoadFilesAndDirectories(string path)
        {
            // Save the current path to history
            if (history.Count == 0 || history.Peek() != path)
            {
                history.Push(path);
                forwardHistory.Clear(); // Clear forward history when opening a new directory
            }

            listView1.Items.Clear();

            // Update back stack
            if (backStack.Count == 0 || backStack.Peek() != path) // Only add if it's not the already opened directory
            {
                backStack.Push(path);
            }
            forwardStack.Clear(); // Clear forwardStack when opening a new directory

            try
            {
                // Load subdirectories
                var dirs = Directory.GetDirectories(path);
                foreach (var dir in dirs)
                {
                    ListViewItem item = new ListViewItem(Path.GetFileName(dir)); // Name column
                    item.SubItems.Add("Folder"); // Type column
                    item.SubItems.Add(Directory.GetLastWriteTime(dir).ToString()); // LastModify column
                    item.Tag = dir;
                    listView1.Items.Add(item);
                }

                // Load files in the directory
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    ListViewItem item = new ListViewItem(Path.GetFileName(file)); // Name column
                    item.SubItems.Add("File"); // Type column
                    item.SubItems.Add(File.GetLastWriteTime(file).ToString()); // LastModify column
                    item.Tag = file;
                    listView1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (!string.IsNullOrEmpty(sourcePath))
            {
                // Check the destination directory
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
                    // Check if the destination already exists
                    if (Directory.Exists(destinationPath) || File.Exists(destinationPath))
                    {
                        try
                        {
                            if (File.Exists(sourcePath))
                            {
                                string fileName = Path.GetFileName(sourcePath);
                                string targetFilePath = Path.Combine(destinationPath, fileName);

                                if (isCutAction)
                                {
                                    // Check if moving between different drives
                                    if (Path.GetPathRoot(sourcePath) != Path.GetPathRoot(targetFilePath))
                                    {
                                        // If different drives, copy the file then delete the original
                                        File.Copy(sourcePath, targetFilePath, true);
                                        File.Delete(sourcePath); // Delete the original file
                                        MessageBox.Show("File has been moved successfully!");
                                    }
                                    else
                                    {
                                        // If on the same drive, use File.Move
                                        File.Move(sourcePath, targetFilePath);
                                        MessageBox.Show("File has been moved successfully!");
                                    }
                                }
                                else
                                {
                                    // Copy action
                                    File.Copy(sourcePath, targetFilePath, true);
                                    MessageBox.Show("File has been copied successfully!");
                                }
                            }

                            LoadFilesAndDirectories(destinationPath); // Refresh ListView to display the new contents
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error while copying/cutting: " + ex.Message);
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
                        MessageBox.Show("Please select a valid destination.");
                    }
                }
                else
                {
                    MessageBox.Show("Please copy or cut a file/folder first.");
                }
            }
            else
            {
                MessageBox.Show("Please copy or cut a file/folder first.");
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
            if (history.Count > 1) // Check if there are at least 2 directories in history
            {
                // Move the current directory to back history
                forwardHistory.Push(history.Pop());

                // Get the previous directory from history
                string previousPath = history.Peek();
                LoadFilesAndDirectories(previousPath); // Load the previous directory
            }
            else
            {
                MessageBox.Show("No previous directory available.");
            }
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (forwardHistory.Count > 0) // Check if there are directories to go forward
            {
                // Get the directory to go forward from forward history
                string nextPath = forwardHistory.Pop();
                LoadFilesAndDirectories(nextPath); // Load the directory to go forward
            }
            else
            {
                MessageBox.Show("No directory to go forward to.");
            }
        }
    }
}
