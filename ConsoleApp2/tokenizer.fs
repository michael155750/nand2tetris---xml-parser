
[<AutoOpen>]
module tokenizer


open System
open System.Xml.Linq

open System.IO


let private isNumber(ch)=
    let numbers=['1';'2';'3';'4';'5';'6';'7';'8';'9']
    if List.exists(fun elem->elem=ch)numbers then
        true
    else
        false
let private isDigit(ch)=
    let digits=['1';'2';'3';'4';'5';'6';'7';'8';'9';'0']
    if List.exists(fun elem->elem=ch)digits then
        true
    else
        false

let symbols=["(";")";"{";"}";"[";"]";",";".";";";"+";"-";"*";"/";"=";"<";">";"~";"|";"&"]
let keywords=["class";"constructor";"function";"method";"field";"static";"var";"int";"char"
              "boolean";"void";"true";"false";"null";"this";"let";"do";"if";"else";"while";"return"]


let tokenizerMain path = 
    
    let filesList = Directory.GetFiles(path,"*.jack")
     
    

    for f in filesList do
        let path2 = path + "\\" + Path.GetFileNameWithoutExtension(f) + "T.xml"
        
        if File.Exists(path2) then
            File.Delete(path2)

        let _zz=XElement(XName.Get("tokens"))
        let file=File.ReadLines(f) 
        
        
        let mutable  blockCommentFlag = false
        let mutable endBlockCommentFlag = false
        let mutable startCommentFlag = false
        
        let mutable tempString=""
        let mutable stringFlag=false
        let mutable tempNumber=""
        let mutable numberFlag=false
        let mutable lineCommentFlag=false
        let mutable wordFlag=false
        let mutable tempWord=""

        let newInteger() = 
            _zz.Add(XElement(XName.Get("integerConstant"),tempNumber))
            numberFlag<-false
            tempNumber<-""

        let newWord() = 
            if List.exists(fun elem->elem=tempWord)keywords then
                _zz.Add(XElement(XName.Get("keyword"),tempWord))
            else
                _zz.Add(XElement(XName.Get("identifier"),tempWord))
            wordFlag<-false
            tempWord<-""
        
        let func(ch)=
            if stringFlag && ch <> '"' then
                 tempString <- tempString + ch.ToString()
            elif ch='"' then
                 if stringFlag then
                     _zz.Add(XElement(XName.Get("stringConstant"),tempString))
                     tempString<-""
                     stringFlag <- false
                 else
                     stringFlag<-true
                     //tempString<-tempString
            
            //check comments
            elif ch = '/' then
                startCommentFlag <- true
            //check if symbol     
            elif List.exists(fun elem->elem=string(ch))symbols then
                if numberFlag then
                    newInteger()
                if wordFlag then
                    newWord()
                _zz.Add(XElement(XName.Get("symbol"),ch))
             
             //check if int
             //if the number is 0
            elif ch='0' then
                if not numberFlag then
                    (_zz.Add(XElement(XName.Get("integerConstant"),"0")))
                else 
                    tempNumber <- tempNumber + "0"
             
             //another numbers
            elif isNumber(ch) then
                 tempNumber<-tempNumber+string(ch)
                 numberFlag<-true
                    
            elif ch = ' ' || ch = '\t' then
                if numberFlag then
                    newInteger()
                if wordFlag then
                   newWord()

            //check if keyboard or identifier
            else
                tempWord<- tempWord + string(ch)
                wordFlag <- true

        for line in file do
            tempString<-""
            stringFlag<-false
            tempNumber<-""
            numberFlag<-false
            lineCommentFlag<-false
            wordFlag<-false
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
                        
               elif endBlockCommentFlag then
                    if ch = '/' then
                        blockCommentFlag <- false
                    else 
                        endBlockCommentFlag <- false
               elif ch = '*' then
                   endBlockCommentFlag <- true 

        let doc=XDocument()
        doc.Add(_zz)
        doc.Save(path2)
        
    