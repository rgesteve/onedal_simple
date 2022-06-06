#!/usr/bin/env python3

import os, sys
import csv

#  ~/projects/oneDAL/

if not 'DAL_REPO' in os.environ:
    print("Need to define appropriate environ variables")
    sys.exit(1)

dal_path = os.environ['DAL_REPO']

if not os.path.isdir(dal_path):
    print("Specified path for DAL is not a valid directory")
    sys.exit(1)

print(f"Looking for DAL in {dal_path}")

relative_data_path = ["examples", "daal", "data", "batch", "k_nearest_neighbors_test.csv"]

data_path = os.path.join(dal_path, os.path.join(*relative_data_path))

if not os.path.exists(data_path):
    print("Cannot find expected data directory in specified path")
    sys.exit(1)

print(f"The version of python is {sys.version_info} and should be looking at path: {data_path}");

rdr = csv.reader(open(data_path, "r"))

for row in rdr:
    print(f"{row} : {type(row)} : {len(row)}")

print("Done!")


