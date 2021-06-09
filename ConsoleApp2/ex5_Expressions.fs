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

    let expressionList (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string) (expNum:byref<int>): XElement=
        let mutable el = XElement(XName.Get("expressionList"))
        let mutable flag = false

        while en.Current.Value.Replace(" ","") <> ")" && en.Current.Value.Replace(" ","") <> ";"
            && en.Current.Value.Replace(" ","") <> "]" do
                // handling ,
                if flag then
                    el.Add(en.Current)
                    en.MoveNext()|>ignore
                   

                el.Add(expression &en f className)
                expNum <- expNum + 1
                flag <- true
            
        el

    let subroutineCall (prev:XElement) (en:byref<Collections.Generic.IEnumerator<XElement>>) (f:StreamWriter) (className:string): XElement=
        let mutable el = XElement(XName.Get("subroutineCall"))
        el.Add(prev)
        let mutable exspNum = 0

        //subRoutinName (expressionList)
        if en.Current.Value.Replace(" ","").Equals("(") then
            el.Add(en.Current)
            en.MoveNext()|>ignore
            f.WriteLine("push " + methodTable.kindOf(prev.Value.Replace(" ","")) + " " + 
                methodTable.indexOf(prev.Value.Replace(" ","")).ToString())
            exspNum <- 1
            el.Add(expressionList &en f className &exspNum)
            el.Add(en.Current)
            en.MoveNext()|>ignore
            f.WriteLine("call " + className + "." + prev.Value.Replace(" ","") + " " + exspNum.ToString())
            
        //className/varName.subRoutineName (expressionList)
        else
            
            el.Add(en.Current)
            en.MoveNext()|>ignore
            el.Add(en.Current)
            let name = en.Current.Value.Replace(" ","")
            en.MoveNext()|>ignore
            el.Add(en.Current)
            en.MoveNext()|>ignore
            if methodTable.varCount(name) > 0 then // push this in case of method
                f.WriteLine("push pointer 0")
                exspNum <- 1
            el.Add(expressionList &en f className &exspNum)
            f.Write("call ")
            //check if it class name or instance name 
            if methodTable.varCount(name) > 0 then
                f.Write(methodTable.typeOf(name))
            else
                f.Write(prev.Value.Replace(" ","") )
            f.WriteLine("." + name + " " + exspNum.ToString())
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
            elif en.Current.Value.Replace(" ","").Equals("[") then//TODO
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
                    f.Write("push ")
                    if methodTable.varCount(prevValValue.ToString()) > 0 then
                        if methodTable.kindOf(prevValValue.ToString()) = "var" then
                            f.Write("local ")
          
                        else
                            f.Write("argument ")
                        f.WriteLine(methodTable.indexOf(prevValValue.ToString()))
                    else
                        if classTables.[className].kindOf(prevValValue.ToString()) = "static" then
                            f.Write("static ")
          
                        else
                            f.Write("this ")
                        f.WriteLine(methodTable.indexOf(prevValValue.ToString()))
                    
                    
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