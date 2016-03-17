open System.IO
open CommonMark

let getLastDirectory (dir : string) =
    let index = dir.Replace("\\", "/").LastIndexOf("/")
    let length = dir.Length - index
    match index with
    | -1 -> dir
    |_ -> dir.Substring(index, length)

let getFiles directory =
    Directory.GetFiles(directory) |> Seq.toList |> List.map (fun x -> Path.GetFileName(x))

let recurFile directory =
    let copyFiles d head =
        let files = getFiles d
        let dir = getLastDirectory(d)
        printfn "head %s - dir %s" head dir
        printfn "%A" files
        let source = Path.Combine(head, dir)
        let output = Path.Combine(head, dir)
        //files |> List.iter (fun x -> (File.Copy(Path.Combine(source, x), Path.Combine(output, x), true)))
        files |> List.iter (fun x -> printfn "Source %s - Output %s" (Path.Combine(source, x)) (Path.Combine(output, x)))
    let rec loop dir head =
        copyFiles dir head
        let sub = Directory.GetDirectories(dir) |> Seq.toList
        match sub with
        |[] -> ()
        |_ -> sub |> List.iter (fun x ->
                //printfn "head %s - GetLast %s" head (getLastDirectory x);
                loop x (Path.Combine(head, getLastDirectory dir)))
    loop directory ""
    //copyFiles directory ""
    0
[<EntryPoint>]
let main args =
    printfn "hello world"
    //let test = "input/sub1"
    recurFile "input"
    //let p = getLastDirectory test
    //printfn "%s" p

    //let dir = Directory.GetDirectories("input")
    //printfn "%A" dir
    //recurFile "input"
    0
