#include "StdAfx.h"
#include "DVDBuilderProject.h"
#include "Mpeg2EncoderException.h"
#include "Mpeg2Encoder.h"


void DVDBuilderProject::createButtons(const std::vector<CString> &videos, std::vector<MenuButton> &buttons)
{
	buttons.clear();

    for(int i = 0; i < (int)videos.size(); i++)
    {
        CString video = videos[i];
        int yStep = 70;
        int yPos = i * yStep + 50;

		MenuButton button;
		TCHAR fileName[_MAX_FNAME + 1];
		memset(fileName, 0, sizeof(fileName));

		_tsplitpath(video, NULL, NULL, fileName, NULL);

		button.Text = fileName;
		button.TextPosition = PointF((Gdiplus::REAL)90, (Gdiplus::REAL)(yPos + 15));
		button.Polygon.push_back(PointF((Gdiplus::REAL)50, (Gdiplus::REAL)yPos));
		button.Polygon.push_back(PointF((Gdiplus::REAL)50, (Gdiplus::REAL)(yPos + 50)));
		button.Polygon.push_back(PointF((Gdiplus::REAL)80, (Gdiplus::REAL)(yPos + 25)));

        button.Rectangle = CRect((int)button.Polygon[0].X, (int)button.Polygon[0].Y,
                                         (int)button.Polygon[2].X,
                                         (int)button.Polygon[1].Y);

		buttons.push_back(button);
    }
}

void DVDBuilderProject::create(const CString &projectFile, const std::vector<CString> &videos, const CString &menusFolder)
{
	CString menuBackgroudBmp = menusFolder + _T("\\") + _T("menu.bmp");
	CString menuBackgroudMpg = menusFolder + _T("\\") + _T("menu.mpg");
	CString menuMaskBmp = menusFolder + _T("\\")+ _T("menuMask.bmp");

	std::vector<MenuButton> buttons;

	CFileStatus status;
	if (CFile::GetStatus(menuBackgroudMpg, status))
		CFile::Remove(menuBackgroudMpg);

	createButtons(videos, buttons);
	generateMenuBackground(menuBackgroudBmp, buttons);
	generateMenuMask(menuMaskBmp, buttons);

	{
		Mpeg2Encoder encoder;
		encoder.setOutputPreset(encoderPreset());

		encoder.setInputFile(menuBackgroudBmp);
		encoder.setOutputFile(menuBackgroudMpg);

		encoder.convert();
	}

    CString xml;

    xml += _T("<?xml version='1.0' encoding='utf-8'?>\r\n");
    xml += _T("<dvd version='2.3' xmlns='http://www.primosoftware.com/dvdbuilder/2.3'>\r\n");
    xml += _T("<videoManager firstPlayNavigate='Menu = 1'>\r\n");
    xml += _T("</videoManager>\r\n");
    xml += _T("<titleSet>\r\n");
    xml += _T("<titles>\r\n");

    for (int i = 0; i < videos.size(); i++)
    {
		 xml.AppendFormat(_T("<title id='%d' postNavigate='Menu = 1' chapters='00:00:00;'>\r\n"), (i + 1));
         xml.AppendFormat(_T("<videoObject file='%s' />\r\n"), videos[i]);  
         xml += _T("</title>\r\n");
    }

    xml += _T("</titles>\r\n");

    xml += _T("<menus>\r\n");
    xml += _T("<menu id='1' entry='root'>\r\n");

    for (int i = 0; i < buttons.size(); i++)
    {
        MenuButton &mb = buttons[i];
        xml.AppendFormat(_T("<button left='%d' top='%d' width='%d' height='%d' navigate='Title = %d; Chapter = 1' />\r\n"),
						mb.Rectangle.TopLeft().x, mb.Rectangle.TopLeft().y, mb.Rectangle.Width(), mb.Rectangle.Height(), i + 1);
    }

	xml.AppendFormat(_T("<background file='%s' />\r\n"), menuBackgroudMpg);

    xml.AppendFormat(_T("<mask file='%s' backgroundColor='#000000' patternColor='#008000' emphasisColor1='#000000' emphasisColor2='#000000' />\r\n"), menuMaskBmp);

	xml += _T("<display   backgroundColor='#000000' backgroundContrast='0' patternColor='#000000' patternContrast='0'  emphasis1Color='#000000' emphasis1Contrast='0' emphasis2Color='#000000' emphasis2Contrast='0' />\r\n");
    xml += _T("<selection backgroundColor='#000000' backgroundContrast='0' patternColor='#FFFF00' patternContrast='15' emphasis1Color='#000000' emphasis1Contrast='0' emphasis2Color='#000000' emphasis2Contrast='0' />\r\n");
    xml += _T("<action    backgroundColor='#000000' backgroundContrast='0' patternColor='#FF00FF' patternContrast='15' emphasis1Color='#000000' emphasis1Contrast='0' emphasis2Color='#000000' emphasis2Contrast='0' />\r\n");
    
	xml += _T("</menu>\r\n");
    xml += _T("</menus>\r\n");

    xml += _T("</titleSet>\r\n");
    xml += _T("</dvd>\r\n");

	FILE* fp = _tfopen(projectFile, _T("wb"));
	if(NULL == fp)
	{
		throw BaseException(_T("Failed to create a file."));
	}
	CStringA axml = toUtf8String(xml);
	fwrite((LPCSTR)axml, 1, axml.GetLength(), fp);
	fclose(fp);
	fp = NULL;
}

static int GetEncoderClsid(const WCHAR* format, CLSID* pClsid)
{
   UINT  num = 0;          // number of image encoders
   UINT  size = 0;         // size of the image encoder array in bytes

   ImageCodecInfo* pImageCodecInfo = NULL;

   GetImageEncodersSize(&num, &size);
   if(size == 0)
      return -1;  // Failure

   pImageCodecInfo = (ImageCodecInfo*)(malloc(size));
   if(pImageCodecInfo == NULL)
      return -1;  // Failure

   GetImageEncoders(num, size, pImageCodecInfo);

   for(UINT j = 0; j < num; ++j)
   {
      if( wcscmp(pImageCodecInfo[j].MimeType, format) == 0 )
      {
         *pClsid = pImageCodecInfo[j].Clsid;
         free(pImageCodecInfo);
         return j;  // Success
      }    
   }

   free(pImageCodecInfo);
   return -1;  // Failure
}


void DVDBuilderProject::generateMenuBackground(const CString &backgroudPath, const std::vector<MenuButton> &buttons)
{
	Bitmap bmp(frameSize().cx, frameSize().cy, PixelFormat32bppRGB);
	Graphics g(&bmp);
	{
		LinearGradientBrush brush(RectF((Gdiplus::REAL)0, (Gdiplus::REAL)0, (Gdiplus::REAL)bmp.GetWidth(), (Gdiplus::REAL)bmp.GetHeight()), Color(0xFF, 0xF5, 0xF5, 0xF5) , Color(0xFF, 0xAD, 0xD8, 0xE6), LinearGradientModeHorizontal);
		g.FillRectangle(&brush, RectF((Gdiplus::REAL)0, (Gdiplus::REAL)0, (Gdiplus::REAL)bmp.GetWidth(), (Gdiplus::REAL)bmp.GetHeight()));
	}

	{
		Font font(L"Arial", 15);
		SolidBrush brushDarkBlue(Color(0xFF, 0x00, 0x00, 0x8B));
		SolidBrush brushBlue(Color(0xFF, 0x00, 0x00, 0xFF));

		for(int i = 0; i < (int)buttons.size(); i++)
		{
			const MenuButton &button = buttons[i];
			g.FillPolygon(&brushBlue, &button.Polygon[0], button.Polygon.size());
			g.DrawString(button.Text, -1, &font, button.TextPosition, &brushDarkBlue); 
		}
	}

   CLSID bmpClsid;
   GetEncoderClsid(L"image/bmp", &bmpClsid);

   if(bmp.Save(backgroudPath, &bmpClsid, NULL) != Gdiplus::Ok)
   {
	   throw BaseException(_T("Failed to create a file."));
   }
}

void DVDBuilderProject::generateMenuMask(const CString &maskPath, const std::vector<MenuButton> &buttons)
{

	Bitmap bmp(frameSize().cx, frameSize().cy, PixelFormat32bppRGB);
	Graphics g(&bmp);
	{
		LinearGradientBrush brush(RectF((Gdiplus::REAL)0, (Gdiplus::REAL)0, (Gdiplus::REAL)bmp.GetWidth(), (Gdiplus::REAL)bmp.GetHeight()), Color(0xFF, 0xF5, 0xF5, 0xF5) , Color(0xFF, 0xAD, 0xD8, 0xE6), LinearGradientModeHorizontal);
		g.FillRectangle(&brush, RectF((Gdiplus::REAL)0, (Gdiplus::REAL)0, (Gdiplus::REAL)bmp.GetWidth(), (Gdiplus::REAL)bmp.GetHeight()));
	}

	{
		SolidBrush brushBlack(Color(0xFF, 0x00, 0x00, 0x00));
		SolidBrush brushGreen(Color(0xFF, 0x00, 0x80, 0x00));

		g.FillRectangle(&brushBlack, RectF((Gdiplus::REAL)0, (Gdiplus::REAL)0, (Gdiplus::REAL)bmp.GetWidth(), (Gdiplus::REAL)bmp.GetHeight()));

		for(int i = 0; i < (int)buttons.size(); i++)
		{
			const MenuButton &button = buttons[i];
			g.FillPolygon(&brushGreen, &button.Polygon[0], button.Polygon.size());
		}
	}

   CLSID bmpClsid;
   GetEncoderClsid(L"image/bmp", &bmpClsid);

   if(bmp.Save(maskPath, &bmpClsid, NULL) != Gdiplus::Ok)
   {
	   throw BaseException(_T("Failed to create a file."));
   }
}
