﻿namespace ex5
[<AutoOpen>]
module symbolClassDef = 
    open System.Collections.Generic

    //record for a row of symbol table
    type tableRaw = 
        {
         mutable name:string
         mutable m_type:string
         mutable kind:string
         mutable index:int
        }

    //class represents symbol table
    type symbolTable(name1) = class 
       
        let mutable fieldIndex = 0 
        let mutable staticIndex = 0 
        let mutable argIndex = 0 
        let mutable localIndex = 0 
         
        member _.name with get() = name1
        member _.data = new List<tableRaw>()
        
        //clear the table
        member this.startSubroutine = 
            this.data.Clear
    
        //insert new row to the table
        member this.define name t kind = 
            let mutable index = 0
            match kind with
            |"static"->index <- staticIndex
            |"field"->index <- fieldIndex
            |"argument"->index <- argIndex
            |_->index <- localIndex
            this.data.Add({name = name; m_type = t; kind = kind;index=index})

            match kind with
            |"static"->staticIndex <- staticIndex + 1
            |"field"->fieldIndex <- fieldIndex + 1
            |"argument"->argIndex <- argIndex + 1
            |_->localIndex <- localIndex + 1

        //find the number elements in the table from certian kind
        member this.varCount kind = 
            this.data.FindAll(fun el->el.kind = kind).Count

        //returns the kind by name
        member this.kindOf name = 
            this.data.Find(fun el->el.name = name).kind

        //returns the type by name
        member this.typeOf name = 
            this.data.Find(fun el->el.name = name).m_type

        //returns the index by name
        member this.indexOf name = 
            this.data.FindIndex(fun el->el.name = name)
            //TODO: if not found return false 
        
        member this.getTheBigIndexOfStatic =
            0

        member this.getTheBigIndexOf sort =
            if sort="static" then
                null
            
                null
            
            
    end

    let mutable methodTable = symbolTable("methodTable")

    