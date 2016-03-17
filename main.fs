open System.IO
open CommonMark

let getLastDirectory (dir : string) =
    let index = dir.Replace("\\", "/").LastIndexOf("/")
    let length = dir.Length - index
    match index with
    | -1 -> dir
    |_ -> dir.Replace("\\", "/").Substring(index, length).Replace("/", "")

let getFiles directory =
    Directory.GetFiles(directory) |> Seq.toList |> List.map (fun x -> Path.GetFileName(x))

let fullPath directory =
    Path.GetFullPath(directory)

let recurFile directory =
    let copyFiles head =
        let source = Path.Combine("input", head)
        let output = Path.Combine("output", head)
        printfn "head %s - dir %s" head source
        let files = getFiles source
        printfn "%A" files
        files |> List.iter (fun x -> 
            Directory.CreateDirectory(output);
            (File.Copy(Path.Combine(source, x), Path.Combine(output, x), true)))
        files |> List.iter (fun x -> printfn "Source %s - Output %s" (Path.Combine(source, x)) (Path.Combine(output, x)))

    let rec loop head =
        let dir = Path.Combine("input", head)
        copyFiles head
        let sub = Directory.GetDirectories(dir) |> Seq.toList
        match sub with
        |[] -> ()
        |_ -> sub |> List.iter (fun x ->
                //printfn "head %s - GetLast %s" head (getLastDirectory x);
                loop (Path.Combine(head, getLastDirectory x)))
    loop ""
    //copyFiles directory ""
    0
[<EntryPoint>]
let main args =
    printfn "hello world"
    //let test = "input/sub1/"
    recurFile "input"
    //let p = getLastDirectory test
    //printfn "%s" p

    //let dir = Directory.GetDirectories("input")
    //printfn "%A" dir
    //recurFile "input"
    0
