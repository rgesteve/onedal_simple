cmake_minimum_required(VERSION 3.5)

project(OneDALNative)

option(WITH_ONEDAL "Build with oneDAL" ON)

if(WITH_ONEDAL)
    message(STATUS "*** oneDAL is specified, building the OneDAL.")
    set(USE_DPCPP no)
    find_package(oneDAL CONFIG REQUIRED)

    include_directories(${oneDAL_INCLUDE_DIRS})
    link_libraries(${oneDAL_IMPORTED_TARGETS})

    add_library(OneDALNative_lib SHARED function.cpp KNNAlgorithm.cpp)
    add_executable(OneDALNative program.cpp)
    target_link_libraries(OneDALNative PUBLIC OneDALNative_lib ${oneDAL_IMPORTED_TARGETS})

    add_executable(OneDALKNNNative test_knn.cpp KNNAlgorithm.cpp)
    #target_link_libraries(OneDALKNNNative PUBLIC OneDALNative_lib ${oneDAL_IMPORTED_TARGETS})
    target_link_libraries(OneDALKNNNative PUBLIC ${oneDAL_IMPORTED_TARGETS})

    # target_include_directories(OneDALNative PRIVATE ${oneDAL_INCLUDE_DIRS})
    # if (UNIX)
    #   target_link_libraries(OneDALNative PRIVATE -Wl,--start-group ${oneDAL_IMPORTED_TARGETS} -Wl,--end-group)
    # else()
    #   target_link_libraries(OneDALNative PRIVATE ${oneDAL_IMPORTED_TARGETS})
    # endif()
    # target_compile_options(OneDALNative PRIVATE ${CUSTOM_OPTIONS})
    # target_link_options(OneDALNative PRIVATE ${CUSTOM_OPTIONS})
    # set_target_properties(OneDALNative PROPERTIES RUNTIME_OUTPUT_DIRECTORY "${PROJECT_SOURCE_DIR}/_cmake_results/intel_intel64_${THREADING_TYPE}_${LINK_TYPE}")

    ##target_include_directories(OneDALNative ${oneDAL_INCLUDE_DIRS})
    ##target_link_libraries(OneDALNative ${oneDAL_IMPORTED_TARGETS})
else()
    message(STATUS "**** oneDAL is not specified, building a OneDAL-free version.")
    add_library(OneDALNative_lib SHARED function_nodal.cpp)
    # Windows wants to link against a "*.lib", not a "*.dll"... tried this but it didn't work
    #set_target_properties(OneDALNative_lib PROPERTIES LINKER_LANGUAGE C)

    add_executable(OneDALNative program_nodal.cpp)
    # Windows wants to link against a "*.lib", not a "*.dll"
    #target_link_libraries(OneDALNative PUBLIC OneDALNative_lib)
    target_link_libraries(OneDALNative OneDALNative_lib)
endif()
