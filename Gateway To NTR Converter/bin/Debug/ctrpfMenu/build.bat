@echo off
cd ctrpfMenu
make re
cd ..
sleep 0.8
copy "ctrpfMenu\ctrpfMenu.plg" "plugin.plg"
@echo off
echo Plugin ready !
pause