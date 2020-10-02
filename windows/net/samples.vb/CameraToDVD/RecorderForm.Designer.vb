Imports Microsoft.VisualBasic
Imports System
Namespace CameraToDVD
    Partial Public Class RecorderForm
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.listAudioDev = New System.Windows.Forms.ComboBox()
            Me.cmdAudioDevProp = New System.Windows.Forms.Button()
            Me.cmdAudioCaptureProp = New System.Windows.Forms.Button()
            Me.listVideoDev = New System.Windows.Forms.ComboBox()
            Me.cmdVideoDevProp = New System.Windows.Forms.Button()
            Me.cmdVideoCaptureProp = New System.Windows.Forms.Button()
            Me.groupBox1 = New System.Windows.Forms.GroupBox()
            Me.groupBox2 = New System.Windows.Forms.GroupBox()
            Me.cmdRecord = New System.Windows.Forms.Button()
            Me.txtRecording = New System.Windows.Forms.Label()
            Me.label2 = New System.Windows.Forms.Label()
            Me.groupBox3 = New System.Windows.Forms.GroupBox()
            Me.txtRemainingSpace = New System.Windows.Forms.Label()
            Me.label12 = New System.Windows.Forms.Label()
            Me.txtRemainingTime = New System.Windows.Forms.Label()
            Me.label9 = New System.Windows.Forms.Label()
            Me.groupBox5 = New System.Windows.Forms.GroupBox()
            Me.groupBox4 = New System.Windows.Forms.GroupBox()
            Me.label8 = New System.Windows.Forms.Label()
            Me.label7 = New System.Windows.Forms.Label()
            Me.txtCurrentFPS = New System.Windows.Forms.Label()
            Me.label10 = New System.Windows.Forms.Label()
            Me.txtVDropped = New System.Windows.Forms.Label()
            Me.txtVProcessed = New System.Windows.Forms.Label()
            Me.txtVCallbacks = New System.Windows.Forms.Label()
            Me.txtADropped = New System.Windows.Forms.Label()
            Me.txtAProcessed = New System.Windows.Forms.Label()
            Me.txtACallbacks = New System.Windows.Forms.Label()
            Me.label6 = New System.Windows.Forms.Label()
            Me.label5 = New System.Windows.Forms.Label()
            Me.label4 = New System.Windows.Forms.Label()
            Me.txtAverageFPS = New System.Windows.Forms.Label()
            Me.label1 = New System.Windows.Forms.Label()
            Me.txtNumNotDropped = New System.Windows.Forms.Label()
            Me.txtNumDropped = New System.Windows.Forms.Label()
            Me.labelNotDropped = New System.Windows.Forms.Label()
            Me.labelDropped = New System.Windows.Forms.Label()
            Me.txtRecTime = New System.Windows.Forms.Label()
            Me.label3 = New System.Windows.Forms.Label()
            Me.cmdFinalize = New System.Windows.Forms.Button()
            Me.previewBox = New System.Windows.Forms.PictureBox()
            Me.cmdSimulate = New System.Windows.Forms.CheckBox()
            Me.cmdPreview = New System.Windows.Forms.CheckBox()
            Me.cmdErase = New System.Windows.Forms.Button()
            Me.cmdExit = New System.Windows.Forms.Button()
            Me.listDrives = New System.Windows.Forms.CheckedListBox()
            Me.cmdVideoInfo = New System.Windows.Forms.Button()
            Me.label11 = New System.Windows.Forms.Label()
            Me.editFolder = New System.Windows.Forms.TextBox()
            Me.cmdBrowseFolder = New System.Windows.Forms.Button()
            Me.cmdClearFolder = New System.Windows.Forms.Button()
            Me.txtUpdatingDrives = New System.Windows.Forms.Label()
            Me.groupBox1.SuspendLayout()
            Me.groupBox2.SuspendLayout()
            Me.groupBox3.SuspendLayout()
            CType(Me.previewBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'listAudioDev
            '
            Me.listAudioDev.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.listAudioDev.FormattingEnabled = True
            Me.listAudioDev.Location = New System.Drawing.Point(17, 17)
            Me.listAudioDev.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.listAudioDev.Name = "listAudioDev"
            Me.listAudioDev.Size = New System.Drawing.Size(197, 21)
            Me.listAudioDev.TabIndex = 0
            '
            'cmdAudioDevProp
            '
            Me.cmdAudioDevProp.Location = New System.Drawing.Point(17, 41)
            Me.cmdAudioDevProp.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.cmdAudioDevProp.Name = "cmdAudioDevProp"
            Me.cmdAudioDevProp.Size = New System.Drawing.Size(81, 24)
            Me.cmdAudioDevProp.TabIndex = 1
            Me.cmdAudioDevProp.Text = "Device..."
            Me.cmdAudioDevProp.UseVisualStyleBackColor = True
            '
            'cmdAudioCaptureProp
            '
            Me.cmdAudioCaptureProp.Location = New System.Drawing.Point(110, 41)
            Me.cmdAudioCaptureProp.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.cmdAudioCaptureProp.Name = "cmdAudioCaptureProp"
            Me.cmdAudioCaptureProp.Size = New System.Drawing.Size(81, 24)
            Me.cmdAudioCaptureProp.TabIndex = 2
            Me.cmdAudioCaptureProp.Text = "Capture..."
            Me.cmdAudioCaptureProp.UseVisualStyleBackColor = True
            Me.cmdAudioCaptureProp.Visible = False
            '
            'listVideoDev
            '
            Me.listVideoDev.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.listVideoDev.FormattingEnabled = True
            Me.listVideoDev.Location = New System.Drawing.Point(17, 17)
            Me.listVideoDev.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.listVideoDev.Name = "listVideoDev"
            Me.listVideoDev.Size = New System.Drawing.Size(197, 21)
            Me.listVideoDev.TabIndex = 3
            '
            'cmdVideoDevProp
            '
            Me.cmdVideoDevProp.Location = New System.Drawing.Point(17, 41)
            Me.cmdVideoDevProp.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.cmdVideoDevProp.Name = "cmdVideoDevProp"
            Me.cmdVideoDevProp.Size = New System.Drawing.Size(81, 24)
            Me.cmdVideoDevProp.TabIndex = 4
            Me.cmdVideoDevProp.Text = "Device..."
            Me.cmdVideoDevProp.UseVisualStyleBackColor = True
            '
            'cmdVideoCaptureProp
            '
            Me.cmdVideoCaptureProp.Location = New System.Drawing.Point(110, 41)
            Me.cmdVideoCaptureProp.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.cmdVideoCaptureProp.Name = "cmdVideoCaptureProp"
            Me.cmdVideoCaptureProp.Size = New System.Drawing.Size(81, 24)
            Me.cmdVideoCaptureProp.TabIndex = 5
            Me.cmdVideoCaptureProp.Text = "Capture..."
            Me.cmdVideoCaptureProp.UseVisualStyleBackColor = True
            '
            'groupBox1
            '
            Me.groupBox1.Controls.Add(Me.listAudioDev)
            Me.groupBox1.Controls.Add(Me.cmdAudioDevProp)
            Me.groupBox1.Controls.Add(Me.cmdAudioCaptureProp)
            Me.groupBox1.Location = New System.Drawing.Point(10, 5)
            Me.groupBox1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.groupBox1.Name = "groupBox1"
            Me.groupBox1.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.groupBox1.Size = New System.Drawing.Size(224, 76)
            Me.groupBox1.TabIndex = 6
            Me.groupBox1.TabStop = False
            Me.groupBox1.Text = "Audio Devices"
            '
            'groupBox2
            '
            Me.groupBox2.Controls.Add(Me.listVideoDev)
            Me.groupBox2.Controls.Add(Me.cmdVideoDevProp)
            Me.groupBox2.Controls.Add(Me.cmdVideoCaptureProp)
            Me.groupBox2.Location = New System.Drawing.Point(10, 83)
            Me.groupBox2.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.groupBox2.Name = "groupBox2"
            Me.groupBox2.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.groupBox2.Size = New System.Drawing.Size(224, 72)
            Me.groupBox2.TabIndex = 7
            Me.groupBox2.TabStop = False
            Me.groupBox2.Text = "Video Devices"
            '
            'cmdRecord
            '
            Me.cmdRecord.Location = New System.Drawing.Point(243, 161)
            Me.cmdRecord.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.cmdRecord.Name = "cmdRecord"
            Me.cmdRecord.Size = New System.Drawing.Size(104, 32)
            Me.cmdRecord.TabIndex = 8
            Me.cmdRecord.Text = "Record"
            Me.cmdRecord.UseVisualStyleBackColor = True
            '
            'txtRecording
            '
            Me.txtRecording.AutoSize = True
            Me.txtRecording.ForeColor = System.Drawing.Color.Red
            Me.txtRecording.Location = New System.Drawing.Point(264, 143)
            Me.txtRecording.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtRecording.Name = "txtRecording"
            Me.txtRecording.Size = New System.Drawing.Size(65, 13)
            Me.txtRecording.TabIndex = 9
            Me.txtRecording.Text = "Recording..."
            '
            'label2
            '
            Me.label2.AutoSize = True
            Me.label2.Location = New System.Drawing.Point(241, 4)
            Me.label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.label2.Name = "label2"
            Me.label2.Size = New System.Drawing.Size(40, 13)
            Me.label2.TabIndex = 10
            Me.label2.Text = "Drives:"
            '
            'groupBox3
            '
            Me.groupBox3.Controls.Add(Me.txtRemainingSpace)
            Me.groupBox3.Controls.Add(Me.label12)
            Me.groupBox3.Controls.Add(Me.txtRemainingTime)
            Me.groupBox3.Controls.Add(Me.label9)
            Me.groupBox3.Controls.Add(Me.groupBox5)
            Me.groupBox3.Controls.Add(Me.groupBox4)
            Me.groupBox3.Controls.Add(Me.label8)
            Me.groupBox3.Controls.Add(Me.label7)
            Me.groupBox3.Controls.Add(Me.txtCurrentFPS)
            Me.groupBox3.Controls.Add(Me.label10)
            Me.groupBox3.Controls.Add(Me.txtVDropped)
            Me.groupBox3.Controls.Add(Me.txtVProcessed)
            Me.groupBox3.Controls.Add(Me.txtVCallbacks)
            Me.groupBox3.Controls.Add(Me.txtADropped)
            Me.groupBox3.Controls.Add(Me.txtAProcessed)
            Me.groupBox3.Controls.Add(Me.txtACallbacks)
            Me.groupBox3.Controls.Add(Me.label6)
            Me.groupBox3.Controls.Add(Me.label5)
            Me.groupBox3.Controls.Add(Me.label4)
            Me.groupBox3.Controls.Add(Me.txtAverageFPS)
            Me.groupBox3.Controls.Add(Me.label1)
            Me.groupBox3.Controls.Add(Me.txtNumNotDropped)
            Me.groupBox3.Controls.Add(Me.txtNumDropped)
            Me.groupBox3.Controls.Add(Me.labelNotDropped)
            Me.groupBox3.Controls.Add(Me.labelDropped)
            Me.groupBox3.Controls.Add(Me.txtRecTime)
            Me.groupBox3.Controls.Add(Me.label3)
            Me.groupBox3.Location = New System.Drawing.Point(10, 194)
            Me.groupBox3.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.groupBox3.Name = "groupBox3"
            Me.groupBox3.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.groupBox3.Size = New System.Drawing.Size(343, 103)
            Me.groupBox3.TabIndex = 12
            Me.groupBox3.TabStop = False
            Me.groupBox3.Text = "Statistics"
            '
            'txtRemainingSpace
            '
            Me.txtRemainingSpace.AutoSize = True
            Me.txtRemainingSpace.Location = New System.Drawing.Point(252, 19)
            Me.txtRemainingSpace.Name = "txtRemainingSpace"
            Me.txtRemainingSpace.Size = New System.Drawing.Size(65, 13)
            Me.txtRemainingSpace.TabIndex = 30
            Me.txtRemainingSpace.Text = "0000.00 MB"
            '
            'label12
            '
            Me.label12.AutoSize = True
            Me.label12.Location = New System.Drawing.Point(239, 19)
            Me.label12.Name = "label12"
            Me.label12.Size = New System.Drawing.Size(12, 13)
            Me.label12.TabIndex = 29
            Me.label12.Text = "/"
            '
            'txtRemainingTime
            '
            Me.txtRemainingTime.AutoSize = True
            Me.txtRemainingTime.Location = New System.Drawing.Point(189, 19)
            Me.txtRemainingTime.Name = "txtRemainingTime"
            Me.txtRemainingTime.Size = New System.Drawing.Size(49, 13)
            Me.txtRemainingTime.TabIndex = 28
            Me.txtRemainingTime.Text = "00:00:00"
            '
            'label9
            '
            Me.label9.AutoSize = True
            Me.label9.Location = New System.Drawing.Point(129, 19)
            Me.label9.Name = "label9"
            Me.label9.Size = New System.Drawing.Size(60, 13)
            Me.label9.TabIndex = 27
            Me.label9.Text = "Remaining:"
            '
            'groupBox5
            '
            Me.groupBox5.Location = New System.Drawing.Point(131, 39)
            Me.groupBox5.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.groupBox5.Name = "groupBox5"
            Me.groupBox5.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.groupBox5.Size = New System.Drawing.Size(2, 57)
            Me.groupBox5.TabIndex = 26
            Me.groupBox5.TabStop = False
            '
            'groupBox4
            '
            Me.groupBox4.Location = New System.Drawing.Point(253, 40)
            Me.groupBox4.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.groupBox4.Name = "groupBox4"
            Me.groupBox4.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.groupBox4.Size = New System.Drawing.Size(2, 57)
            Me.groupBox4.TabIndex = 26
            Me.groupBox4.TabStop = False
            '
            'label8
            '
            Me.label8.AutoSize = True
            Me.label8.Location = New System.Drawing.Point(275, 40)
            Me.label8.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.label8.Name = "label8"
            Me.label8.Size = New System.Drawing.Size(34, 13)
            Me.label8.TabIndex = 25
            Me.label8.Text = "Video"
            '
            'label7
            '
            Me.label7.AutoSize = True
            Me.label7.Location = New System.Drawing.Point(198, 40)
            Me.label7.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.label7.Name = "label7"
            Me.label7.Size = New System.Drawing.Size(34, 13)
            Me.label7.TabIndex = 24
            Me.label7.Text = "Audio"
            '
            'txtCurrentFPS
            '
            Me.txtCurrentFPS.Location = New System.Drawing.Point(85, 81)
            Me.txtCurrentFPS.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtCurrentFPS.Name = "txtCurrentFPS"
            Me.txtCurrentFPS.Size = New System.Drawing.Size(42, 13)
            Me.txtCurrentFPS.TabIndex = 21
            Me.txtCurrentFPS.Text = "[curfps]"
            Me.txtCurrentFPS.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'label10
            '
            Me.label10.AutoSize = True
            Me.label10.Location = New System.Drawing.Point(8, 81)
            Me.label10.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.label10.Name = "label10"
            Me.label10.Size = New System.Drawing.Size(61, 13)
            Me.label10.TabIndex = 20
            Me.label10.Text = "Current fps:"
            '
            'txtVDropped
            '
            Me.txtVDropped.Location = New System.Drawing.Point(298, 81)
            Me.txtVDropped.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtVDropped.Name = "txtVDropped"
            Me.txtVDropped.Size = New System.Drawing.Size(40, 13)
            Me.txtVDropped.TabIndex = 19
            Me.txtVDropped.Text = "[vdrop]"
            Me.txtVDropped.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'txtVProcessed
            '
            Me.txtVProcessed.Location = New System.Drawing.Point(298, 67)
            Me.txtVProcessed.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtVProcessed.Name = "txtVProcessed"
            Me.txtVProcessed.Size = New System.Drawing.Size(40, 13)
            Me.txtVProcessed.TabIndex = 18
            Me.txtVProcessed.Text = "[vproc]"
            Me.txtVProcessed.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'txtVCallbacks
            '
            Me.txtVCallbacks.Location = New System.Drawing.Point(307, 53)
            Me.txtVCallbacks.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtVCallbacks.Name = "txtVCallbacks"
            Me.txtVCallbacks.Size = New System.Drawing.Size(31, 13)
            Me.txtVCallbacks.TabIndex = 17
            Me.txtVCallbacks.Text = "[vcb]"
            Me.txtVCallbacks.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'txtADropped
            '
            Me.txtADropped.Location = New System.Drawing.Point(211, 81)
            Me.txtADropped.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtADropped.Name = "txtADropped"
            Me.txtADropped.Size = New System.Drawing.Size(40, 13)
            Me.txtADropped.TabIndex = 16
            Me.txtADropped.Text = "[adrop]"
            Me.txtADropped.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'txtAProcessed
            '
            Me.txtAProcessed.Location = New System.Drawing.Point(211, 67)
            Me.txtAProcessed.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtAProcessed.Name = "txtAProcessed"
            Me.txtAProcessed.Size = New System.Drawing.Size(40, 13)
            Me.txtAProcessed.TabIndex = 15
            Me.txtAProcessed.Text = "[aproc]"
            Me.txtAProcessed.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'txtACallbacks
            '
            Me.txtACallbacks.Location = New System.Drawing.Point(220, 53)
            Me.txtACallbacks.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtACallbacks.Name = "txtACallbacks"
            Me.txtACallbacks.Size = New System.Drawing.Size(31, 13)
            Me.txtACallbacks.TabIndex = 14
            Me.txtACallbacks.Text = "[acb]"
            Me.txtACallbacks.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'label6
            '
            Me.label6.AutoSize = True
            Me.label6.Location = New System.Drawing.Point(145, 80)
            Me.label6.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.label6.Name = "label6"
            Me.label6.Size = New System.Drawing.Size(49, 13)
            Me.label6.TabIndex = 10
            Me.label6.Text = "dropped:"
            '
            'label5
            '
            Me.label5.AutoSize = True
            Me.label5.Location = New System.Drawing.Point(135, 67)
            Me.label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.label5.Name = "label5"
            Me.label5.Size = New System.Drawing.Size(59, 13)
            Me.label5.TabIndex = 9
            Me.label5.Text = "processed:"
            '
            'label4
            '
            Me.label4.AutoSize = True
            Me.label4.Location = New System.Drawing.Point(139, 53)
            Me.label4.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.label4.Name = "label4"
            Me.label4.Size = New System.Drawing.Size(55, 13)
            Me.label4.TabIndex = 8
            Me.label4.Text = "callbacks:"
            '
            'txtAverageFPS
            '
            Me.txtAverageFPS.Location = New System.Drawing.Point(82, 66)
            Me.txtAverageFPS.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtAverageFPS.Name = "txtAverageFPS"
            Me.txtAverageFPS.Size = New System.Drawing.Size(45, 13)
            Me.txtAverageFPS.TabIndex = 7
            Me.txtAverageFPS.Text = "[avgfps]"
            Me.txtAverageFPS.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'label1
            '
            Me.label1.AutoSize = True
            Me.label1.Location = New System.Drawing.Point(2, 66)
            Me.label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(67, 13)
            Me.label1.TabIndex = 6
            Me.label1.Text = "Average fps:"
            '
            'txtNumNotDropped
            '
            Me.txtNumNotDropped.Location = New System.Drawing.Point(76, 52)
            Me.txtNumNotDropped.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtNumNotDropped.Name = "txtNumNotDropped"
            Me.txtNumNotDropped.Size = New System.Drawing.Size(51, 13)
            Me.txtNumNotDropped.TabIndex = 5
            Me.txtNumNotDropped.Text = "[dsvproc]"
            Me.txtNumNotDropped.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'txtNumDropped
            '
            Me.txtNumDropped.Location = New System.Drawing.Point(76, 37)
            Me.txtNumDropped.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtNumDropped.Name = "txtNumDropped"
            Me.txtNumDropped.Size = New System.Drawing.Size(51, 13)
            Me.txtNumDropped.TabIndex = 4
            Me.txtNumDropped.Text = "[dsvdrop]"
            Me.txtNumDropped.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'labelNotDropped
            '
            Me.labelNotDropped.AutoSize = True
            Me.labelNotDropped.Location = New System.Drawing.Point(9, 52)
            Me.labelNotDropped.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.labelNotDropped.Name = "labelNotDropped"
            Me.labelNotDropped.Size = New System.Drawing.Size(60, 13)
            Me.labelNotDropped.TabIndex = 3
            Me.labelNotDropped.Text = "Processed:"
            '
            'labelDropped
            '
            Me.labelDropped.AutoSize = True
            Me.labelDropped.Location = New System.Drawing.Point(18, 37)
            Me.labelDropped.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.labelDropped.Name = "labelDropped"
            Me.labelDropped.Size = New System.Drawing.Size(51, 13)
            Me.labelDropped.TabIndex = 2
            Me.labelDropped.Text = "Dropped:"
            '
            'txtRecTime
            '
            Me.txtRecTime.AutoSize = True
            Me.txtRecTime.Location = New System.Drawing.Point(76, 19)
            Me.txtRecTime.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.txtRecTime.Name = "txtRecTime"
            Me.txtRecTime.Size = New System.Drawing.Size(43, 13)
            Me.txtRecTime.TabIndex = 1
            Me.txtRecTime.Text = "0:00:00"
            '
            'label3
            '
            Me.label3.AutoSize = True
            Me.label3.Location = New System.Drawing.Point(8, 19)
            Me.label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
            Me.label3.Name = "label3"
            Me.label3.Size = New System.Drawing.Size(59, 13)
            Me.label3.TabIndex = 0
            Me.label3.Text = "Rec. Time:"
            '
            'cmdFinalize
            '
            Me.cmdFinalize.Location = New System.Drawing.Point(331, 114)
            Me.cmdFinalize.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.cmdFinalize.Name = "cmdFinalize"
            Me.cmdFinalize.Size = New System.Drawing.Size(79, 24)
            Me.cmdFinalize.TabIndex = 14
            Me.cmdFinalize.Text = "Finalize..."
            Me.cmdFinalize.UseVisualStyleBackColor = True
            '
            'previewBox
            '
            Me.previewBox.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.previewBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.previewBox.Location = New System.Drawing.Point(364, 162)
            Me.previewBox.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.previewBox.Name = "previewBox"
            Me.previewBox.Size = New System.Drawing.Size(140, 103)
            Me.previewBox.TabIndex = 15
            Me.previewBox.TabStop = False
            '
            'cmdSimulate
            '
            Me.cmdSimulate.AutoSize = True
            Me.cmdSimulate.Location = New System.Drawing.Point(170, 169)
            Me.cmdSimulate.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.cmdSimulate.Name = "cmdSimulate"
            Me.cmdSimulate.Size = New System.Drawing.Size(66, 17)
            Me.cmdSimulate.TabIndex = 17
            Me.cmdSimulate.Text = "Simulate"
            Me.cmdSimulate.UseVisualStyleBackColor = True
            '
            'cmdPreview
            '
            Me.cmdPreview.AutoSize = True
            Me.cmdPreview.Checked = True
            Me.cmdPreview.CheckState = System.Windows.Forms.CheckState.Checked
            Me.cmdPreview.Location = New System.Drawing.Point(360, 142)
            Me.cmdPreview.Name = "cmdPreview"
            Me.cmdPreview.Size = New System.Drawing.Size(149, 17)
            Me.cmdPreview.TabIndex = 18
            Me.cmdPreview.Text = "Preview (during recording)"
            Me.cmdPreview.UseVisualStyleBackColor = True
            Me.cmdPreview.Visible = False
            '
            'cmdErase
            '
            Me.cmdErase.Location = New System.Drawing.Point(415, 114)
            Me.cmdErase.Name = "cmdErase"
            Me.cmdErase.Size = New System.Drawing.Size(79, 24)
            Me.cmdErase.TabIndex = 19
            Me.cmdErase.Text = "Erase..."
            Me.cmdErase.UseVisualStyleBackColor = True
            '
            'cmdExit
            '
            Me.cmdExit.Location = New System.Drawing.Point(399, 275)
            Me.cmdExit.Name = "cmdExit"
            Me.cmdExit.Size = New System.Drawing.Size(80, 24)
            Me.cmdExit.TabIndex = 20
            Me.cmdExit.Text = "Exit"
            Me.cmdExit.UseVisualStyleBackColor = True
            '
            'listDrives
            '
            Me.listDrives.CheckOnClick = True
            Me.listDrives.FormattingEnabled = True
            Me.listDrives.Location = New System.Drawing.Point(243, 18)
            Me.listDrives.Name = "listDrives"
            Me.listDrives.Size = New System.Drawing.Size(266, 34)
            Me.listDrives.TabIndex = 21
            '
            'cmdVideoInfo
            '
            Me.cmdVideoInfo.Location = New System.Drawing.Point(247, 114)
            Me.cmdVideoInfo.Name = "cmdVideoInfo"
            Me.cmdVideoInfo.Size = New System.Drawing.Size(79, 24)
            Me.cmdVideoInfo.TabIndex = 22
            Me.cmdVideoInfo.Text = "Video Info"
            Me.cmdVideoInfo.UseVisualStyleBackColor = True
            '
            'label11
            '
            Me.label11.AutoSize = True
            Me.label11.Location = New System.Drawing.Point(241, 72)
            Me.label11.Name = "label11"
            Me.label11.Size = New System.Drawing.Size(71, 13)
            Me.label11.TabIndex = 23
            Me.label11.Text = "Output folder:"
            '
            'editFolder
            '
            Me.editFolder.Location = New System.Drawing.Point(243, 88)
            Me.editFolder.Name = "editFolder"
            Me.editFolder.Size = New System.Drawing.Size(180, 20)
            Me.editFolder.TabIndex = 24
            '
            'cmdBrowseFolder
            '
            Me.cmdBrowseFolder.Location = New System.Drawing.Point(427, 87)
            Me.cmdBrowseFolder.Name = "cmdBrowseFolder"
            Me.cmdBrowseFolder.Size = New System.Drawing.Size(32, 23)
            Me.cmdBrowseFolder.TabIndex = 25
            Me.cmdBrowseFolder.Text = "..."
            Me.cmdBrowseFolder.UseVisualStyleBackColor = True
            '
            'cmdClearFolder
            '
            Me.cmdClearFolder.Location = New System.Drawing.Point(460, 87)
            Me.cmdClearFolder.Name = "cmdClearFolder"
            Me.cmdClearFolder.Size = New System.Drawing.Size(43, 23)
            Me.cmdClearFolder.TabIndex = 26
            Me.cmdClearFolder.Text = "Clear"
            Me.cmdClearFolder.UseVisualStyleBackColor = True
            '
            'txtUpdatingDrives
            '
            Me.txtUpdatingDrives.AutoSize = True
            Me.txtUpdatingDrives.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.txtUpdatingDrives.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
            Me.txtUpdatingDrives.Location = New System.Drawing.Point(283, 3)
            Me.txtUpdatingDrives.Name = "txtUpdatingDrives"
            Me.txtUpdatingDrives.Size = New System.Drawing.Size(59, 13)
            Me.txtUpdatingDrives.TabIndex = 27
            Me.txtUpdatingDrives.Text = "Updating..."
            Me.txtUpdatingDrives.Visible = False
            '
            'RecorderForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(514, 301)
            Me.Controls.Add(Me.txtUpdatingDrives)
            Me.Controls.Add(Me.cmdClearFolder)
            Me.Controls.Add(Me.cmdBrowseFolder)
            Me.Controls.Add(Me.editFolder)
            Me.Controls.Add(Me.label11)
            Me.Controls.Add(Me.cmdVideoInfo)
            Me.Controls.Add(Me.listDrives)
            Me.Controls.Add(Me.cmdExit)
            Me.Controls.Add(Me.cmdErase)
            Me.Controls.Add(Me.cmdPreview)
            Me.Controls.Add(Me.cmdSimulate)
            Me.Controls.Add(Me.previewBox)
            Me.Controls.Add(Me.cmdFinalize)
            Me.Controls.Add(Me.groupBox3)
            Me.Controls.Add(Me.label2)
            Me.Controls.Add(Me.txtRecording)
            Me.Controls.Add(Me.cmdRecord)
            Me.Controls.Add(Me.groupBox2)
            Me.Controls.Add(Me.groupBox1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
            Me.MaximizeBox = False
            Me.Name = "RecorderForm"
            Me.Text = "CameraToDVD Recorder (VB)"
            Me.groupBox1.ResumeLayout(False)
            Me.groupBox2.ResumeLayout(False)
            Me.groupBox3.ResumeLayout(False)
            Me.groupBox3.PerformLayout()
            CType(Me.previewBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

        Private WithEvents listAudioDev As System.Windows.Forms.ComboBox
        Private WithEvents cmdAudioDevProp As System.Windows.Forms.Button
        Private WithEvents cmdAudioCaptureProp As System.Windows.Forms.Button
        Private WithEvents listVideoDev As System.Windows.Forms.ComboBox
        Private WithEvents cmdVideoDevProp As System.Windows.Forms.Button
        Private WithEvents cmdVideoCaptureProp As System.Windows.Forms.Button
        Private groupBox1 As System.Windows.Forms.GroupBox
        Private groupBox2 As System.Windows.Forms.GroupBox
        Private WithEvents cmdRecord As System.Windows.Forms.Button
        Private txtRecording As System.Windows.Forms.Label
        Private label2 As System.Windows.Forms.Label
        Private groupBox3 As System.Windows.Forms.GroupBox
        Private label3 As System.Windows.Forms.Label
        Private txtRecTime As System.Windows.Forms.Label
        Private labelNotDropped As System.Windows.Forms.Label
        Private labelDropped As System.Windows.Forms.Label
        Private txtNumNotDropped As System.Windows.Forms.Label
        Private txtNumDropped As System.Windows.Forms.Label
        Private txtAverageFPS As System.Windows.Forms.Label
        Private label1 As System.Windows.Forms.Label
        Private WithEvents cmdFinalize As System.Windows.Forms.Button
        Private label4 As System.Windows.Forms.Label
        Private label5 As System.Windows.Forms.Label
        Private txtADropped As System.Windows.Forms.Label
        Private txtAProcessed As System.Windows.Forms.Label
        Private txtACallbacks As System.Windows.Forms.Label
        Private label6 As System.Windows.Forms.Label
        Private txtVDropped As System.Windows.Forms.Label
        Private txtVProcessed As System.Windows.Forms.Label
        Private txtVCallbacks As System.Windows.Forms.Label
        Private txtCurrentFPS As System.Windows.Forms.Label
        Private label10 As System.Windows.Forms.Label
        Private groupBox4 As System.Windows.Forms.GroupBox
        Private label8 As System.Windows.Forms.Label
        Private label7 As System.Windows.Forms.Label
        Private previewBox As System.Windows.Forms.PictureBox
        Private WithEvents cmdSimulate As System.Windows.Forms.CheckBox
        Private WithEvents cmdPreview As System.Windows.Forms.CheckBox
        Private WithEvents cmdErase As System.Windows.Forms.Button
        Private WithEvents cmdExit As System.Windows.Forms.Button
        Private groupBox5 As System.Windows.Forms.GroupBox
        Private txtRemainingSpace As System.Windows.Forms.Label
        Private label12 As System.Windows.Forms.Label
        Private txtRemainingTime As System.Windows.Forms.Label
        Private label9 As System.Windows.Forms.Label
        Private listDrives As System.Windows.Forms.CheckedListBox
        Private WithEvents cmdVideoInfo As System.Windows.Forms.Button
        Private label11 As System.Windows.Forms.Label
        Private editFolder As System.Windows.Forms.TextBox
        Private WithEvents cmdBrowseFolder As System.Windows.Forms.Button
        Private WithEvents cmdClearFolder As System.Windows.Forms.Button
        Private txtUpdatingDrives As System.Windows.Forms.Label

    End Class
End Namespace

