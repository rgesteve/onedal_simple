#include "KNNAlgorithm.h"

using namespace std;
using namespace daal;
using namespace daal::algorithms;
using namespace daal::data_management;

// TODO -- this is just for debugging
#if 1
#include <iostream>
#endif

KNNAlgorithm::KNNAlgorithm(size_t numClasses = 2)
{
  _numClasses = numClasses;
}

void KNNAlgorithm::set_train_data(NumericTablePtr train_features_dataset, NumericTablePtr train_labels_dataset)
{
  train_algorithm.input.set(classifier::training::data, train_features_dataset);
  train_algorithm.input.set(classifier::training::labels, train_labels_dataset);
  train_algorithm.parameter().nClasses = _numClasses;
}

void KNNAlgorithm::train()
{
  train_algorithm.compute();
}

void KNNAlgorithm::set_test_data(NumericTablePtr test_features_dataset)
{
  bf_knn_classification::training::ResultPtr train_result = train_algorithm.getResult();
  predict_algorithm.input.set(classifier::prediction::data, test_features_dataset);
  predict_algorithm.input.set(classifier::prediction::model, train_result->get(classifier::training::model));
  predict_algorithm.parameter().nClasses = _numClasses;
  predict_algorithm.parameter().resultsToCompute = bf_knn_classification::computeDistances | bf_knn_classification::computeIndicesOfNeighbors;
}

void KNNAlgorithm::predict()
{
  predict_algorithm.compute();
}

void KNNAlgorithm::print_results()
{
  bf_knn_classification::prediction::ResultPtr predict_result;
  predict_result = predict_algorithm.getResult();
    
  NumericTablePtr results = predict_result->get(bf_knn_classification::prediction::prediction);
  lookAtTable<int>(results.get());
}

float KNNAlgorithm::sanity_check_data(void* dataBlock, int blockSize, void* outputArray)
{
  using namespace std;

  float acc = 0.f;
  float* original = (float*)dataBlock;
  float* output = (float*)outputArray;

  for (int i = 0; i < blockSize; i++) {
    acc += original[i];
    output[i] = 2.f * original[i];
  }
  
  return acc;
}

int KNNAlgorithm::create_knn_table(void* dataBlock, int numCols, int numRows)
{
  float* original = (float*)dataBlock;
  NumericTablePtr table(new HomogenNumericTable<float>(original, numCols, numRows));
  lookAtTable<float>(table.get());

  return 0;
}
