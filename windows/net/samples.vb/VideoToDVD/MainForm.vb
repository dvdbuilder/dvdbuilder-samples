Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports PrimoSoftware.Burner
Imports PrimoSoftware.AVBlocks

Namespace VideoToDVD
    Partial Public Class MainForm
        Inherits Form
#Region "Fields"
        Private m_Burner As New Burner()

        Private Enum Task
            CreateDVDProject
            CreateVideoTs
            CreateDiscImage
            BurnDisc
        End Enum

        Private Class VideoDVDCreatorSettings
            Public Task As Task
            Public DeviceIndex As Integer
            Public ImageFileName As String

            Public VolumeName As String
            Public TVSystem As TVSystem
            Public DvdPrjFolder As String
            Public VideoTsFolder As String
            Public InputFiles As New List(Of String)()

            Public ReadOnly Property EncodingPreset() As String
                Get
                    Return If((TVSystem = TVSystem.PAL), Preset.Video.DVD.PAL_4x3_MP2, Preset.Video.DVD.NTSC_4x3_PCM)
                End Get
            End Property
        End Class

        Private m_settings As New VideoDVDCreatorSettings()

#End Region

#Region "ctor/dtor"
        Public Sub New()
            InitializeComponent()
            cbTVSystem.SelectedIndex = 0

            AddHandler m_Burner.Status, AddressOf m_Burner_Status
            AddHandler m_Burner.Continue, AddressOf m_Burner_Continue
            AddHandler m_Burner.ImageProgress, AddressOf m_Burner_ImageProgress
            AddHandler m_Burner.FileProgress, AddressOf m_Burner_FileProgress
            AddHandler m_Burner.FormatProgress, AddressOf m_Burner_FormatProgress
            AddHandler m_Burner.EraseProgress, AddressOf m_Burner_EraseProgress

            Try
                m_Burner.Open()

                Dim devices() As DeviceInfo = m_Burner.EnumerateDevices()

                If devices.Length = 0 Then
                    Throw New Exception("No devices found.")
                End If

                For i As Integer = 0 To devices.Length - 1
                    Dim dev As DeviceInfo = devices(i)
                    cbDevice.Items.Add(dev)
                Next i

                ' Device combo
                cbDevice.SelectedIndex = 0
                UpdateDeviceInformation()
            Catch bme As BurnerException
                ShowError(bme.Message)
            Catch e As Exception
                ShowError(e.Message)
            End Try
        End Sub

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If

            If disposing Then
                ' Close Burner
                m_Burner.Close()
                m_Burner = Nothing
            End If

            MyBase.Dispose(disposing)
        End Sub
#End Region

#Region "Burner event handlers"
        Private Sub m_Burner_EraseProgress(ByVal sender As Object, ByVal e As Burner.EraseEventArgs)
            SetProgress(CInt(Fix(e.Percent)))
        End Sub

        Private Sub m_Burner_FormatProgress(ByVal sender As Object, ByVal e As Burner.FormatEventArgs)
            SetProgress(CInt(Fix(e.Percent)))
        End Sub

        Private Sub m_Burner_FileProgress(ByVal sender As Object, ByVal e As Burner.FileProgressEventArgs)
            ' no progress report
        End Sub

        Private Sub m_Burner_ImageProgress(ByVal sender As Object, ByVal e As Burner.ImageProgressEventArgs)
            If e.All > 0 Then
                SetProgress(CInt(Fix((100 * e.Pos) / e.All)))
            End If
        End Sub

        Private Sub m_Burner_Continue(ByVal sender As Object, ByVal e As CancelEventArgs)
            e.Cancel = backgroundWorker.CancellationPending
        End Sub

        Private Sub m_Burner_Status(ByVal sender As Object, ByVal e As Burner.StatusEventArgs)
            LogEvent(e.Message)
        End Sub

        Private Const WM_DEVICECHANGE As Integer = &H219
        Protected Overrides Sub WndProc(ByRef msg As Message)
            If WM_DEVICECHANGE = msg.Msg Then
                UpdateDeviceInformation()
            End If

            MyBase.WndProc(msg)
        End Sub

        Private Sub UpdateDeviceInformation()
            If -1 = cbDevice.SelectedIndex Then
                Return
            End If

            Try
                Dim dev As DeviceInfo = CType(cbDevice.SelectedItem, DeviceInfo)

                ' Select device. Exclusive access is not required.
                m_Burner.SelectDevice(dev.Index, False)

                Dim freeSpace As Long = m_Burner.MediaFreeSpace * CInt(Fix(BlockSize.Dvd))
                lblFreeSpace.Text = String.Format("{0}GB", (CDbl(freeSpace) / (1000000000.0)).ToString("0.00"))
                lblMediaType.Text = m_Burner.MediaProfileString

                m_Burner.ReleaseDevice()
            Catch bme As BurnerException
                ' Ignore the error when it is DEVICE_ALREADY_SELECTED
                If Burner.DEVICE_ALREADY_SELECTED = bme.ErrorCode Then
                    Return
                End If

                m_Burner.ReleaseDevice()
                ShowError(bme.Message)
            End Try
        End Sub

        Private Sub cbDevice_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbDevice.SelectedIndexChanged
            If -1 <> cbDevice.SelectedIndex Then
                UpdateDeviceInformation()
            End If
        End Sub
#End Region

#Region "helper"
        Private Delegate Sub ShowErrorDelegate(ByVal msg As String)
        Private Sub ShowError(ByVal msg As String)
            If InvokeRequired Then
                Dim d As New ShowErrorDelegate(AddressOf ShowError)
                Me.Invoke(d, New Object() {msg})
            Else
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Sub

        Private Delegate Sub LogEventDelegate(ByVal message As String)
        Private Sub LogEvent(ByVal message As String)
            If InvokeRequired Then
                Dim d As New LogEventDelegate(AddressOf LogEvent)
                Me.Invoke(d, New Object() {message})
            Else
                Dim lvi As New ListViewItem(DateTime.Now.ToLongTimeString())
                lvi.SubItems.Add(message)
                lvLog.Items.Add(lvi)
                lvLog.EnsureVisible(lvLog.Items.Count - 1)
            End If
        End Sub

        Private Sub ClearLogEvents()
            lvLog.Items.Clear()
        End Sub

        Private Delegate Sub SetProgressDelegate(ByVal percent As Integer)
        Private Sub SetProgress(ByVal percent As Integer)
            If InvokeRequired Then
                Dim d As New SetProgressDelegate(AddressOf SetProgress)
                Me.Invoke(d, New Object() {percent})
            Else
                percent = Math.Min(percent, 100)
                percent = Math.Max(percent, 0)
                progressBar.Value = percent
            End If
        End Sub
#End Region

        Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            UpdateUI()
        End Sub

        Private Sub MainForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
            If backgroundWorker.IsBusy Then
                e.Cancel = True
                MessageBox.Show("An operation is in progress. Plese stop the program.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Sub

        Private Sub UpdateUI()
            Dim busy As Boolean = backgroundWorker.IsBusy
            gbInputFiles.Enabled = Not busy
            gbBurnSettings.Enabled = Not busy
            gbDVDVideoSettings.Enabled = Not busy
            gbIntermediateFiles.Enabled = Not busy
            btnStop.Enabled = busy AndAlso Not backgroundWorker.CancellationPending
            btnCreateDVDVideoDisc.Enabled = Not busy
            btnCreateDiscImage.Enabled = Not busy
            btnCreateDVDBProject.Enabled = Not busy
            btnCreateVideoTsFolder.Enabled = Not busy
        End Sub

        Private Function GetVideoStream(ByVal avinfo As PrimoSoftware.AVBlocks.MediaInfo) As PrimoSoftware.AVBlocks.VideoStreamInfo
            For Each stream As StreamInfo In avinfo.Streams
                If stream.MediaType = PrimoSoftware.AVBlocks.MediaType.Video Then
                    Return CType(stream, PrimoSoftware.AVBlocks.VideoStreamInfo)
                End If
            Next stream
            Return Nothing
        End Function

        Private Sub btnAddFiles_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddFiles.Click
            Using dlg As New OpenFileDialog()
                dlg.Filter = "Video files (*.mpg,*.mpeg,*.avi,*.wmv)|*.mpg;*.mpeg;*.avi;*.wmv|All files (*.*)|*.*"
                dlg.Multiselect = True
                If dlg.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                    Dim avinfo As PrimoSoftware.AVBlocks.MediaInfo = New PrimoSoftware.AVBlocks.MediaInfo()

                    For Each fileName As String In dlg.FileNames
                        avinfo.InputFile = fileName

                        If avinfo.Load() Then
                            Dim videoInfo As VideoStreamInfo = GetVideoStream(avinfo)

                            If videoInfo IsNot Nothing Then
                                m_settings.InputFiles.Add(fileName)
                                Dim lvi As New ListViewItem(fileName)
                                lvi.SubItems.Add(FormatVideoInfo(videoInfo))
                                lvFiles.Items.Add(lvi)
                            End If
                        End If
                    Next fileName
                End If
            End Using
        End Sub

        Private Function FormatVideoInfo(ByVal vsi As PrimoSoftware.AVBlocks.VideoStreamInfo) As String
            Return String.Format("{0}x{1} {2:.00}fps {3:.0}sec.", vsi.FrameWidth, vsi.FrameHeight, vsi.FrameRate, vsi.Duration)
        End Function

        Private Sub btnClearFiles_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClearFiles.Click
            m_settings.InputFiles.Clear()
            lvFiles.Items.Clear()
        End Sub

        Private Sub btnBrowseVideoTsFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnBrowseVideoTsFolder.Click
            Using dlg As New FolderBrowserDialog()
                If dlg.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                    txtVideoTsFolder.Text = dlg.SelectedPath
                End If
            End Using
        End Sub

        Private Sub btnBrowseDvdProjectFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnBrowseDvdProjectFolder.Click
            Using dlg As New FolderBrowserDialog()
                If dlg.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                    txtDvdProjectFolder.Text = dlg.SelectedPath
                End If
            End Using
        End Sub

        Private Sub btnCreateDVDVideoDisc_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreateDVDVideoDisc.Click
            ClearLogEvents()

            SetSettings()
            m_settings.Task = Task.BurnDisc

            If (Not ValidateTask()) Then
                Return
            End If

            backgroundWorker.RunWorkerAsync()
            UpdateUI()
        End Sub

        Private Sub btnCreateDiscImage_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreateDiscImage.Click
            ClearLogEvents()

            SetSettings()
            m_settings.ImageFileName = String.Empty
            m_settings.Task = Task.CreateDiscImage

            If (Not ValidateTask()) Then
                Return
            End If

            Using dlg As New SaveFileDialog()
                dlg.Filter = "ISO files (*.iso)|*.iso|All files (*.*)|*.*"
                If dlg.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                    m_settings.ImageFileName = dlg.FileName
                Else
                    Return
                End If
            End Using

            backgroundWorker.RunWorkerAsync()
            UpdateUI()
        End Sub

        Private Sub btnCreateDVDBProject_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreateDVDBProject.Click
            ClearLogEvents()

            SetSettings()
            m_settings.Task = Task.CreateDVDProject

            If (Not ValidateTask()) Then
                Return
            End If

            backgroundWorker.RunWorkerAsync()
            UpdateUI()
        End Sub

        Private Sub btnCreateVideoTsFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreateVideoTsFolder.Click
            ClearLogEvents()

            SetSettings()
            m_settings.Task = Task.CreateVideoTs

            If (Not ValidateTask()) Then
                Return
            End If

            backgroundWorker.RunWorkerAsync()
            UpdateUI()
        End Sub

        Private Function ValidateTask() As Boolean
            Try
                If m_settings.InputFiles.Count = 0 Then
                    ShowError("No input files specified.")
                    Return False
                End If

                If m_settings.Task = Task.BurnDisc Then
                    If (Not ValidateMedia()) Then
                        Return False
                    End If
                End If

                If String.IsNullOrEmpty(m_settings.DvdPrjFolder) OrElse (Not System.IO.Directory.Exists(m_settings.DvdPrjFolder)) Then
                    ShowError("Please select DVDB project folder.")
                    Return False
                End If

                If (m_settings.Task = Task.CreateVideoTs) OrElse (m_settings.Task = Task.CreateDiscImage) OrElse (m_settings.Task = Task.BurnDisc) Then
                    If String.IsNullOrEmpty(m_settings.VideoTsFolder) OrElse (Not System.IO.Directory.Exists(m_settings.VideoTsFolder)) Then
                        ShowError("Please select VIDEO_TS project folder.")
                        Return False
                    End If

                    If (Not IsDirectoryEmpty(m_settings.VideoTsFolder)) Then
                        Dim msg As String = String.Format("The folder  {0}  is not empty. All files and folders within that folder will be deleted. Do you want to continue?", m_settings.VideoTsFolder)
                        If MessageBox.Show(Me, msg, "Delete Files!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> System.Windows.Forms.DialogResult.Yes Then
                            Return False
                        End If

                        CleanDirectory(m_settings.VideoTsFolder)
                    End If
                End If
            Catch ex As Exception
                ShowError(ex.Message)
                Return False
            End Try

            Return True
        End Function

        Private Function IsDirectoryEmpty(ByVal path As String) As Boolean
            Dim di As New System.IO.DirectoryInfo(path)
            Return (di.GetFiles().Length = 0) AndAlso (di.GetDirectories().Length = 0)
        End Function

        Private Sub CleanDirectory(ByVal path As String)
            Dim di As New System.IO.DirectoryInfo(path)

            For Each fi As System.IO.FileInfo In di.GetFiles()
                fi.Delete()
            Next fi

            For Each sdi As System.IO.DirectoryInfo In di.GetDirectories()
                sdi.Delete(True)
            Next sdi
        End Sub

        Private Sub SetSettings()
            If cbDevice.SelectedIndex >= 0 Then
                m_settings.DeviceIndex = (CType(cbDevice.SelectedItem, DeviceInfo)).Index
            End If

            m_settings.VolumeName = txtVolumeName.Text
            m_settings.VideoTsFolder = txtVideoTsFolder.Text
            m_settings.DvdPrjFolder = txtDvdProjectFolder.Text

            If cbTVSystem.SelectedIndex = 0 Then
                m_settings.TVSystem = TVSystem.NTSC
            Else
                m_settings.TVSystem = TVSystem.PAL
            End If
        End Sub

        Private Function ValidateMedia() As Boolean
            If -1 = cbDevice.SelectedIndex Then
                Return False
            End If

            Try
                Dim dev As DeviceInfo = CType(cbDevice.SelectedItem, DeviceInfo)
                m_Burner.SelectDevice(dev.Index, True)

                Select Case m_Burner.MediaProfile
                    Case MediaProfile.DvdPlusR, MediaProfile.DvdPlusRDL, MediaProfile.DvdMinusRSeq, MediaProfile.DvdMinusRwSeq, MediaProfile.DvdMinusRDLJump, MediaProfile.DvdMinusRDLSeq
                        If (Not m_Burner.MediaIsBlank) Then
                            ShowError("The media is not blank. Please insert a blank DVD media.")
                            Return False
                        End If

                    Case MediaProfile.DvdPlusRw, MediaProfile.DvdMinusRwRo, MediaProfile.DvdRam
                        ' random rewriteable, no MediaIsBlank validation

                    Case Else
                        ShowError("There is no DVD media into the drive. Please insert a DVD media (DVD-R, DVD+R, DVD-RW, DVD+RW, DVD-RAM).")
                        Return False
                End Select
            Catch bme As BurnerException
                ShowError(bme.Message)
                Return False
            Finally
                m_Burner.ReleaseDevice()
            End Try

            Return True
        End Function

        Private Sub btnStop_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStop.Click
            backgroundWorker.CancelAsync()
            UpdateUI()
        End Sub

        Private Sub backgroundWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles backgroundWorker.RunWorkerCompleted
            UpdateUI()
            UpdateDeviceInformation()
        End Sub

        Private Sub backgroundWorker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles backgroundWorker.DoWork
            Try
                Dim encodedFiles As List(Of String) = EncodeInputFiles(m_settings.DvdPrjFolder)
                Dim dvdbProjectFile As String = CreateDVDBProject(encodedFiles, m_settings.DvdPrjFolder)

                If backgroundWorker.CancellationPending Then
                    Return
                End If

                Select Case m_settings.Task
                    Case Task.CreateVideoTs
                        CreateVideoTs(dvdbProjectFile, m_settings.VideoTsFolder)

                    Case Task.CreateDiscImage
                        CreateVideoTs(dvdbProjectFile, m_settings.VideoTsFolder)

                        If backgroundWorker.CancellationPending Then
                            Return
                        End If

                        CreateDiscImage(m_settings.VideoTsFolder)

                    Case Task.BurnDisc
                        CreateVideoTs(dvdbProjectFile, m_settings.VideoTsFolder)

                        If backgroundWorker.CancellationPending Then
                            Return
                        End If

                        BurnDisc(m_settings.VideoTsFolder)
                End Select
            Catch ex As Exception
                ShowError(ex.Message)
                LogEvent(ex.Message)
            End Try
        End Sub

        Private Sub BurnDisc(ByVal videoTsFolder As String)
            LogEvent("Burn started.")

            m_Burner.SelectDevice(m_settings.DeviceIndex, True)

            Dim bs As New BurnSettings()
            bs.SourceFolder = videoTsFolder
            bs.VolumeLabel = m_settings.VolumeName

            bs.ImageType = ImageType.UdfIso
            bs.VideoDVD = True
            bs.WriteMethod = WriteMethod.DvdIncremental
            bs.WriteSpeedKB = m_Burner.MaxWriteSpeedKB

            bs.Simulate = False
            bs.CloseDisc = True
            bs.Eject = True

            m_Burner.Burn(bs)

            m_Burner.ReleaseDevice()

            LogEvent("Burn completed.")
        End Sub

        Private Sub CreateDiscImage(ByVal videoTsFolder As String)
            LogEvent("Create image started.")

            Dim cis As New CreateImageSettings()
            cis.SourceFolder = videoTsFolder
            cis.VolumeLabel = m_settings.VolumeName

            cis.ImageType = ImageType.UdfIso
            cis.VideoDVD = True
            cis.ImageFile = m_settings.ImageFileName

            m_Burner.CreateImage(cis)

            LogEvent("Create image completed.")
        End Sub

#Region "EncoderException"
        Public Class EncoderException
            Inherits Exception
            Private m_error As PrimoSoftware.AVBlocks.ErrorInfo


            Public Sub New(ByVal errorInfo As PrimoSoftware.AVBlocks.ErrorInfo)
                m_error = errorInfo
            End Sub

            Public Overrides ReadOnly Property Message() As String
                Get
                    Return m_error.Message
                End Get
            End Property
        End Class
#End Region

        Private Function EncodeInputFiles(ByVal tmpFolder As String) As List(Of String)
            LogEvent("Encoding started.")

            Dim encodedFiles As New List(Of String)()

            For i As Integer = 0 To m_settings.InputFiles.Count - 1
                Using transcoder As Transcoder = New Transcoder()
                    ' In order to use the OEM release for testing (without a valid license) the transcoder demo mode must be enabled.
                    transcoder.AllowDemoMode = True

                    AddHandler transcoder.OnContinue, AddressOf transcoder_OnContinue
                    AddHandler transcoder.OnProgress, AddressOf transcoder_OnProgress

                    Dim inputFile As String = m_settings.InputFiles(i)
                    Dim outputFile As String = System.IO.Path.Combine(tmpFolder, System.IO.Path.GetFileNameWithoutExtension(inputFile) & ".mpg")

                    LogEvent("Encoding " & inputFile)

                    Dim outputSocket As MediaSocket = MediaSocket.FromPreset(m_settings.EncodingPreset)
                    outputSocket.File = outputFile
                    transcoder.Outputs.Add(outputSocket)

                    Dim inputSocket As MediaSocket = New MediaSocket()
                    inputSocket.File = inputFile
                    transcoder.Inputs.Add(inputSocket)

                    If (Not transcoder.Open()) Then
                        Throw New EncoderException(transcoder.Error)
                    End If

                    If (Not transcoder.Run()) Then
                        Throw New EncoderException(transcoder.Error)
                    End If

                    encodedFiles.Add(outputFile)
                End Using
            Next i

            LogEvent("Encoding completed.")

            Return encodedFiles
        End Function

        Private Sub transcoder_OnProgress(ByVal sender As Object, ByVal e As PrimoSoftware.AVBlocks.TranscoderProgressEventArgs)
            If e.TotalTime > 0 Then
                Dim percent As Integer = CInt(Fix((e.CurrentTime * 100) / e.TotalTime))
                SetProgress(percent)
            End If
        End Sub

        Private Sub transcoder_OnContinue(ByVal sender As Object, ByVal e As PrimoSoftware.AVBlocks.TranscoderContinueEventArgs)
            e.Continue = Not backgroundWorker.CancellationPending
        End Sub

        Private Sub CreateVideoTs(ByVal projectFile As String, ByVal outputFolder As String)
            LogEvent("DVD-Video building started.")

            Using dvdBuilder As New PrimoSoftware.DVDBuilder.DVDBuilder()
                dvdBuilder.ProjectFile = projectFile
                dvdBuilder.OutputFolder = outputFolder

                AddHandler dvdBuilder.OnContinue, AddressOf dvdBuilder_OnContinue
                AddHandler dvdBuilder.OnProgress, AddressOf dvdBuilder_OnProgress
                AddHandler dvdBuilder.OnStatus, AddressOf dvdBuilder_OnStatus

                If (Not dvdBuilder.Build()) Then
                    Throw New DVDBuilderException(dvdBuilder.Error.Code, CInt(Fix(dvdBuilder.Error.Facility)), dvdBuilder.Error.Hint)
                End If
            End Using

            LogEvent("DVD-Video building completed.")
        End Sub

        Private Function CreateDVDBProject(ByVal encodedFiles As List(Of String), ByVal encodedFilesTmpFolder As String) As String
            Dim projectFile As String = System.IO.Path.Combine(encodedFilesTmpFolder, "project.xml")

            Dim projectCreator As New DVDBuilderProject()
            projectCreator.TVSystem = m_settings.TVSystem

            projectCreator.Create(projectFile, encodedFiles, encodedFilesTmpFolder)
            Return projectFile
        End Function

#Region "DVDBuilderException"
        Public Class DVDBuilderException
            Inherits Exception
            Private m_message As String
            Private m_error As Integer
            Private m_errorFacility As Integer
            Private m_hint As String

            Public Sub New(ByVal [error] As Integer, ByVal errorFacility As Integer, ByVal hint As String)
                m_error = [error]
                m_errorFacility = errorFacility
                m_hint = hint

                Select Case m_errorFacility
                    Case CInt(Fix(PrimoSoftware.DVDBuilder.ErrorFacility.Success))
                        m_message = "Success."
                        Return

                    Case CInt(Fix(PrimoSoftware.DVDBuilder.ErrorFacility.SystemWindows))
                        Dim sysex As New System.ComponentModel.Win32Exception(m_error)
                        m_message = "System error: " & sysex.Message
                        Return

                    Case CInt(Fix(PrimoSoftware.DVDBuilder.ErrorFacility.DVDBuilder))
                        m_message = "DVDBuilderError: " & (CType(m_error, PrimoSoftware.DVDBuilder.DVDBuilderError)).ToString()
                        Return

                    Case Else
                        m_message = "Unknown error."
                        Return
                End Select
            End Sub

            Public Overrides ReadOnly Property Message() As String
                Get
                    Return m_message
                End Get
            End Property

            Public ReadOnly Property ErrorFacility() As Integer
                Get
                    Return m_errorFacility
                End Get
            End Property

            Public ReadOnly Property [Error]() As Integer
                Get
                    Return m_error
                End Get
            End Property
        End Class
#End Region

        Private Sub dvdBuilder_OnStatus(ByVal sender As Object, ByVal e As PrimoSoftware.DVDBuilder.DVDBuilderStatusEventArgs)
            Select Case e.Status
                Case PrimoSoftware.DVDBuilder.DVDBuilderStatus.WritingVOB
                    LogEvent("WritingVOB")
                Case PrimoSoftware.DVDBuilder.DVDBuilderStatus.WritingIFO
                    LogEvent("WritingIFO")
            End Select
        End Sub

        Private Sub dvdBuilder_OnProgress(ByVal sender As Object, ByVal e As PrimoSoftware.DVDBuilder.DVDBuilderProgressEventArgs)
            SetProgress(e.Percent)
        End Sub

        Private Sub dvdBuilder_OnContinue(ByVal sender As Object, ByVal e As PrimoSoftware.DVDBuilder.DVDBuilderContinueEventArgs)
            e.Continue = Not backgroundWorker.CancellationPending
        End Sub
    End Class
End Namespace
