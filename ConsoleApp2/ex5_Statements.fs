namespace ex5
[<AutoOpen>]
module rec Statements = 

    open System
    open System.Xml.Linq
    open System.IO

    

    

    let statementsWords = ["let";"if";"while";"do";"return"]

    

            
       

    let letStatement (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string)=
        let mutable el = XElement(XName.Get("letStatement"))
        let mutable i = 0
        while  en.Current.Value.Replace(" ","") <> ";" do
            if i < 3 || i = 4 || i = 5 then
                el.Add(en.Current)
                en.MoveNext()|>ignore 
            elif i = 3 || i = 6 then
                 el.Add(expression &en f className)
            i <- i + 1
        
        el.Add(en.Current)
        en.MoveNext()|>ignore 
        el

    let ifStatement (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string)=
        let mutable el = XElement(XName.Get("ifStatement"))
    
    
        for i=1 to 2 do
            el.Add(en.Current)
            en.MoveNext()|>ignore
    
        el.Add(expression &en f className)
        f.WriteLine("if-goto IF_TRUE"+ ifIndex.ToString())
        f.WriteLine("goto IF_FALSE"+ ifIndex.ToString())
        f.WriteLine("label IF_TRUE"+ ifIndex.ToString())
        for i=1 to 2 do
            el.Add(en.Current)
            en.MoveNext()|>ignore
        el.Add(statements &en f className)
        el.Add(en.Current)
        en.MoveNext()|>ignore
        f.WriteLine("goto IF_END"+ ifIndex.ToString())
        f.WriteLine("label IF_FALSE"+ ifIndex.ToString())

        if en.Current.Value.Replace(" ","").Equals("else") then
            for i=1 to 2 do
                el.Add(en.Current)
                en.MoveNext()|>ignore
            el.Add(statements &en f className)
            el.Add(en.Current)
            en.MoveNext()|>ignore
        f.WriteLine("label IF_END"+ ifIndex.ToString())
        ifIndex <- ifIndex + 1
        el

    let whileStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)(f:StreamWriter) (className:string)=
        let mutable el = XElement(XName.Get("whileStatement"))
        
        el.Add(en.Current)
        en.MoveNext()|>ignore
  
        el.Add(en.Current)
        en.MoveNext()|>ignore
        f.WriteLine("WHILE_EXP"+ whileIndex.ToString())
        
        el.Add(expression &en f className)
        el.Add(en.Current)
        en.MoveNext()|>ignore
        el.Add(en.Current)
        en.MoveNext()|>ignore
        f.WriteLine("not")
        f.WriteLine("if-goto WHILE_END"+ whileIndex.ToString())
        el.Add(statements &en f className)
        f.WriteLine("goto WHILE_EXP"+ whileIndex.ToString())
        f.WriteLine("label WHILE_END"+ whileIndex.ToString())
        
        el.Add(en.Current)
        en.MoveNext()|>ignore
        
        whileIndex <-whileIndex + 1
        el

    let doStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)(f:StreamWriter) (className:string)=
        let mutable el = XElement(XName.Get("doStatement"))
        el.Add(en.Current)
        en.MoveNext()|>ignore
        let prev = en.Current
        en.MoveNext()|>ignore
        el.Add(subroutineCall prev &en f className)
        el.Add(en.Current)
        en.MoveNext()|>ignore
        el

    let returnStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)(f:StreamWriter) (className:string)=
        let mutable el = XElement(XName.Get("returnStatement"))
        el.Add(en.Current)
        en.MoveNext()|>ignore
        if en.Current.Value.Replace(" ","") <> ";" then
            el.Add(expression &en f className)
        el.Add(en.Current)
        en.MoveNext()|>ignore
        el

    let statement (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string)=
        if en.Current.Value.Replace(" ","").Equals("let") then
            letStatement &en f className
        elif en.Current.Value.Replace(" ","").Equals("if") then
            ifStatement &en f className
        elif en.Current.Value.Replace(" ","").Equals("while") then
            whileStatement &en f className
        elif en.Current.Value.Replace(" ","").Equals("do") then
            doStatement &en f className
        else 
            returnStatement &en f className
    
    
    let statements (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string): XElement=
       let mutable el = XElement(XName.Get("statements"))

       let mutable temp = en.Current.Value.Replace(" ","")
       while List.exists(fun elem->elem=temp) statementsWords do
            el.Add(statement &en f className)
            temp <- en.Current.Value.Replace(" ","")
        
       el

    let translateVarToVm varName className=
           let mutable index=""
           let mutable varType=""
           if not (methodTable.indexOf(varName)=(-1)) then
               index<-string:methodTable.indexOf(varName)
               if methodTable.typeOf(varName)="var" then varType="local"
                   else varType="argument"
                   varType+" "+index//return value
               else
                   index<-string:(classTables.[className]).indexof(varName)
                   if (classTables.[className]).typeOf(varName)="field" then varType="this"
                   else varType="static"
            varType+" "+index//return value
           