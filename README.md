# Simple OneDAL app (and C# wrapper)

Just playing around with OneDAL API, verifying builds on Linux and Windows.

(requires system installation of OneDAL via the installer on Windows and the apt package on Ubuntu... essentially anything that includes cmake modules to find OneDAL)

## Building on Linux

Install OneDAL [for Ubuntu](https://software.intel.com/content/www/us/en/develop/articles/installing-intel-free-libs-and-python-apt-repo.html)  (or from this [alternate set of instructions](https://neelravi.com/post/intel-oneapi-install/)).  In principle, other packagings of OneDAL/OneAPI should work (in particular, if you install OneDAL using `conda` from the `conda-forge` channel).  However, some packagings do not include the CMake modules on which our build script depends.

Be sure to issue:
```bash
apt install intel-oneapi-daal-devel
```

(there's a hopefully helpful script included called `install_onedal_ubuntu.sh`)

Once you've bult (say, in directory "build"), you can run the program issuing:
```bash
$ LD_LIBRARY_PATH=/opt/intel/oneapi/tbb/2021.4.0/lib/intel64/gcc4.8 build/OneDALNative
```
you can also do
```bash
source /opt/intel/oneapi/setvars.sh
```
before you run the libraries.

## Building on Windows

Install OneDAL using the [OneAPI installer](https://software.intel.com/content/www/us/en/develop/tools/oneapi.html).  This is a little cumbersome as it is an interactive download that cannot be easily scripted (e.g., for a CI) but we're working towards resolving this.  Another downside of using the installer is that it's a little heavy on the space usage side.  You can ameliorate this by customizing the installation.  For this library you only need tbb and onedal (some components are required and cannot be un-selected).

To run the program, you need to first add the OneAPI installation directory to the `PATH` environment variable
```powershell
> $env:PATH = $env:PATH + ";C:\Program Files (x86)\Intel\oneAPI\dal\2021.4.0\redist\intel64"
```

You can use the provided script "build.ps1", in which case, please call it from a Visual Studio prompt (as it will have CMake available):
```cmd
%ProgramFiles(x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\Launch-VsDevShell.ps1
```
If you're not sure where VS is installed, or you have several installations, you can manage which one to use with the `vswhere` command line (think of something similar to `xcode-select` on macOS), which you can install via `winget`:
```pwsh
> winget install vswhere
> $VSPath = $(vswhere -latest -property installationPath)
> $VSLaunch = $VSPath + "\Common7\Tools\Launch-VsDevShell.ps1"
> & $VSLaunch
```

## OneDALStandalone

This is meant to be a "playground" to try out OneDAL algorithms in C++, to understand/verify their behavior.
