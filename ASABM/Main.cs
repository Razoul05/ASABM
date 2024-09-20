using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Configuration;
using System.Reflection;

namespace ASABM
{
    public partial class Main : Form
    {
        public Main()
        {
            rowAdding = true; //flag row adding to prevent edit events from executing while setup is occuring
            selectedRow = -1;
            theWatchers = new List<FileSystemWatcher> ();

            InitializeComponent();
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath); //Get app config from current exe path

            if (config.AppSettings.Settings["version"] == null) //This config is from Version 1.0; we neet to update the config format
            {
                for (int cnt = 0; cnt < config.AppSettings.Settings.Count; cnt++)
                {
                    //This will update each config from: worldAndPath;backupCount 
                    //  to: worldAndPath;backupPath;backupCount
                    List<String> thisKey = config.AppSettings.Settings["world" + cnt.ToString()].Value.Split(';').ToList();
                    config.AppSettings.Settings["world" + cnt.ToString()].Value = thisKey[0] + ";" + Path.GetDirectoryName(thisKey[0]) + ";" + thisKey[1];
                }

                config.AppSettings.Settings.Add("version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                config.Save();
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            }

            //We want to loop through this loop one fewer times than the count because of the "version" key that
            // is included in the total count
            for (int cnt = 0; cnt < config.AppSettings.Settings.Count - 1; cnt++)
            {
                List<String> thisKey = config.AppSettings.Settings["world" + cnt.ToString()].Value.Split(';').ToList(); //each rule has a worldPath;backupPath;count, split them up
                String ark_world = thisKey[0]; 
                String backup_path = thisKey[1];
                int backup_count;
                try
                {
                    backup_count = Int32.Parse(thisKey[2]); //attempt to convert the count to an integer
                }
                catch
                {
                    backup_count = 10; //default to 10 if int conversion fails
                    config.AppSettings.Settings["world" + cnt.ToString()].Value = thisKey[0] + ";" + thisKey[1] + ";" + backup_count.ToString();
                    config.Save();
                }

                //Add new item to datagrid
                int newrow = d_BackupList.Rows.Add();
                d_BackupList["c_ARKName", newrow].Value = ark_world;
                d_BackupList["c_BackupPath", newrow].Value = backup_path;
                d_BackupList["c_BackupCount", newrow].Value = backup_count;
                d_BackupList.ClearSelection();

                //start file system watcher for this file
                FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(ark_world));
                watcher.Filter = Path.GetFileName(ark_world);
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += WatcherFileChanged;
                watcher.EnableRaisingEvents = true;

                theWatchers.Add(watcher); //Add to watcher list to stop if removed from grid
            }

            rowAdding = false; //setup is complete, datagrid events can be processed now
        }

        private void d_BackupList_SelectionChanged(object sender, EventArgs e)
        {
            if (rowAdding) //a row is being added, either manually or object create so we want to ignore selection changes
                return;

            if (d_BackupList.SelectedCells.Count > 0 && d_BackupList.SelectedCells[0].ColumnIndex != d_BackupList.Columns["c_BackupCount"].Index) //we'll deselect the cell UNLESS its the backup count column selected
                d_BackupList.ClearSelection();

        }

        private void d_BackupList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (rowAdding || e.ColumnIndex != d_BackupList.Columns["c_BackupCount"].Index) //a row is being added, either manually or object create so we want to ignore value changes OR the edited row was NOT the backup count row
                return;

            d_BackupList.ClearSelection(); //deselect the current selectd cell
            
            int newCount;
            try
            {
                newCount = Int32.Parse(d_BackupList[e.ColumnIndex, e.RowIndex].Value.ToString()); //read new count from datagrid and attempt to convert to INT
            }
            catch
            {
                newCount = 10; //int conversion failed, default to value of 10
                d_BackupList[e.ColumnIndex, e.RowIndex].Value = 10;
            }

            //Update new backup count in config file
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings["world" + e.RowIndex.ToString()].Value = 
                d_BackupList["c_ARKName", e.RowIndex].Value.ToString() 
                + ";" + d_BackupList["c_BackupPath", e.RowIndex].Value.ToString() 
                + ";" + newCount.ToString();
            config.Save();
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        private void d_BackupList_Click(object sender, EventArgs e)
        {
            //We handel the context menu displaying manually rather than binding it to the data grid so we can ensure
            // that the remove option is enalbed/disabled as needed depending if a click occured in a row. without this
            // the menu would display instantally from a right click and this method would be called

            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Right) //If its a right click we want to show the context menu
            {
                if (d_BackupList.HitTest(me.X, me.Y).RowIndex >= 0)  //if the click occured on a row with data we can enable the remove menu item
                {
                    selectedRow = d_BackupList.HitTest(me.X, me.Y).RowIndex; //Save the row we clicked on in case we neeed it for a delete later
                    cm_Remove.Enabled = true; //Enable the remove option
                }
                else
                    cm_Remove.Enabled = false; //disable remove since a row was not clicked

                cm_ContextMenu.Show(Cursor.Position); //show context menu at current mouse position.
            }
            else if (me.Button == MouseButtons.Left)
            {
                DataGridView.HitTestInfo hti = d_BackupList.HitTest(me.X, me.Y);
                if (hti.RowIndex >= 0 && hti.ColumnIndex == d_BackupList.Columns["c_BackupPath"].Index) //This is a LEFT CLICK in the backup column of the grid
                {
                    int thisRow = hti.RowIndex;
                    f_BackupFolderBrowser.Reset();
                    f_BackupFolderBrowser.SelectedPath = d_BackupList["c_BackupPath", thisRow].Value.ToString(); //Set dialog path to current backup path
                    if (f_BackupFolderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        //OK was clicked on the dialg, grap and store the updated path
                        d_BackupList["c_BackupPath", thisRow].Value = f_BackupFolderBrowser.SelectedPath;
                        Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                        config.AppSettings.Settings["world" + thisRow.ToString()].Value =
                            d_BackupList["c_ARKName", thisRow].Value.ToString()
                            + ";" + d_BackupList["c_BackupPath", thisRow].Value.ToString()
                            + ";" + d_BackupList["c_BackupCount", thisRow].Value.ToString();
                        config.Save();
                        ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                    }
                }
            }
        }

        private void cm_Add_Click(object sender, EventArgs e)
        {
            if (ofd_FindArk.ShowDialog() == DialogResult.OK) //Launch the open file dialog and if OK was pressed we can continue
            {
                for (int row = 0; row < d_BackupList.RowCount; row++) //loop through all files already in the datagrid to ensure this is not a duplicate
                    if (d_BackupList["c_ARKName", row].Value.ToString() == ofd_FindArk.FileName)
                    {
                        //It was a duplicate, return error and cancel adding to application
                        MessageBox.Show("ARK file already in backup list!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }

                rowAdding = true; //We're adding a row so set this to prevent value changed events from being processed
                int newrow = d_BackupList.Rows.Add();
                d_BackupList["c_ARKName", newrow].Value = ofd_FindArk.FileName; //the full filename goes in the column ARKName
                d_BackupList["c_BackupPath", newrow].Value = Path.GetDirectoryName(ofd_FindArk.FileName); //use the path to the world for default backup path
                d_BackupList["c_BackupCount", newrow].Value = 10; //We'll default the backup count to 10
                d_BackupList.ClearSelection(); //Clear the selected row
                rowAdding = false; //we're done adding so events can be handeled again.

                //Write this new world to the config file
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                config.AppSettings.Settings.Add("world" + (config.AppSettings.Settings.Count -1).ToString(), ofd_FindArk.FileName + ";" + Path.GetDirectoryName(ofd_FindArk.FileName) + ";10");
                config.Save();
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);

                //Spawn file system watcher for this file
                FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(ofd_FindArk.FileName));
                watcher.Filter = ofd_FindArk.SafeFileName;
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += WatcherFileChanged;
                watcher.EnableRaisingEvents = true;

                theWatchers.Add(watcher); //add new watcher to the list
            }
        }

        private void cm_Remove_Click(object sender, EventArgs e)
        {
            theWatchers[selectedRow].Dispose(); //disable the filewatcher for this world
            theWatchers.RemoveAt(selectedRow); //remove this filewatcher from list
            d_BackupList.Rows.RemoveAt(selectedRow); //Remove this row from grid

            //we need to rewrite the entire config file. The world are orginized as world0, wolrd1, etc.
            // if world0 were to be removed that would leave only world1 and when the program is restarted
            // and the config file is read it will see 1 total worlds but expect that count to start at 0.
            //By rewritting the entire file we can ensure the worlds are always a contigious set starting at 0
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            int maxCount = config.AppSettings.Settings.Count; //Get the count of the CURRENT number of worlds in the config
            for (int cnt = 0; cnt < maxCount; cnt++) //loop through each world
                config.AppSettings.Settings.Remove("world" + cnt.ToString()); //remove this world from the config file

            for (int cnt = 0;cnt < d_BackupList.RowCount; cnt++) //Loop through each world remaining in the grid
                    config.AppSettings.Settings.Add("world" + cnt.ToString(), d_BackupList[0, cnt].Value.ToString() + ";" + d_BackupList[1, cnt].Value.ToString()); //readd this world to the config file but with the updated world#

            config.Save(); //Save the config file
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            selectedRow = -1; //reset selected row
        }

        private void WatcherFileChanged(object sender, FileSystemEventArgs e)
        {
            FileSystemWatcher watcher = (FileSystemWatcher)sender;
            watcher.EnableRaisingEvents = false; //pause the raising of events for this watcher while we process this one. This prevents duplicate events from being processed

            Thread.Sleep(10000); //Sleep for 10 seconds before acting, the file SHOULD be done being written to by now

            String thisFile = e.FullPath;
            for (int row = 0; row < d_BackupList.RowCount; row++) //Find this file in the datagrid so we can get the targeted number of backups
                if (d_BackupList["c_ARKName", row].Value.ToString() == thisFile)
                {
                    String AntiCorruption = thisFile.Replace(Path.GetExtension(thisFile), @"_AntiCorruptionBackup.bak");
                    if (File.Exists(AntiCorruption) && Math.Abs(((TimeSpan)(File.GetLastWriteTime(thisFile) - File.GetLastWriteTime(AntiCorruption))).TotalMinutes) <= 1)
                    {
                        watcher.EnableRaisingEvents = true; //allow events to be raised again
                        return; //If the target file and anticorruption have the same timestamp +- 1 minute we are on server startup and can skip this backup
                    }

                    //We'll backup ALL arkprofiles and arktribes in the same folder as the world file even if they were not updated
                    List<String> otherFiles = new List<String>();
                    otherFiles.AddRange(Directory.GetFiles(Path.GetDirectoryName(thisFile), "*.arkprofile").ToList());
                    otherFiles.AddRange(Directory.GetFiles(Path.GetDirectoryName(thisFile), "*.arktribe").ToList());

                    String backupPath = d_BackupList["c_BackupPath", row].Value.ToString(); //This is the path to write the backup to
                    ZipArchive zip = ZipFile.Open(backupPath + @"\Backup_" + Path.GetFileNameWithoutExtension(thisFile) + "_" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss") + ".zip", ZipArchiveMode.Create);
                    zip.CreateEntryFromFile(thisFile, Path.GetFileName(thisFile)); //Add world file to new zip file
                    foreach (String file in otherFiles)
                        zip.CreateEntryFromFile(file, Path.GetFileName(file)); //add other files to zip file
                    zip.Dispose(); //close zip file

                    List<String> otherBackups = Directory.GetFiles(backupPath, @"Backup_" + Path.GetFileNameWithoutExtension(thisFile) + "*.zip").ToList(); //Find all backups for this world
                    otherBackups.Sort(); //sort the list, this should put the oldest at the top

                    while (otherBackups.Count > Int32.Parse(d_BackupList["c_BackupCount", row].Value.ToString())) //loop as long as we have more than the max count
                    {
                        File.Delete(otherBackups[0]); //delete the top 1 file from the disk
                        otherBackups.RemoveAt(0); //delete file record from list
                    }

                    d_BackupList.Invoke((MethodInvoker)delegate { d_BackupList["c_LastBackup", row].Value = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"); }); //update datagrid with update time

                    watcher.EnableRaisingEvents = true; //allow events to be raised again
                    return;
                }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Confirm IF we REALLY want to close
            if (MessageBox.Show("Are you sure you want to close?\n\nBackup operations will stop!", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
