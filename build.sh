#!/usr/bin/env bash

BUILDDIR=build

mkdir -p ${BUILDDIR}
cmake -S . -B ${BUILDDIR}
cmake --build ${BUILDDIR}
