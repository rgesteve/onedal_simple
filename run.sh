#!/usr/bin/env bash

CONDA_PREFIX=/root/miniconda3
ONEDAL_LIBDIR=${CONDA_PREFIX}/lib

#LD_LIBRARY_PATH=${ONEDAL_LIBDIR};. ./program
LD_LIBRARY_PATH=. ./program

echo "Done!"
