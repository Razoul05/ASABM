using System.Collections.Generic;
using System.IO;
using System;

namespace ASABM
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool rowAdding;
        private int selectedRow;
        private List<FileSystemWatcher> theWatchers;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ofd_FindArk = new System.Windows.Forms.OpenFileDialog();
            this.d_BackupList = new System.Windows.Forms.DataGridView();
            this.cm_ContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cm_Add = new System.Windows.Forms.ToolStripMenuItem();
            this.cm_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.f_BackupFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.c_ARKName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.c_BackupPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.c_BackupCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.c_LastBackup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.d_BackupList)).BeginInit();
            this.cm_ContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ofd_FindArk
            // 
            this.ofd_FindArk.DefaultExt = "ark";
            this.ofd_FindArk.Filter = "ARK files (*.ark)|*.ark";
            // 
            // d_BackupList
            // 
            this.d_BackupList.AllowUserToAddRows = false;
            this.d_BackupList.AllowUserToDeleteRows = false;
            this.d_BackupList.AllowUserToResizeRows = false;
            this.d_BackupList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.d_BackupList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.c_ARKName,
            this.c_BackupPath,
            this.c_BackupCount,
            this.c_LastBackup});
            this.d_BackupList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.d_BackupList.Location = new System.Drawing.Point(0, 0);
            this.d_BackupList.Name = "d_BackupList";
            this.d_BackupList.RowHeadersVisible = false;
            this.d_BackupList.Size = new System.Drawing.Size(619, 187);
            this.d_BackupList.TabIndex = 1;
            this.d_BackupList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.d_BackupList_CellValueChanged);
            this.d_BackupList.SelectionChanged += new System.EventHandler(this.d_BackupList_SelectionChanged);
            this.d_BackupList.Click += new System.EventHandler(this.d_BackupList_Click);
            // 
            // cm_ContextMenu
            // 
            this.cm_ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cm_Add,
            this.cm_Remove});
            this.cm_ContextMenu.Name = "c_ContextMenu";
            this.cm_ContextMenu.Size = new System.Drawing.Size(118, 48);
            // 
            // cm_Add
            // 
            this.cm_Add.Name = "cm_Add";
            this.cm_Add.Size = new System.Drawing.Size(117, 22);
            this.cm_Add.Text = "Add";
            this.cm_Add.Click += new System.EventHandler(this.cm_Add_Click);
            // 
            // cm_Remove
            // 
            this.cm_Remove.Enabled = false;
            this.cm_Remove.Name = "cm_Remove";
            this.cm_Remove.Size = new System.Drawing.Size(117, 22);
            this.cm_Remove.Text = "Remove";
            this.cm_Remove.Click += new System.EventHandler(this.cm_Remove_Click);
            // 
            // c_ARKName
            // 
            this.c_ARKName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.c_ARKName.HeaderText = "ARK Name";
            this.c_ARKName.Name = "c_ARKName";
            this.c_ARKName.ReadOnly = true;
            this.c_ARKName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.c_ARKName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // c_BackupPath
            // 
            this.c_BackupPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.c_BackupPath.HeaderText = "Backup Path";
            this.c_BackupPath.Name = "c_BackupPath";
            this.c_BackupPath.ReadOnly = true;
            this.c_BackupPath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // c_BackupCount
            // 
            this.c_BackupCount.HeaderText = "Backup Count";
            this.c_BackupCount.Name = "c_BackupCount";
            // 
            // c_LastBackup
            // 
            this.c_LastBackup.HeaderText = "Last Backup";
            this.c_LastBackup.MinimumWidth = 150;
            this.c_LastBackup.Name = "c_LastBackup";
            this.c_LastBackup.ReadOnly = true;
            this.c_LastBackup.Width = 150;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 187);
            this.Controls.Add(this.d_BackupList);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(635, 226);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ASA Backup Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Shown += new System.EventHandler(this.Main_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.d_BackupList)).EndInit();
            this.cm_ContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog ofd_FindArk;
        private System.Windows.Forms.DataGridView d_BackupList;
        private System.Windows.Forms.ContextMenuStrip cm_ContextMenu;
        private System.Windows.Forms.ToolStripMenuItem cm_Add;
        private System.Windows.Forms.ToolStripMenuItem cm_Remove;
        private System.Windows.Forms.FolderBrowserDialog f_BackupFolderBrowser;
        private System.Windows.Forms.DataGridViewTextBoxColumn c_ARKName;
        private System.Windows.Forms.DataGridViewTextBoxColumn c_BackupPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn c_BackupCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn c_LastBackup;
    }
}

