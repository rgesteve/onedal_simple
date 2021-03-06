#!/usr/bin/env bash

# use wget to fetch the Intel repository public key
wget https://apt.repos.intel.com/intel-gpg-keys/GPG-PUB-KEY-INTEL-SW-PRODUCTS.PUB

# add to your apt sources keyring so that archives signed with this key will be trusted.
sudo apt-key add GPG-PUB-KEY-INTEL-SW-PRODUCTS.PUB

# remove the public key
rm GPG-PUB-KEY-INTEL-SW-PRODUCTS.PUB

# echo "deb https://apt.repos.intel.com/oneapi all main" | sudo tee /etc/apt/sources.list.d/oneAPI.list
sudo add-apt-repository "deb https://apt.repos.intel.com/oneapi all main"

# sudo apt install intel-basekit
# sudo apt install intel-hpckit
sudo apt update
sudo apt install -y intel-oneapi-dal-devel
