#ifndef _FUNCTION_H_
#define _FUNCTION_H_

#ifdef __cplusplus
#define ONEDAL_EXTERN_C extern "C"
#else
#define ONEDAL_EXTERN_C 
#endif

#ifdef _MSC_VER
#define ONEDAL_EXPORT __declspec(dllexport)
#define ONEDAL_C_EXPORT ONEDAL_EXTERN_C __declspec(dllexport)
#else
#define ONEDAL_EXPORT 
#define ONEDAL_C_EXPORT ONEDAL_EXTERN_C
#endif

ONEDAL_C_EXPORT void createTable(float* const data, size_t numFeatures, size_t numObservations);

#endif