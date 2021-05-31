namespace ex4
[<AutoOpen>]
module rec Expressions = 
    open System
    open System.Xml.Linq

    let opList = ["+";"=";"-";"*";"/";"&";"|";"<";">"]

    let expressionList (en:byref<Collections.Generic.IEnumerator<XElement>>): XElement=
        let mutable el = XElement(XName.Get("expressionList"))
        let mutable flag = false

        while en.Current.Value.Replace(" ","") <> ")" && en.Current.Value.Replace(" ","") <> ";"
            && en.Current.Value.Replace(" ","") <> "]" do
                // handling ,
                if flag then
                    el.Add(en.Current)
                    en.MoveNext()|>ignore

                el.Add(expression &en)
                flag <- true
            
        el

    let subroutineCall (prev:XElement) (en:byref<Collections.Generic.IEnumerator<XElement>>): XElement=
        let mutable el = XElement(XName.Get("subroutineCall"))
        el.Add(prev)

        //(expressionList)
        if en.Current.Value.Replace(" ","").Equals("(") then
            el.Add(en.Current)
            en.MoveNext()|>ignore
            el.Add(expressionList &en)
            el.Add(en.Current)
            en.MoveNext()|>ignore
        else
            for i = 1 to 3 do
                el.Add(en.Current)
                en.MoveNext()|>ignore
    
            el.Add(expressionList &en)
        
            el.Add(en.Current)
            en.MoveNext()|>ignore

        el

    let term (en:byref<Collections.Generic.IEnumerator<XElement>>): XElement=
        let mutable el = XElement(XName.Get("term"))
        let mutable prevVal = en.Current
        en.MoveNext()|>ignore
    
        // subroutineCall
        if (en.Current.Value.Replace(" ","").Equals("(") || en.Current.Value.Replace(" ","").Equals("."))
            && not (prevVal.Name.LocalName.Replace(" ","").Equals("symbol")) then
                el.Add(subroutineCall prevVal &en)
        else 
            el.Add(prevVal)
           // unaryOp term
            if prevVal.Value.Replace(" ","").Equals("-") || prevVal.Value.Replace(" ","").Equals("~") then
                el.Add(term &en)
        
            // (expression)
            elif prevVal.Value.Replace(" ","").Equals("(") then            
                el.Add(expression &en)
                el.Add(en.Current)
                en.MoveNext()|>ignore
       
           // varName [expression]
            elif en.Current.Value.Replace(" ","").Equals("[") then
                el.Add(en.Current)
                en.MoveNext()|>ignore
                el.Add(expression &en)
                el.Add(en.Current)
                en.MoveNext()|>ignore
        

        el
    
    let expression (en:byref<Collections.Generic.IEnumerator<XElement>>): XElement=
        let mutable el = XElement(XName.Get("expression"))
        el.Add(term &en)
   
        let mutable temp = en.Current.Value.Replace(" ","")
        while List.exists(fun elem-> elem = temp) opList do
           el.Add(en.Current)
           en.MoveNext()|>ignore
           el.Add(term &en)
           temp <- en.Current.Value.Replace(" ","")
    
        el