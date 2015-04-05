#include "GLESHelper.h"
#include <OpenGLES/ES2/glext.h>

namespace Unity
{
	class Color32
	{
		unsigned char	r,g,b,a;
	};
}

extern "C"
{
	extern bool PopReadPixels(uintptr_t TextureId,Unity::Color32* ColourBuffer,int TextureWidth,int TextureHeight);
}

bool PopReadPixels(uintptr_t TextureId,Unity::Color32* ColourBuffer,int TextureWidth,int TextureHeight)
{
	if ( !glIsTexture(TextureId) )
		return false;
	
	//	assume is bound
	//	glReadPixels(0, 0, 1, 1, GL_RGBA, GL_UNSIGNED_BYTE, ColourBuffer);
	auto Error = glGetError();
	
	if ( Error != GL_NONE )
		return false;
	
	return true;
}
