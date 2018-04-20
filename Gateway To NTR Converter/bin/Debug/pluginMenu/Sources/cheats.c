#include "cheats.h"
#include <stdbool.h>
#include "hid.h"
#include "values.h"
#include <string.h>

u32 offset = 0;
u32 data = 0;
u32 patch_address = 0;

void	code_OneTwoThreeFour(void)
{
	offset = 0x00000000;
	WRITEU8(offset + 0x0001234, 0x000000FF);
	offset = 0;
	data = 0;
}

