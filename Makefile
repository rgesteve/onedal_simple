.PHONY: clean

#CONDA_PREFIX=/root/miniconda3
CONDA_PREFIX=/opt/intel/oneapi/dal/2021.4.0

ONEDAL_INCLUDEDIR=$(CONDA_PREFIX)/include
ONEDAL_LIBDIR=$(CONDA_PREFIX)/lib/intel64

program: program.cpp lib_function.so
	g++ $< -I. -L. -Wl,--start-group -L${ONEDAL_LIBDIR} -L/opt/intel/oneapi/tbb/latest/lib/intel64/gcc4.8 -lonedal_core -lonedal_thread -ltbb -ltbbmalloc -Wl,--end-group -l_function -o program
#	g++ $< -I. -L. -Wl,--start-group -L${ONEDAL_LIBDIR} -lonedal_core -lonedal_sequential -lpthread -ldl -ltbb -ltbbmalloc -Wl,--end-group -l_function -o program
#   g++ program.cpp -I${ONEDAL_INCLUDEDIR} -L${ONEDAL_LIBDIR} -Wl,--start-group -lonedal_core -lonedal_sequential -lpthread -ldl -Wl,--end-group -o program

lib_function.so: function.cpp
	g++ $< -shared -fPIC -I$(ONEDAL_INCLUDEDIR) -L$(ONEDAL_LIBDIR) -Wl,--start-group -lonedal_core -lonedal_thread -Wl,--end-group -o lib_function.so
#	g++ $< -shared -fPIC -I$(ONEDAL_INCLUDEDIR) -L$(ONEDAL_LIBDIR) -Wl,--start-group -lonedal_core -lonedal_thread -lpthread -ldl -ltbb -ltbbmalloc -Wl,--end-group -o lib_function.so
#	g++ $< -shared -fPIC -I$(ONEDAL_INCLUDEDIR) -L$(ONEDAL_LIBDIR) -Wl,--start-group -lonedal_core -lonedal_sequential -lpthread -ldl -Wl,--end-group -o lib_function.so	
#   g++ function.cpp -shared -fPIC -I${ONEDAL_INCLUDEDIR} -L${ONEDAL_LIBDIR} -Wl,--start-group -lonedal_core -lonedal_sequential -lpthread -ldl -Wl,--end-group -o lib_function.so

clean:
	rm -rf program lib_function.so
