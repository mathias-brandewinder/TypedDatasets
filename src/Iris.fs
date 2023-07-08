namespace Datasets

[<RequireQualifiedAccess>]
module Iris =

    open System
    open System.IO
    open System.Net.Http

    // Temporary: replace with online location
    let private cacheDirectory =
        let cacheLocation =
            Environment.SpecialFolder.LocalApplicationData
            |> Environment.GetFolderPath
        Path.Combine(cacheLocation, "TypedDatasets", "iris")
        |> DirectoryInfo

    let private url = "https://raw.githubusercontent.com/mathias-brandewinder/TypedDatasets/main/data/iris/iris.data"

    let download () =
        async {
            use client = new HttpClient()
            let! content =
                url
                |> client.GetStringAsync
                |> Async.AwaitTask
            return content
            }

    let splitIntoLines (input: string): seq<string> =
        input.Split([|"\r\n"; "\r"; "\n"|], StringSplitOptions.None)

    type Observation = {
        /// sepal length in cm
        SepalLength: float<cm>
        SepalWidth: float<cm>
        PetalLength: float<cm>
        PetalWidth: float<cm>
        }

    type Example = Example<Observation,string>

    let read () : seq<Example> =
        let cache =
            if not (Directory.Exists(cacheDirectory.FullName))
            then
                printfn $"Creating cache at {cacheDirectory.FullName}"
                Directory.CreateDirectory(cacheDirectory.FullName)
            else cacheDirectory

        if not (File.Exists(Path.Combine(cache.FullName, "iris.data")))
        then
            printfn $"Dowloading dataset in {cache.FullName}"
            download ()
            |> Async.RunSynchronously
            |> fun data ->
                File.WriteAllText((Path.Combine(cache.FullName,"iris.data")), data)

        Path.Combine(cache.FullName,"iris.data")
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
