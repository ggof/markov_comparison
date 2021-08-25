// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp
open System
open System.IO
open Argu

let badChars = ",./\\\"'?!:;[]{}|()-_\n\t"
let dir = "../Texts/"

let badCharToSpace (c: Char) = if badChars.Contains c then ' ' else c

let removeUnwantedCharacters (line: String) =
    line.ToLower() |> String.map badCharToSpace

let splitInWords (line: String) = line.Split ' '

let smallWords word = (String.length word) > 2

let removeSmallWords = Seq.filter smallWords

let bigWordsWithoutBadChars =
    removeUnwantedCharacters
    >> splitInWords
    >> removeSmallWords


let parseFile len name = async {
    return File.ReadLines name
    |> Seq.collect bigWordsWithoutBadChars
    |> Seq.windowed len
}

let printMostPopular author (ngram, times) =
    printfn $"Auteur \"%s{author}\": \"%s{ngram}\" avec %d{times} repetitions"

let parseAuthor length (dir: String) =
    let len (_, len) = len

    async {
        let name = dir.Split "/" |> Array.last

        let! ngrams = 
            dir
            |> Directory.EnumerateFiles
            |> Seq.map (parseFile length)
            |> Async.Parallel

        ngrams
        |> Seq.concat
        |> Seq.countBy (String.concat " ")
        |> Seq.maxBy len
        |> printMostPopular name
    }


type CliArgs =
    | Author of name: string
    | Length of len: int32

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Author _ -> "Parse only author <name>. If ommited, runs all authors in Texts/ directory"
            | Length _ -> "How long should the ngram be"


let getParams args = 
    let parser = ArgumentParser.Create<CliArgs>()
    let results = parser.Parse args

    let maybeAuthor = results.TryGetResult Author
    let len = results.TryGetResult Length

    let dirs =
        List.ofSeq (Directory.EnumerateDirectories dir)

    let author = 
        match maybeAuthor with
        | Some author -> [ dir + author ]
        | None -> dirs

    let length = 
        match len with
        | Some l -> l
        | None -> 3

    (author, length)


[<EntryPoint>]
let main args =
    let now = DateTime.Now

    let (authors, len) = getParams args

    authors
    |> Seq.map (parseAuthor len)
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore

    printfn $"Done executing. Took %f{(DateTime.Now - now).TotalMilliseconds} ms"
    0 // return an integer exit code
