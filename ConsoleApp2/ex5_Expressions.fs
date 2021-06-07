namespace ex5
[<AutoOpen>]
module rec Expressions = 
    open System
    open System.Xml.Linq
    open System.IO

    let opList = ["+";"=";"-";"*";"/";"&";"|";"<";">"]

    //temp
    let f = File.CreateText("a.txt")

    let opVMtranslator op =
        
        match op with
            |"+" -> "add"
            |"-" -> "sub"
            |"*" ->"call Math.multiply 2" 
            |"/"->"call Math.divide 2"
            |"&"->"and"
            |"|"->"or"
            |"<"->"lt"
            |">"->"gt"
            |_->"eq"

    let expressionList (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string): XElement=
        let mutable el = XElement(XName.Get("expressionList"))
        let mutable flag = false

        while en.Current.Value.Replace(" ","") <> ")" && en.Current.Value.Replace(" ","") <> ";"
            && en.Current.Value.Replace(" ","") <> "]" do
                // handling ,
                if flag then
                    el.Add(en.Current)
                    en.MoveNext()|>ignore

                el.Add(expression &en f className)
                flag <- true
            
        el

    let subroutineCall (prev:XElement) (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string): XElement=
        let mutable el = XElement(XName.Get("subroutineCall"))
        el.Add(prev)

        //(expressionList)
        if en.Current.Value.Replace(" ","").Equals("(") then
            el.Add(en.Current)
            en.MoveNext()|>ignore
            el.Add(expressionList &en f className)
            el.Add(en.Current)
            en.MoveNext()|>ignore
        else
            for i = 1 to 3 do
                el.Add(en.Current)
                en.MoveNext()|>ignore
    
            el.Add(expressionList &en f className)
        
            el.Add(en.Current)
            en.MoveNext()|>ignore

        el

    let term (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string): XElement=
        let mutable el = XElement(XName.Get("term"))
        let mutable prevVal = en.Current
        let prevValValue = prevVal.Value.Replace(" ","")
        let prevValName = prevVal.Name.LocalName.Replace(" ","")
        en.MoveNext()|>ignore
    
        // subroutineCall
        if (en.Current.Value.Replace(" ","").Equals("(") || en.Current.Value.Replace(" ","").Equals("."))
            && not (prevValName.Equals("symbol")) then
                el.Add(subroutineCall prevVal &en  f className)
        else 
            el.Add(prevVal)
           // unaryOp term
            if prevValValue.Equals("-")  then
                el.Add(term &en  f className)
                f.WriteLine("neg")
            elif  prevValValue.Equals("~") then
                el.Add(term &en  f className)
                f.WriteLine("not")

            // (expression)
            elif prevValValue.Equals("(") then            
                el.Add(expression &en f className)
                el.Add(en.Current)
                en.MoveNext()|>ignore
       
           // varName [expression]
            elif en.Current.Value.Replace(" ","").Equals("[") then
                el.Add(en.Current)
                en.MoveNext()|>ignore
                el.Add(expression &en f className)
                el.Add(en.Current)
                en.MoveNext()|>ignore
            
            else
                //integerConstant
                if prevValName.Equals("integerConstant") then 
                    f.WriteLine("push constant " + prevVal.Value.Replace(" ",""))
                //keywordConstant
                elif prevValValue.Equals("false") 
                    || prevValValue.Equals("null") then
                    f.WriteLine("push constant 0")
                elif prevValValue.Equals("true") then
                    f.WriteLine("push constant 0")
                    f.WriteLine("not")
                //elif prevValValue.Equals("this") then
                    //f.WriteLine("push this 0")
                
                //stringConstant
                elif prevValName.Equals("stringConstant") then 
                    f.Write("push constant")
                    f.WriteLine(prevValValue.Length)
                    f.WriteLine("call String.new")
                    for ch in prevValValue.ToCharArray() do 
                        f.Write("push constant")
                        f.WriteLine(Convert.ToInt32(ch))
                //varName
                else
                    f.Write("push ")//TODO
                    if methodTable.varCount(prevValValue.ToString()) > 0 then
                        if methodTable.kindOf(prevValValue.ToString()) = "var" then
                            f.Write("local ")
                            //f.WriteLine(index)
                    
                    
                    //else
                    //class table
        el
    
    let expression (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string): XElement =
        let mutable el = XElement(XName.Get("expression"))
        el.Add(term &en f className)
   
        let mutable temp = en.Current.Value.Replace(" ","")
        while List.exists(fun elem-> elem = temp) opList do
           el.Add(en.Current)
           en.MoveNext()|>ignore
           el.Add(term &en f className)
           f.WriteLine(opVMtranslator temp)
           temp <- en.Current.Value.Replace(" ","")
    
        el