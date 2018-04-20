#include "cheats.hpp"

namespace CTRPluginFramework
{
	void	PatchProcess(FwkSettings &settings) { }

	int	main()
	{
		PluginMenu *menu = new PluginMenu("My NTR Plugin", 1, 0, 0);
		menu->SynchronizeWithFrame(true);


		MenuFolder *folder = nullptr, *subfolder = nullptr, *subsubfolder = nullptr;

		menu->Append(new MenuEntry("code 1234", code_OneTwoThreeFour)); 

		menu->Run();
		return 0;
	}
}
