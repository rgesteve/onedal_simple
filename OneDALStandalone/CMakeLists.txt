cmake_minimum_required(VERSION 3.5)

project(OneDALStandaloneTest)

set(USE_DPCPP no)
find_package(oneDAL CONFIG REQUIRED)

include_directories(${oneDAL_INCLUDE_DIRS})
link_libraries(${oneDAL_IMPORTED_TARGETS})

set(copy_files
  data/linear_regression_train.csv)

#set(DATA_FILES_DIR ${CMAKE_BINARY_DIR}/data)
#file(MAKE_DIRECTORY ${DATA_FILES_DIR})
#file(COPY ${copy_files} DESTINATION ${DATA_FILES_DIR})

#configure_file(config.h.in config.h @ONLY)
#include_directories(${CMAKE_CURRENT_BINARY_DIR})

add_executable(OneDALStandaloneTest program.cpp)

#foreach(copy_file ${copy_files})
#  # get_filename_component(target ${copy_file} NAME_WE)
#  add_custom_target(${target}_${method} ALL
#    COMMAND ${CMAKE_COMMAND} -E copy
#    ${CMAKE_CURRENT_SOURCE_DIR}/${copy_file}
#    ${dest})
#endforeach(copy_file ${copy_file})

#target_link_libraries(OneDALStandaloneTest PUBLIC OneDALNative_lib ${oneDAL_IMPORTED_TARGETS})

