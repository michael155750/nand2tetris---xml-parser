[<AutoOpen>]
module rec Statements

open System
open System.Xml.Linq

open System.IO

let statementsWords = ["let";"if";"while";"do";"return"]

let letStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)=
    let mutable el = XElement(XName.Get("letStatement"))
    let mutable i = 0
    while  en.Current.Value <> ";" do
        if i < 3 || i = 4 || i = 5 then
            el.Add(en.Current)
            en.MoveNext()|>ignore 
        elif i = 3 || i = 6 then
             el.Add(expression &en)
        
    el.Add(en.Current)
    en.MoveNext()|>ignore 
    el

let ifStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)=
    let mutable el = XElement(XName.Get("ifStatement"))
    let mutable i = 0
    while en.Current.Value <> ";" do
        if i < 2 || i = 3 || i = 4 || i = 6 || i = 7 || i = 8 || i = 10 then
            el.Add(en.Current)
            en.MoveNext()|>ignore
        elif i = 2  then
             el.Add(expression &en)
        elif i = 5 || i = 9 then
             el.Add(statements &en)
        
    el.Add(en.Current)
    en.MoveNext()|>ignore
    el

let whileStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)=
    let mutable el = XElement(XName.Get("whileStatement"))
    let mutable i = 0
    while en.Current.Value <> ";" do
        if i < 2 || i = 3 || i = 4 || i = 6 then
            el.Add(en.Current)
            en.MoveNext()|>ignore
        elif i = 2  then
             el.Add(expression &en)
        elif i = 5  then
             el.Add(statements &en)
        
    el.Add(en.Current)
    en.MoveNext()|>ignore
    el

let doStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)=
    let mutable el = XElement(XName.Get("doStatement"))
    el.Add(en.Current)
    en.MoveNext()|>ignore
    //el.Add(subroutineCall &en)
    en.MoveNext()|>ignore
    el.Add(en.Current)
    en.MoveNext()|>ignore
    el

let returnStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)=
    let mutable el = XElement(XName.Get("returnStatement"))
    el.Add(en.Current)
    en.MoveNext()|>ignore
    if en.Current.Value <> ";" then
        el.Add(expression &en)
    el.Add(en.Current)
    en.MoveNext()|>ignore
    el

let statement (en:byref<Collections.Generic.IEnumerator<XElement>>) =
    if en.Current.Value.Contains("let") then
        letStatement &en
    elif en.Current.Value.Contains("if") then
        ifStatement &en
    elif en.Current.Value.Contains("while") then
        whileStatement &en
    elif en.Current.Value.Contains("do") then
        doStatement &en
    else 
        returnStatement &en
    
    
let statements (en:byref<Collections.Generic.IEnumerator<XElement>>): XElement=
    let mutable el = XElement(XName.Get("statements"))
   // while List.exists(fun elem->elem=en.Current.Value) statementsWords do
    //    el.Add(statement &en)
    el