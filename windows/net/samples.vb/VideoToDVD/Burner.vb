Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel
Imports System.IO
Imports System.Collections
Imports System.Collections.Generic
Imports PrimoSoftware.Burner

Namespace VideoToDVD
    ''' <summary>
    ''' Container for device information
    ''' </summary>
    Public Structure DeviceInfo
        ''' <summary>
        ''' Device index
        ''' </summary>
        Public Index As Integer

        ''' <summary>
        ''' Device description
        ''' </summary>
        Public Title As String

        ''' <summary>
        ''' Returns string representation of this object
        ''' </summary>
        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Structure

    ''' <summary>
    ''' Container for speed information
    ''' </summary>
    Public Structure SpeedInfo
        Public TransferRateKB As Double
        Public TransferRate1xKB As Double
        Public Overrides Function ToString() As String
            Return String.Format("{0}x", Math.Round(CDbl(TransferRateKB) / TransferRate1xKB, 1))
        End Function
    End Structure

    Public Class Burner
#Region "Construct / Finalize"
        Public Sub New()
        End Sub

        Protected Overrides Sub Finalize()
            ' Close
            If m_isOpen Then
                Close()
            End If
        End Sub
#End Region

#Region "Public Events"
        Public Class StatusEventArgs
            Inherits EventArgs
            Public Sub New(ByVal message As String)
                Me.Message = message
            End Sub

            Public Message As String
        End Class

        Public Class ImageProgressEventArgs
            Inherits EventArgs
            Public Sub New(ByVal pos As Long, ByVal all As Long)
                Me.Pos = pos
                Me.All = all
            End Sub

            Public Pos As Long
            Public All As Long
        End Class

        Public Class FileProgressEventArgs
            Inherits EventArgs
            Public Sub New(ByVal file As Integer, ByVal fileName As String, ByVal percentCompleted As Integer)
                FileNumber = file
                Me.FileName = fileName
                PpercentCompleted = percentCompleted
            End Sub

            Public FileNumber As Integer
            Public FileName As String
            Public PpercentCompleted As Integer
        End Class


        Public Class FormatEventArgs
            Inherits EventArgs
            Public Sub New(ByVal percent As Double)
                Me.Percent = percent
            End Sub

            Public Percent As Double
        End Class

        Public Class EraseEventArgs
            Inherits EventArgs
            Public Sub New(ByVal percent As Double)
                Me.Percent = percent
            End Sub

            Public Percent As Double
        End Class

        Public Event Status As EventHandler(Of StatusEventArgs)
        Public Event ImageProgress As EventHandler(Of ImageProgressEventArgs)
        Public Event FileProgress As EventHandler(Of FileProgressEventArgs)
        Public Event FormatProgress As EventHandler(Of FormatEventArgs)
        Public Event EraseProgress As EventHandler(Of EraseEventArgs)
        Public Event [Continue] As EventHandler(Of CancelEventArgs)

        Private Sub FireStatus(ByVal message As String)
            RaiseEvent Status(Me, New StatusEventArgs(message))
        End Sub

        Private Sub FireImageProgress(ByVal pos As Long, ByVal all As Long)
            RaiseEvent ImageProgress(Me, New ImageProgressEventArgs(pos, all))
        End Sub

        Private Sub FireFileProgress(ByVal file As Integer, ByVal fileName As String, ByVal percentCompleted As Integer)
            RaiseEvent FileProgress(Me, New FileProgressEventArgs(file, fileName, percentCompleted))
        End Sub

        Private Sub FireFormatProgress(ByVal percent As Double)
            RaiseEvent FormatProgress(Me, New FormatEventArgs(percent))
        End Sub

        Private Sub FireEraseProgress(ByVal percent As Double)
            RaiseEvent EraseProgress(Me, New EraseEventArgs(percent))
        End Sub
#End Region

#Region "Public Properties"
        Public ReadOnly Property IsOpen() As Boolean
            Get
                Return m_isOpen
            End Get
        End Property

        Public ReadOnly Property MediaIsBlank() As Boolean
            Get
                If Nothing Is m_device Then
                    Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
                End If

                Return m_device.MediaIsBlank
            End Get
        End Property

        Public ReadOnly Property MediaIsFullyFormatted() As Boolean
            Get
                If Nothing Is m_device Then
                    Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
                End If

                ' Get media profile
                Dim mp As MediaProfile = m_device.MediaProfile

                ' DVD+RW
                If MediaProfile.DvdPlusRw = mp Then
                    Return (BgFormatStatus.Completed = m_device.BgFormatStatus)
                End If

                ' DVD-RW for Restricted Overwrite
                If MediaProfile.DvdMinusRwRo = mp Then
                    Return (m_device.MediaFreeSpace = m_device.MediaCapacity)
                End If

                Return False
            End Get
        End Property

        Public ReadOnly Property DeviceCacheSize() As Integer
            Get
                If Nothing Is m_device Then
                    Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
                End If

                Return m_device.InternalCacheCapacity
            End Get
        End Property

        Public ReadOnly Property DeviceCacheUsedSpace() As Integer
            Get
                If Nothing Is m_device Then
                    Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
                End If

                Return m_device.InternalCacheUsedSpace
            End Get
        End Property

        Public ReadOnly Property WriteTransferKB() As Integer
            Get
                If Nothing Is m_device Then
                    Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
                End If

                Return m_device.WriteTransferRate
            End Get
        End Property

        Public ReadOnly Property MediaFreeSpace() As Long
            Get
                If Nothing Is m_device Then
                    Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
                End If

                Return m_device.MediaFreeSpace
            End Get
        End Property

        Public ReadOnly Property MaxWriteSpeedKB() As Integer
            Get
                If Nothing Is m_device Then
                    Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
                End If

                Return m_device.MaxWriteSpeedKB
            End Get
        End Property

        Public ReadOnly Property MediaProfile() As PrimoSoftware.Burner.MediaProfile
            Get
                If Nothing Is m_device Then
                    Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
                End If

                Return m_device.MediaProfile
            End Get
        End Property

        Public ReadOnly Property MediaProfileString() As String
            Get
                Dim profile As PrimoSoftware.Burner.MediaProfile = Me.MediaProfile
                Select Case profile
                    Case MediaProfile.CdRom
                        Return "CD-ROM. Read only CD."

                    Case MediaProfile.CdR
                        Return "CD-R. Write once CD."

                    Case MediaProfile.CdRw
                        Return "CD-RW. Re-writable CD."

                    Case MediaProfile.DvdRom
                        Return "DVD-ROM. Read only DVD."

                    Case MediaProfile.DvdMinusRSeq
                        Return "DVD-R Sequential Recording. Write once DVD."

                    Case MediaProfile.DvdMinusRDLSeq
                        Return "DVD-R DL 8.54GB for Sequential Recording. Write once DVD."

                    Case MediaProfile.DvdMinusRDLJump
                        Return "DVD-R DL 8.54GB for Layer Jump Recording. Write once DVD."

                    Case MediaProfile.DvdRam
                        Return "DVD-RAM ReWritable DVD."

                    Case MediaProfile.DvdMinusRwRo
                        Return "DVD-RW Restricted Overwrite ReWritable. ReWritable DVD using restricted overwrite."

                    Case MediaProfile.DvdMinusRwSeq
                        Return "DVD-RW Sequential Recording ReWritable. ReWritable DVD using sequential recording."

                    Case MediaProfile.DvdPlusRw
                        Dim fmt As BgFormatStatus = m_device.BgFormatStatus
                        Select Case fmt
                            Case BgFormatStatus.NotFormatted
                                Return "DVD+RW ReWritable DVD. Not formatted."
                            Case BgFormatStatus.Partial
                                Return "DVD+RW ReWritable DVD. Partially formatted."
                            Case BgFormatStatus.Pending
                                Return "DVD+RW ReWritable DVD. Background formatting is pending ..."
                            Case BgFormatStatus.Completed
                                Return "DVD+RW ReWritable DVD. Formatted."
                        End Select
                        Return "DVD+RW ReWritable DVD."

                    Case MediaProfile.DvdPlusR
                        Return "DVD+R. Write once DVD."

                    Case MediaProfile.DvdPlusRDL
                        Return "DVD+R DL 8.5GB. Write once DVD."

                    Case Else
                        Return "Unknown Profile."
                End Select
            End Get
        End Property

#End Region

#Region "Public Methods"
        Public Sub Open()
            If m_isOpen Then
                Return
            End If

            m_engine = New Engine()
            If (Not m_engine.Initialize()) Then
                m_engine.Dispose()
                m_engine = Nothing

                Throw New BurnerException(ENGINE_INITIALIZATION, ENGINE_INITIALIZATION_TEXT)
            End If

            m_isOpen = True
        End Sub

        Public Sub Close()
            If Nothing IsNot m_device Then
                m_device.Dispose()
            End If
            m_device = Nothing

            If Nothing IsNot m_engine Then
                m_engine.Shutdown()
                m_engine.Dispose()
            End If
            m_engine = Nothing

            m_isOpen = False
        End Sub

        Public Function EnumerateDevices() As DeviceInfo()
            If (Not m_isOpen) Then
                Throw New BurnerException(BURNER_NOT_OPEN, BURNER_NOT_OPEN_TEXT)
            End If

            m_deviceArray.Clear()

            Using enumerator As DeviceEnumerator = m_engine.CreateDeviceEnumerator()
                Dim devices As Integer = enumerator.Count
                If 0 = devices Then
                    Throw New BurnerException(NO_DEVICES, NO_DEVICES_TEXT)
                End If

                For i As Integer = 0 To devices - 1
                    Using device As Device = enumerator.CreateDevice(i)
                        If Nothing IsNot device Then
                            Dim dev As New DeviceInfo()
                            dev.Index = i
                            dev.Title = GetDeviceTitle(device)
                            m_deviceArray.Add(dev)
                        End If
                    End Using
                Next i
            End Using

            Return CType(m_deviceArray.ToArray(GetType(DeviceInfo)), DeviceInfo())
        End Function

        Public Sub SelectDevice(ByVal deviceIndex As Integer, ByVal exclusive As Boolean)
            If Nothing IsNot m_device Then
                Throw New BurnerException(DEVICE_ALREADY_SELECTED, DEVICE_ALREADY_SELECTED_TEXT)
            End If

            Using enumerator As DeviceEnumerator = m_engine.CreateDeviceEnumerator()
                Dim dev As Device = enumerator.CreateDevice(deviceIndex, exclusive)
                If Nothing Is dev Then
                    Throw New BurnerException(INVALID_DEVICE_INDEX, INVALID_DEVICE_INDEX_TEXT)
                End If

                m_device = dev
            End Using
        End Sub

        Public Sub ReleaseDevice()
            If Nothing IsNot m_device Then
                m_device.Dispose()
            End If

            m_device = Nothing
        End Sub

        Public Function EnumerateWriteSpeeds() As SpeedInfo()
            If Nothing Is m_device Then
                Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
            End If

            m_speedArray.Clear()

            Dim speeds As IList(Of SpeedDescriptor) = m_device.GetWriteSpeeds()

            For i As Integer = 0 To speeds.Count - 1
                Dim speed As SpeedDescriptor = speeds(i)

                Dim speedInfo As New SpeedInfo()
                speedInfo.TransferRateKB = speed.TransferRateKB
                If m_device.MediaIsDVD Then
                    speedInfo.TransferRate1xKB = Speed1xKB.DVD
                ElseIf m_device.MediaIsCD Then
                    speedInfo.TransferRate1xKB = Speed1xKB.CD
                Else
                    speedInfo.TransferRate1xKB = Speed1xKB.BD
                End If

                m_speedArray.Add(speedInfo)

            Next i

            Return CType(m_speedArray.ToArray(GetType(SpeedInfo)), SpeedInfo())
        End Function

        Public Function CalculateImageSize(ByVal settings As BurnSettings) As Long
            Using dataDisc As DataDisc = New DataDisc()
                dataDisc.ImageType = settings.ImageType
                SetVolumeProperties(dataDisc, settings.VolumeLabel)
                SetImageLayoutFromFolder(dataDisc, settings.VideoDVD, settings.SourceFolder)
                Return dataDisc.ImageSizeInBytes
            End Using
        End Function

        Public Sub Eject()
            If Nothing Is m_device Then
                Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
            End If

            m_device.Eject(True)
        End Sub

        Public Sub CloseTray()
            If Nothing Is m_device Then
                Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
            End If

            m_device.Eject(False)
        End Sub

        Public Sub Burn(ByVal settings As BurnSettings)
            If Nothing Is m_device Then
                Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
            End If

            Using dataDisc As DataDisc = New DataDisc()
                ' Add event handlers
                AddHandler dataDisc.OnStatus, AddressOf DataDisc_OnStatus
                AddHandler dataDisc.OnFileStatus, AddressOf DataDisc_OnFileStatus
                AddHandler dataDisc.OnProgress, AddressOf DataDisc_OnProgress
                AddHandler dataDisc.OnContinueBurn, AddressOf DataDisc_OnContinueBurn

                FormatMedia(m_device)

                m_device.WriteSpeedKB = settings.WriteSpeedKB

                dataDisc.Device = m_device
                dataDisc.SimulateBurn = settings.Simulate
                dataDisc.WriteMethod = settings.WriteMethod
                dataDisc.CloseDisc = settings.CloseDisc

                ' Set the session start address. This must be done before intializing the file system.
                dataDisc.SessionStartAddress = m_device.NewSessionStartAddress

                ' Set burning parameters
                dataDisc.ImageType = settings.ImageType
                SetVolumeProperties(dataDisc, settings.VolumeLabel)
                ' Set image layout
                SetImageLayoutFromFolder(dataDisc, settings.VideoDVD, settings.SourceFolder)

                If (Not dataDisc.WriteToDisc(True)) Then
                    Throw New BurnerException(dataDisc.Error)
                End If

                If settings.Eject Then
                    m_device.Eject(True)
                End If
            End Using
        End Sub

        Public Sub CreateImage(ByVal settings As CreateImageSettings)
            Using data As DataDisc = New DataDisc()
                ' Add event handlers
                AddHandler data.OnStatus, AddressOf DataDisc_OnStatus
                AddHandler data.OnFileStatus, AddressOf DataDisc_OnFileStatus
                AddHandler data.OnProgress, AddressOf DataDisc_OnProgress
                AddHandler data.OnContinueBurn, AddressOf DataDisc_OnContinueBurn

                data.ImageType = settings.ImageType
                SetVolumeProperties(data, settings.VolumeLabel)
                ' Create image file system
                SetImageLayoutFromFolder(data, settings.VideoDVD, settings.SourceFolder)

                ' Create the image file
                If (Not data.CreateImageFile(settings.ImageFile)) Then
                    Throw New BurnerException(data.Error)
                End If
            End Using
        End Sub

        Public Sub [Erase](ByVal settings As EraseSettings)
            If Nothing Is m_device Then
                Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
            End If

            Dim mp As MediaProfile = m_device.MediaProfile

            If MediaProfile.DvdMinusRwSeq <> mp AndAlso MediaProfile.DvdMinusRwRo <> mp AndAlso MediaProfile.CdRw <> mp Then
                Throw New BurnerException(ERASE_NOT_SUPPORTED, ERASE_NOT_SUPPORTED_TEXT)
            End If

            If m_device.MediaIsBlank AndAlso (Not settings.Force) Then
                Return
            End If

            AddHandler m_device.OnErase, AddressOf Device_Erase

            Dim et As EraseType = EraseType.Minimal

            If settings.Quick Then
                et = EraseType.Minimal
            Else
                et = EraseType.Disc
            End If

            Dim bRes As Boolean = m_device.Erase(et)

            RemoveHandler m_device.OnErase, AddressOf Device_Erase

            If (Not bRes) Then
                Throw New BurnerException(m_device.Error)
            End If

            ' Refresh to reload disc information
            m_device.Refresh()
        End Sub

        Public Sub Format(ByVal settings As FormatSettings)
            If Nothing Is m_device Then
                Throw New BurnerException(NO_DEVICE, NO_DEVICE_TEXT)
            End If

            Dim mp As MediaProfile = m_device.MediaProfile

            If MediaProfile.DvdMinusRwSeq <> mp AndAlso MediaProfile.DvdMinusRwRo <> mp AndAlso MediaProfile.DvdPlusRw <> mp Then
                Throw New BurnerException(FORMAT_NOT_SUPPORTED, FORMAT_NOT_SUPPORTED_TEXT)
            End If

            AddHandler m_device.OnFormat, AddressOf Device_Format

            Dim bRes As Boolean = True
            Select Case mp
                Case MediaProfile.DvdMinusRwRo
                    If settings.Quick Then
                        bRes = m_device.Format(FormatType.DvdMinusRwQuick)
                    Else
                        bRes = m_device.Format(FormatType.DvdMinusRwFull)
                    End If
                Case MediaProfile.DvdMinusRwSeq
                    If settings.Quick Then
                        bRes = m_device.Format(FormatType.DvdMinusRwQuick)
                    Else
                        bRes = m_device.Format(FormatType.DvdMinusRwFull)
                    End If

                Case MediaProfile.DvdPlusRw
                    Dim fmt As BgFormatStatus = m_device.BgFormatStatus
                    Select Case fmt
                        Case BgFormatStatus.Completed
                            If settings.Force Then
                                bRes = m_device.Format(FormatType.DvdPlusRwFull, 0, (Not settings.Quick))
                            End If
                        Case BgFormatStatus.NotFormatted
                            bRes = m_device.Format(FormatType.DvdPlusRwFull, 0, (Not settings.Quick))
                        Case BgFormatStatus.Partial
                            bRes = m_device.Format(FormatType.DvdPlusRwRestart, 0, (Not settings.Quick))
                    End Select
            End Select

            RemoveHandler m_device.OnFormat, AddressOf Device_Format

            If (Not bRes) Then
                Throw New BurnerException(m_device.Error)
            End If

            ' Refresh to reload disc information
            m_device.Refresh()
        End Sub
#End Region

#Region "Device Event Handlers"

        Private Sub Device_Format(ByVal sender As Object, ByVal e As DeviceFormatEventArgs)
            FireFormatProgress(e.Progress)
        End Sub

        Public Sub Device_Erase(ByVal sender As Object, ByVal e As DeviceEraseEventArgs)
            FireEraseProgress(e.Progress)
        End Sub

#End Region

#Region "DataDisc Event Handlers"
        Public Sub DataDisc_OnStatus(ByVal sender As Object, ByVal e As DataDiscStatusEventArgs)
            FireStatus(GetDataDiscStatusString(e.Status))
        End Sub

        Public Sub DataDisc_OnFileStatus(ByVal sender As Object, ByVal e As DataDiscFileStatusEventArgs)
            FireFileProgress(e.FileNumber, e.FileName, e.PercentWritten)
        End Sub

        Public Sub DataDisc_OnProgress(ByVal sender As Object, ByVal e As DataDiscProgressEventArgs)
            FireImageProgress(e.Position, e.All)
        End Sub

        Public Sub DataDisc_OnContinueBurn(ByVal sender As Object, ByVal e As DataDiscContinueEventArgs)
            If Nothing Is ContinueEvent Then
                Return
            End If

            Dim args As New CancelEventArgs()
            RaiseEvent Continue(Me, args)

            e.Continue = Not args.Cancel
        End Sub
#End Region

#Region "Private Methods"
        Private Function GetDeviceTitle(ByVal device As Device) As String
            Return String.Format("({0}:) - {1}", device.DriveLetter, device.Description)
        End Function


        Private Sub CreateFileTree(ByVal currentDirectory As DataFile, ByVal currentPath As String)
            Const allImageTypes As ImageType = (ImageType.Iso9660 Or ImageType.Joliet Or ImageType.Udf)

            Dim filesAndDirs As New ArrayList()
            filesAndDirs.AddRange(Directory.GetFiles(currentPath, "*"))
            filesAndDirs.AddRange(Directory.GetDirectories(currentPath, "*"))

            For Each path As String In filesAndDirs
                If Directory.Exists(path) Then
                    ' Get directory information
                    Dim di As New DirectoryInfo(path)

                    ' Create a new directory
                    Dim newDirectory As New DataFile()

                    newDirectory.IsDirectory = True
                    newDirectory.LongFilename = di.Name

                    newDirectory.FilePath = di.Name
                    newDirectory.FileTime = di.CreationTime

                    If (di.Attributes And FileAttributes.Hidden) = FileAttributes.Hidden Then
                        newDirectory.HiddenMask = CInt(Fix(allImageTypes))
                    End If

                    ' Call CreateFileTree recursively to process all the files from the new directory
                    CreateFileTree(newDirectory, di.FullName)

                    ' Add the new directory to the image tree
                    currentDirectory.Children.Add(newDirectory)
                Else
                    ' Get file information
                    Dim fi As New FileInfo(path)
                    ' Create a new file
                    Dim newFile As New DataFile()
                    newFile.IsDirectory = False
                    newFile.LongFilename = fi.Name

                    newFile.FilePath = fi.FullName
                    newFile.FileTime = fi.CreationTime

                    If (fi.Attributes And FileAttributes.Hidden) = FileAttributes.Hidden Then
                        newFile.HiddenMask = CInt(Fix(allImageTypes))
                    End If

                    ' Add the new file to the image tree
                    currentDirectory.Children.Add(newFile)
                End If
            Next path
        End Sub

        Private Sub SetImageLayoutFromFolder(ByVal data As DataDisc, ByVal isVideoDVD As Boolean, ByVal sourceFolder As String)
            If isVideoDVD Then
                data.DvdVideo = True
                data.CloseDisc = True
            End If

            Dim fileSystemRoot As New DataFile()

            ' Create directory structure

            ' Set up the root of the file system
            fileSystemRoot.IsDirectory = True
            fileSystemRoot.LongFilename = "\"
            fileSystemRoot.FilePath = "\"

            ' Import all files
            CreateFileTree(fileSystemRoot, sourceFolder)

            ' Set image layout
            If isVideoDVD Then
                Using dvd As VideoDVD = New VideoDVD()
                    ' Pass the raw layout to VideoDVD
                    If (Not dvd.SetImageLayout(fileSystemRoot)) Then
                        Throw New BurnerException(dvd.Error)
                    End If

                    ' Get the correct dvd layout
                    If (Not data.SetImageLayout(dvd.ImageLayout)) Then
                        Throw New BurnerException(data.Error)
                    End If
                End Using
            Else
                If (Not data.SetImageLayout(fileSystemRoot)) Then
                    Throw New BurnerException(data.Error)
                End If
            End If
        End Sub

        Private Sub SetVolumeProperties(ByVal data As DataDisc, ByVal volumeLabel As String)
            ' Sample settings. Replace with your own data or leave empty

            data.UdfVolumeProps.VolumeLabel = volumeLabel
            data.IsoVolumeProps.VolumeLabel = volumeLabel
            data.JolietVolumeProps.VolumeLabel = volumeLabel

            data.UdfVolumeProps.VolumeSet = "SET"
            data.IsoVolumeProps.VolumeSet = "SET"
            data.JolietVolumeProps.VolumeSet = "SET"

            data.IsoVolumeProps.SystemID = "WINDOWS"
            data.JolietVolumeProps.SystemID = "WINDOWS"

            data.IsoVolumeProps.Publisher = "PUBLISHER"
            data.JolietVolumeProps.Publisher = "PUBLISHER"

            data.IsoVolumeProps.DataPreparer = "PREPARER"
            data.JolietVolumeProps.DataPreparer = "PREPARER"

            data.IsoVolumeProps.Application = "DVDBURNER"
            data.JolietVolumeProps.Application = "DVDBURNER"

            Dim now As DateTime = DateTime.Now
            data.UdfVolumeProps.CreationTime = now
            data.IsoVolumeProps.CreationTime = now
            data.JolietVolumeProps.CreationTime = now
        End Sub

        Private Function GetDataDiscStatusString(ByVal status As DataDiscStatus) As String
            Select Case status
                Case DataDiscStatus.BuildingFileSystem
                    Return "Building filesystem..."
                Case DataDiscStatus.LoadingImageLayout
                    Return "Loading image layout..."
                Case DataDiscStatus.WritingFileSystem
                    Return "Writing filesystem..."
                Case DataDiscStatus.WritingImage
                    Return "Writing image..."
                Case DataDiscStatus.CachingSmallFiles
                    Return "Caching small files..."
                Case DataDiscStatus.CachingNetworkFiles
                    Return "Caching network files..."
                Case DataDiscStatus.CachingCDRomFiles
                    Return "Caching CDROM files..."
                Case DataDiscStatus.Initializing
                    Return "Initializing and writing lead-in..."
                Case DataDiscStatus.Writing
                    Return "Writing..."
                Case DataDiscStatus.WritingLeadOut
                    Return "Writing lead-out and flushing cache..."
            End Select

            Return "Unknown status..."
        End Function

        Private Function FormatMedia(ByVal dev As Device) As Boolean
            AddHandler dev.OnErase, AddressOf Device_Erase
            AddHandler dev.OnFormat, AddressOf Device_Format

            Select Case dev.MediaProfile
                ' DVD+RW (needs to be formatted before the disc can be used)
                Case MediaProfile.DvdPlusRw
                    FireStatus("Formatting...")

                    Select Case dev.BgFormatStatus
                        Case BgFormatStatus.NotFormatted
                            dev.Format(FormatType.DvdPlusRwFull)

                        Case BgFormatStatus.Partial
                            dev.Format(FormatType.DvdPlusRwRestart)
                    End Select
            End Select

            RemoveHandler dev.OnErase, AddressOf Device_Erase
            RemoveHandler dev.OnFormat, AddressOf Device_Format

            ' Must be DVD-R, DVD+R
            Return True
        End Function
#End Region

#Region "Private Property Members"
        Private m_isOpen As Boolean = False
        Private m_deviceArray As New ArrayList()
        Private m_speedArray As New ArrayList()
#End Region

#Region "Private Members"
        Private m_engine As Engine = Nothing
        Private m_device As Device = Nothing
#End Region

#Region "Error Definitions"
        ' User Errors
        Public Const ENGINE_INITIALIZATION As Integer = (-1)
        Public Const ENGINE_INITIALIZATION_TEXT As String = "PrimoBurner engine initialization error."

        Public Const BURNER_NOT_OPEN As Integer = (-2)
        Public Const BURNER_NOT_OPEN_TEXT As String = "Burner not open."

        Public Const NO_DEVICES As Integer = (-3)
        Public Const NO_DEVICES_TEXT As String = "No CD/DVD/BD devices are available."

        Public Const NO_DEVICE As Integer = (-4)
        Public Const NO_DEVICE_TEXT As String = "No device selected."

        Public Const DEVICE_ALREADY_SELECTED As Integer = (-5)
        Public Const DEVICE_ALREADY_SELECTED_TEXT As String = "Device already selected."

        Public Const INVALID_DEVICE_INDEX As Integer = (-6)
        Public Const INVALID_DEVICE_INDEX_TEXT As String = "Invalid device index."

        Public Const ERASE_NOT_SUPPORTED As Integer = (-7)
        Public Const ERASE_NOT_SUPPORTED_TEXT As String = "Erasing is supported only for CD-RW and DVD-RW media."

        Public Const FORMAT_NOT_SUPPORTED As Integer = (-8)
        Public Const FORMAT_NOT_SUPPORTED_TEXT As String = "Format is supported only for DVD-RW and DVD+RW media."

        Public Const NO_WRITER_DEVICES As Integer = (-10)
        Public Const NO_WRITER_DEVICES_TEXT As String = "No CD/DVD/BD writers are available."

        ' Special Errors
        Public Const DEVICE_ERROR As Integer = (-100)
        Public Const DATADISC_ERROR As Integer = (-200)
        Public Const VIDEODVD_ERROR As Integer = (-300)
#End Region
    End Class

#Region "Settings"
    ' Burn Settings
    Public Class BurnSettings
        Public SourceFolder As String
        Public VolumeLabel As String

        Public ImageType As PrimoSoftware.Burner.ImageType = ImageType.None
        Public VideoDVD As Boolean = False

        Public WriteMethod As PrimoSoftware.Burner.WriteMethod = WriteMethod.DvdIncremental
        Public WriteSpeedKB As Integer = 0

        Public Simulate As Boolean = False
        Public CloseDisc As Boolean = True
        Public Eject As Boolean = True
    End Class

    Public Class CreateImageSettings
        Public ImageFile As String = ""
        Public SourceFolder As String = ""

        Public VolumeLabel As String = ""
        Public ImageType As PrimoSoftware.Burner.ImageType = PrimoSoftware.Burner.ImageType.None
        Public VideoDVD As Boolean = False
    End Class

    ' Format Settings
    Public Class FormatSettings
        Public Quick As Boolean = True ' Quick format
        Public Force As Boolean = False ' Format even if disc is already formatted
    End Class

    ' Erase Settings
    Public Class EraseSettings
        Public Quick As Boolean = True ' Quick erase
        Public Force As Boolean = False ' Erase even if disc is already blank
    End Class
#End Region

    Public Class BurnerException
        Inherits System.Exception
        Private message_Renamed As String
        Private errorCode_Renamed As Integer

        Public ReadOnly Property ErrorCode() As Integer
            Get
                If errorInfo IsNot Nothing Then
                    Return errorInfo.Code
                End If

                Return errorCode_Renamed
            End Get
        End Property

        Public Overrides ReadOnly Property Message() As String
            Get
                Return message_Renamed
            End Get
        End Property

        Private errorInfo As PrimoSoftware.Burner.ErrorInfo

        Public Sub New(ByVal errorCode As Integer, ByVal errorMessage As String)
            Me.errorCode_Renamed = errorCode
            Me.message_Renamed = errorMessage
        End Sub

        Public Sub New(ByVal errorInfo As PrimoSoftware.Burner.ErrorInfo)
            If errorInfo Is Nothing Then
                Return
            End If

            Me.errorInfo = CType(errorInfo.Clone(), PrimoSoftware.Burner.ErrorInfo)

            Select Case errorInfo.Facility
                Case ErrorFacility.SystemWindows
                    message_Renamed = New System.ComponentModel.Win32Exception(errorInfo.Code).Message

                Case ErrorFacility.Success
                    message_Renamed = "Success"

                Case ErrorFacility.DataDisc
                    message_Renamed = String.Format("DataDisc error: 0x{0:x8}: {1}", errorInfo.Code, errorInfo.Message)

                Case ErrorFacility.Device
                    message_Renamed = String.Format("Device error: 0x{0:x8}: {1}", errorInfo.Code, errorInfo.Message)

                Case ErrorFacility.VideoDVD
                    message_Renamed = String.Format("VideoDVD error: 0x{0:x8}: {1}", errorInfo.Code, errorInfo.Message)

                Case Else
                    message_Renamed = String.Format("Facility:{0} error :0x{1:x8}: {2}", errorInfo.Facility, errorInfo.Code, errorInfo.Message)

            End Select
        End Sub
    End Class

End Namespace
