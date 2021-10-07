cmake_minimum_required(VERSION 3.5)

project(OneDALNative)

find_package(oneDAL CONFIG REQUIRED)
#get_property(_loc TARGET FFTW3::fftw3 PROPERTY LOCATION)
#message(STATUS "Found FFTW3: ${_loc} (found version ${FFTW3_VERSION})")
message(STATUS "*** Found OneDAL in ${oneDAL_ROOT_DIR} ... ***")
message(STATUS "*** Version of OneDAL found is ${oneDAL_VERSION} ... ***")
