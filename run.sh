#!/usr/bin/env bash

CONDA_PREFIX=/root/miniconda3
ONEDAL_LIBDIR=${CONDA_PREFIX}/lib
# /opt/intel/oneapi/tbb/2021.4.0/lib/intel64/gcc4.8

#LD_LIBRARY_PATH=${ONEDAL_LIBDIR};. ./program
LD_LIBRARY_PATH=. ./program

echo "Done!"
