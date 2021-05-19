﻿// Learn more about F# at http://fsharp.org

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
    
    let digits=["1";"2";"3";"4";"5";"6";"7";"8";"9";"10"]
    let path = Console.ReadLine()
    let _zz=XElement(XName.Get("token"));
    let path2 = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "T.xml"
    let file=File.ReadLines(path) 
    
    for line in file do
        let mutable temp=""
        for word in line.Split(" ") do
            //check if keyboard
            if List.exists(fun elem->elem=word)keywords then
                _zz.Add(XElement(XName.Get("keyword"),word))
           //check if symbol     
            elif List.exists(fun elem->elem=word)symbols then
                _zz.Add(XElement(XName.Get("symbol"),word))
            
            //check if int
                //if the number is 0
            elif word.Length>0 && word.ToCharArray().[0]='0' then
                (_zz.Add(XElement(XName.Get("integerConstant"),"0")))
                //another numbers
            elif word.Length>0 && isNumber(word.ToCharArray().[0]) then
                temp<-""+string (word.ToCharArray().[0])
                let mutable i=1
                while isDigit(word.ToCharArray().[i]) && i<word.Length do
                    temp<-temp+string (word.ToCharArray().[i])
                    i<-i+1
                _zz.Add(XElement(XName.Get("integerConstant"),temp))
            
    
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


