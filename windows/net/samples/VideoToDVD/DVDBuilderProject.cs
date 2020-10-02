using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PrimoSoftware.AVBlocks;

namespace VideoToDVD
{
    public enum TVSystem
    {
        PAL,
        NTSC
    }

    

    class DVDBuilderProject
    {
        public TVSystem TVSystem { get; set; }
        readonly Size PalFrameSize = new Size(720, 576);
        readonly Size NtscFrameSize = new Size(720, 480);

        Size FrameSize
        {
            get 
            {
                if (TVSystem == TVSystem.PAL)
                {
                    return PalFrameSize;
                }
                else
                {
                    return NtscFrameSize;                    
                }
            }
        }

        private string TVSystemToPreset(TVSystem tvsystem)
        {
            if (tvsystem == TVSystem.PAL)
            {
                return Preset.Video.DVD.PAL_4x3_MP2;
            }
            else
            {
                return Preset.Video.DVD.NTSC_4x3_PCM;
            }
        }

        public void Create(string projectFile, List<string> videos, string menusFolder)
        {
            string menuBackgroudBmp = System.IO.Path.Combine(menusFolder, "menu.bmp");
            string menuBackgroudMpg = System.IO.Path.Combine(menusFolder, "menu.mpg");
            string menuMaskBmp = System.IO.Path.Combine(menusFolder, "menuMask.bmp");

            if (File.Exists(menuBackgroudMpg))
                File.Delete(menuBackgroudMpg);

            List<MenuButton> buttons = CreateButtons(videos);

            GenerateMenuBackground(menuBackgroudBmp, buttons);
            GenerateMenuMask(menuMaskBmp, buttons);

            using (var transcoder = new Transcoder())
            {
                // In order to use the OEM release for testing (without a valid license) the transcoder demo mode must be enabled.
                transcoder.AllowDemoMode = true;

                transcoder.Inputs.Add(new MediaSocket { File = menuBackgroudBmp });

                var outputSocket = MediaSocket.FromPreset(TVSystemToPreset(TVSystem));
                outputSocket.File = menuBackgroudMpg;
                
                transcoder.Outputs.Add(outputSocket);

                if (!transcoder.Open())
                    throw new Exception(transcoder.Error.Message);

                if (!transcoder.Run())
                    throw new Exception(transcoder.Error.Message);
            }


            string xml = string.Empty;

            xml += "<?xml version='1.0' encoding='utf-8'?>\r\n";
            xml += "<dvd version='2.3' xmlns='http://www.primosoftware.com/dvdbuilder/2.3'>\r\n";
            xml += "<videoManager firstPlayNavigate='Menu = 1'>\r\n";
            xml += "</videoManager>\r\n";
            xml += "<titleSet>\r\n";
            xml += "<titles>\r\n";

            for (int i = 0; i < videos.Count; i++)
            {
                 xml +=  string.Format("<title id='{0}' postNavigate='Menu = 1' chapters='00:00:00;'>\r\n", (i + 1));
                 xml += string.Format("<videoObject file='{0}' />\r\n", videos[i]);  
                 xml +=  "</title>\r\n";
            }

            xml += "</titles>\r\n";

            xml += "<menus>\r\n";
            xml += "<menu id='1' entry='root'>\r\n";

            for (int i = 0; i < buttons.Count; i++)
            {
                MenuButton mb = buttons[i];
                xml += string.Format("<button left='{0}' top='{1}' width='{2}' height='{3}' navigate='Title = {4}; Chapter = 1' />\r\n",
                    mb.Rectangle.Location.X, mb.Rectangle.Location.Y, mb.Rectangle.Width, mb.Rectangle.Height, i + 1);
            }

            xml += string.Format("<background file='{0}' />\r\n", menuBackgroudMpg);
            
            xml += string.Format("<mask file='{0}' backgroundColor='#000000' patternColor='#008000' emphasisColor1='#000000' emphasisColor2='#000000' />\r\n", menuMaskBmp);

            xml += "<display   backgroundColor='#000000' backgroundContrast='0' patternColor='#000000' patternContrast='0'  emphasis1Color='#000000' emphasis1Contrast='0' emphasis2Color='#000000' emphasis2Contrast='0' />\r\n";
            xml += "<selection backgroundColor='#000000' backgroundContrast='0' patternColor='#FFFF00' patternContrast='15' emphasis1Color='#000000' emphasis1Contrast='0' emphasis2Color='#000000' emphasis2Contrast='0' />\r\n";
            xml += "<action    backgroundColor='#000000' backgroundContrast='0' patternColor='#FF00FF' patternContrast='15' emphasis1Color='#000000' emphasis1Contrast='0' emphasis2Color='#000000' emphasis2Contrast='0' />\r\n";

            xml += "</menu>\r\n";
            xml += "</menus>\r\n";

            xml += "</titleSet>\r\n";
            xml += "</dvd>\r\n";

            File.WriteAllText(projectFile, xml, System.Text.Encoding.UTF8);
        }

        public class MenuButton
        {
            public string Text;
            public PointF  TextPosition;
            public List<Point> Polygon;
            public Rectangle Rectangle;
        }

        List<MenuButton> CreateButtons(List<string> videos)
        {
            List<MenuButton> buttons = new List<MenuButton>();

            for(int i = 0; i < videos.Count; i++)
            {
                string video = videos[i];
                int yStep = 70;
                int yPos = i * yStep + 50;

                MenuButton button = new MenuButton();
                button.Text = System.IO.Path.GetFileNameWithoutExtension(video);
                button.TextPosition = new PointF(90, yPos + 15);
                button.Polygon = new List<Point>(new Point[] { new Point(50, yPos), new Point(50, yPos + 50), new Point(80, yPos + 25) });
                button.Rectangle = new Rectangle(button.Polygon[0].X, button.Polygon[0].Y,
                                                 button.Polygon[2].X - button.Polygon[0].X,
                                                 button.Polygon[1].Y - button.Polygon[0].Y);
                buttons.Add(button);
            }

            return buttons;
        }

        public void GenerateMenuBackground(string backgroudPath, List<MenuButton> buttons)
        {
            using (Bitmap bmp = new Bitmap(FrameSize.Width, FrameSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, bmp.Width, bmp.Height), Color.WhiteSmoke, Color.LightBlue, LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(brush, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    }

                    using(Font font = new Font("Arial", 15))
                    {
                        foreach (MenuButton button in buttons)
                        {
                            g.FillPolygon(Brushes.Blue, button.Polygon.ToArray());
                            g.DrawString(button.Text, font, Brushes.DarkBlue, button.TextPosition);
                        }
                    }
                }

                bmp.Save(backgroudPath, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        public void GenerateMenuMask(string backgroudPath, List<MenuButton> buttons)
        {
            using (Bitmap bmp = new Bitmap(FrameSize.Width, FrameSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.FillRectangle(Brushes.Black, new Rectangle(0, 0, bmp.Width, bmp.Height));

                    foreach (MenuButton button in buttons)
                    {
                        g.FillPolygon(Brushes.Green, button.Polygon.ToArray());
                    }
                }

                bmp.Save(backgroudPath, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }
    }
}
