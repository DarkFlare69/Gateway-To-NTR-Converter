#include "cheats.h"
#include <stdbool.h>
#include "hid.h"
#include "values.h"
#include <string.h>

u32 offset = 0;
u32 data = 0;
u32 patch_address = 0;

void	MaxInf_AP_vOne_Five(void)
{
	if (READU32(offset + 0x81FB284) < 0x8200000)
	{
		if (READU32(offset + 0x81FB624) < 0x8200000)
		{
			offset = READU32(offset + 0x81FB284);
			for (int i = 0; i < 0x2; i++)
			{
				for (int i = 0; i < 0x4; i++)
				{
					WRITEU8(offset + 0x118, 0x63);
					WRITEU8(offset + 0x119, 0x63);
					WRITEU8(offset + 0x11E, 0x63);
					WRITEU8(offset + 0x11F, 0x63);
					offset += 0xE;
					continue;
				}
				offset = 0;
				offset = READU32(offset + 0x81FB624);
				continue;
			}
		}
	}
}

