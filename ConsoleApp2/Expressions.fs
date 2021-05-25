[<AutoOpen>]
module rec Expressions
open System
open System.Xml.Linq

let opList = ["+";"=";"-";"*";"/";"&";"|";"<";">"]

let expression (en:byref<Collections.Generic.IEnumerator<XElement>>): XElement=
    let mutable el = XElement(XName.Get("expression"))
   // while not (List.exists(fun elem->elem=en.Current.Value)  opList) do
       // el.Add(statement &en)
    el