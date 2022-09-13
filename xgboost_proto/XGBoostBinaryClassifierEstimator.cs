using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if false
namespace Microsoft.ML.Trainers.XGBoost
{

    public sealed class XGBoostBinaryClassificationTransformer : OneToOneTransformerBase
    {
        internal XGBoostBinaryClassificationTransformer(IHost host, params (string outputColumnName, string inputColumnName)[] columns) : base(host, columns)
        {
        }

        internal XGBoostBinaryClassificationTransformer(IHost host, ModelLoadContext ctx) : base(host, ctx)
        {
        }

        private protected override IRowMapper MakeRowMapper(DataViewSchema schema) => new Mapper(this, schema);

        private protected override void SaveModel(ModelSaveContext ctx)
        {
            Host.CheckValue(ctx, nameof(ctx));
        }

        private sealed class Mapper : OneToOneMapperBase, ISaveAsOnnx
        {
            private readonly XGBoostBinaryClassificationTransformer _parent;
            public Mapper(XGBoostBinaryClassificationTransformer parent, DataViewSchema inputSchema)
                : base(parent.Host.Register(nameof(Mapper)), parent, inputSchema)
            {
                _parent = parent;

            }

            public bool CanSaveOnnx(OnnxContext ctx) => true;

            public void SaveAsOnnx(OnnxContext ctx)
            {
                throw new NotImplementedException();
            }

            protected override DataViewSchema.DetachedColumn[] GetOutputColumnsCore()
            {
                throw new NotImplementedException();
            }

            protected override Delegate MakeGetter(DataViewRow input, int iinfo, Func<int, bool> activeOutput, out Action disposer)
            {
                throw new NotImplementedException();
            }
        }
    }

#if false
    public sealed class XGBoostBinaryClassificationEstimator : TrivialEstimator<XGBoostBinaryClassificationTransformer>
#else
    public sealed class XGBoostBinaryClassificationEstimator : IEstimator<XGBoostBinaryClassificationTransformer>
#endif
    {
        private readonly IHost _host;


       public sealed class Options : TrainerInputBase
       {
            /// <summary>
            /// Maximum tree depth for base learners
            /// </summary>
            [Argument(ArgumentType.AtMostOnce, HelpText = "Maximum tree depth for base learners.", ShortName = "us")]
            public int MaxDepth = 3;
       }

        public XGBoostBinaryClassificationEstimator(IHost host, XGBoostBinaryClassificationTransformer transformer) /*: base(host, transformer)*/
        {
            _host = Contracts.CheckRef(host, nameof(host)).Register(nameof(XGBoostBinaryClassificationEstimator));
        }

        public XGBoostBinaryClassificationTransformer Fit(IDataView input)
        {
	#if false
            throw new NotImplementedException();
	    #else
	    	var featuresColumn = input.Schema["Features"];
		var labelColumn = input.Schema["Label"];
		int featureDimensionality = default(int);
		if (featuresColumn.Type is VectorDataViewType vt) {
		  featureDimensionality = vt.Size;
		} else {
		  _host.Except($"A vector input is expected");
		}
		int samples = 0;
		int maxSamples = 10000;

		float[] data = new float[ maxSamples * featureDimensionality];
		float[] dataLabels = new float[ maxSamples ];
		Span<float> dataSpan = new Span<float>(data);

		using (var cursor = input.GetRowCursor(new[] { featuresColumn, labelColumn })) {

		  float labelValue = default;
		  VBuffer<float> featureValues = default(VBuffer<float>);

		  var featureGetter = cursor.GetGetter< VBuffer<float> >(featuresColumn);
		  var labelGetter = cursor.GetGetter<float>(labelColumn);

		  while (cursor.MoveNext() && samples < maxSamples) {
		    featureGetter(ref featureValues);
		    labelGetter(ref labelValue);
		    // DisplaySpan<float>($"features at row {samples}: ", featureValues.GetValues());

		    int offset = samples * featureDimensionality;
		    // Span<float> target = new Span<float>(dataSpan, offset, featureDimensionality);
		    Span<float> target = dataSpan.Slice(offset, featureDimensionality);
		    featureValues.GetValues().CopyTo(target);
		    dataLabels[samples] = labelValue;
		    	    	    #if false  	  
		    samples++;
		    #endif
	    	  }
	  	}

		return new XGBoostBinaryClassificationTransformer(_host, ("Features", "PredictedLabel"));
		#endif
        }

#if true
        public SchemaShape GetOutputSchema(SchemaShape inputSchema)
        {
            throw new NotImplementedException();
        }
#else
        // Used for schema propagation and verification in a pipeline (i.e., in an Estimator chain).
        public override SchemaShape GetOutputSchema(SchemaShape inputSchema)
        {
            _host.CheckValue(inputSchema, nameof(inputSchema));
            return new SchemaShape(inputSchema);
        }
#endif
    }
}
#endif