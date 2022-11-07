#!/usr/bin/env bash

BUILDDIR=build
PUBLISHDIR=publish

rm -rf ${BUILDDIR}

mkdir -p ${BUILDDIR}
cmake -S . -B ${BUILDDIR} -DWITH_ONEDAL=NO
cmake --build ${BUILDDIR}

if hash nuget 2>/dev/null; then
    echo "Should be packaging using nuget"
    nuget pack TestNative.nuspec
else
    echo "Need to have nuget available to pack"
fi
