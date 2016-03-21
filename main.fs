open System.IO
open NSoup
open CommonMark
 
let CreateElement tag text=
    (NSoup.Nodes.Element(NSoup.Parse.Tag.ValueOf(tag), "test")).Text(text)

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

let getIndex dir =
    let candidates = Directory.GetFiles(dir, "index.*") |> Seq.toList
    match candidates with
    |[] -> None
    |_ -> Some(candidates.Head)



let recurFile directory =
    let copyFiles head oldHead =
        let getOutputName dir =
            Directory.GetParent(dir).FullName |> getLastDirectory

        let source = Path.Combine("input", head)
        let output = Path.Combine("output", oldHead)
        let files = getFiles source
        files |> List.iter (fun x ->
            Directory.CreateDirectory(output);
            let t = Path.Combine(source, x);
            match (Path.GetFileName(x) = "index.html") with
            |false -> ()
            |true -> (File.Copy(t, Path.Combine(output, (getOutputName t) + ".html"), true)))


        (*
        files |> List.iter (fun x -> 
            Directory.CreateDirectory(output);
            (File.Copy(Path.Combine(source, x), Path.Combine(output, x), true)))
*)
        files |> List.iter (fun x -> printfn "Source %s - Output %s" (Path.Combine(source, x)) (Path.Combine(output, x)))

    let rec loop head oldHead =
        let dir = Path.Combine("input", head)
        copyFiles head oldHead
        let sub = Directory.GetDirectories(dir) |> Seq.toList
        match sub with
        |[] -> ()
        |_ -> sub |> List.iter (fun x ->
                loop (Path.Combine(head, getLastDirectory x)) head)
    loop "" ""
    0


        //html is the parent tag which children is being added to
let getNav directory =
    let addtoHtml sub (html : NSoup.Nodes.Element) =
        sub |> List.iter (fun x ->
            printfn "%s" (getLastDirectory x);
            html.AppendChild(CreateElement "p" (getLastDirectory x)) |> ignore)
    let findSub tags sub =
        tags |> List.find (fun (x : NSoup.Nodes.Element) -> x.OwnText() = sub)
    let rec loop head (html : NSoup.Nodes.Element) =
        let dir = Path.Combine("input", head)
        let sub = Directory.GetDirectories(dir) |> Seq.toList
        addtoHtml sub html
        //run some function here
        match sub with
        |[] -> ()
        |_ -> sub |> List.iter (fun x -> 
                printfn "next - %s" (Path.Combine(head, getLastDirectory x));
                loop (Path.Combine(head, getLastDirectory x)) (findSub (html.Children |> Seq.toList) (getLastDirectory x)))
    let sub = Directory.GetDirectories("input") |> Seq.toList
    let html = CreateElement "nav" "top"
    //addtoHtml sub html
    loop "" html
    printfn "%s" (html.Html())
    0

[<EntryPoint>]
let main args =
    //let folders = Directory.GetDirectories("input") |> Seq.toList
    //let path = "index/fruit/index.html"
    //let result = (Directory.GetParent(path)).FullName |> getLastDirectory
    //printfn "%s" result
    getNav "fucK"
    //printfn "hello world"
    //let t = CreateElement "li"
    //printfn "%s" (t.ToString())
    //let test = "input/sub1/"
    //recurFile "input"
    0
