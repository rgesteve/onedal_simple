cmake_minimum_required(VERSION 3.5)

project(OneDALStandaloneTest)

set(USE_DPCPP no)
find_package(oneDAL CONFIG REQUIRED)

include_directories(${oneDAL_INCLUDE_DIRS})
link_libraries(${oneDAL_IMPORTED_TARGETS})

add_executable(OneDALStandaloneTest program.cpp)
#target_link_libraries(OneDALStandaloneTest PUBLIC OneDALNative_lib ${oneDAL_IMPORTED_TARGETS})

