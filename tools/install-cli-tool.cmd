pushd ..\src\SysTec.CSharpToTypeScript.CLITool
dotnet pack -c Release
dotnet tool uninstall --global SysTec.CSharpToTypeScript.CLITool
dotnet tool install --no-cache --global --add-source ./nupkg SysTec.CSharpToTypeScript.CLITool
popd