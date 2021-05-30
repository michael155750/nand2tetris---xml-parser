[<AutoOpen>]
module symbolClassDef 
open System.Collections.Generic

//record for a row of symbol table
type tableRaw = 
    {
     mutable name:string
     mutable m_type:string
     mutable kind:string

    }

//class represents symbol table
type symbolTable() = class 
    
    member _.data = new List<tableRaw>()
        
    //clear the table
    member this.startSubroutine = 
        this.data.Clear
    
    //insert new row to the table
    member this.define name t kind = 
        this.data.Add({name = name; m_type = t; kind = kind})

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
end

let mutable methodTable = symbolTable()