namespace CameraToDVD
{
    partial class RecorderForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listAudioDev = new System.Windows.Forms.ComboBox();
            this.cmdAudioDevProp = new System.Windows.Forms.Button();
            this.cmdAudioCaptureProp = new System.Windows.Forms.Button();
            this.listVideoDev = new System.Windows.Forms.ComboBox();
            this.cmdVideoDevProp = new System.Windows.Forms.Button();
            this.cmdVideoCaptureProp = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdRecord = new System.Windows.Forms.Button();
            this.txtRecording = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtRemainingSpace = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtRemainingTime = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCurrentFPS = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtVDropped = new System.Windows.Forms.Label();
            this.txtVProcessed = new System.Windows.Forms.Label();
            this.txtVCallbacks = new System.Windows.Forms.Label();
            this.txtADropped = new System.Windows.Forms.Label();
            this.txtAProcessed = new System.Windows.Forms.Label();
            this.txtACallbacks = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAverageFPS = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNumNotDropped = new System.Windows.Forms.Label();
            this.txtNumDropped = new System.Windows.Forms.Label();
            this.labelNotDropped = new System.Windows.Forms.Label();
            this.labelDropped = new System.Windows.Forms.Label();
            this.txtRecTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdFinalize = new System.Windows.Forms.Button();
            this.previewBox = new System.Windows.Forms.PictureBox();
            this.cmdSimulate = new System.Windows.Forms.CheckBox();
            this.cmdPreview = new System.Windows.Forms.CheckBox();
            this.cmdErase = new System.Windows.Forms.Button();
            this.cmdExit = new System.Windows.Forms.Button();
            this.listDrives = new System.Windows.Forms.CheckedListBox();
            this.cmdVideoInfo = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.editFolder = new System.Windows.Forms.TextBox();
            this.cmdBrowseFolder = new System.Windows.Forms.Button();
            this.cmdClearFolder = new System.Windows.Forms.Button();
            this.txtUpdatingDrives = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
            this.SuspendLayout();
            // 
            // listAudioDev
            // 
            this.listAudioDev.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listAudioDev.FormattingEnabled = true;
            this.listAudioDev.Location = new System.Drawing.Point(17, 17);
            this.listAudioDev.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.listAudioDev.Name = "listAudioDev";
            this.listAudioDev.Size = new System.Drawing.Size(197, 21);
            this.listAudioDev.TabIndex = 0;
            this.listAudioDev.SelectedIndexChanged += new System.EventHandler(this.listAudioDev_SelectedIndexChanged);
            // 
            // cmdAudioDevProp
            // 
            this.cmdAudioDevProp.Location = new System.Drawing.Point(17, 41);
            this.cmdAudioDevProp.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmdAudioDevProp.Name = "cmdAudioDevProp";
            this.cmdAudioDevProp.Size = new System.Drawing.Size(81, 24);
            this.cmdAudioDevProp.TabIndex = 1;
            this.cmdAudioDevProp.Text = "Device...";
            this.cmdAudioDevProp.UseVisualStyleBackColor = true;
            this.cmdAudioDevProp.Click += new System.EventHandler(this.cmdAudioDevProp_Click);
            // 
            // cmdAudioCaptureProp
            // 
            this.cmdAudioCaptureProp.Location = new System.Drawing.Point(110, 41);
            this.cmdAudioCaptureProp.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmdAudioCaptureProp.Name = "cmdAudioCaptureProp";
            this.cmdAudioCaptureProp.Size = new System.Drawing.Size(81, 24);
            this.cmdAudioCaptureProp.TabIndex = 2;
            this.cmdAudioCaptureProp.Text = "Capture...";
            this.cmdAudioCaptureProp.UseVisualStyleBackColor = true;
            this.cmdAudioCaptureProp.Visible = false;
            this.cmdAudioCaptureProp.Click += new System.EventHandler(this.cmdAudioCaptureProp_Click);
            // 
            // listVideoDev
            // 
            this.listVideoDev.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listVideoDev.FormattingEnabled = true;
            this.listVideoDev.Location = new System.Drawing.Point(17, 17);
            this.listVideoDev.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.listVideoDev.Name = "listVideoDev";
            this.listVideoDev.Size = new System.Drawing.Size(197, 21);
            this.listVideoDev.TabIndex = 3;
            this.listVideoDev.SelectedIndexChanged += new System.EventHandler(this.listVideoDev_SelectedIndexChanged);
            // 
            // cmdVideoDevProp
            // 
            this.cmdVideoDevProp.Location = new System.Drawing.Point(17, 41);
            this.cmdVideoDevProp.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmdVideoDevProp.Name = "cmdVideoDevProp";
            this.cmdVideoDevProp.Size = new System.Drawing.Size(81, 24);
            this.cmdVideoDevProp.TabIndex = 4;
            this.cmdVideoDevProp.Text = "Device...";
            this.cmdVideoDevProp.UseVisualStyleBackColor = true;
            this.cmdVideoDevProp.Click += new System.EventHandler(this.cmdVideoDevProp_Click);
            // 
            // cmdVideoCaptureProp
            // 
            this.cmdVideoCaptureProp.Location = new System.Drawing.Point(110, 41);
            this.cmdVideoCaptureProp.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmdVideoCaptureProp.Name = "cmdVideoCaptureProp";
            this.cmdVideoCaptureProp.Size = new System.Drawing.Size(81, 24);
            this.cmdVideoCaptureProp.TabIndex = 5;
            this.cmdVideoCaptureProp.Text = "Capture...";
            this.cmdVideoCaptureProp.UseVisualStyleBackColor = true;
            this.cmdVideoCaptureProp.Click += new System.EventHandler(this.cmdVideoCaptureProp_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listAudioDev);
            this.groupBox1.Controls.Add(this.cmdAudioDevProp);
            this.groupBox1.Controls.Add(this.cmdAudioCaptureProp);
            this.groupBox1.Location = new System.Drawing.Point(10, 5);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(224, 76);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Audio Devices";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listVideoDev);
            this.groupBox2.Controls.Add(this.cmdVideoDevProp);
            this.groupBox2.Controls.Add(this.cmdVideoCaptureProp);
            this.groupBox2.Location = new System.Drawing.Point(10, 83);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(224, 72);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Video Devices";
            // 
            // cmdRecord
            // 
            this.cmdRecord.Location = new System.Drawing.Point(243, 161);
            this.cmdRecord.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmdRecord.Name = "cmdRecord";
            this.cmdRecord.Size = new System.Drawing.Size(104, 32);
            this.cmdRecord.TabIndex = 8;
            this.cmdRecord.Text = "Record";
            this.cmdRecord.UseVisualStyleBackColor = true;
            this.cmdRecord.Click += new System.EventHandler(this.cmdRecord_Click);
            // 
            // txtRecording
            // 
            this.txtRecording.AutoSize = true;
            this.txtRecording.ForeColor = System.Drawing.Color.Red;
            this.txtRecording.Location = new System.Drawing.Point(264, 143);
            this.txtRecording.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtRecording.Name = "txtRecording";
            this.txtRecording.Size = new System.Drawing.Size(65, 13);
            this.txtRecording.TabIndex = 9;
            this.txtRecording.Text = "Recording...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(241, 4);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Drives:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtRemainingSpace);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.txtRemainingTime);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtCurrentFPS);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.txtVDropped);
            this.groupBox3.Controls.Add(this.txtVProcessed);
            this.groupBox3.Controls.Add(this.txtVCallbacks);
            this.groupBox3.Controls.Add(this.txtADropped);
            this.groupBox3.Controls.Add(this.txtAProcessed);
            this.groupBox3.Controls.Add(this.txtACallbacks);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtAverageFPS);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.txtNumNotDropped);
            this.groupBox3.Controls.Add(this.txtNumDropped);
            this.groupBox3.Controls.Add(this.labelNotDropped);
            this.groupBox3.Controls.Add(this.labelDropped);
            this.groupBox3.Controls.Add(this.txtRecTime);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(10, 194);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Size = new System.Drawing.Size(343, 103);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Statistics";
            // 
            // txtRemainingSpace
            // 
            this.txtRemainingSpace.AutoSize = true;
            this.txtRemainingSpace.Location = new System.Drawing.Point(252, 19);
            this.txtRemainingSpace.Name = "txtRemainingSpace";
            this.txtRemainingSpace.Size = new System.Drawing.Size(65, 13);
            this.txtRemainingSpace.TabIndex = 30;
            this.txtRemainingSpace.Text = "0000.00 MB";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(239, 19);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(12, 13);
            this.label12.TabIndex = 29;
            this.label12.Text = "/";
            // 
            // txtRemainingTime
            // 
            this.txtRemainingTime.AutoSize = true;
            this.txtRemainingTime.Location = new System.Drawing.Point(189, 19);
            this.txtRemainingTime.Name = "txtRemainingTime";
            this.txtRemainingTime.Size = new System.Drawing.Size(49, 13);
            this.txtRemainingTime.TabIndex = 28;
            this.txtRemainingTime.Text = "00:00:00";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(129, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Remaining:";
            // 
            // groupBox5
            // 
            this.groupBox5.Location = new System.Drawing.Point(131, 39);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox5.Size = new System.Drawing.Size(2, 57);
            this.groupBox5.TabIndex = 26;
            this.groupBox5.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(253, 40);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Size = new System.Drawing.Size(2, 57);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(275, 40);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "Video";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(198, 40);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Audio";
            // 
            // txtCurrentFPS
            // 
            this.txtCurrentFPS.Location = new System.Drawing.Point(85, 81);
            this.txtCurrentFPS.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtCurrentFPS.Name = "txtCurrentFPS";
            this.txtCurrentFPS.Size = new System.Drawing.Size(42, 13);
            this.txtCurrentFPS.TabIndex = 21;
            this.txtCurrentFPS.Text = "[curfps]";
            this.txtCurrentFPS.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 81);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Current fps:";
            // 
            // txtVDropped
            // 
            this.txtVDropped.Location = new System.Drawing.Point(298, 81);
            this.txtVDropped.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtVDropped.Name = "txtVDropped";
            this.txtVDropped.Size = new System.Drawing.Size(40, 13);
            this.txtVDropped.TabIndex = 19;
            this.txtVDropped.Text = "[vdrop]";
            this.txtVDropped.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtVProcessed
            // 
            this.txtVProcessed.Location = new System.Drawing.Point(298, 67);
            this.txtVProcessed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtVProcessed.Name = "txtVProcessed";
            this.txtVProcessed.Size = new System.Drawing.Size(40, 13);
            this.txtVProcessed.TabIndex = 18;
            this.txtVProcessed.Text = "[vproc]";
            this.txtVProcessed.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtVCallbacks
            // 
            this.txtVCallbacks.Location = new System.Drawing.Point(307, 53);
            this.txtVCallbacks.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtVCallbacks.Name = "txtVCallbacks";
            this.txtVCallbacks.Size = new System.Drawing.Size(31, 13);
            this.txtVCallbacks.TabIndex = 17;
            this.txtVCallbacks.Text = "[vcb]";
            this.txtVCallbacks.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtADropped
            // 
            this.txtADropped.Location = new System.Drawing.Point(211, 81);
            this.txtADropped.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtADropped.Name = "txtADropped";
            this.txtADropped.Size = new System.Drawing.Size(40, 13);
            this.txtADropped.TabIndex = 16;
            this.txtADropped.Text = "[adrop]";
            this.txtADropped.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtAProcessed
            // 
            this.txtAProcessed.Location = new System.Drawing.Point(211, 67);
            this.txtAProcessed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtAProcessed.Name = "txtAProcessed";
            this.txtAProcessed.Size = new System.Drawing.Size(40, 13);
            this.txtAProcessed.TabIndex = 15;
            this.txtAProcessed.Text = "[aproc]";
            this.txtAProcessed.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtACallbacks
            // 
            this.txtACallbacks.Location = new System.Drawing.Point(220, 53);
            this.txtACallbacks.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtACallbacks.Name = "txtACallbacks";
            this.txtACallbacks.Size = new System.Drawing.Size(31, 13);
            this.txtACallbacks.TabIndex = 14;
            this.txtACallbacks.Text = "[acb]";
            this.txtACallbacks.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(145, 80);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "dropped:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(135, 67);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "processed:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(139, 53);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "callbacks:";
            // 
            // txtAverageFPS
            // 
            this.txtAverageFPS.Location = new System.Drawing.Point(82, 66);
            this.txtAverageFPS.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtAverageFPS.Name = "txtAverageFPS";
            this.txtAverageFPS.Size = new System.Drawing.Size(45, 13);
            this.txtAverageFPS.TabIndex = 7;
            this.txtAverageFPS.Text = "[avgfps]";
            this.txtAverageFPS.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 66);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Average fps:";
            // 
            // txtNumNotDropped
            // 
            this.txtNumNotDropped.Location = new System.Drawing.Point(76, 52);
            this.txtNumNotDropped.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtNumNotDropped.Name = "txtNumNotDropped";
            this.txtNumNotDropped.Size = new System.Drawing.Size(51, 13);
            this.txtNumNotDropped.TabIndex = 5;
            this.txtNumNotDropped.Text = "[dsvproc]";
            this.txtNumNotDropped.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtNumDropped
            // 
            this.txtNumDropped.Location = new System.Drawing.Point(76, 37);
            this.txtNumDropped.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtNumDropped.Name = "txtNumDropped";
            this.txtNumDropped.Size = new System.Drawing.Size(51, 13);
            this.txtNumDropped.TabIndex = 4;
            this.txtNumDropped.Text = "[dsvdrop]";
            this.txtNumDropped.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelNotDropped
            // 
            this.labelNotDropped.AutoSize = true;
            this.labelNotDropped.Location = new System.Drawing.Point(9, 52);
            this.labelNotDropped.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelNotDropped.Name = "labelNotDropped";
            this.labelNotDropped.Size = new System.Drawing.Size(60, 13);
            this.labelNotDropped.TabIndex = 3;
            this.labelNotDropped.Text = "Processed:";
            // 
            // labelDropped
            // 
            this.labelDropped.AutoSize = true;
            this.labelDropped.Location = new System.Drawing.Point(18, 37);
            this.labelDropped.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelDropped.Name = "labelDropped";
            this.labelDropped.Size = new System.Drawing.Size(51, 13);
            this.labelDropped.TabIndex = 2;
            this.labelDropped.Text = "Dropped:";
            // 
            // txtRecTime
            // 
            this.txtRecTime.AutoSize = true;
            this.txtRecTime.Location = new System.Drawing.Point(76, 19);
            this.txtRecTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtRecTime.Name = "txtRecTime";
            this.txtRecTime.Size = new System.Drawing.Size(43, 13);
            this.txtRecTime.TabIndex = 1;
            this.txtRecTime.Text = "0:00:00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 19);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Rec. Time:";
            // 
            // cmdFinalize
            // 
            this.cmdFinalize.Location = new System.Drawing.Point(331, 114);
            this.cmdFinalize.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmdFinalize.Name = "cmdFinalize";
            this.cmdFinalize.Size = new System.Drawing.Size(79, 24);
            this.cmdFinalize.TabIndex = 14;
            this.cmdFinalize.Text = "Finalize...";
            this.cmdFinalize.UseVisualStyleBackColor = true;
            this.cmdFinalize.Click += new System.EventHandler(this.cmdFinalize_Click);
            // 
            // previewBox
            // 
            this.previewBox.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.previewBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.previewBox.Location = new System.Drawing.Point(364, 162);
            this.previewBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(140, 103);
            this.previewBox.TabIndex = 15;
            this.previewBox.TabStop = false;
            // 
            // cmdSimulate
            // 
            this.cmdSimulate.AutoSize = true;
            this.cmdSimulate.Location = new System.Drawing.Point(170, 169);
            this.cmdSimulate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmdSimulate.Name = "cmdSimulate";
            this.cmdSimulate.Size = new System.Drawing.Size(66, 17);
            this.cmdSimulate.TabIndex = 17;
            this.cmdSimulate.Text = "Simulate";
            this.cmdSimulate.UseVisualStyleBackColor = true;
            this.cmdSimulate.CheckedChanged += new System.EventHandler(this.cmdSimulate_CheckedChanged);
            // 
            // cmdPreview
            // 
            this.cmdPreview.AutoSize = true;
            this.cmdPreview.Checked = true;
            this.cmdPreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cmdPreview.Location = new System.Drawing.Point(360, 142);
            this.cmdPreview.Name = "cmdPreview";
            this.cmdPreview.Size = new System.Drawing.Size(149, 17);
            this.cmdPreview.TabIndex = 18;
            this.cmdPreview.Text = "Preview (during recording)";
            this.cmdPreview.UseVisualStyleBackColor = true;
            this.cmdPreview.Visible = false;
            this.cmdPreview.CheckedChanged += new System.EventHandler(this.cmdPreview_CheckedChanged);
            // 
            // cmdErase
            // 
            this.cmdErase.Location = new System.Drawing.Point(415, 114);
            this.cmdErase.Name = "cmdErase";
            this.cmdErase.Size = new System.Drawing.Size(79, 24);
            this.cmdErase.TabIndex = 19;
            this.cmdErase.Text = "Erase...";
            this.cmdErase.UseVisualStyleBackColor = true;
            this.cmdErase.Click += new System.EventHandler(this.cmdEraseDisc_Click);
            // 
            // cmdExit
            // 
            this.cmdExit.Location = new System.Drawing.Point(399, 275);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(80, 24);
            this.cmdExit.TabIndex = 20;
            this.cmdExit.Text = "Exit";
            this.cmdExit.UseVisualStyleBackColor = true;
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // listDrives
            // 
            this.listDrives.CheckOnClick = true;
            this.listDrives.FormattingEnabled = true;
            this.listDrives.Location = new System.Drawing.Point(243, 18);
            this.listDrives.Name = "listDrives";
            this.listDrives.Size = new System.Drawing.Size(266, 34);
            this.listDrives.TabIndex = 21;
            // 
            // cmdVideoInfo
            // 
            this.cmdVideoInfo.Location = new System.Drawing.Point(247, 114);
            this.cmdVideoInfo.Name = "cmdVideoInfo";
            this.cmdVideoInfo.Size = new System.Drawing.Size(79, 24);
            this.cmdVideoInfo.TabIndex = 22;
            this.cmdVideoInfo.Text = "Video Info";
            this.cmdVideoInfo.UseVisualStyleBackColor = true;
            this.cmdVideoInfo.Click += new System.EventHandler(this.cmdVideoInfo_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(241, 72);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Output folder:";
            // 
            // editFolder
            // 
            this.editFolder.Location = new System.Drawing.Point(243, 88);
            this.editFolder.Name = "editFolder";
            this.editFolder.Size = new System.Drawing.Size(180, 20);
            this.editFolder.TabIndex = 24;
            // 
            // cmdBrowseFolder
            // 
            this.cmdBrowseFolder.Location = new System.Drawing.Point(427, 87);
            this.cmdBrowseFolder.Name = "cmdBrowseFolder";
            this.cmdBrowseFolder.Size = new System.Drawing.Size(32, 23);
            this.cmdBrowseFolder.TabIndex = 25;
            this.cmdBrowseFolder.Text = "...";
            this.cmdBrowseFolder.UseVisualStyleBackColor = true;
            this.cmdBrowseFolder.Click += new System.EventHandler(this.cmdBrowseFolder_Click);
            // 
            // cmdClearFolder
            // 
            this.cmdClearFolder.Location = new System.Drawing.Point(460, 87);
            this.cmdClearFolder.Name = "cmdClearFolder";
            this.cmdClearFolder.Size = new System.Drawing.Size(43, 23);
            this.cmdClearFolder.TabIndex = 26;
            this.cmdClearFolder.Text = "Clear";
            this.cmdClearFolder.UseVisualStyleBackColor = true;
            this.cmdClearFolder.Click += new System.EventHandler(this.cmdClearFolder_Click);
            // 
            // txtUpdatingDrives
            // 
            this.txtUpdatingDrives.AutoSize = true;
            this.txtUpdatingDrives.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtUpdatingDrives.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtUpdatingDrives.Location = new System.Drawing.Point(283, 3);
            this.txtUpdatingDrives.Name = "txtUpdatingDrives";
            this.txtUpdatingDrives.Size = new System.Drawing.Size(59, 13);
            this.txtUpdatingDrives.TabIndex = 27;
            this.txtUpdatingDrives.Text = "Updating...";
            this.txtUpdatingDrives.Visible = false;
            // 
            // RecorderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 301);
            this.Controls.Add(this.txtUpdatingDrives);
            this.Controls.Add(this.cmdClearFolder);
            this.Controls.Add(this.cmdBrowseFolder);
            this.Controls.Add(this.editFolder);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.cmdVideoInfo);
            this.Controls.Add(this.listDrives);
            this.Controls.Add(this.cmdExit);
            this.Controls.Add(this.cmdErase);
            this.Controls.Add(this.cmdPreview);
            this.Controls.Add(this.cmdSimulate);
            this.Controls.Add(this.previewBox);
            this.Controls.Add(this.cmdFinalize);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRecording);
            this.Controls.Add(this.cmdRecord);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "RecorderForm";
            this.Text = "CameraToDVD Recorder (C#)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RecorderForm_FormClosing);
            this.Load += new System.EventHandler(this.RecorderForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox listAudioDev;
        private System.Windows.Forms.Button cmdAudioDevProp;
        private System.Windows.Forms.Button cmdAudioCaptureProp;
        private System.Windows.Forms.ComboBox listVideoDev;
        private System.Windows.Forms.Button cmdVideoDevProp;
        private System.Windows.Forms.Button cmdVideoCaptureProp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdRecord;
        private System.Windows.Forms.Label txtRecording;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label txtRecTime;
        private System.Windows.Forms.Label labelNotDropped;
        private System.Windows.Forms.Label labelDropped;
        private System.Windows.Forms.Label txtNumNotDropped;
        private System.Windows.Forms.Label txtNumDropped;
        private System.Windows.Forms.Label txtAverageFPS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdFinalize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label txtADropped;
        private System.Windows.Forms.Label txtAProcessed;
        private System.Windows.Forms.Label txtACallbacks;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label txtVDropped;
        private System.Windows.Forms.Label txtVProcessed;
        private System.Windows.Forms.Label txtVCallbacks;
        private System.Windows.Forms.Label txtCurrentFPS;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox previewBox;
        private System.Windows.Forms.CheckBox cmdSimulate;
        private System.Windows.Forms.CheckBox cmdPreview;
        private System.Windows.Forms.Button cmdErase;
        private System.Windows.Forms.Button cmdExit;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label txtRemainingSpace;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label txtRemainingTime;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckedListBox listDrives;
        private System.Windows.Forms.Button cmdVideoInfo;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox editFolder;
        private System.Windows.Forms.Button cmdBrowseFolder;
        private System.Windows.Forms.Button cmdClearFolder;
        private System.Windows.Forms.Label txtUpdatingDrives;

    }
}

