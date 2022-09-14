# To run tests

The whole suite:
```sh
dotnet test
```

A selected test (in this case, `TestVersion`

```sh
dotnet test --filter "FullyQualifiedName=xgboost_proto_test.SanityTest.TestVersion"
```