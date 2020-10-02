#pragma once


namespace TVSystem
{
	enum Enum
	{
		PAL,
		NTSC
	};
}


class DVDBuilderProject
{
	CSize frameSize()
	{
		if(tvSystem() == TVSystem::NTSC)
		{
			return CSize(720, 480);
		}
		else
		{
			return CSize(720, 576);
		}
	}

	const char* encoderPreset()
	{
		return (tvSystem() == TVSystem::PAL) ? 
					primo::avblocks::Preset::Video::DVD::PAL_4x3_MP2 : primo::avblocks::Preset::Video::DVD::NTSC_4x3_PCM;

	}

	TVSystem::Enum _TVSystem;

    class MenuButton
    {
	public:
		CString Text;
		PointF  TextPosition;
		std::vector<PointF> Polygon;
		CRect Rectangle;
    };

public:
	DVDBuilderProject()
	{
		_TVSystem = TVSystem::NTSC;
	}

	virtual ~DVDBuilderProject(){ }

    TVSystem::Enum tvSystem()
    {
        return _TVSystem;
    }

    void setTvSystem(TVSystem::Enum tvSystem)
    {
        _TVSystem = tvSystem;
    }

	void createButtons(const std::vector<CString> &videos, std::vector<MenuButton> &buttons);
	void create(const CString &projectFile, const std::vector<CString> &videos, const CString &menusFolder);
	void generateMenuBackground(const CString &backgroudPath, const std::vector<MenuButton> &buttons);
	void generateMenuMask(const CString &maskPath, const std::vector<MenuButton> &buttons);
};
