#ifndef KNNALGORITHM_H
#define KNNALGORITHM_H

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
  int sanity_check_data(void* dataBlock, int blockSize);
};

#endif
