#include "daal.h"
#include <cstdio>
#include <cstdlib>

#include <algorithm>
#include <string>
#include <iostream>
#include <fstream>
#include <sstream>
#include <iomanip>
#include <cstdarg>
#include <vector>
#include <queue>
#include <chrono>

#include "KNNAlgorithm.h"

#if 0
/**
 * @brief Class for defining printing precision
 * @tparam type type name to define presicion value
 */
template <typename type>
struct PrecisionValue
{
    static int get() { return 0; }
};

/**
 * @brief Partial specialization for float type
 * @return Pronting precision for float type
 */
template <>
struct PrecisionValue<float>
{
    static int get() { return 3; }
};

/**
 * @brief Partial specialization for double type
 * @return Pronting precision for double type
 */
template <>
struct PrecisionValue<double>
{
    static int get() { return 3; }
};

template <typename type1, typename type2>
void printNumericTables(daal::data_management::NumericTable * dataTable1, daal::data_management::NumericTable * dataTable2, const char * title1 = "",
                        const char * title2 = "", const char * message = "", size_t nPrintedRows = 0, size_t interval = 15)
{
    using namespace daal::data_management;

    size_t nRows1 = dataTable1->getNumberOfRows();
    size_t nRows2 = dataTable2->getNumberOfRows();
    size_t nCols1 = dataTable1->getNumberOfColumns();
    size_t nCols2 = dataTable2->getNumberOfColumns();

    BlockDescriptor<type1> block1;
    BlockDescriptor<type2> block2;

    size_t nRows = std::min(nRows1, nRows2);
    if (nPrintedRows != 0)
    {
        nRows = std::min(std::min(nRows1, nRows2), nPrintedRows);
    }

    dataTable1->getBlockOfRows(0, nRows, readOnly, block1);
    dataTable2->getBlockOfRows(0, nRows, readOnly, block2);

    type1 * data1 = block1.getBlockPtr();
    type2 * data2 = block2.getBlockPtr();

    std::cout << std::setiosflags(std::ios::left);
    std::cout << message << std::endl;
    std::cout << std::setw(interval * nCols1) << title1;
    std::cout << std::setw(interval * nCols2) << title2 << std::endl;
    for (size_t i = 0; i < nRows; i++)
    {
        for (size_t j = 0; j < nCols1; j++)
        {
            std::cout << std::setw(interval) << std::setiosflags(std::ios::fixed) << std::setprecision(PrecisionValue<type1>::get());
            std::cout << data1[i * nCols1 + j];
        }
        for (size_t j = 0; j < nCols2; j++)
        {
            std::cout << std::setprecision(PrecisionValue<type2>::get()) << std::setw(interval) << data2[i * nCols2 + j];
        }
        std::cout << std::endl;
    }
    std::cout << std::endl;

    dataTable1->releaseBlockOfRows(block1);
    dataTable2->releaseBlockOfRows(block2);
}

template <typename type1, typename type2>
void printNumericTables(daal::data_management::NumericTablePtr dataTable1, daal::data_management::NumericTablePtr dataTable2,
                        const char * title1 = "", const char * title2 = "", const char * message = "", size_t nPrintedRows = 0, size_t interval = 15)
{
    printNumericTables<type1, type2>(dataTable1.get(), dataTable2.get(), title1, title2, message, nPrintedRows, interval);
}
#endif

using namespace std;
using namespace daal;
using namespace daal::algorithms;
using namespace daal::data_management;
using namespace std::chrono;

/* Input data set parameters */
#if 1
// Looks like CSV reader as currently configured doesn't like headers
#if 0
string trainDatasetFileName = "../data/batch/k_nearest_neighbors_train.csv";
string testDatasetFileName  = "../data/batch/k_nearest_neighbors_test.csv";
#else
string trainDatasetFileName = "/home/rgesteve/projects/onedal_simple/OneDALStandalone/data/k_nearest_neighbors_train.csv";
string testDatasetFileName  = "/home/rgesteve/projects/onedal_simple/OneDALStandalone/data/k_nearest_neighbors_test.csv";
#endif
#else
string trainDatasetFileName = "/home/rgesteve/projects/oneDAL/examples/daal/data/batch/k_nearest_neighbors_train.csv";
string testDatasetFileName  = "/home/rgesteve/projects/oneDAL/examples/daal/data/batch/k_nearest_neighbors_test.csv";
#endif

size_t nFeatures = 5;
size_t nClasses = 5;

bf_knn_classification::training::ResultPtr trainingResult;
bf_knn_classification::prediction::ResultPtr predictionResult;
NumericTablePtr testGroundTruth;
NumericTablePtr testData;

#if 0
void trainModel();
#else
void trainModel(NumericTablePtr, NumericTablePtr);
#endif
void testModel();
#if 0
void printResults();
#endif

KNNAlgorithm knn(nClasses);

int main(int argc, char *argv[]) {

    if(const char* env_p = std::getenv("PATH"))
        std::cout << "Your PATH is: " << env_p << '\n';

    cout << "Going to read test data from [" << testDatasetFileName << "]" << endl;

    cout << "Begin training!!\n" << endl;

    // Initialize FileDataSource<CSVFeatureManager> to retrieve the input data from a .csv file
    FileDataSource<CSVFeatureManager> trainDataSource(trainDatasetFileName, DataSource::notAllocateNumericTable, DataSource::doDictionaryFromContext);

    // Create Numeric Tables for training data and labels 
    NumericTablePtr trainData(new HomogenNumericTable<>(nFeatures, 0, NumericTable::doNotAllocate));
    NumericTablePtr trainGroundTruth(new HomogenNumericTable<>(1, 0, NumericTable::doNotAllocate));
    NumericTablePtr mergedData(new MergedNumericTable(trainData, trainGroundTruth));

    // Retrieve the data from the input file 
    trainDataSource.loadDataBlock(mergedData.get());

    auto startTime = std::chrono::system_clock::now();
    trainModel(trainData, trainGroundTruth);
    auto endTime = std::chrono::system_clock::now();
    testModel();

#if 0
    printResults();
#endif
    knn.print_results();

    cout << duration_cast<nanoseconds>(endTime - startTime).count() << " nanosec \n";
    return 0;
}

void trainModel(NumericTablePtr trainData, NumericTablePtr trainGroundTruth) {
#if 0
    /* Initialize FileDataSource<CSVFeatureManager> to retrieve the input data
     * from a .csv file */
    FileDataSource<CSVFeatureManager> trainDataSource(
        trainDatasetFileName, DataSource::notAllocateNumericTable, DataSource::doDictionaryFromContext);

  /* Create Numeric Tables for training data and labels */
    NumericTablePtr trainData(new HomogenNumericTable<>(nFeatures, 0, NumericTable::doNotAllocate));
    NumericTablePtr trainGroundTruth(new HomogenNumericTable<>(1, 0, NumericTable::doNotAllocate));
    NumericTablePtr mergedData(new MergedNumericTable(trainData, trainGroundTruth));

    /* Retrieve the data from the input file */
    trainDataSource.loadDataBlock(mergedData.get());
#endif

    cout << "------ setting training data -------" <<endl;
    knn.set_train_data(trainData, trainGroundTruth);
    cout << "------ before calling compute -------" <<endl;
    knn.train();
    cout << "------ done training -------" <<endl;
}

void testModel() {
    /* Initialize FileDataSource<CSVFeatureManager> to retrieve the test data from
     * a .csv file */
    FileDataSource<CSVFeatureManager> testDataSource(
        testDatasetFileName, DataSource::notAllocateNumericTable,
        DataSource::doDictionaryFromContext);

    /* Create Numeric Tables for testing data and labels */
    testData = NumericTablePtr(new HomogenNumericTable<>(nFeatures, 0, NumericTable::doNotAllocate));
    testGroundTruth = NumericTablePtr(new HomogenNumericTable<>(1, 0, NumericTable::doNotAllocate));
    NumericTablePtr mergedData(new MergedNumericTable(testData, testGroundTruth));

    /* Retrieve the data from input file */
    testDataSource.loadDataBlock(mergedData.get());

    cout << "------ setting data in class -------" <<endl;
    knn.set_test_data(testData);

    cout << "------ start predicting -------" <<endl;

    /* Compute prediction results */
    knn.predict();

    cout << "------ done predicting -------" <<endl;
}

#if 0
void printResults() {
    printNumericTables<int, int>(
        testGroundTruth,
        predictionResult->get(bf_knn_classification::prediction::prediction),
        "Ground truth", "Classification results",
        "Brute force kNN classification results (first 20 observations):", 20);
    printNumericTables<int, float>(
        predictionResult->get(bf_knn_classification::prediction::indices),
        predictionResult->get(bf_knn_classification::prediction::distances),
        "Indices", "Distances",
        "Brute force kNN classification results (first 20 observations):", 20);
}
#endif