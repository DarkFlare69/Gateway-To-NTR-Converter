#include "cheats.hpp"
u32 offset = 0;
u32 data32 = 0;
u8 data8 = 0;
u16 data16 = 0;
u32 patch_address = 0;
u16 cmp16 = 0;
u32 cmp32 = 0;

namespace CTRPluginFramework
{

void	code_OneTwoThreeFour(MenuEntry *entry)
{
	offset = 0;
	Process::Write8(offset + 0x1234, 0xFF);
	offset = 0;
	data32 = 0;
	data16 = 0;
	data8 = 0;
}

}