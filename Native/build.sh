#!/usr/bin/env bash

BUILDDIR=build
PUBLISHDIR=publish

rm -rf ${BUILDDIR}

mkdir -p ${BUILDDIR}
cmake -S . -B ${BUILDDIR}
cmake --build ${BUILDDIR}

nuget pack TestNative.nuspec
