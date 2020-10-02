Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports PrimoSoftware.AVBlocks

Namespace VideoToDVD
    Public Enum TVSystem
        PAL
        NTSC
    End Enum



    Friend Class DVDBuilderProject
        Private privateTVSystem As TVSystem
        Public Property TVSystem() As TVSystem
            Get
                Return privateTVSystem
            End Get
            Set(ByVal value As TVSystem)
                privateTVSystem = value
            End Set
        End Property
        Private ReadOnly PalFrameSize As New Size(720, 576)
        Private ReadOnly NtscFrameSize As New Size(720, 480)

        Private ReadOnly Property FrameSize() As Size
            Get
                If TVSystem = TVSystem.PAL Then
                    Return PalFrameSize
                Else
                    Return NtscFrameSize
                End If
            End Get
        End Property

        Private Function TVSystemToPreset(ByVal tvsystem As TVSystem) As String
            If tvsystem = TVSystem.PAL Then
                Return Preset.Video.DVD.PAL_4x3_MP2
            Else
                Return Preset.Video.DVD.NTSC_4x3_PCM
            End If
        End Function

        Public Sub Create(ByVal projectFile As String, ByVal videos As List(Of String), ByVal menusFolder As String)

            Dim menuBackgroudBmp As String = System.IO.Path.Combine(menusFolder, "menu.bmp")
            Dim menuBackgroudMpg As String = System.IO.Path.Combine(menusFolder, "menu.mpg")
            Dim menuMaskBmp As String = System.IO.Path.Combine(menusFolder, "menuMask.bmp")

            If (File.Exists(menuBackgroudMpg)) Then
                File.Delete(menuBackgroudMpg)
            End If

            Dim buttons As List(Of MenuButton) = CreateButtons(videos)

            GenerateMenuBackground(menuBackgroudBmp, buttons)
            GenerateMenuMask(menuMaskBmp, buttons)

            Using transcoder As PrimoSoftware.AVBlocks.Transcoder = New PrimoSoftware.AVBlocks.Transcoder()

                ' In order to use the OEM release for testing (without a valid license) the transcoder demo mode must be enabled.
                transcoder.AllowDemoMode = True

                transcoder.Inputs.Add(New MediaSocket With {.File = menuBackgroudBmp})

                Dim outputSocket As MediaSocket = MediaSocket.FromPreset(TVSystemToPreset(TVSystem))
                outputSocket.File = menuBackgroudMpg

                transcoder.Outputs.Add(outputSocket)

                If (Not transcoder.Open()) Then
                    Throw New Exception(transcoder.Error.Message)
                End If

                If (Not transcoder.Run()) Then
                    Throw New Exception(transcoder.Error.Message)
                End If
            End Using


            Dim xml As String = String.Empty

            xml &= "<?xml version='1.0' encoding='utf-8'?>" & Constants.vbCrLf
            xml &= "<dvd version='2.3' xmlns='http://www.primosoftware.com/dvdbuilder/2.3'>" & Constants.vbCrLf
            xml &= "<videoManager firstPlayNavigate='Menu = 1'>" & Constants.vbCrLf
            xml &= "</videoManager>" & Constants.vbCrLf
            xml &= "<titleSet>" & Constants.vbCrLf
            xml &= "<titles>" & Constants.vbCrLf

            For i As Integer = 0 To videos.Count - 1
                xml &= String.Format("<title id='{0}' postNavigate='Menu = 1' chapters='00:00:00;'>" & Constants.vbCrLf, (i + 1))
                xml &= String.Format("<videoObject file='{0}' />" & Constants.vbCrLf, videos(i))
                xml &= "</title>" & Constants.vbCrLf
            Next i

            xml &= "</titles>" & Constants.vbCrLf

            xml &= "<menus>" & Constants.vbCrLf
            xml &= "<menu id='1' entry='root'>" & Constants.vbCrLf

            For i As Integer = 0 To buttons.Count - 1
                Dim mb As MenuButton = buttons(i)
                xml &= String.Format("<button left='{0}' top='{1}' width='{2}' height='{3}' navigate='Title = {4}; Chapter = 1' />" & Constants.vbCrLf, mb.Rectangle.Location.X, mb.Rectangle.Location.Y, mb.Rectangle.Width, mb.Rectangle.Height, i + 1)
            Next i

            xml &= String.Format("<background file='{0}' />" & Constants.vbCrLf, menuBackgroudMpg)

            xml &= String.Format("<mask file='{0}' backgroundColor='#000000' patternColor='#008000' emphasisColor1='#000000' emphasisColor2='#000000' />" & Constants.vbCrLf, menuMaskBmp)

            xml &= "<display   backgroundColor='#000000' backgroundContrast='0' patternColor='#000000' patternContrast='0'  emphasis1Color='#000000' emphasis1Contrast='0' emphasis2Color='#000000' emphasis2Contrast='0' />" & Constants.vbCrLf
            xml &= "<selection backgroundColor='#000000' backgroundContrast='0' patternColor='#FFFF00' patternContrast='15' emphasis1Color='#000000' emphasis1Contrast='0' emphasis2Color='#000000' emphasis2Contrast='0' />" & Constants.vbCrLf
            xml &= "<action    backgroundColor='#000000' backgroundContrast='0' patternColor='#FF00FF' patternContrast='15' emphasis1Color='#000000' emphasis1Contrast='0' emphasis2Color='#000000' emphasis2Contrast='0' />" & Constants.vbCrLf

            xml &= "</menu>" & Constants.vbCrLf
            xml &= "</menus>" & Constants.vbCrLf

            xml &= "</titleSet>" & Constants.vbCrLf
            xml &= "</dvd>" & Constants.vbCrLf

            File.WriteAllText(projectFile, xml, System.Text.Encoding.UTF8)
        End Sub

        Public Class MenuButton
            Public Text As String
            Public TextPosition As PointF
            Public Polygon As List(Of Point)
            Public Rectangle As Rectangle
        End Class

        Private Function CreateButtons(ByVal videos As List(Of String)) As List(Of MenuButton)
            Dim buttons As New List(Of MenuButton)()

            For i As Integer = 0 To videos.Count - 1
                Dim video As String = videos(i)
                Dim yStep As Integer = 70
                Dim yPos As Integer = i * yStep + 50

                Dim button As New MenuButton()
                button.Text = System.IO.Path.GetFileNameWithoutExtension(video)
                button.TextPosition = New PointF(90, yPos + 15)
                button.Polygon = New List(Of Point)(New Point() {New Point(50, yPos), New Point(50, yPos + 50), New Point(80, yPos + 25)})
                button.Rectangle = New Rectangle(button.Polygon(0).X, button.Polygon(0).Y, button.Polygon(2).X - button.Polygon(0).X, button.Polygon(1).Y - button.Polygon(0).Y)
                buttons.Add(button)
            Next i

            Return buttons
        End Function

        Public Sub GenerateMenuBackground(ByVal backgroudPath As String, ByVal buttons As List(Of MenuButton))
            Using bmp As New Bitmap(FrameSize.Width, FrameSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    Using brush As New LinearGradientBrush(New Rectangle(0, 0, bmp.Width, bmp.Height), Color.WhiteSmoke, Color.LightBlue, LinearGradientMode.Horizontal)
                        g.FillRectangle(brush, New Rectangle(0, 0, bmp.Width, bmp.Height))
                    End Using

                    Using font As New Font("Arial", 15)
                        For Each button As MenuButton In buttons
                            g.FillPolygon(Brushes.Blue, button.Polygon.ToArray())
                            g.DrawString(button.Text, font, Brushes.DarkBlue, button.TextPosition)
                        Next button
                    End Using
                End Using

                bmp.Save(backgroudPath, System.Drawing.Imaging.ImageFormat.Bmp)
            End Using
        End Sub

        Public Sub GenerateMenuMask(ByVal backgroudPath As String, ByVal buttons As List(Of MenuButton))
            Using bmp As New Bitmap(FrameSize.Width, FrameSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.FillRectangle(Brushes.Black, New Rectangle(0, 0, bmp.Width, bmp.Height))

                    For Each button As MenuButton In buttons
                        g.FillPolygon(Brushes.Green, button.Polygon.ToArray())
                    Next button
                End Using

                bmp.Save(backgroudPath, System.Drawing.Imaging.ImageFormat.Bmp)
            End Using
        End Sub
    End Class
End Namespace
