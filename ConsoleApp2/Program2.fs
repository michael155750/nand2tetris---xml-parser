// Learn more about F# at http://fsharp.org

open System
open System.Xml.Linq
open System.Xml
open System.Xml.Linq
open System.Security.Cryptography
open FSharp.Data
open System.IO


let isNumber(ch)=
    let numbers=['1';'2';'3';'4';'5';'6';'7';'8';'9']
    if List.exists(fun elem->elem=ch)numbers then
        true
    else
        false
let isDigit(ch)=
    let digits=['1';'2';'3';'4';'5';'6';'7';'8';'9';'0']
    if List.exists(fun elem->elem=ch)digits then
        true
    else
        false



[<EntryPoint>]
let main argv =
    let symbols=["(";")";"{";"}";"[";"]";",";".";";";"+";"-";"*";"/";"=";"<";">";"~";"|";"&"]
    let keywords=["class";"constructor";"function";"method";"field";"static";"var";"int";"char"
                  "boolean";"void";"true";"false";"null";"this";"let";"do";"if";"else";"while";"return"]
    
    let digits=["1";"2";"3";"4";"5";"6";"7";"8";"9";"0"]
    let path = Console.ReadLine()
    let _zz=XElement(XName.Get("token"));
    let path2 = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "T.xml"
    let file=File.ReadLines(path) 
    
    
    let mutable  blockCommentFlag = false
    let mutable endBlockCommentFlag = false
    let mutable startCommentFlag = false
    
    let mutable tempString=""
    let mutable stringFlag=false
    let mutable tempNumber=""
    let mutable numberFlag=false
    let mutable lineCommentFlag=false
    let mutable tempWord=""

    let func(ch)=
        if stringFlag && ch <> '"' then
             tempString <- tempString + ch.ToString()
        elif ch='"' then
             if stringFlag then
                 _zz.Add(XElement(XName.Get("stringConstant"),tempString))
                 tempString<-""
             else
                 stringFlag<-true
                 //tempString<-tempString
        
        //check comments
        elif ch = '/' then
            startCommentFlag <- true
        //check if symbol     
        elif List.exists(fun elem->elem=string(ch))symbols then
            if numberFlag then
                 _zz.Add(XElement(XName.Get("integerConstant"),tempNumber))
                 numberFlag<-false
                 tempNumber<-""
            _zz.Add(XElement(XName.Get("symbol"),ch))
         
         //check if int
         //if the number is 0
        elif ch='0' then
             (_zz.Add(XElement(XName.Get("integerConstant"),"0")))
         
         //another numbers
        elif isNumber(ch) then
             tempNumber<-""+string(ch)
             numberFlag<-true
                
        //check if keyboard
        elif List.exists(fun elem->elem=string(ch))keywords then
             _zz.Add(XElement(XName.Get("keyword"),ch))


    for line in file do
        tempString<-""
        stringFlag<-false
        tempNumber<-""
        numberFlag<-false
        lineCommentFlag<-false
        tempWord<-""

        //let mutable startCommentFlag=false

        for ch in line.ToCharArray() do
           
           if not blockCommentFlag then
               if not lineCommentFlag  then
                    if startCommentFlag then
                        if ch = '*' then
                            blockCommentFlag <- true
                            startCommentFlag<-false //update Michael
                        elif ch = '/' then
                            lineCommentFlag <- true
                            startCommentFlag<-false //update Michael
                        else 
                            startCommentFlag <- false
                            _zz.Add(XElement(XName.Get("symbol"),'/'))
                            func(ch)
                    else
                        func(ch)
                    (*if stringFlag && ch <> '"' then
                         tempString <- tempString + ch.ToString()
                    elif ch='"' then
                         if stringFlag then
                             _zz.Add(XElement(XName.Get("stringConstant"),tempString))
                             tempString<-""
                         else
                             stringFlag<-true
                             //tempString<-tempString
                    
                    //check comments
                    elif ch == '/' then
                        startCommentFlag <- true
                    //check if symbol     
                    elif List.exists(fun elem->elem=string(ch))symbols then
                        if numberFlag then
                             _zz.Add(XElement(XName.Get("integerConstant"),tempNumber))
                             numberFlag<-false
                             tempNumber<-""
                        _zz.Add(XElement(XName.Get("symbol"),ch))
                     
                     //check if int
                     //if the number is 0
                    elif ch='0' then
                         (_zz.Add(XElement(XName.Get("integerConstant"),"0")))
                     
                     //another numbers
                    elif isNumber(ch) then
                         tempNumber<-""+string(ch)
                         numberFlag<-true
                
                    //check if keyboard
                    elif List.exists(fun elem->elem=string(ch))keywords then
                         _zz.Add(XElement(XName.Get("keyword"),ch))*)
           elif endBlockCommentFlag && ch = '/' then
                blockCommentFlag <- false
           elif ch = '*' then
               endBlockCommentFlag <- true 

    let doc=XDocument()
    doc.Add(_zz)
    doc.Save("C:/Users/user/Downloads/check1.xml")
    //let xd=XDocument.Load("C:/Users/user/Downloads/check.xml")
    //let xn =XName.Get("note")
    //let props=xd.Element(xn)
    
    (*_zz.Add(XElement(XName.Get("symbol"),")"))//child element 
    let xd2=XDocument()
    xd2.Add(_zz)
    
    xd2.Save("C:/Users/user/Downloads/check1.xml")*)
    
    
    
    
    printfn "Hello World from F#!"
    
    0 // return an integer exit code


