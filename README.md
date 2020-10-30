# ExpressionParser
Infix String -> C# Expression Tree Parser

# Functionality
- Prefix Operators
- Postfix Operators
- Infix Operators
- Operator Precedence
- Parentheses
- Argument Count Operator Overloading
- Integral Constants
- Floating-point Constants
- Implicit Integral -> Floating-point Parameter Conversion

# How it works
- Uses `OperatorInfo` arrays to descibe Functions/Operators
- Tokenization using Regular Expressions
- Parsing using a modified [Shunting-yard Algorithm](https://en.wikipedia.org/wiki/Shunting-yard_algorithm)
- Compilation using [C# Expression Trees](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/)

# Getting Start
Demo using `ExpressionParser.Cli`

`dotnet run -p ./src/ExpressionParser.Cli/ -- "1 + 2 * 3"`

# Running Tests
`dotnet test ./src/ExpressionParser.UTests`
