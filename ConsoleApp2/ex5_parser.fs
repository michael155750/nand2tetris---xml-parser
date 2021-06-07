﻿namespace ex5
[<AutoOpen>]
module parser = 

    open System
    open System.Xml.Linq
    open System.Collections.Generic
    open System.IO

    //TODO: change to list
    let mutable classTables:Map<String,symbolTable> =Map.empty
    
    let private varDec (en:byref<Collections.Generic.IEnumerator<XElement>>)=
        let varDecEl=new XElement(XName.Get("varDec"))
        while not(en.Current.Value.Replace(" ","").Equals(";")) do
            varDecEl.Add(en.Current)
            en.MoveNext()|>ignore
        varDecEl.Add(en.Current)
        en.MoveNext()|>ignore
        varDecEl

    let private subroutineBody (en:byref<Collections.Generic.IEnumerator<XElement>>)=
        let subroutineBodyEl=new XElement(XName.Get("subroutineBody"))
        subroutineBodyEl.Add(en.Current) //add {
        en.MoveNext()|>ignore
        while (en.Current.Value.Replace(" ","").Equals("var"))  do
            subroutineBodyEl.Add(varDec &en)
        subroutineBodyEl.Add(statements &en)
        subroutineBodyEl.Add(en.Current) //add }
        en.MoveNext()|>ignore
        subroutineBodyEl
     
    let private parameterList (en:byref<Collections.Generic.IEnumerator<XElement>> )=
        let parameterListEl=new XElement(XName.Get("parameterList"))
        
        en.MoveNext()|>ignore
        while not (en.Current.Value.Replace(" ","").Equals(")")) do
            parameterListEl.Add(en.Current)//add type
            en.MoveNext()|>ignore
            parameterListEl.Add(en.Current)//add varName
            en.MoveNext()|>ignore
        parameterListEl

    let private subroutineDec (en:byref<Collections.Generic.IEnumerator<XElement>>)=
    
        let mutable subEl = XElement(XName.Get("subroutineDec"))
        classTables.["methodTable"].startSubroutine|>ignore //clear the method table
        subEl.Add(en.Current)//add the word function/method /constructor
        en.MoveNext()|>ignore
        subEl.Add(en.Current)//add void/type
        en.MoveNext()|>ignore
        subEl.Add(en.Current)//add name of function
        en.MoveNext()|>ignore
        subEl.Add(en.Current)//add '('
        en.MoveNext()|>ignore
        subEl.Add(parameterList &en)//add parameterList
        subEl.Add(en.Current)//add ')'
        en.MoveNext()|>ignore
        subEl.Add(subroutineBody &en)
        subEl

    let private classVarDec (en:byref<Collections.Generic.IEnumerator<XElement>>) className= 
        let mutable varEl=new XElement(XName.Get("classVarDec"))
        varEl.Add(en.Current) //add the sort of the fields 'static' or 'field'
        let varKind=en.Current.Value
        en.MoveNext()|>ignore
        varEl.Add(en.Current) //add the type of the fields 'int' or 'String' etc.
        let varType=en.Current.Value
        en.MoveNext()|>ignore
        let mutable isVariable=true
        while not(en.Current.Value.Replace(" ","").Equals(";")) do
            varEl.Add(en.Current) //add the name of variable or ','
            isVariable<-not isVariable
            if isVariable then
                (classTables.[className]).define en.Current.Value varType varKind 
            en.MoveNext()|>ignore
        varEl.Add(en.Current)//add ';'
        en.MoveNext()|>ignore
        varEl


    let private classParse (rootEl:XElement) (en:byref<Collections.Generic.IEnumerator<XElement>>) =
    
    
        let mutable i = 0
        let mutable classEl = XElement(XName.Get("class"))
   
    
        while en.MoveNext() do
            let mutable className=""
            if i = 0  then  // add the word 'class'
                classEl.Add(en.Current)
            elif i = 1 then //add the name of the class
                className<-(en.Current.Value) 
                
                classTables<-classTables.Add(className,new symbolTable(en.Current.Value)) //create the symbol table
                classEl.Add(en.Current) 
            elif i = 2 then
                classEl.Add(en.Current)
                
            elif i >= 3 then
                while (en.Current.Value.Replace(" ","").Equals("static")) || (en.Current.Value.Replace(" ","").Equals("field")) do
                    classEl.Add(classVarDec &en className)
                while en.Current.Value.Replace(" ","").Equals("constructor") || (en.Current.Value.Replace(" ","").Equals("function")) || (en.Current.Value.Replace(" ","").Equals("method")) do
                    classEl.Add(subroutineDec &en)
        
            i <- i + 1
        classEl.Add(en.Current)
        classEl    
    

    let parserMain path = 
        let filesList = Directory.GetFiles(path,"*T.xml")
        for f in filesList do 
            let name = Path.GetFileNameWithoutExtension(f)
            let path2 = path + "\\" + name.[0..name.Length - 2] + ".xml"
            if File.Exists(path2) then
                File.Delete(path2)
            let mutable el = XElement.Load(f)
            
        
            let mutable en = el.Elements().GetEnumerator()
        
            let result = classParse el &en

            let doc=XDocument()
            doc.Add(result)
            doc.Save(path2)