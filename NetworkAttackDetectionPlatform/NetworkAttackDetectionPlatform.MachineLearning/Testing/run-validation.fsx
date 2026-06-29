#!/usr/bin/env dotnet fsi

#r "nuget: Microsoft.ML, 3.0.0"
#r "../bin/Debug/net8.0/NetworkAttackDetectionPlatform.MachineLearning.dll"

open System
open System.IO
open System.Threading.Tasks
open NetworkAttackDetectionPlatform.MachineLearning.Testing

printfn "Starting CICIDS2017 ML Pipeline Validation..."
printfn ""

let result = ValidationRunner.RunValidationAsync().GetAwaiter().GetResult()

if result.Success then
    Environment.Exit(0)
else
    Environment.Exit(1)
