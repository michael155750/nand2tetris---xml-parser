[<AutoOpen>]
module parser

open System
open System.Xml.Linq

open System.IO

let private subroutineDec (rootEl:XElement) (en:Collections.Generic.IEnumerator<XElement>) = null

let private classVarDec (rootEl:XElement) (en:Collections.Generic.IEnumerator<XElement>) = null

let private classParse (rootEl:XElement) (en:Collections.Generic.IEnumerator<XElement>) =
    
    let mutable classVarDecF = false
    let mutable subroutineDecF = false
    let mutable i = 0
    let mutable classEl = XElement(XName.Get("class"))
    ///////////////
    
    while en.MoveNext() do
        if i < 3 then
            classEl.Add(en.Current)
        elif i = 3 then
            while (en.Current.Value = "static") || (en.Current.Value = "field") do
                classEl.Add(classVarDec rootEl en)
            while (en.Current.Value = "constractor") || (en.Current.Value = "static") 
            || (en.Current.Value = "method") do
                classEl.Add(subroutineDec rootEl en)
        i <- i + 1
    ////////////////
    for el in rootEl.Elements() do
        if i < 3 then
            classEl.Add(el)
        elif i = 3 then
            if el.Value = "static" || el.Value = "field" then
        i <- i + 1

let parserMain path = 
    let filesList = Directory.GetFiles(path,"*T.xml")
    for f in filesList do 
        let name = Path.GetFileNameWithoutExtension(f)
        let path2 = path + "\\" + name.[0..name.Length - 2] + ".xml"
        if File.Exists(path2) then
            File.Delete(path2)
        let mutable el = XElement.Load(f)
        el.Name <- XName.Get("class")
        /////////////////
        let en =  el.Elements().GetEnumerator()
        //////////////////
        classParse el en