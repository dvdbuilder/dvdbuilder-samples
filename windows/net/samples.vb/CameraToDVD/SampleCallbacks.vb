Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Diagnostics
Imports DirectShowLib
Imports PrimoSoftware.AVBlocks

Namespace CameraToDVD
    Friend Class SampleGrabberCB
        Protected lastMediaTime As Long = -1
        Protected lastSampleTime As Double = 0
        Protected startTime As DateTime = DateTime.MinValue

        Protected name As String
        Protected filename As String
        Protected file As FileStream
        Protected mediaState_Renamed As MediaState
        Protected bProcess As Boolean = True

        ' how many times the callback has been called
        Protected sampleIndex_Renamed As Long

        ' how many samples are processed (based on the media time)
        Protected sampleProcessed As Long

        ' how many samples are dropped (based on the media time)
        Protected sampleDropped As Long

        Protected streamNumber_Renamed As Integer
        Protected mainWindow_Renamed As IntPtr

        Protected Enum StreamType
            Audio = 0
            Video = 1
        End Enum

        Public Function BufferCB(ByVal SampleTime As Double, ByVal pBuffer As IntPtr, ByVal BufferLen As Integer) As Integer
            sampleIndex_Renamed += 1
            Dim msg As String = String.Format("BufferCB ({0}) {1}. time:{2} length:{3}", name, sampleIndex_Renamed, SampleTime, BufferLen)

            System.Diagnostics.Trace.WriteLine(msg)

            If file IsNot Nothing Then
                Dim buf(BufferLen - 1) As Byte
                Marshal.Copy(pBuffer, buf, 0, BufferLen)
                file.Write(buf, 0, BufferLen)
            End If

            Return 0
        End Function

        Public Function SampleCB(ByVal SampleTime As Double, ByVal pSample As IMediaSample) As Integer
            If (Not bProcess) Then
                lastSampleTime = SampleTime
                Return WinAPI.E_FAIL
            End If

            ' internal stats
            sampleIndex_Renamed += 1
            Dim tStart, tEnd As Long

            pSample.GetMediaTime(tStart, tEnd)
            Debug.Assert(tStart < tEnd)
            Debug.Assert(tStart > lastMediaTime)
            sampleProcessed += tEnd - tStart
            sampleDropped += tStart - lastMediaTime - 1
            lastMediaTime = tEnd - 1

            Dim dataLen As Integer = pSample.GetActualDataLength()
            Dim bufPtr As IntPtr
            Dim hr As Integer = pSample.GetPointer(bufPtr)
            Debug.Assert(0 = hr)

            ' BEGIN TRACE

            Dim bufSize As Integer = pSample.GetSize()

            Dim timeStart, timeEnd As Long
            pSample.GetTime(timeStart, timeEnd)

            Dim msg As String = String.Format("SampleCB ({0}) {1}, sampleTime:{2} datalen:{3} bufsize:{4} mediaTime:{5}-{6} time:{7}-{8}", name, sampleIndex_Renamed, SampleTime, dataLen, bufSize, tStart, tEnd, timeStart, timeEnd)

            Trace.WriteLine(msg)

            If tStart - lastMediaTime - 1 > 0 Then
                msg = String.Format("!!! Frame drop: {0}", tStart - lastMediaTime - 1 > 0)
                Trace.WriteLine(msg)
            End If

            'END TRACE

            Dim buf(dataLen - 1) As Byte
            Marshal.Copy(bufPtr, buf, 0, dataLen)

            If file IsNot Nothing Then
                file.Write(buf, 0, dataLen)
            End If

            'DBG - simulate encoding error
            'if (sampleIndex > 100)
            '    goto STOP_CAPTURE;

            If mediaState_Renamed IsNot Nothing AndAlso mediaState_Renamed.mpeg2Enc IsNot Nothing Then
                Dim enc As PrimoSoftware.AVBlocks.Transcoder = mediaState_Renamed.mpeg2Enc
                Dim inputSample As New MediaSample()
                inputSample.Buffer = New MediaBuffer(buf)
                inputSample.StartTime = Math.Max(SampleTime, 0)
                'TODO: end time

                Try
                    Dim pushed As Boolean = False

                    ' transcoder.Push() is not threads safe.
                    ' lock (enc){ } ensure that only one thread is calling transcoder.Push()
                    SyncLock enc
                        pushed = enc.Push(StreamNumber, inputSample)
                    End SyncLock

                    If pushed Then
                        Return 0
                    End If
                Catch ex As Exception
                    System.Diagnostics.Trace.WriteLine(ex.ToString())
                End Try

                Trace.WriteLine("PushSample FAILED")

            End If

            'STOP_CAPTURE:

            Trace.WriteLine("SampleCB: Before Post STOP_CAPTURE")
            WinAPI.PostMessage(MainWindow, Util.WM_STOP_CAPTURE, New IntPtr(streamNumber_Renamed), IntPtr.Zero)
            Trace.WriteLine("SampleCB: After Post STOP_CAPTURE")
            bProcess = False
            Return WinAPI.E_FAIL

        End Function ' end of SampleCB

        Public Sub Start(ByVal time As DateTime)
            startTime = time
        End Sub

        Public Property MainWindow() As IntPtr
            Get
                Return mainWindow_Renamed
            End Get
            Set(ByVal value As IntPtr)
                mainWindow_Renamed = value
            End Set
        End Property

        Public ReadOnly Property SampleIndex() As Long
            Get
                Return sampleIndex_Renamed
            End Get
        End Property

        Public ReadOnly Property StreamNumber() As Integer
            Get
                Return streamNumber_Renamed
            End Get
        End Property

        Public ReadOnly Property ProcessedSamples() As Long
            Get
                Return sampleProcessed
            End Get
        End Property

        Public ReadOnly Property DroppedSamples() As Long
            Get
                Return sampleDropped
            End Get
        End Property

        Public Sub New(ByVal name As String)
            If (Not String.IsNullOrEmpty(name)) Then
                Me.name = name
            Else
                Me.name = "SampleGrabberCB"
            End If
        End Sub

        Protected Overrides Sub Finalize()
            Reset()
        End Sub

        Public Function SetOutputFile(ByVal filename As String) As Boolean
            If file IsNot Nothing Then
                file.Close()
                file = Nothing
            End If

            If String.IsNullOrEmpty(filename) Then
                Me.filename = Nothing
                Return True
            End If

            Try
                file = New FileStream(filename, FileMode.Create, FileAccess.Write)
            Catch
                Return False
            End Try

            Me.filename = filename
            Return True
        End Function

        Public Property MediaState() As MediaState
            Get
                Return mediaState_Renamed
            End Get
            Set(ByVal value As MediaState)
                mediaState_Renamed = value
            End Set
        End Property

        Public Sub Reset()
            If file IsNot Nothing Then
                file.Close()
                file = Nothing
            End If

            sampleIndex_Renamed = 0
            sampleProcessed = 0
            sampleDropped = 0

            mediaState_Renamed = Nothing
            bProcess = True

            lastMediaTime = -1
            lastSampleTime = 0
        End Sub



    End Class ' end of SampleGrabberCB

    Friend Class AudioGrabberCB
        Inherits SampleGrabberCB
        Implements ISampleGrabberCB
        Public Sub New(ByVal name As String)
            MyBase.New(name)
            streamNumber_Renamed = CInt(Fix(StreamType.Audio))
        End Sub

        Private Overloads Function BufferCB(ByVal SampleTime As Double, ByVal pBuffer As IntPtr, ByVal BufferLen As Integer) As Integer Implements ISampleGrabberCB.BufferCB
            Return MyBase.BufferCB(SampleTime, pBuffer, BufferLen)
        End Function

        Private Overloads Function SampleCB(ByVal SampleTime As Double, ByVal pSample As IMediaSample) As Integer Implements ISampleGrabberCB.SampleCB
            Dim hr As Integer = MyBase.SampleCB(SampleTime, pSample)
            Marshal.ReleaseComObject(pSample)
            Return hr
        End Function

    End Class

    Friend Class VideoGrabberCB
        Inherits SampleGrabberCB
        Implements ISampleGrabberCB
        Public Sub New(ByVal name As String)
            MyBase.New(name)
            streamNumber_Renamed = CInt(Fix(StreamType.Video))
        End Sub
        Private Overloads Function BufferCB(ByVal SampleTime As Double, ByVal pBuffer As IntPtr, ByVal BufferLen As Integer) As Integer Implements ISampleGrabberCB.BufferCB

            MyBase.BufferCB(SampleTime, pBuffer, BufferLen)


            ' own processing
            If MediaState Is Nothing Then
                Return 0
            End If

            Dim mt As AMMediaType = MediaState.videoType

            ' NOTE: not all subtypes can by dumped to bmp file

            If (mt.majorType <> DirectShowLib.MediaType.Video) OrElse (mt.formatType <> DirectShowLib.FormatType.VideoInfo) OrElse (mt.formatPtr = IntPtr.Zero) Then
                System.Diagnostics.Trace.WriteLine("Invalid media type")
                Return 0
            End If

            If (SampleIndex Mod 10) <> 0 Then
                Return 0
            End If

            Dim sampleFilename As String = String.Format("d:\tmp\capture\{0}_{1:d5}.bmp", Me.name, SampleIndex)


            Dim fs As FileStream = Nothing
            Try
                fs = New FileStream(sampleFilename, FileMode.Create, FileAccess.Write)
            Catch ex As Exception
                System.Diagnostics.Trace.WriteLine(ex.ToString())
                Return WinAPI.E_FAIL
            End Try

            Dim bw As New BinaryWriter(fs)
            Const bmFileHeaderSize As Integer = 14
            Dim vih As VideoInfoHeader = CType(Marshal.PtrToStructure(mt.formatPtr, GetType(VideoInfoHeader)), VideoInfoHeader)

            System.Diagnostics.Trace.WriteLine(String.Format("BmiHeader: {0}x{1}", vih.BmiHeader.Width, vih.BmiHeader.Height))

            bw.Write(CByte(AscW("B"c))) ' type
            bw.Write(CByte(AscW("M"c)))
            Dim bmSize As Integer = bmFileHeaderSize + vih.BmiHeader.Size + BufferLen
            bw.Write(bmSize) ' size
            bw.Write(CShort(Fix(0))) ' reserved
            bw.Write(CShort(Fix(0))) ' reserved
            bw.Write(CInt(Fix(bmSize - BufferLen))) ' offset bits

            Dim vihSize As Integer = Marshal.SizeOf(vih)
            Dim bihBuf(vih.BmiHeader.Size - 1) As Byte
            Dim bihStart As New IntPtr(mt.formatPtr.ToInt64() + vihSize - vih.BmiHeader.Size)
            Marshal.Copy(bihStart, bihBuf, 0, vih.BmiHeader.Size)
            bw.Write(bihBuf)

            Dim bmpData(BufferLen - 1) As Byte
            Marshal.Copy(pBuffer, bmpData, 0, BufferLen)
            bw.Write(bmpData)
            'bw.Flush();
            bw.Close()

            Return 0
        End Function

        Private Overloads Function SampleCB(ByVal SampleTime As Double, ByVal pSample As IMediaSample) As Integer Implements ISampleGrabberCB.SampleCB
            Dim hr As Integer = MyBase.SampleCB(SampleTime, pSample)
            Marshal.ReleaseComObject(pSample)
            Return hr
        End Function

    End Class ' end of VideoGrabberCB

    ' Sample grabber callback method
    Friend Enum CBMethod
        Sample = 0 ' the original sample from the upstream filter
        Buffer = 1 ' a copy of the sample of the upstream filter
    End Enum

End Namespace