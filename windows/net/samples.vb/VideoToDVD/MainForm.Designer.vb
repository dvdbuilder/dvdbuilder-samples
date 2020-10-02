Imports Microsoft.VisualBasic
Imports System
Namespace VideoToDVD
    Partial Public Class MainForm
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

#Region "Windows Form Designer generated code"

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.gbInputFiles = New System.Windows.Forms.GroupBox()
            Me.btnClearFiles = New System.Windows.Forms.Button()
            Me.btnAddFiles = New System.Windows.Forms.Button()
            Me.lvFiles = New System.Windows.Forms.ListView()
            Me.columnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.columnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.gbDVDVideoSettings = New System.Windows.Forms.GroupBox()
            Me.cbTVSystem = New System.Windows.Forms.ComboBox()
            Me.label1 = New System.Windows.Forms.Label()
            Me.gbBurnSettings = New System.Windows.Forms.GroupBox()
            Me.lblFreeSpace = New System.Windows.Forms.Label()
            Me.label5 = New System.Windows.Forms.Label()
            Me.label4 = New System.Windows.Forms.Label()
            Me.lblMediaType = New System.Windows.Forms.Label()
            Me.txtVolumeName = New System.Windows.Forms.TextBox()
            Me.label3 = New System.Windows.Forms.Label()
            Me.cbDevice = New System.Windows.Forms.ComboBox()
            Me.label2 = New System.Windows.Forms.Label()
            Me.btnCreateDVDVideoDisc = New System.Windows.Forms.Button()
            Me.btnStop = New System.Windows.Forms.Button()
            Me.lvLog = New System.Windows.Forms.ListView()
            Me.columnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.columnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.progressBar = New System.Windows.Forms.ProgressBar()
            Me.backgroundWorker = New System.ComponentModel.BackgroundWorker()
            Me.gbIntermediateFiles = New System.Windows.Forms.GroupBox()
            Me.btnBrowseVideoTsFolder = New System.Windows.Forms.Button()
            Me.txtVideoTsFolder = New System.Windows.Forms.TextBox()
            Me.label7 = New System.Windows.Forms.Label()
            Me.btnBrowseDvdProjectFolder = New System.Windows.Forms.Button()
            Me.txtDvdProjectFolder = New System.Windows.Forms.TextBox()
            Me.label6 = New System.Windows.Forms.Label()
            Me.btnCreateDVDBProject = New System.Windows.Forms.Button()
            Me.btnCreateVideoTsFolder = New System.Windows.Forms.Button()
            Me.btnCreateDiscImage = New System.Windows.Forms.Button()
            Me.gbInputFiles.SuspendLayout()
            Me.gbDVDVideoSettings.SuspendLayout()
            Me.gbBurnSettings.SuspendLayout()
            Me.gbIntermediateFiles.SuspendLayout()
            Me.SuspendLayout()
            '
            'gbInputFiles
            '
            Me.gbInputFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.gbInputFiles.Controls.Add(Me.btnClearFiles)
            Me.gbInputFiles.Controls.Add(Me.btnAddFiles)
            Me.gbInputFiles.Controls.Add(Me.lvFiles)
            Me.gbInputFiles.Location = New System.Drawing.Point(12, 12)
            Me.gbInputFiles.Name = "gbInputFiles"
            Me.gbInputFiles.Size = New System.Drawing.Size(718, 150)
            Me.gbInputFiles.TabIndex = 0
            Me.gbInputFiles.TabStop = False
            Me.gbInputFiles.Text = "Input Files"
            '
            'btnClearFiles
            '
            Me.btnClearFiles.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnClearFiles.Location = New System.Drawing.Point(635, 117)
            Me.btnClearFiles.Name = "btnClearFiles"
            Me.btnClearFiles.Size = New System.Drawing.Size(75, 23)
            Me.btnClearFiles.TabIndex = 2
            Me.btnClearFiles.Text = "Clear Files"
            Me.btnClearFiles.UseVisualStyleBackColor = True
            '
            'btnAddFiles
            '
            Me.btnAddFiles.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnAddFiles.Location = New System.Drawing.Point(551, 117)
            Me.btnAddFiles.Name = "btnAddFiles"
            Me.btnAddFiles.Size = New System.Drawing.Size(75, 23)
            Me.btnAddFiles.TabIndex = 1
            Me.btnAddFiles.Text = "Add Files"
            Me.btnAddFiles.UseVisualStyleBackColor = True
            '
            'lvFiles
            '
            Me.lvFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lvFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeader3, Me.columnHeader4})
            Me.lvFiles.FullRowSelect = True
            Me.lvFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.lvFiles.Location = New System.Drawing.Point(9, 19)
            Me.lvFiles.Name = "lvFiles"
            Me.lvFiles.Size = New System.Drawing.Size(703, 90)
            Me.lvFiles.TabIndex = 0
            Me.lvFiles.UseCompatibleStateImageBehavior = False
            Me.lvFiles.View = System.Windows.Forms.View.Details
            '
            'columnHeader3
            '
            Me.columnHeader3.Text = "File"
            Me.columnHeader3.Width = 487
            '
            'columnHeader4
            '
            Me.columnHeader4.Text = "Info"
            Me.columnHeader4.Width = 161
            '
            'gbDVDVideoSettings
            '
            Me.gbDVDVideoSettings.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.gbDVDVideoSettings.Controls.Add(Me.cbTVSystem)
            Me.gbDVDVideoSettings.Controls.Add(Me.label1)
            Me.gbDVDVideoSettings.Location = New System.Drawing.Point(12, 265)
            Me.gbDVDVideoSettings.Name = "gbDVDVideoSettings"
            Me.gbDVDVideoSettings.Size = New System.Drawing.Size(202, 131)
            Me.gbDVDVideoSettings.TabIndex = 3
            Me.gbDVDVideoSettings.TabStop = False
            Me.gbDVDVideoSettings.Text = "DVD-Video Settings"
            '
            'cbTVSystem
            '
            Me.cbTVSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cbTVSystem.FormattingEnabled = True
            Me.cbTVSystem.Items.AddRange(New Object() {"NTSC", "PAL"})
            Me.cbTVSystem.Location = New System.Drawing.Point(80, 19)
            Me.cbTVSystem.Name = "cbTVSystem"
            Me.cbTVSystem.Size = New System.Drawing.Size(102, 21)
            Me.cbTVSystem.TabIndex = 1
            '
            'label1
            '
            Me.label1.AutoSize = True
            Me.label1.Location = New System.Drawing.Point(10, 23)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(61, 13)
            Me.label1.TabIndex = 0
            Me.label1.Text = "TV System:"
            '
            'gbBurnSettings
            '
            Me.gbBurnSettings.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.gbBurnSettings.Controls.Add(Me.lblFreeSpace)
            Me.gbBurnSettings.Controls.Add(Me.label5)
            Me.gbBurnSettings.Controls.Add(Me.label4)
            Me.gbBurnSettings.Controls.Add(Me.lblMediaType)
            Me.gbBurnSettings.Controls.Add(Me.txtVolumeName)
            Me.gbBurnSettings.Controls.Add(Me.label3)
            Me.gbBurnSettings.Controls.Add(Me.cbDevice)
            Me.gbBurnSettings.Controls.Add(Me.label2)
            Me.gbBurnSettings.Location = New System.Drawing.Point(220, 265)
            Me.gbBurnSettings.Name = "gbBurnSettings"
            Me.gbBurnSettings.Size = New System.Drawing.Size(510, 131)
            Me.gbBurnSettings.TabIndex = 4
            Me.gbBurnSettings.TabStop = False
            Me.gbBurnSettings.Text = "Burn Settings"
            '
            'lblFreeSpace
            '
            Me.lblFreeSpace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.lblFreeSpace.Location = New System.Drawing.Point(341, 81)
            Me.lblFreeSpace.Name = "lblFreeSpace"
            Me.lblFreeSpace.Size = New System.Drawing.Size(140, 23)
            Me.lblFreeSpace.TabIndex = 9
            Me.lblFreeSpace.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'label5
            '
            Me.label5.AutoSize = True
            Me.label5.Location = New System.Drawing.Point(270, 86)
            Me.label5.Name = "label5"
            Me.label5.Size = New System.Drawing.Size(65, 13)
            Me.label5.TabIndex = 8
            Me.label5.Text = "Free Space:"
            '
            'label4
            '
            Me.label4.AutoSize = True
            Me.label4.Location = New System.Drawing.Point(15, 53)
            Me.label4.Name = "label4"
            Me.label4.Size = New System.Drawing.Size(39, 13)
            Me.label4.TabIndex = 7
            Me.label4.Text = "Media:"
            '
            'lblMediaType
            '
            Me.lblMediaType.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.lblMediaType.Location = New System.Drawing.Point(95, 48)
            Me.lblMediaType.Name = "lblMediaType"
            Me.lblMediaType.Size = New System.Drawing.Size(386, 23)
            Me.lblMediaType.TabIndex = 6
            Me.lblMediaType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'txtVolumeName
            '
            Me.txtVolumeName.Location = New System.Drawing.Point(95, 84)
            Me.txtVolumeName.Name = "txtVolumeName"
            Me.txtVolumeName.Size = New System.Drawing.Size(160, 20)
            Me.txtVolumeName.TabIndex = 5
            Me.txtVolumeName.Text = "DVDVIDEO"
            '
            'label3
            '
            Me.label3.AutoSize = True
            Me.label3.Location = New System.Drawing.Point(15, 86)
            Me.label3.Name = "label3"
            Me.label3.Size = New System.Drawing.Size(74, 13)
            Me.label3.TabIndex = 4
            Me.label3.Text = "Volume name:"
            '
            'cbDevice
            '
            Me.cbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cbDevice.FormattingEnabled = True
            Me.cbDevice.Location = New System.Drawing.Point(95, 20)
            Me.cbDevice.Name = "cbDevice"
            Me.cbDevice.Size = New System.Drawing.Size(386, 21)
            Me.cbDevice.TabIndex = 3
            '
            'label2
            '
            Me.label2.AutoSize = True
            Me.label2.Location = New System.Drawing.Point(15, 24)
            Me.label2.Name = "label2"
            Me.label2.Size = New System.Drawing.Size(44, 13)
            Me.label2.TabIndex = 2
            Me.label2.Text = "Device:"
            '
            'btnCreateDVDVideoDisc
            '
            Me.btnCreateDVDVideoDisc.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCreateDVDVideoDisc.Location = New System.Drawing.Point(492, 543)
            Me.btnCreateDVDVideoDisc.Name = "btnCreateDVDVideoDisc"
            Me.btnCreateDVDVideoDisc.Size = New System.Drawing.Size(135, 23)
            Me.btnCreateDVDVideoDisc.TabIndex = 5
            Me.btnCreateDVDVideoDisc.Text = "Burn DVD Disc"
            Me.btnCreateDVDVideoDisc.UseVisualStyleBackColor = True
            '
            'btnStop
            '
            Me.btnStop.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnStop.Location = New System.Drawing.Point(652, 543)
            Me.btnStop.Name = "btnStop"
            Me.btnStop.Size = New System.Drawing.Size(75, 23)
            Me.btnStop.TabIndex = 6
            Me.btnStop.Text = "Stop"
            Me.btnStop.UseVisualStyleBackColor = True
            '
            'lvLog
            '
            Me.lvLog.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lvLog.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeader1, Me.columnHeader2})
            Me.lvLog.FullRowSelect = True
            Me.lvLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.lvLog.Location = New System.Drawing.Point(12, 402)
            Me.lvLog.Name = "lvLog"
            Me.lvLog.Size = New System.Drawing.Size(718, 83)
            Me.lvLog.TabIndex = 7
            Me.lvLog.UseCompatibleStateImageBehavior = False
            Me.lvLog.View = System.Windows.Forms.View.Details
            '
            'columnHeader1
            '
            Me.columnHeader1.Text = "Time"
            Me.columnHeader1.Width = 71
            '
            'columnHeader2
            '
            Me.columnHeader2.Text = "Event"
            Me.columnHeader2.Width = 587
            '
            'progressBar
            '
            Me.progressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.progressBar.Location = New System.Drawing.Point(12, 491)
            Me.progressBar.Name = "progressBar"
            Me.progressBar.Size = New System.Drawing.Size(718, 23)
            Me.progressBar.Step = 1
            Me.progressBar.TabIndex = 8
            '
            'backgroundWorker
            '
            Me.backgroundWorker.WorkerReportsProgress = True
            Me.backgroundWorker.WorkerSupportsCancellation = True
            '
            'gbIntermediateFiles
            '
            Me.gbIntermediateFiles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.gbIntermediateFiles.Controls.Add(Me.btnBrowseVideoTsFolder)
            Me.gbIntermediateFiles.Controls.Add(Me.txtVideoTsFolder)
            Me.gbIntermediateFiles.Controls.Add(Me.label7)
            Me.gbIntermediateFiles.Controls.Add(Me.btnBrowseDvdProjectFolder)
            Me.gbIntermediateFiles.Controls.Add(Me.txtDvdProjectFolder)
            Me.gbIntermediateFiles.Controls.Add(Me.label6)
            Me.gbIntermediateFiles.Location = New System.Drawing.Point(12, 168)
            Me.gbIntermediateFiles.Name = "gbIntermediateFiles"
            Me.gbIntermediateFiles.Size = New System.Drawing.Size(718, 81)
            Me.gbIntermediateFiles.TabIndex = 9
            Me.gbIntermediateFiles.TabStop = False
            Me.gbIntermediateFiles.Text = "Intermediate Files"
            '
            'btnBrowseVideoTsFolder
            '
            Me.btnBrowseVideoTsFolder.Location = New System.Drawing.Point(635, 44)
            Me.btnBrowseVideoTsFolder.Name = "btnBrowseVideoTsFolder"
            Me.btnBrowseVideoTsFolder.Size = New System.Drawing.Size(75, 23)
            Me.btnBrowseVideoTsFolder.TabIndex = 5
            Me.btnBrowseVideoTsFolder.Text = "Browse"
            Me.btnBrowseVideoTsFolder.UseVisualStyleBackColor = True
            '
            'txtVideoTsFolder
            '
            Me.txtVideoTsFolder.Location = New System.Drawing.Point(120, 48)
            Me.txtVideoTsFolder.Name = "txtVideoTsFolder"
            Me.txtVideoTsFolder.Size = New System.Drawing.Size(506, 20)
            Me.txtVideoTsFolder.TabIndex = 4
            '
            'label7
            '
            Me.label7.AutoSize = True
            Me.label7.Location = New System.Drawing.Point(10, 54)
            Me.label7.Name = "label7"
            Me.label7.Size = New System.Drawing.Size(92, 13)
            Me.label7.TabIndex = 3
            Me.label7.Text = "VIDEO_TS folder:"
            '
            'btnBrowseDvdProjectFolder
            '
            Me.btnBrowseDvdProjectFolder.Location = New System.Drawing.Point(635, 15)
            Me.btnBrowseDvdProjectFolder.Name = "btnBrowseDvdProjectFolder"
            Me.btnBrowseDvdProjectFolder.Size = New System.Drawing.Size(75, 23)
            Me.btnBrowseDvdProjectFolder.TabIndex = 2
            Me.btnBrowseDvdProjectFolder.Text = "Browse"
            Me.btnBrowseDvdProjectFolder.UseVisualStyleBackColor = True
            '
            'txtDvdProjectFolder
            '
            Me.txtDvdProjectFolder.Location = New System.Drawing.Point(120, 19)
            Me.txtDvdProjectFolder.Name = "txtDvdProjectFolder"
            Me.txtDvdProjectFolder.Size = New System.Drawing.Size(506, 20)
            Me.txtDvdProjectFolder.TabIndex = 1
            '
            'label6
            '
            Me.label6.AutoSize = True
            Me.label6.Location = New System.Drawing.Point(10, 25)
            Me.label6.Name = "label6"
            Me.label6.Size = New System.Drawing.Size(104, 13)
            Me.label6.TabIndex = 0
            Me.label6.Text = "DVDB project folder:"
            '
            'btnCreateDVDBProject
            '
            Me.btnCreateDVDBProject.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCreateDVDBProject.Location = New System.Drawing.Point(12, 543)
            Me.btnCreateDVDBProject.Name = "btnCreateDVDBProject"
            Me.btnCreateDVDBProject.Size = New System.Drawing.Size(135, 23)
            Me.btnCreateDVDBProject.TabIndex = 10
            Me.btnCreateDVDBProject.Text = "Create DVDB project"
            Me.btnCreateDVDBProject.UseVisualStyleBackColor = True
            '
            'btnCreateVideoTsFolder
            '
            Me.btnCreateVideoTsFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCreateVideoTsFolder.Location = New System.Drawing.Point(172, 543)
            Me.btnCreateVideoTsFolder.Name = "btnCreateVideoTsFolder"
            Me.btnCreateVideoTsFolder.Size = New System.Drawing.Size(135, 23)
            Me.btnCreateVideoTsFolder.TabIndex = 11
            Me.btnCreateVideoTsFolder.Text = "Create VIDEO_TS folder"
            Me.btnCreateVideoTsFolder.UseVisualStyleBackColor = True
            '
            'btnCreateDiscImage
            '
            Me.btnCreateDiscImage.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCreateDiscImage.Location = New System.Drawing.Point(332, 543)
            Me.btnCreateDiscImage.Name = "btnCreateDiscImage"
            Me.btnCreateDiscImage.Size = New System.Drawing.Size(135, 23)
            Me.btnCreateDiscImage.TabIndex = 12
            Me.btnCreateDiscImage.Text = "Create DVD Image"
            Me.btnCreateDiscImage.UseVisualStyleBackColor = True
            '
            'MainForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(742, 578)
            Me.Controls.Add(Me.btnCreateDiscImage)
            Me.Controls.Add(Me.btnCreateVideoTsFolder)
            Me.Controls.Add(Me.btnCreateDVDBProject)
            Me.Controls.Add(Me.gbIntermediateFiles)
            Me.Controls.Add(Me.progressBar)
            Me.Controls.Add(Me.lvLog)
            Me.Controls.Add(Me.btnStop)
            Me.Controls.Add(Me.btnCreateDVDVideoDisc)
            Me.Controls.Add(Me.gbBurnSettings)
            Me.Controls.Add(Me.gbDVDVideoSettings)
            Me.Controls.Add(Me.gbInputFiles)
            Me.Name = "MainForm"
            Me.Text = "DVD-Video Creator"
            Me.gbInputFiles.ResumeLayout(False)
            Me.gbDVDVideoSettings.ResumeLayout(False)
            Me.gbDVDVideoSettings.PerformLayout()
            Me.gbBurnSettings.ResumeLayout(False)
            Me.gbBurnSettings.PerformLayout()
            Me.gbIntermediateFiles.ResumeLayout(False)
            Me.gbIntermediateFiles.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

#End Region

        Private gbInputFiles As System.Windows.Forms.GroupBox
        Private lvFiles As System.Windows.Forms.ListView
        Private WithEvents btnClearFiles As System.Windows.Forms.Button
        Private WithEvents btnAddFiles As System.Windows.Forms.Button
        Private gbDVDVideoSettings As System.Windows.Forms.GroupBox
        Private gbBurnSettings As System.Windows.Forms.GroupBox
        Private WithEvents btnCreateDVDVideoDisc As System.Windows.Forms.Button
        Private WithEvents btnStop As System.Windows.Forms.Button
        Private lvLog As System.Windows.Forms.ListView
        Private progressBar As System.Windows.Forms.ProgressBar
        Private label1 As System.Windows.Forms.Label
        Private cbTVSystem As System.Windows.Forms.ComboBox
        Private WithEvents cbDevice As System.Windows.Forms.ComboBox
        Private label2 As System.Windows.Forms.Label
        Private columnHeader1 As System.Windows.Forms.ColumnHeader
        Private columnHeader2 As System.Windows.Forms.ColumnHeader
        Private columnHeader3 As System.Windows.Forms.ColumnHeader
        Private columnHeader4 As System.Windows.Forms.ColumnHeader
        Private WithEvents backgroundWorker As System.ComponentModel.BackgroundWorker
        Private label3 As System.Windows.Forms.Label
        Private txtVolumeName As System.Windows.Forms.TextBox
        Private lblMediaType As System.Windows.Forms.Label
        Private label4 As System.Windows.Forms.Label
        Private label5 As System.Windows.Forms.Label
        Private lblFreeSpace As System.Windows.Forms.Label
        Private gbIntermediateFiles As System.Windows.Forms.GroupBox
        Private WithEvents btnBrowseVideoTsFolder As System.Windows.Forms.Button
        Private txtVideoTsFolder As System.Windows.Forms.TextBox
        Private label7 As System.Windows.Forms.Label
        Private WithEvents btnBrowseDvdProjectFolder As System.Windows.Forms.Button
        Private txtDvdProjectFolder As System.Windows.Forms.TextBox
        Private label6 As System.Windows.Forms.Label
        Private WithEvents btnCreateDVDBProject As System.Windows.Forms.Button
        Private WithEvents btnCreateVideoTsFolder As System.Windows.Forms.Button
        Private WithEvents btnCreateDiscImage As System.Windows.Forms.Button
    End Class
End Namespace

