if not exist "$(ProjectDir)mods\$(TargetName)" (mkdir $(ProjectDir)\mods\$(TargetName))
copy /Y "$(TargetPath)" "$(ProjectDir)mods\$(TargetName)\$(TargetName).dll"
if exist "$(ProjectDir)anims" (xcopy /Y /R /I /E /Q  "$(ProjectDir)anims" "$(ProjectDir)mods\$(TargetName)\anims")
if exist "$(ProjectDir)Config.json" (copy /Y "$(ProjectDir)Config.json" "$(ProjectDir)mods\$(TargetName)\Config.json")
if exist "$(ProjectDir)mod_info.yaml" (copy /Y "$(ProjectDir)mod_info.yaml" "$(ProjectDir)mods\$(TargetName)\mod_info.yaml")
echo F | xcopy /Y /R /I /E /Q "$(ProjectDir)mods\$(TargetName)" "C:%HOMEPATH%\Documents\Klei\OxygenNotIncluded\mods\dev\$(TargetName)"
rmdir /s /q "$(ProjectDir)mods"