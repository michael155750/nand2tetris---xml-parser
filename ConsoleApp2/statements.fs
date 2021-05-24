[<AutoOpen>]
module rec Statements

open System
open System.Xml.Linq

open System.IO

let statementsWords = ["let";"if";"while";"do";"return"]

let letStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)=
    let mutable el = XElement(XName.Get("letStatement"))
    let mutable i = 0
    while en.MoveNext() && en.Current.Value <> ";" do
        if i < 3 || i = 4 || i = 5 then
            el.Add(en.Current)
        elif i = 3 || i = 6 then
             el.Add(expression &en)
    el.Add(en.Current)

let ifStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)=
    let mutable el = XElement(XName.Get("ifStatement"))
    let mutable i = 0
    while en.MoveNext() && en.Current.Value <> ";" do
        if i < 2 || i = 3 || i = 4 || i = 6 || i = 7 || i = 8 || i = 10 then
            el.Add(en.Current)
        elif i = 2  then
             el.Add(expression &en)
        elif i = 5 || i = 9 then
             el.Add(statements &en)

let whileStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)=

let doStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)=

let returnStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)=

let statement (en:byref<Collections.Generic.IEnumerator<XElement>>)=
    if en.Current.Value = "let" then
        letStatement &en
    elif en.Current.Value = "if" then
        ifStatement &en
    elif en.Current.Value = "while" then
        whileStatement &en
    elif en.Current.Value = "do" then
        doStatement &en
    elif en.Current.Value = "return" then
        returnStatement &en

let statements (en:byref<Collections.Generic.IEnumerator<XElement>>)=
    let mutable el = XElement(XName.Get("statements"))
    while List.exists(fun elem->elem=en.Current.Value) statementsWords do
        el.Add(statement &en)