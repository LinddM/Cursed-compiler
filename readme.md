# C# compiler for decaf

Run the program
```
dotnet run
```

Flags and params
```
-o

-target
    scan
    parse
    semantic
    irt
    codegen

-opt
    constant
    algebraic

-debug
    scan
    parse
    semantic
    irt
    codegen
```

## Design
Files and directories
```bash

────class
│   ├───ast
│   │       Ast.cs
│   │
│   ├───codegen
│   │       Codegen.cs
│   │
│   ├───compiler
│   │       Compiler.cs
│   │
│   ├───irt
│   │       Irt.cs
│   │
│   ├───opt
│   │       Algebraic.cs
│   │       ConstantF.cs
│   │
│   ├───parser
│   │       Parser.cs
│   │
│   ├───scanner
│   │       Scanner.cs
│   │
│   └───semantic
│           Semantic.cs
```

### Scanner
Tokenizes and classifies

#### Steps
1. Split incoming file by lines and separate by spaces
```
List<List<String>> myCleanTokens = cleanTokens(text);
```

2. Separate by operators
```
Dictionary<string, List<string>> tokensAndTypes = new Dictionary<string, List<string>>(classify(myCleanTokens));
```

3. Classify reserved words, exluding variables, objects and numbers
```
if(classifyTypes.ContainsKey(tokens[i][j])){
    // lineType positions: line, type, value
    lineType = new List<string>(){(i+1).ToString(), classifyTypes[tokens[i][j]].ToString(), tokens[i][j]};
                        
}
```

4. Define states for variables, objects and numbers. Evaluate
```
if(isVariable(tokens[i][j])){
    hasType=true;
    lineType = new List<string>(){(i+1).ToString(),"<variable>", tokens[i][j]};
}
if(isObject(tokens[i][j])){
    hasType=true;
    lineType = new List<string>(){(i+1).ToString(),"<object>", tokens[i][j]};
}
if(isNumber(tokens[i][j])){
    hasType=true;
    lineType = new List<string>(){(i+1).ToString(),"<number>", tokens[i][j]};
}
```

5. Throw errors in console if tokens doesn't belong to any type
```
if(entry[1]=="error"){
    Console.WriteLine("Error en la linea "+entry[0]+": "+entry[2]);
}
```