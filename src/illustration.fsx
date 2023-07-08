#load "Types.fs"
#load "Iris.fs"

open Datasets

let dataset = Iris.dataset
dataset.Source
dataset.Read()