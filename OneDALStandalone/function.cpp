#include <iostream>
#include <iomanip>

#include "function.hpp"

using namespace daal;
using namespace daal::data_management;
using namespace daal::algorithms::linear_regression;

using namespace std;

void printNumericTable(const daal::data_management::NumericTablePtr & dataTable, const char * message)
{
    using namespace daal::data_management;

    size_t nRows = dataTable->getNumberOfRows();
    size_t nCols = dataTable->getNumberOfColumns();

    cout << "The characteristics of the table are: \n";
    cout << "nRows = " << nRows << endl;
    cout << "nCols = " << nCols << endl;

    size_t interval = 3; // separation between numbers
    //BlockDescriptor<DAAL_DATA_TYPE> block;
    BlockDescriptor<int> block;
    {
        dataTable->getBlockOfRows(0, nRows, readOnly, block);
        printArray<int>(block.getBlockPtr(), /* nPrintedCols */ nRows, /* nPrintedRows */ nCols, nCols, message, interval);
    }
    dataTable->releaseBlockOfRows(block);

    return;
}
