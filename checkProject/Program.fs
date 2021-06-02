// Learn more about F# at http://fsharp.org

open System
open System
open System.Xml.Linq
open System.Collections.Generic
open System.IO


[<EntryPoint>]
let main argv =
    let mutable map= Map.empty
    map<-map.Add("first","First")
    map<-map.Add("sec","Second")
    printfn "%A" map.["first"]
    0 // return an integer exit code
