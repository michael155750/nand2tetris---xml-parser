// Learn more about F# at http://fsharp.org
open System



[<EntryPoint>]
let main argv =
    Console.WriteLine("Please enter the path:")
    let path = Console.ReadLine()

    let mutable ans = ""
    Console.WriteLine("Act excercise 4?:")
    Console.WriteLine("instert y for yes and n for no")
    ans <- Console.ReadLine()
    while ans <> "y" && ans <> "n" do
        Console.WriteLine("instert y for yes and n for no")
        ans <- Console.ReadLine()
    if ans = "y" then
        ex4.tokenizer.tokenizerMain path
    
        ex4.parser.parserMain path
    ans <- ""


    
    0 // return an integer exit code


