#include <iostream>
#include <iomanip>
#include <cstdlib>
#include <vector>
#include <numeric>
#include <algorithm>
#include <iterator>
#include <filesystem>

#include "daal.h"
#if 0
#include "data_management/data/internal/finiteness_checker.h"
#endif

#include "function.hpp"

#include "config.h"

using namespace std;
using namespace std::filesystem;

using namespace daal;
using namespace daal::data_management;
using namespace daal::algorithms::linear_regression;

const size_t nFeatures         = 2; // dimensionality
const size_t nObservations     = 10;

int main(int argc, char* argv[])
{
#if 0
    float data[nFeatures * nObservations] = { 0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1,    1.0f, 1.1f, 1.2f, 1.3f, 1.4f,
                                              1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2,    2.0f, 2.1f, 2.2f, 2.3f, 2.4f, 2.5f, 2.6f, 2.7f, 2.8f, 2.9f,
                                              3,    3.0f, 3.1f, 3.2f, 3.3f, 3.4f, 3.5f, 3.6f, 3.7f, 3.8f, 3.9f, 4,    4.0f, 4.1f, 4.2f, 4.3f,
                                              4.4f, 4.5f, 4.6f, 4.7f, 4.8f, 4.9f, 5,    5.0f, 5.1f, 5.2f, 5.3f, 5.4f, 5.5f, 5.6f, 5.7f, 5.8f,
                                              5.9f, 1,    6.0f, 6.1f, 6.2f, 6.3f, 6.4f, 6.5f, 6.6f, 6.7f, 6.8f, 6.9f, 2,    7.0f, 7.1f, 7.2f,
                                              7.3f, 7.4f, 7.5f, 7.6f, 7.7f, 7.8f, 7.9f, 3,    8.0f, 8.1f, 8.2f, 8.3f, 8.4f, 8.5f, 8.6f, 8.7f,
                                              8.8f, 8.9f, 4,    9.0f, 9.1f, 9.2f, 9.3f, 9.4f, 9.5f, 9.6f, 9.7f, 9.8f, 9.9f, 5 };
#endif

    cout << "Looking for data files at: " DATA_FILES_DIR << "." << endl;
    if (! exists(DATA_FILES_DIR)) {
        cout << "Expected data files path (" DATA_FILES_DIR << ") doestn't seem to exist" << endl;
        return EXIT_FAILURE;
    } else {
        cout << "Looks like data files are where expected." << endl;
    }

    std::vector<int> data(nFeatures * nObservations); 

    cout << "Creating data: \n";
    //std::iota(data.begin(), data.end(), 1); 
    std::iota(begin(data), end(data), -5); 
    std::copy(begin(data), end(data), ostream_iterator<int>{cout, ", "});
    cout << '\n';

    cout << "Creating Numeric tables: \n";
    NumericTablePtr featuresTable(new HomogenNumericTable<int>(data.data(), nFeatures, nObservations));
    printNumericTable(featuresTable);

    // examples\daal\cpp\source\linear_regression\lin_reg_qr_dense_batch.cpp
    cout << "Reading tables from CSV: \n";
    const path& datasetFilename = path(DATA_FILES_DIR) / "linear_regression_train.csv";
    if (!exists(datasetFilename)) {
        cout << "Cannot find actual dataset file" << endl;
        return EXIT_FAILURE;
    }
    const size_t nFeatures           = 10; /* Number of features in training and testing data sets */
    const size_t nDependentVariables = 2;  /* Number of dependent variables that correspond to each observation */
    FileDataSource<CSVFeatureManager> dataSource(datasetFilename.lexically_normal().string(), DataSource::notAllocateNumericTable, DataSource::doDictionaryFromContext);
    NumericTablePtr dataFeatures(new HomogenNumericTable<>(nFeatures, 0, NumericTable::doNotAllocate));
    NumericTablePtr dependentVariables(new HomogenNumericTable<>(1, 0, NumericTable::doNotAllocate));
    NumericTablePtr mergedData(new MergedNumericTable(dataFeatures, dependentVariables));
    printNumericTable(mergedData);

    cout << "Loading read data into numeric table: \n";
    /* Retrieve the data from input file */
    dataSource.loadDataBlock(mergedData.get());
    //printNumericTable(mergedData);
    printNumericTable(dataFeatures);

    cout << "Done!" << endl;

    return EXIT_SUCCESS;
}

