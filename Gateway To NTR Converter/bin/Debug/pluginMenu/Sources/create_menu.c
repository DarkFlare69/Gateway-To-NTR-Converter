#include "cheats.h"

#define ENTRY_COUNT 300

typedef struct s_menu
{
	int         count;
	int         status;
	u32         f[ENTRY_COUNT];
	u32         s[ENTRY_COUNT];
	int         a[ENTRY_COUNT];
	const char  *t[ENTRY_COUNT];
	const char  *n[ENTRY_COUNT];
	void        (*fp[ENTRY_COUNT])();
}             t_menu;


typedef void    (*FuncPointer)(void);
extern t_menu menu;


void    new_super_unselectable_entry(char *str, FuncPointer function)
{
	int index;

	index = menu.count;
	if (index >= 300)
		return;
	new_unselectable_entry(str);
	menu.f[index] |= BIT(0) | BIT(1);
	menu.fp[index] = function;
}

void with_note_common(const char *name, const char *note, void (*cheatfunction)(void), int type)
{
	int     index;

	if (type == 0)
		index = new_entry((char *)name, cheatfunction);
	else if (type == 1)
		index = new_radio_entry((char *)name, cheatfunction);
	else if (type == 2)
		index = new_spoiler((char *)name);
	else return;
	set_note(note, index);
}

inline void new_entry_with_note(const char *name, const char *note, void (*cheatfunction)(void))
{
	with_note_common(name, note, cheatfunction, 0);
}

inline void new_radio_entry_with_note(const char *name, const char *note, void (*cheatfunction)(void))
{
	with_note_common(name, note, cheatfunction, 1);
}

inline void new_spoiler_with_note(const char *name, const char *note)
{
	with_note_common(name, note, NULL, 2);
}

char	*builder_name = "";

	/* static const char * const code1 = "This is a note for code1";     #format: new_entry_with_note("Visible Code Name", note_name, code_name); */

void    my_menus(void)
{
	new_entry("cock()", cock__);
}
