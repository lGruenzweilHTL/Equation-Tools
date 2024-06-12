# Equation-Tools
 Tools for evaluating and simplifying mathematical expressions and evaluating equations

## Data Types
### Calculation (Internal)
A Calculation doesn't have any variables.

An example for a calculation is `1+2`, which will be evaluated to `3`.

Any calculation can also be evaluated using the `Expression` type.

### Expression
An Expression is just like a calculation but is allowed to have variables.

An example for an expression is `5x+3`

### Equation
An Equation portraits equality of two sides. Every equation must have the `=` sign.

An example for an Equation is `5x=10`, which will be simplified to `x=2`

## Features
- Basic Arithmetic operators
- Exponentiation of Variable
- Integer Exponentiation of Brackets
- Short forms like `5x`
- Short forms like `5(x+1)`
- The irrational numbers `e` and `pi`

## Problems
- Variables in the exponent (logarithmic expressions)
- Dividing by brackets (The expression `1/(x+1)` would give the wrong answer)

## Code Examples
### Calculation

```csharp
using MathTools.Internal;
string myCalc = "1+2+3";
string eval = new Calculation(myCalc).Evaluate(); // eval = "6"
```

### Expression

```csharp
using MathTools;
string myCalc = "4x+2x";
string eval = new Expression(myCalc).Simplify(); // eval = "6x"
```

### Equation
```csharp
using MathTools;

// There are 2 ways to use the Equation type
// 1: Whole string
string myEq = "5x=10";
string eval = new Equation(myEq).Solve(); // eval = "x=2"

// 2: Individual sides
string leftSide = "5x";
string rightSide = "10";
string eval = new Equation(leftSide, rightSide).Solve(); // eval = "x=2"
```

### Formatting
Every Math Tool automatically formats any strings you input.
But you can also format them manually

```csharp
using MathTools.Internal;

// This string intentionally has some brackets missing at the end
string unformatted = "2+5(2+pi)+(x+1)^2-(2+(2";

unformatted = unformatted
    .FillMissingEndBrackets() // Inserts any missing brackets at the end
    .ReplaceEAndPi() // Replaces the values e and pi the their actual numbers
    .HandleVariableCoefficients() // Places a multiplication sign between forms like "5x"
    .HandleBracketExponents() // Multiplies out exponentiated brackets (only integer exponents)
    .HandleBracketCoefficients(); // Places a multiplication sign between forms like "5(x+1)"
```