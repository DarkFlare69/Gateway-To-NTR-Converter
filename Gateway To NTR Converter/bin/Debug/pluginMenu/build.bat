set PATH=%PATH%;C:\devkitPro\devkitARM\bin;C:\devkitPro\msys\bin
cd pluginMenu
"build.py"
cd ..
copy "pluginMenu\plugin.plg" "plugin.plg"
pause
