#ifndef KNNALGORITHM_H
#define KNNALGORITHM_H

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

using namespace std;
using namespace daal;
using namespace daal::algorithms;
using namespace daal::data_management;

template <typename T>
void lookAtTable(daal::data_management::NumericTable * dataTable)
{
    using namespace daal::data_management;

    size_t nRows = dataTable->getNumberOfRows();
    size_t nCols = dataTable->getNumberOfColumns();

    std::cout << "----------------- printing (float) table ---------" << std::endl;
    std::cout << "Rows: [" << nRows << "], Cols: " << nCols << "]" << std::endl;

    BlockDescriptor<T> block;

    size_t nRowsToGet = 10;

    dataTable->getBlockOfRows(0, nRowsToGet, readOnly, block);
    T* data = block.getBlockPtr();

#if 0
    std::cout << std::setiosflags(std::ios::left);
    std::cout << message << std::endl;
    std::cout << std::setw(interval * nCols1) << title1;
    std::cout << std::setw(interval * nCols2) << title2 << std::endl;
#endif

    for (size_t i = 0; i < nRowsToGet; i++) {
      std::cout << i << ": ";
        for (size_t j = 0; j < nCols; j++) {
#if 0
            std::cout << std::setw(interval) << std::setiosflags(std::ios::fixed) << std::setprecision(PrecisionValue<T>::get());
#endif
            std::cout << data[i * nCols + j] << " ";
        }
        std::cout << std::endl;
    }
    std::cout << std::endl;

    dataTable->releaseBlockOfRows(block);
}

class KNNAlgorithm
{
private:
  
  bf_knn_classification::training::Batch<> train_algorithm;
  bf_knn_classification::prediction::Batch<> predict_algorithm;
  size_t _numClasses;

public:
  KNNAlgorithm(size_t numClasses);
  void set_train_data(NumericTablePtr train_features_dataset, NumericTablePtr train_labels_dataset);
  void train();
  void set_test_data(NumericTablePtr test_features_dataset);
  void predict();
  void print_results();
  void train_with_data(void* trainData, void* labelData, int numFeatures, int numObservations);
  float sanity_check_data(void* dataBlock, int blockSize, void* outputArray);
  int create_knn_table(void* dataBlock, int numCols, int numRows);
};

#endif
