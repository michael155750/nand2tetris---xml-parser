namespace ex5



[<AutoOpen>]
module symbolClassDef = 
    open System
    open System.Collections.Generic

    //record for a row of symbol table
    type tableRaw  = 
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
         
        let data = new Collections.Generic.List<tableRaw>()
        member _.name with get() = name1
        
        
        //clear the table
        member this.startSubroutine = 
            data.Clear|>ignore //reset all records
            argIndex<-0//reset counters of index
            localIndex<-0
    
        //insert new row to the table
        member this.define name t kind = 
            let mutable index = 0
            match kind with
            |"static"->index <- staticIndex
            |"field"->index <- fieldIndex
            |"argument"->index <- argIndex
            |_->index <- localIndex
            let raw1 = {name = name; m_type = t; kind = kind;index=index}
            data.Add(raw1)

            match kind with
            |"static"->staticIndex <- staticIndex + 1
            |"field"->fieldIndex <- fieldIndex + 1
            |"argument"->argIndex <- argIndex + 1
            |_->localIndex <- localIndex + 1

        //find the number elements in the table from certian kind
        member this.varKindCount kind = 
            data.FindAll(fun el->el.kind = kind).Count

        //find the number of elements with the name
        member this.varCount name = 
             data.FindAll(fun el->el.name = name).Count

        //returns the kind by name
        member this.kindOf name = 
            if not (System.Object.ReferenceEquals(data.Find(fun el->el.name = name), null)) then
                data.Find(fun el->el.name = name).kind
            else
                ""
            

        //returns the type by name
        member this.typeOf name =
            if not (Object.ReferenceEquals(data.Find(fun el->el.name = name),null)) then
                data.Find(fun el->el.name = name).m_type
            else
                ""

        //returns the index by name
        member this.indexOf name = 
            if not (Object.ReferenceEquals(data.Find(fun el->el.name = name),null)) then
                data.Find(fun el->el.name = name).index
            else
                -1
            
            
    end

    
    
    let classTables:Dictionary<string,symbolTable> = new Dictionary<string,symbolTable>()

    

    let a = 0

   
            

                
        
                