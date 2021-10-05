open System
open System.Collections.Generic
open System.IO

let badChars = ",./\\\"'?!:;[]{}|()-_\n\t"
let dir = "../Texts/"

let badCharToSpace (c: Char) = if badChars.Contains c then ' ' else c

let removeUnwantedCharacters (line: String) =
    line.ToLower() |> String.map badCharToSpace

let splitInWords (line: String) = line.Split ' '

let bigWords word = String.length word > 2

let bigWordsWithoutBadChars =
    removeUnwantedCharacters
    >> splitInWords
    >> Seq.filter bigWords

let parseFile len name =
    File.ReadLines name
    |> Seq.collect bigWordsWithoutBadChars
    |> Seq.windowed len

let parseFileAsync len name = async { return parseFile len name }

let printMostPopular (author, ngram, times) =
    printfn $"Auteur \"%s{author}\": \"%s{ngram}\" avec %d{times} repetitions"

let readAsync len (file: String) =
    async {
        let reader = new StreamReader(file)
        let! content = reader.ReadToEndAsync() |> Async.AwaitTask

        return
            content
            |> bigWordsWithoutBadChars
            |> Seq.windowed len
    }

let parseAuthor length (dir: String) =
    async {
        let len (_, len) = len
        let name = dir.Split "/" |> Array.last

        let! seq =
            dir
            |> Directory.EnumerateFiles
            |> Seq.map (readAsync length)
            |> Async.Parallel

        let ngram, value =
            seq
            |> Seq.concat
            |> Seq.countBy (String.concat " ")
            |> Seq.maxBy len

        return (name, ngram, value)
    }

let getLenOrDefault =
    Array.tryHead
    >> Option.map int
    >> Option.defaultValue 3

[<EntryPoint>]
let main args =
    let now = DateTime.Now

    let len = getLenOrDefault args

    Directory.EnumerateDirectories dir
    |> Seq.map (parseAuthor len)
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Seq.iter printMostPopular

    printfn $"Done executing. Took %f{(DateTime.Now - now).TotalMilliseconds} ms"
    0 // return an integer exit code
