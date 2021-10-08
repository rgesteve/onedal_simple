#include <iostream>

//#include <daal.h>

#include "function.h"

using namespace std;

using namespace daal;
using namespace daal::algorithms;
using namespace daal::data_management;

ONEDAL_C_EXPORT int createTable(float* data, int numFeatures, int numObservations)
{
    cout << "Looking at table from a function" << endl;
    //NumericTablePtr dataTable(new HomogenNumericTable<double>::create(data, nFeatures, nObservations));
    NumericTablePtr dataTable(new HomogenNumericTable<float>(data, numFeatures, numObservations));
    // services::SharedPtr<HomogenNumericTable<> > dataTable = HomogenNumericTable<>::create(data, nFeatures, nObservations);

    cout << "Getting the rows from table: " << dataTable->getNumberOfRows() << "..." << endl;
    cout << "Getting the columns from table: " << dataTable->getNumberOfColumns() << "..." << endl;

    return (dataTable->getNumberOfRows()) * (dataTable->getNumberOfColumns());
}

ONEDAL_C_EXPORT int ridgeRegressionOnlineCompute(void * featuresPtr, void * labelsPtr, int nRows, int nColumns, float l2Reg, void * partialResultPtr, int partialResultSize)
{
    return ridgeRegressionOnlineComputeTemplate<double>((double *)featuresPtr, (double *)labelsPtr, nRows, nColumns, l2Reg, (byte *)partialResultPtr, partialResultSize);
}

ONEDAL_C_EXPORT void ridgeRegressionOnlineFinalize(void * featuresPtr, void * labelsPtr, long long int nAllRows, int nRows, int nColumns, float l2Reg, void * partialResultPtr, int partialResultSize,
    void * betaPtr, void * xtyPtr, void * xtxPtr)
{
    ridgeRegressionOnlineFinalizeTemplate<double>((double *)featuresPtr, (double *)labelsPtr, nAllRows, nRows, nColumns, l2Reg, (byte *)partialResultPtr, partialResultSize,
        (double *)betaPtr, (double *)xtyPtr, (double *)xtxPtr);
}

ONEDAL_C_EXPORT void linearRegressionDouble(void * features, void * label, void * betas, int nRows, int nColumns)
{
    linearRegression<double>((double *)features, (double *)label, (double *)betas, nRows, nColumns);
}

ONEDAL_C_EXPORT void linearRegressionSingle(void * features, void * label, void * betas, int nRows, int nColumns)
{
    linearRegression<float>((float *)features, (float *)label, (float *)betas, nRows, nColumns);
}

ONEDAL_C_EXPORT void ridgeRegressionDouble(void * features, void * label, void * betas, int nRows, int nColumns, float l2Reg)
{
    ridgeRegression<double>((double *)features, (double *)label, (double *)betas, nRows, nColumns, l2Reg);
}

ONEDAL_C_EXPORT void ridgeRegressionSingle(void * features, void * label, void * betas, int nRows, int nColumns, float l2Reg)
{
    ridgeRegression<float>((float *)features, (float *)label, (float *)betas, nRows, nColumns, l2Reg);
}
