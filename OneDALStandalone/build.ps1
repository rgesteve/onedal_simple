$builddir="build"

Remove-Item -Path $builddir -Recurse -ErrorAction Ignore
New-Item -Path $builddir -Type Directory 
cmake -S . -B $builddir
cmake --build $builddir
