#include <iostream>

#include <daal.h>

#include "function.h"

using namespace std;

using namespace daal;
using namespace daal::algorithms;
using namespace daal::data_management;

ONEDAL_C_EXPORT void createTable(float* const data, size_t numFeatures, size_t numObservations)
{
    cout << "Looking at table from a function" << endl;
    //NumericTablePtr dataTable(new HomogenNumericTable<double>::create(data, nFeatures, nObservations));
    NumericTablePtr dataTable(new HomogenNumericTable<float>(data, numFeatures, numObservations));
    // services::SharedPtr<HomogenNumericTable<> > dataTable = HomogenNumericTable<>::create(data, nFeatures, nObservations);

    cout << "Getting the rows from table: " << dataTable->getNumberOfRows() << "..." << endl;
    cout << "Getting the columns from table: " << dataTable->getNumberOfColumns() << "..." << endl;
}
