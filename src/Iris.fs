namespace Datasets

[<RequireQualifiedAccess>]
module Iris =

    open System
    open System.IO

    // Temporary: replace with online location
    let private directory =
        Path.Combine(__SOURCE_DIRECTORY__, "../data/iris")
        |> DirectoryInfo

    type Observation = {
        /// sepal length in cm
        SepalLength: float<cm>
        SepalWidth: float<cm>
        PetalLength: float<cm>
        PetalWidth: float<cm>
        }

    type Example = Example<Observation,string>

    let read () : seq<Example> =
        Path.Combine(directory.FullName, "iris.data")
        |> File.ReadAllLines
        |> Seq.filter (fun row -> not (String.IsNullOrWhiteSpace row))
        |> Seq.map (fun row ->
            let block = row.Split ','
            {
                Label = block.[4]
                Observation = {
                    SepalLength = block.[0] |> float |> (*) 1.0<cm>
                    SepalWidth = block.[1] |> float |> (*) 1.0<cm>
                    PetalLength = block.[2] |> float |> (*) 1.0<cm>
                    PetalWidth = block.[3] |> float |> (*) 1.0<cm>
                    }
            }
            )

    let dataset = {
        new Dataset<Example> with
            member this.Read() = read ()
            member this.Source = "https://archive.ics.uci.edu/dataset/53/iris"
            member this.License = "https://creativecommons.org/licenses/by/4.0/"
        }
