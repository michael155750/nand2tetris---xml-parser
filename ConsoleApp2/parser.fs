[<AutoOpen>]
module parser

open System
open System.Xml.Linq

open System.IO

let private subroutineDec (en:byref<Collections.Generic.IEnumerator<XElement>>)= null

let private classVarDec (en:byref<Collections.Generic.IEnumerator<XElement>>)= null

let private classParse (rootEl:XElement) (en:byref<Collections.Generic.IEnumerator<XElement>>) =
    
    
    let mutable i = 0
    let mutable classEl = XElement(XName.Get("class"))
   
    
    while en.MoveNext() do
        if i < 3 then
            classEl.Add(en.Current)
        elif i >= 3 then
            while (en.Current.Value = "static") || (en.Current.Value = "field") do
                classEl.Add(classVarDec &en)
            while (en.Current.Value = "constructor") || (en.Current.Value = "function") || (en.Current.Value = "method") do
                classEl.Add(subroutineDec &en)
            else
                classEl.Add(en.Current)
        i <- i + 1
    
    

let parserMain path = 
    let filesList = Directory.GetFiles(path,"*T.xml")
    for f in filesList do 
        let name = Path.GetFileNameWithoutExtension(f)
        let path2 = path + "\\" + name.[0..name.Length - 2] + ".xml"
        if File.Exists(path2) then
            File.Delete(path2)
        let mutable el = XElement.Load(f)
        //el.Name <- XName.Get("class")
        
        let mutable en =  el.Elements().GetEnumerator()
        
        classParse el &en