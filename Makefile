.PHONY: clean

CONDA_PREFIX=/root/miniconda3

ONEDAL_INCLUDEDIR=$(CONDA_PREFIX)/include
ONEDAL_LIBDIR=$(CONDA_PREFIX)/lib

program: program.cpp lib_function.so
	g++ $< -I. -L. -Wl,--start-group -L${ONEDAL_LIBDIR} -lonedal_core -lonedal_sequential -lpthread -ldl -Wl,--end-group -l_function -o program
#   g++ program.cpp -I${ONEDAL_INCLUDEDIR} -L${ONEDAL_LIBDIR} -Wl,--start-group -lonedal_core -lonedal_sequential -lpthread -ldl -Wl,--end-group -o program

lib_function.so: function.cpp
	g++ $< -shared -fPIC -I$(ONEDAL_INCLUDEDIR) -L$(ONEDAL_LIBDIR) -Wl,--start-group -lonedal_core -lonedal_sequential -lpthread -ldl -Wl,--end-group -o lib_function.so
#   g++ function.cpp -shared -fPIC -I${ONEDAL_INCLUDEDIR} -L${ONEDAL_LIBDIR} -Wl,--start-group -lonedal_core -lonedal_sequential -lpthread -ldl -Wl,--end-group -o lib_function.so

clean:
	rm -rf program lib_function.so
