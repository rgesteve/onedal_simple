$builddir="build"

New-Item -Path $builddir -Type Directory 
cmake -S . -B $builddir
cmake --build $builddir
