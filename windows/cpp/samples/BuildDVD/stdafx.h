// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#define _CRT_SECURE_NO_WARNINGS

#include <iostream>
#include <cassert>
#include <string>

#include <tchar.h>

// reference additional headers your program requires here
#include "windows.h"

#if defined(_UNICODE) || defined(UNICODE)
typedef std::wstring tstring;
#else
typedef std::string tstring;
#endif
