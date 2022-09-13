using System;
using System.Runtime.InteropServices;

namespace XGBoostProto
{
    /// <summary>
    /// Wrapper of DMatrix object of XGBoost
    /// </summary>
    internal sealed class DMatrix : IDisposable
    {

        private WrappedXGBoostInterface.SafeDMatrixHandle _handle;
        public WrappedXGBoostInterface.SafeDMatrixHandle Handle => _handle;

        /// <summary>
        /// Create a <see cref="DMatrix"/> for storing training and prediction data under XGBoost framework.
#if true
        public unsafe DMatrix(double[] data, float[] labels = null)
	{
	  _handle = null;
	}
#else
        public unsafe Dataset(double[][] sampleValuePerColumn,
            int[][] sampleIndicesPerColumn,
            int numCol,
            int[] sampleNonZeroCntPerColumn,
            int numSampleRow,
            int numTotalRow,
            string param, float[] labels, float[] weights = null, int[] groups = null)
        {
            _handle = null;

            // Use GCHandle to pin the memory, avoid the memory relocation.
            GCHandle[] gcValues = new GCHandle[numCol];
            GCHandle[] gcIndices = new GCHandle[numCol];
            try
            {
                double*[] ptrArrayValues = new double*[numCol];
                int*[] ptrArrayIndices = new int*[numCol];
                for (int i = 0; i < numCol; i++)
                {
                    gcValues[i] = GCHandle.Alloc(sampleValuePerColumn[i], GCHandleType.Pinned);
                    ptrArrayValues[i] = (double*)gcValues[i].AddrOfPinnedObject().ToPointer();
                    gcIndices[i] = GCHandle.Alloc(sampleIndicesPerColumn[i], GCHandleType.Pinned);
                    ptrArrayIndices[i] = (int*)gcIndices[i].AddrOfPinnedObject().ToPointer();
                }
                fixed (double** ptrValues = ptrArrayValues)
                fixed (int** ptrIndices = ptrArrayIndices)
                {
                    // Create container. Examples will pushed in later.
                    LightGbmInterfaceUtils.Check(WrappedLightGbmInterface.DatasetCreateFromSampledColumn(
                        (IntPtr)ptrValues, (IntPtr)ptrIndices, numCol, sampleNonZeroCntPerColumn, numSampleRow, numTotalRow,
                        param, out _handle));
                }
            }
            finally
            {
                for (int i = 0; i < numCol; i++)
                {
                    if (gcValues[i].IsAllocated)
                        gcValues[i].Free();
                    if (gcIndices[i].IsAllocated)
                        gcIndices[i].Free();
                }
            }
            // Before adding examples (i.e., feature vectors of the original data set), the original labels, weights, and groups are added.
            SetLabel(labels);
            SetWeights(weights);
            SetGroup(groups);

#if false
            Contracts.Assert(GetNumCols() == numCol);
            Contracts.Assert(GetNumRows() == numTotalRow);
	    #endif
        }

        public Dataset(Dataset reference, int numTotalRow, float[] labels, float[] weights = null, int[] groups = null)
        {
            WrappedLightGbmInterface.SafeDataSetHandle refHandle = reference?.Handle;

            LightGbmInterfaceUtils.Check(WrappedLightGbmInterface.DatasetCreateByReference(refHandle, numTotalRow, out _handle));

            SetLabel(labels);
            SetWeights(weights);
            SetGroup(groups);
        }
#endif

        public void Dispose()
        {
            _handle?.Dispose();
            _handle = null;
        }

#if false
        public int GetNumRows()
        {
            int res = 0;
            LightGbmInterfaceUtils.Check(WrappedLightGbmInterface.DatasetGetNumData(_handle, ref res));
            return res;
        }

        public int GetNumCols()
        {
            int res = 0;
            LightGbmInterfaceUtils.Check(WrappedLightGbmInterface.DatasetGetNumFeature(_handle, ref res));
            return res;
        }
#endif

        public unsafe void SetLabel(float[] labels)
        {
	#if false
            Contracts.AssertValue(labels);
            Contracts.Assert(labels.Length == GetNumRows());
            fixed (float* ptr = labels)
                LightGbmInterfaceUtils.Check(WrappedLightGbmInterface.DatasetSetField(_handle, "label", (IntPtr)ptr, labels.Length,
                    WrappedLightGbmInterface.CApiDType.Float32));
#endif
        }
    }
}
