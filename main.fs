open System.IO
open NSoup
open CommonMark

let read path =
    try
        Some(File.ReadAllText(path))
    with
    |_ -> None

let CreateElement tag text c =
    let tag = (NSoup.Nodes.Element(NSoup.Parse.Tag.ValueOf(tag), "test")).Text(text)
    match c with
    |"" -> tag
    |_ -> (tag.AddClass(c))

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


let getNav directory =
    let addtoHtml sub (html : NSoup.Nodes.Element) =
        sub |> List.iter (fun x ->
            printfn "%s" (getLastDirectory x);
            html.AppendChild(CreateElement "li" (getLastDirectory x) "col-md-1") |> ignore)

    let findSub tags sub =
        tags |> List.find (fun (x : NSoup.Nodes.Element) -> x.OwnText() = sub)

    let rec loop head (html : NSoup.Nodes.Element) =
        let dir = Path.Combine("input", head)
        let sub = Directory.GetDirectories(dir) |> Seq.toList
        addtoHtml sub html
        match sub with
        |[] -> ()
        |_ -> sub |> List.iter (fun x -> 
                printfn "next - %s" (Path.Combine(head, getLastDirectory x));
                loop (Path.Combine(head, getLastDirectory x)) (findSub (html.Children |> Seq.toList) (getLastDirectory x)))

    let sub = Directory.GetDirectories("input") |> Seq.toList
    let html = CreateElement "ul" "" "collapse navbar-collapse"
    loop "" html
    (html.Html())

let renderTemplate head body =
    let HEADER_REPLACE = "|HEADER|"
    let BODY_REPLACE = "|BODY|"
    
    let template = match (read "templates/template.html") with
    |None -> "|BODY|"
    |Some(x) -> x
    
    let templateWithHeader = match(template.Contains(HEADER_REPLACE)) with
    |true -> template.Replace(HEADER_REPLACE, head)
    |false -> template
    
    let templateWithBody = match(templateWithHeader.Contains(BODY_REPLACE)) with
    |true -> templateWithHeader.Replace(BODY_REPLACE, body)
    |false -> templateWithHeader

    templateWithBody

let recurFile directory =
    let processFile source destination =
        let file = File.ReadAllText(source)
        let processedFile = renderTemplate (getNav "") file
        File.WriteAllText(destination, processedFile)

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
            |true -> processFile t (Path.Combine(output, (getOutputName t)) + ".html"))
            //|true -> (File.Copy(t, Path.Combine(output, (getOutputName t) + ".html"), true)))


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
[<EntryPoint>]
let main args =
    recurFile "" 
    //File.WriteAllText("test.html", (renderTemplate head body))
    0
