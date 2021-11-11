#ifndef _FUNCTION_HPP_
#define _FUNCTION_HPP_

#include "daal.h"
#if 0
#include "data_management/data/internal/finiteness_checker.h"
#endif

using namespace std;

// samples/daal/cpp/mysql/sources/utils.h
void printNumericTable(const daal::data_management::NumericTablePtr & dataTable, const char * message = "");

template <typename T>
inline void printArray(T* array, const size_t nPrintedCols, const size_t nPrintedRows, const size_t nCols, const std::string & message, size_t interval = 10)
{
    std::cout << std::setiosflags(std::ios::left);
    std::cout << message << std::endl;
    for (size_t i = 0; i < nPrintedRows; i++) {
        for (size_t j = 0; j < nPrintedCols; j++) {
            std::cout << std::setw(interval) << std::setiosflags(std::ios::fixed) << std::setprecision(3);
            std::cout << array[i * nCols + j];
            std::cout << " "; // FIXME -- this should not be needed
        }
        std::cout << std::endl;
    }
    std::cout << std::endl;
}

#endif