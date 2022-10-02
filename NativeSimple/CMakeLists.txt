cmake_minimum_required(VERSION 3.21)

project(
  nativesimple
  VERSION 0.1
  DESCRIPTION "Simple library with no dependencies to test out nuget generator for cpack"
  LANGUAGES CXX)

add_library(nativesimple STATIC)
target_sources(nativesimple PRIVATE src/lib.cpp)
target_include_directories(nativesimple PUBLIC include)
target_compile_features(nativesimple PRIVATE cxx_std_11)

include(GNUInstallDirs) # Defines the ${CMAKE_INSTALL_INCLUDEDIR} variable.
install(TARGETS nativesimple)
# Install the header files. Since header files are not listed as output artifacts, they have
# to be installed separately. 
install (
    DIRECTORY include/
    DESTINATION "${CMAKE_INSTALL_INCLUDEDIR}"
)
