namespace VideoToDVD
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbInputFiles = new System.Windows.Forms.GroupBox();
            this.btnClearFiles = new System.Windows.Forms.Button();
            this.btnAddFiles = new System.Windows.Forms.Button();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbDVDVideoSettings = new System.Windows.Forms.GroupBox();
            this.cbTVSystem = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbBurnSettings = new System.Windows.Forms.GroupBox();
            this.lblFreeSpace = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblMediaType = new System.Windows.Forms.Label();
            this.txtVolumeName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbDevice = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCreateDVDVideoDisc = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lvLog = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.gbIntermediateFiles = new System.Windows.Forms.GroupBox();
            this.btnBrowseVideoTsFolder = new System.Windows.Forms.Button();
            this.txtVideoTsFolder = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnBrowseDvdProjectFolder = new System.Windows.Forms.Button();
            this.txtDvdProjectFolder = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCreateDVDBProject = new System.Windows.Forms.Button();
            this.btnCreateVideoTsFolder = new System.Windows.Forms.Button();
            this.btnCreateDiscImage = new System.Windows.Forms.Button();
            this.gbInputFiles.SuspendLayout();
            this.gbDVDVideoSettings.SuspendLayout();
            this.gbBurnSettings.SuspendLayout();
            this.gbIntermediateFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbInputFiles
            // 
            this.gbInputFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbInputFiles.Controls.Add(this.btnClearFiles);
            this.gbInputFiles.Controls.Add(this.btnAddFiles);
            this.gbInputFiles.Controls.Add(this.lvFiles);
            this.gbInputFiles.Location = new System.Drawing.Point(12, 12);
            this.gbInputFiles.Name = "gbInputFiles";
            this.gbInputFiles.Size = new System.Drawing.Size(718, 150);
            this.gbInputFiles.TabIndex = 0;
            this.gbInputFiles.TabStop = false;
            this.gbInputFiles.Text = "Input Files";
            // 
            // btnClearFiles
            // 
            this.btnClearFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearFiles.Location = new System.Drawing.Point(635, 117);
            this.btnClearFiles.Name = "btnClearFiles";
            this.btnClearFiles.Size = new System.Drawing.Size(75, 23);
            this.btnClearFiles.TabIndex = 2;
            this.btnClearFiles.Text = "Clear Files";
            this.btnClearFiles.UseVisualStyleBackColor = true;
            this.btnClearFiles.Click += new System.EventHandler(this.btnClearFiles_Click);
            // 
            // btnAddFiles
            // 
            this.btnAddFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFiles.Location = new System.Drawing.Point(551, 117);
            this.btnAddFiles.Name = "btnAddFiles";
            this.btnAddFiles.Size = new System.Drawing.Size(75, 23);
            this.btnAddFiles.TabIndex = 1;
            this.btnAddFiles.Text = "Add Files";
            this.btnAddFiles.UseVisualStyleBackColor = true;
            this.btnAddFiles.Click += new System.EventHandler(this.btnAddFiles_Click);
            // 
            // lvFiles
            // 
            this.lvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvFiles.Location = new System.Drawing.Point(9, 19);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(703, 90);
            this.lvFiles.TabIndex = 0;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "File";
            this.columnHeader3.Width = 487;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Info";
            this.columnHeader4.Width = 161;
            // 
            // gbDVDVideoSettings
            // 
            this.gbDVDVideoSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbDVDVideoSettings.Controls.Add(this.cbTVSystem);
            this.gbDVDVideoSettings.Controls.Add(this.label1);
            this.gbDVDVideoSettings.Location = new System.Drawing.Point(12, 265);
            this.gbDVDVideoSettings.Name = "gbDVDVideoSettings";
            this.gbDVDVideoSettings.Size = new System.Drawing.Size(202, 131);
            this.gbDVDVideoSettings.TabIndex = 3;
            this.gbDVDVideoSettings.TabStop = false;
            this.gbDVDVideoSettings.Text = "DVD-Video Settings";
            // 
            // cbTVSystem
            // 
            this.cbTVSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTVSystem.FormattingEnabled = true;
            this.cbTVSystem.Items.AddRange(new object[] {
            "NTSC",
            "PAL"});
            this.cbTVSystem.Location = new System.Drawing.Point(80, 19);
            this.cbTVSystem.Name = "cbTVSystem";
            this.cbTVSystem.Size = new System.Drawing.Size(102, 21);
            this.cbTVSystem.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "TV System:";
            // 
            // gbBurnSettings
            // 
            this.gbBurnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbBurnSettings.Controls.Add(this.lblFreeSpace);
            this.gbBurnSettings.Controls.Add(this.label5);
            this.gbBurnSettings.Controls.Add(this.label4);
            this.gbBurnSettings.Controls.Add(this.lblMediaType);
            this.gbBurnSettings.Controls.Add(this.txtVolumeName);
            this.gbBurnSettings.Controls.Add(this.label3);
            this.gbBurnSettings.Controls.Add(this.cbDevice);
            this.gbBurnSettings.Controls.Add(this.label2);
            this.gbBurnSettings.Location = new System.Drawing.Point(220, 265);
            this.gbBurnSettings.Name = "gbBurnSettings";
            this.gbBurnSettings.Size = new System.Drawing.Size(510, 131);
            this.gbBurnSettings.TabIndex = 4;
            this.gbBurnSettings.TabStop = false;
            this.gbBurnSettings.Text = "Burn Settings";
            // 
            // lblFreeSpace
            // 
            this.lblFreeSpace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFreeSpace.Location = new System.Drawing.Point(341, 81);
            this.lblFreeSpace.Name = "lblFreeSpace";
            this.lblFreeSpace.Size = new System.Drawing.Size(140, 23);
            this.lblFreeSpace.TabIndex = 9;
            this.lblFreeSpace.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(270, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Free Space:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Media:";
            // 
            // lblMediaType
            // 
            this.lblMediaType.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMediaType.Location = new System.Drawing.Point(95, 48);
            this.lblMediaType.Name = "lblMediaType";
            this.lblMediaType.Size = new System.Drawing.Size(386, 23);
            this.lblMediaType.TabIndex = 6;
            this.lblMediaType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtVolumeName
            // 
            this.txtVolumeName.Location = new System.Drawing.Point(95, 84);
            this.txtVolumeName.Name = "txtVolumeName";
            this.txtVolumeName.Size = new System.Drawing.Size(160, 20);
            this.txtVolumeName.TabIndex = 5;
            this.txtVolumeName.Text = "DVDVIDEO";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Volume name:";
            // 
            // cbDevice
            // 
            this.cbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDevice.FormattingEnabled = true;
            this.cbDevice.Location = new System.Drawing.Point(95, 20);
            this.cbDevice.Name = "cbDevice";
            this.cbDevice.Size = new System.Drawing.Size(386, 21);
            this.cbDevice.TabIndex = 3;
            this.cbDevice.SelectedIndexChanged += new System.EventHandler(this.cbDevice_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Device:";
            // 
            // btnCreateDVDVideoDisc
            // 
            this.btnCreateDVDVideoDisc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateDVDVideoDisc.Location = new System.Drawing.Point(492, 543);
            this.btnCreateDVDVideoDisc.Name = "btnCreateDVDVideoDisc";
            this.btnCreateDVDVideoDisc.Size = new System.Drawing.Size(135, 23);
            this.btnCreateDVDVideoDisc.TabIndex = 5;
            this.btnCreateDVDVideoDisc.Text = "Burn DVD Disc";
            this.btnCreateDVDVideoDisc.UseVisualStyleBackColor = true;
            this.btnCreateDVDVideoDisc.Click += new System.EventHandler(this.btnCreateDVDVideoDisc_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(652, 543);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lvLog
            // 
            this.lvLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvLog.FullRowSelect = true;
            this.lvLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvLog.Location = new System.Drawing.Point(12, 402);
            this.lvLog.Name = "lvLog";
            this.lvLog.Size = new System.Drawing.Size(718, 83);
            this.lvLog.TabIndex = 7;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            this.lvLog.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            this.columnHeader1.Width = 71;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Event";
            this.columnHeader2.Width = 587;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 491);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(718, 23);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 8;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // gbIntermediateFiles
            // 
            this.gbIntermediateFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbIntermediateFiles.Controls.Add(this.btnBrowseVideoTsFolder);
            this.gbIntermediateFiles.Controls.Add(this.txtVideoTsFolder);
            this.gbIntermediateFiles.Controls.Add(this.label7);
            this.gbIntermediateFiles.Controls.Add(this.btnBrowseDvdProjectFolder);
            this.gbIntermediateFiles.Controls.Add(this.txtDvdProjectFolder);
            this.gbIntermediateFiles.Controls.Add(this.label6);
            this.gbIntermediateFiles.Location = new System.Drawing.Point(12, 168);
            this.gbIntermediateFiles.Name = "gbIntermediateFiles";
            this.gbIntermediateFiles.Size = new System.Drawing.Size(718, 81);
            this.gbIntermediateFiles.TabIndex = 9;
            this.gbIntermediateFiles.TabStop = false;
            this.gbIntermediateFiles.Text = "Intermediate Files";
            // 
            // btnBrowseVideoTsFolder
            // 
            this.btnBrowseVideoTsFolder.Location = new System.Drawing.Point(635, 44);
            this.btnBrowseVideoTsFolder.Name = "btnBrowseVideoTsFolder";
            this.btnBrowseVideoTsFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseVideoTsFolder.TabIndex = 5;
            this.btnBrowseVideoTsFolder.Text = "Browse";
            this.btnBrowseVideoTsFolder.UseVisualStyleBackColor = true;
            this.btnBrowseVideoTsFolder.Click += new System.EventHandler(this.btnBrowseVideoTsFolder_Click);
            // 
            // txtVideoTsFolder
            // 
            this.txtVideoTsFolder.Location = new System.Drawing.Point(120, 48);
            this.txtVideoTsFolder.Name = "txtVideoTsFolder";
            this.txtVideoTsFolder.Size = new System.Drawing.Size(506, 20);
            this.txtVideoTsFolder.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "VIDEO_TS folder:";
            // 
            // btnBrowseDvdProjectFolder
            // 
            this.btnBrowseDvdProjectFolder.Location = new System.Drawing.Point(635, 15);
            this.btnBrowseDvdProjectFolder.Name = "btnBrowseDvdProjectFolder";
            this.btnBrowseDvdProjectFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseDvdProjectFolder.TabIndex = 2;
            this.btnBrowseDvdProjectFolder.Text = "Browse";
            this.btnBrowseDvdProjectFolder.UseVisualStyleBackColor = true;
            this.btnBrowseDvdProjectFolder.Click += new System.EventHandler(this.btnBrowseDvdProjectFolder_Click);
            // 
            // txtDvdProjectFolder
            // 
            this.txtDvdProjectFolder.Location = new System.Drawing.Point(120, 19);
            this.txtDvdProjectFolder.Name = "txtDvdProjectFolder";
            this.txtDvdProjectFolder.Size = new System.Drawing.Size(506, 20);
            this.txtDvdProjectFolder.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "DVDB project folder:";
            // 
            // btnCreateDVDBProject
            // 
            this.btnCreateDVDBProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateDVDBProject.Location = new System.Drawing.Point(12, 543);
            this.btnCreateDVDBProject.Name = "btnCreateDVDBProject";
            this.btnCreateDVDBProject.Size = new System.Drawing.Size(135, 23);
            this.btnCreateDVDBProject.TabIndex = 10;
            this.btnCreateDVDBProject.Text = "Create DVDB project";
            this.btnCreateDVDBProject.UseVisualStyleBackColor = true;
            this.btnCreateDVDBProject.Click += new System.EventHandler(this.btnCreateDVDBProject_Click);
            // 
            // btnCreateVideoTsFolder
            // 
            this.btnCreateVideoTsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateVideoTsFolder.Location = new System.Drawing.Point(172, 543);
            this.btnCreateVideoTsFolder.Name = "btnCreateVideoTsFolder";
            this.btnCreateVideoTsFolder.Size = new System.Drawing.Size(135, 23);
            this.btnCreateVideoTsFolder.TabIndex = 11;
            this.btnCreateVideoTsFolder.Text = "Create VIDEO_TS folder";
            this.btnCreateVideoTsFolder.UseVisualStyleBackColor = true;
            this.btnCreateVideoTsFolder.Click += new System.EventHandler(this.btnCreateVideoTsFolder_Click);
            // 
            // btnCreateDiscImage
            // 
            this.btnCreateDiscImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateDiscImage.Location = new System.Drawing.Point(332, 543);
            this.btnCreateDiscImage.Name = "btnCreateDiscImage";
            this.btnCreateDiscImage.Size = new System.Drawing.Size(135, 23);
            this.btnCreateDiscImage.TabIndex = 12;
            this.btnCreateDiscImage.Text = "Create DVD Image";
            this.btnCreateDiscImage.UseVisualStyleBackColor = true;
            this.btnCreateDiscImage.Click += new System.EventHandler(this.btnCreateDiscImage_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 578);
            this.Controls.Add(this.btnCreateDiscImage);
            this.Controls.Add(this.btnCreateVideoTsFolder);
            this.Controls.Add(this.btnCreateDVDBProject);
            this.Controls.Add(this.gbIntermediateFiles);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lvLog);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnCreateDVDVideoDisc);
            this.Controls.Add(this.gbBurnSettings);
            this.Controls.Add(this.gbDVDVideoSettings);
            this.Controls.Add(this.gbInputFiles);
            this.Name = "MainForm";
            this.Text = "DVD-Video Creator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.gbInputFiles.ResumeLayout(false);
            this.gbDVDVideoSettings.ResumeLayout(false);
            this.gbDVDVideoSettings.PerformLayout();
            this.gbBurnSettings.ResumeLayout(false);
            this.gbBurnSettings.PerformLayout();
            this.gbIntermediateFiles.ResumeLayout(false);
            this.gbIntermediateFiles.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbInputFiles;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.Button btnClearFiles;
        private System.Windows.Forms.Button btnAddFiles;
        private System.Windows.Forms.GroupBox gbDVDVideoSettings;
        private System.Windows.Forms.GroupBox gbBurnSettings;
        private System.Windows.Forms.Button btnCreateDVDVideoDisc;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ListView lvLog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbTVSystem;
        private System.Windows.Forms.ComboBox cbDevice;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtVolumeName;
        private System.Windows.Forms.Label lblMediaType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblFreeSpace;
        private System.Windows.Forms.GroupBox gbIntermediateFiles;
        private System.Windows.Forms.Button btnBrowseVideoTsFolder;
        private System.Windows.Forms.TextBox txtVideoTsFolder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnBrowseDvdProjectFolder;
        private System.Windows.Forms.TextBox txtDvdProjectFolder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnCreateDVDBProject;
        private System.Windows.Forms.Button btnCreateVideoTsFolder;
        private System.Windows.Forms.Button btnCreateDiscImage;
    }
}

