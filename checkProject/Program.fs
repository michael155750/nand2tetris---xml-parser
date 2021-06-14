// Learn more about F# at http://fsharp.org

open System
open System
open System.Xml.Linq
open System.Collections.Generic
open System.IO

type tableRaw  = 
    {
     mutable name:string
     mutable m_type:string
     mutable kind:string
     mutable index:int
    }
[<EntryPoint>]
let main argv =
    
    let raw1 = {name = "a"; m_type = "b"; kind = "c";index=1}
    let booksList = new List<tableRaw>()
    booksList.Add(raw1)
    //booksList.Add("Atlas Shrugged")
    //booksList.Add("Fountainhead")
    //booksList.Add("Thornbirds")
    //booksList.Add("Rebecca")
    //booksList.Add("Narnia")
    
    
    //booksList |> Seq.iteri (fun index item -> printfn "%i: %s" index booksList.[index])
    0
    