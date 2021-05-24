// Learn more about F# at http://fsharp.org
open System


[<EntryPoint>]
let main argv =
    Console.WriteLine("Please enter the path:")
    let path = Console.ReadLine()
    tokenizerMain path
    
    //parserMain path
    
    0 // return an integer exit code


