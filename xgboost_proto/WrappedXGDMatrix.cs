using System;
using System.Runtime.InteropServices;

namespace XGBoostProto
{
    /// <summary>
    /// Wrapper of DMatrix object of XGBoost
    /// </summary>
    #if false
    internal
    #else
    public
    #endif
    sealed class DMatrix : IDisposable
    {

        private WrappedXGBoostInterface.SafeDMatrixHandle _handle;
        public WrappedXGBoostInterface.SafeDMatrixHandle Handle => _handle;
	private const float Missing = 0f;

        /// <summary>
        /// Create a <see cref="DMatrix"/> for storing training and prediction data under XGBoost framework.
#if true
        public unsafe DMatrix(float[] data, uint nrows, uint ncols, float[]? labels = null)
	{
	  int errp = WrappedXGBoostInterface.XGDMatrixCreateFromMat(data, nrows, ncols, Missing, out _handle);
	  if (errp == -1)
	  {
	      string reason = WrappedXGBoostInterface.XGBGetLastError();
              throw new XGBoostDLLException(reason);
	  }

          if (labels != null) {
	    SetLabel(labels);
	  }

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

	public ulong GetNumRows()
	{
	  ulong numRows;
	  int errp = WrappedXGBoostInterface.XGDMatrixNumRow(_handle, out numRows);
	  if (errp == -1) {
	      string reason = WrappedXGBoostInterface.XGBGetLastError();
              throw new XGBoostDLLException(reason);	  
	  }
	  return numRows;
	}

	public ulong GetNumCols()
	{
	  ulong numCols;
	  int errp = WrappedXGBoostInterface.XGDMatrixNumCol(_handle, out numCols);
	  if (errp == -1) {
	      string reason = WrappedXGBoostInterface.XGBGetLastError();
              throw new XGBoostDLLException(reason);	  
	  }
	  return numCols;
	}

        public unsafe void SetLabel(float[] labels)
        {
	#if false
            Contracts.AssertValue(labels);
            Contracts.Assert(labels.Length == GetNumRows());
	    #endif
            fixed (float* ptr = labels) {
	    	  int errp = WrappedXGBoostInterface.XGDMatrixSetFloatInfo(_handle, "label", (IntPtr)ptr, (ulong)labels.Length);
	  	  if (errp == -1) {
		    string reason = WrappedXGBoostInterface.XGBGetLastError();
		    throw new XGBoostDLLException(reason);	  
		  }
	    }
        }

        public void Dispose()
        {
            _handle?.Dispose();
//            _handle = null;
        }
    }
}
