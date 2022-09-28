using XGBoostProto;
using xgboost_proto_test_support;

using System;
using System.Linq;
using NotVisualBasic.FileIO;

namespace xgboost_proto_test;

public static class TestUtils
{
    public static string GetDataPath()
    {
	// var dataRootPath = Path.Combine((Environment.GetEnvironmentVariable("USERPROFILE")??""), "Documents", "Projects");
	var dataRootPath = Path.Combine("/data", "projects", "XGBoost.Net", "XGBoostTests", "test_files");
	return dataRootPath;
    }
}
