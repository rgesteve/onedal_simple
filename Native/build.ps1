$builddir="build"

Remove-Item -Path $builddir -Recurse 
New-Item -Path $builddir -Type Directory 
cmake -S . -B $builddir
cmake --build $builddir
