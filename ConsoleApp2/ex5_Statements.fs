namespace ex5
[<AutoOpen>]
module rec Statements = 

    open System
    open System.Xml.Linq
    open System.IO

    

    

    let statementsWords = ["let";"if";"while";"do";"return"]

    

            
       

    let letStatement (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string)=
        let mutable el = XElement(XName.Get("letStatement"))
        let mutable isArray = false

        el.Add(en.Current)
        en.MoveNext()|>ignore 
        let varName = en.Current.Value.Replace(" ","")
        el.Add(en.Current)
        en.MoveNext()|>ignore
        if en.Current.Value.Replace(" ","").Equals("[") then
            el.Add(en.Current)
            en.MoveNext()|>ignore
            f.Write("push ")
            varInStackVM (varName) f className
            el.Add(expression &en f className)
            f.WriteLine("add")
            el.Add(en.Current)
            en.MoveNext()|>ignore
            isArray <- true
        el.Add(en.Current)
        en.MoveNext()|>ignore 
        el.Add(expression &en f className)

        el.Add(en.Current)
        en.MoveNext()|>ignore 

        if not isArray then
            f.Write("pop ")
            varInStackVM (varName) f className
        else
            f.WriteLine("pop temp 0")
            f.WriteLine("pop pointer 1")
            f.WriteLine("push temp 0")
            f.WriteLine("pop that 0")
        el
    
    let ifStatement (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string)=
        let mutable el = XElement(XName.Get("ifStatement"))
        let localIndex = ifIndex.ToString()
        ifIndex <- ifIndex + 1
        for i=1 to 2 do
            el.Add(en.Current)
            en.MoveNext()|>ignore
    
        el.Add(expression &en f className)
        f.WriteLine("if-goto IF_TRUE"+ localIndex)
        f.WriteLine("goto IF_FALSE"+ localIndex)
        f.WriteLine("label IF_TRUE"+ localIndex)
        for i=1 to 2 do
            el.Add(en.Current)
            en.MoveNext()|>ignore
        el.Add(statements &en f className)
        el.Add(en.Current)
        en.MoveNext()|>ignore
        f.WriteLine("goto IF_END"+ localIndex)
        f.WriteLine("label IF_FALSE"+ localIndex)

        if en.Current.Value.Replace(" ","").Equals("else") then
            for i=1 to 2 do
                el.Add(en.Current)
                en.MoveNext()|>ignore
            el.Add(statements &en f className)
            el.Add(en.Current)
            en.MoveNext()|>ignore
        f.WriteLine("label IF_END"+ localIndex)
        
        el

    let whileStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)(f:StreamWriter) (className:string)=
        let mutable el = XElement(XName.Get("whileStatement"))
        let localIndex = whileIndex.ToString()
        whileIndex <-whileIndex + 1
        el.Add(en.Current)
        en.MoveNext()|>ignore
  
        el.Add(en.Current)
        en.MoveNext()|>ignore
        f.WriteLine("label WHILE_EXP"+ localIndex)
        
        el.Add(expression &en f className)
        el.Add(en.Current)
        en.MoveNext()|>ignore
        el.Add(en.Current)
        en.MoveNext()|>ignore
        f.WriteLine("not")
        f.WriteLine("if-goto WHILE_END"+ localIndex)
        el.Add(statements &en f className)
        f.WriteLine("goto WHILE_EXP"+ localIndex)
        f.WriteLine("label WHILE_END"+ localIndex)
        
        el.Add(en.Current)
        en.MoveNext()|>ignore
        
        
        el

    let doStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)(f:StreamWriter) (className:string)=
        let mutable el = XElement(XName.Get("doStatement"))
        el.Add(en.Current)
        en.MoveNext()|>ignore
        let prev = en.Current
        en.MoveNext()|>ignore
        el.Add(subroutineCall prev &en f className)
        f.WriteLine("pop temp 0")
        el.Add(en.Current)
        en.MoveNext()|>ignore
        el

    let returnStatement (en:byref<Collections.Generic.IEnumerator<XElement>>)(f:StreamWriter) (className:string)=
        let mutable el = XElement(XName.Get("returnStatement"))
        el.Add(en.Current)
        en.MoveNext()|>ignore
        if en.Current.Value.Replace(" ","") <> ";" then
            el.Add(expression &en f className)
        else
            f.WriteLine("push constant 0")
        el.Add(en.Current)
        en.MoveNext()|>ignore
        f.WriteLine("return")
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

    let translateVarToVm varName className:string=
           let mutable index = ""
           let mutable varType = ""
           if  Expressions.methodTable.varCount(varName) > 0 then
               index <- Expressions.methodTable.indexOf(varName).ToString()
               if Expressions.methodTable.typeOf(varName)="var" then
                    varType <- "local"
               else 
                    varType <- "argument"
                    //varType + " " + index.ToString()//return value
                  
           else
               index <- symbolClassDef.classTables.[className].indexOf(varName).ToString()
               if (symbolClassDef.classTables.[className]).typeOf(varName)="field" then
                   varType <- "this"
               else 
                   varType <- "static"
           varType+" "+index//return value
           