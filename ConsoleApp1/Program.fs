// Learn more about F# at http://fsharp.org

open System
open System.Xml.Linq
open System.Xml
open System.Xml.Linq
open System.Security.Cryptography
open FSharp.Data
open System.IO


[<EntryPoint>]
let main argv =
    let symbols=["(";")";"{";"}";"[";"]";",";".";";";"+";"-";"*";"/";"=";"<";">";"~";"|";"&"]
    let keywords=["class";"constructor";"function";"method";"field";"static";"var";"int";"char"
                  "boolean";"void";"true";"false";"null";"this";"let";"do";"if";"else";"while";"return"]
    let path = Console.ReadLine()
    let _zz=XElement(XName.Get("token"));
    let path2 = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "T.xml"
    let file=File.ReadLines(path) 
    
    for line in file do
        let temp=""
        for word in line.Split(" ") do
            if List.exists(fun elem->elem=word)keywords then
                _zz.Add(XElement(XName.Get("keyword"),word))
           
    //let xd=XDocument.Load("C:/Users/user/Downloads/check.xml")
    //let xn =XName.Get("note")
    //let props=xd.Element(xn)
    
    _zz.Add(XElement(XName.Get("symbol"),")"))//child element 
    let xd2=XDocument()
    xd2.Add(_zz)
    
    xd2.Save("C:/Users/user/Downloads/check1.xml")
    
    
    
    
    printfn "Hello World from F#!"
    
    0 // return an integer exit code


