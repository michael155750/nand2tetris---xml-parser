[<AutoOpen>]
module parser

open System
open System.Xml.Linq

open System.IO

let classParse (rootEl:XElement) =
    let mutable classVarDecF = false
    let mutable subroutineDecF = false
    let mutable i = 0
    let mutable classEl = XElement(XName.Get("class"))
    for el in rootEl.Elements() do
        if i < 3 then
            classEl.Add(el)
        elif i = 3 then
            if el.Value = "static" || el.Value = "field" then
        i <- i + 1