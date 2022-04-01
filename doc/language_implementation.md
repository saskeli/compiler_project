# Language implementation

Based on the [language definition](language_definition.md)

Below is the regular definition of the language as implemented (barring bugs).

After the definition, implementation details are discussed with mentions of differences and curiosities compared to the language definition. Beyond that are some examples.

## Regular definition

Non-literal tokens are defined by escaping with a backslash for clarity. When regular expression tokens are to be considered literals, they are also escaped. For example `\read` means the token while `read` is a literal, `\(` means the literal while `(` denotes the start of a grouping.

<pre>
\program    ⭢ \statement ; (\statement ;)*

\statement  ⭢ (\definition | \assignment | \loop | \read | \print | \assert)

\definition ⭢ var \name : \type (:= \expression)?

\assignment ⭢ \name := \expression

\loop       ⭢ for \name in \expression .. \expression do \program end for

\read       ⭢ read \name

\print      ⭢ print \expression

\assert     ⭢  assert \( \expression \)

\name       ⭢ \w (\w | _ | \d)*

\type       ⭢ (int | string | bool)

\expression ⭢  (! \operand | \operand \operator \operand )

\operand    ⭢ (\d\d* | \s | \name | \( \expression \) )

\operator   ⭢ (+ | - | \* | / | = | < | &)

\w          ⭢ (a | b | c | d | e | f | g | h | i | j | k | l | m | 
               n | o | p | q | r | s | t | u | v | w | x | y | z | 
               A | B | C | D | E | F | G | H | I | J | K | L | M | 
               N | O | P | Q | R | S | T | U | V | W | X | Y | Z)

\d          ⭢ (0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9)

\s          ⭢ " (. | \escape )* "

\escape     ⭢ \\ (n | r | t | " | \\ )
</pre>

## Implementation details

### Primitives

**int**. This is internally represented by a 32-bit signed integer. No overflow checking is done for calculations. The default value of a variable of type int is 0. That is, the following will not throw an exception.

```pascal
var x : int;
assert (x = 0);
```

**string**. The internal representation here uses the .NET `System.string` implementation, which in turn is represented by a sequence of utf-16 code points. While the string primitive is internally represented as a data structre, the primitive itself is immutable. The default value of a variable of type string is the epty string. That is.

```pascal
var x: string;
assert (x = "");
```

**bool**. Internally represented as a .NET `System.Boolean`. Can take one of two values "true" and "false". There is no reserved keyword for initializing booleans with specific values. The default value of a varible of type bool is "false".

### Scoping

Contrary to what the Language definition states, the implementation does support local scoping. The only construct that contains a nested scope is the "for" loop. Variables that are declared in a loop will fall out of scope at the end of the loop. As per the language definition, variable have to be defined and typed using the "var" construct before referencing is allowed.

### Operators

The language definition contains 8 operators.

#### +: Addition is supported both for strings and integers.

<code>+(int a, int b) ⭢ int </code> Denotes typical integral addition but is subject to integer overflow.

<code>+(string a, string b) ⭢ string </code> Denotes string concatenation. Output will be a new string where the start matches the first input string and the end matches the second input.

#### -: Subtraction is supported for integers only.

<code>-(int a, int b) ⭢ int </code> Denotes typical integral subtraction but is subject to integer overflow.

#### \*: Multiplication is supported for integers only.

<code>*(int a, int b) ⭢ int </code> Denotes typical integral multiplication but is subject to integer overflow.

#### /: Division is supported for integers only.

<code>/(int a, int b) ⭢ int </code> Denotes integral division, that is the output integer is the rounded down result of the natrual rivision for reals.

#### &: Logical and operation is supported for booleans only.

<code>&(bool a, bool b) ⭢ bool </code> Denotes the logical and. That is the output value will be "true" if and only if both a and b have the value "true".

#### !: Logical negation is supported for booleans only.

<code>!(bool a) ⭢ bool </code> Denotes logical negation. Ths is, if a is "true" the "false" value will be output, and if a is "false" the "true" value will be output.

#### =: Equality comparison is supported for all types. (but not between types).

<code>=(int a, int b) ⭢ bool </code> Will yield "true" if the integral values of a and b are the same, and "false" otherwise.

<code>=(bool a, bool b) ⭢ bool </code> Will yield "true" if the boolean values of a and b are the same, and "false" otherwise.

<code>=(string a, string b) ⭢ string </code> Will yield "true" if the character content of the strins are equal, and "false" otherwise.

#### <: Less than comparison is supported for all types. (but not between types).

<code> <(int a, int b) ⭢ bool </code> Will yield "true" if the integral value of a is less than the value of b, and "false" otherwise.

<code> <(string a, string b) ⭢ bool </code> Will yield "true" if a is loxicographically smaller than b, and "false" otherwise.

<code> <(bool a, bool b) ⭢ bool </code> Not naturally defined for booleans but required by the language definition, will yield "true" if a is "false" and b is "true", and "false" otherwise.

### Language constructs

#### Variable definition

For user defined variables to be usable in a program, they have to be defined using the variable definition construct. The construct is entered using the var construct. After this the desired name is entered, followed by a colon ":" and the type of the variable. Additionally this may be followed by an assignemnt denoted by ":=", followed by an experssion that ecaluates to a value of the specified type.

Note that variable names have to start with a character in `[A-Za-z]` and subsequent characters can be any of `[A-Za-z0-9_]`.

For example, to define an integer "i" with the default initial value:
```pascal
var i : int;
```

Or to define a string hello with the intial value "Hello, world!":
```pascal
var hello : string := "Hello, world!";
```

#### Reading input

Input can be read using the "read" construct. This is triggered by the "read" keyword, followed by a variable reference. This will read input from the standard input, and the result will be stored as the value of the given variable. There will be an attempt to coerce the given input into the target type.

Contrary to the language definition, when reading strings, the input will only be broken by a line terminator (carriage return or new line).

```pascal
var x : string;
read x;
```

#### Writing output

Output can be written using the "print" keyword, followed by an operand. The operand will be evaluated, and the result output.

```pascal
print "Hello, world!";
```

#### Assertions

Conditions can be checked using the "assert" keyword, followed by an operand, that evaluates to bool. If the operand evaluates to true, program execution will continue after the assertion. If the operand evaluates to false, program execution will be terminated and a message will be output denoting the failed assertion.

```pascal
var x : bool;
x := !x;
assert x;
```

#### Loops

The only loop construct in the language definition is the so called "for do" -loop. The construct looks as follows:

```pascal
var x : int;
for x in 1..10 do
    print x;
end for;
```

The loop variable (here x) has to be an integer variable declared before the loop. The loop will execute by assigning all values defined by the range (here 1..10) to the loop variable in order, and executing the loop body (here `print x;`). The range is inclusive, and assignments to the loop variable in the body of the loop are forbidden.

The end points of the range have to evaluate to integers but need not be in order. `1..4` will assign `1, 2, 3, 4` while `4..1` will assign `4, 3, 2, 1`.

The loop can contain any valid statements including nested loops.

### Comments

The language also supports comments in source code, that are ignored by syntactic and semantic analysis. These can either be line comments or block comments. Line comments are started by "//" and ended by a new line. Block comments are started by "\*/" and ended by "/\*". Comments can occur at anywhere in the code between tokens.

## Examples

### 1

The following program will print "16" to the standard ouput.

```pascal
var X : int := 4 + (6 * 2);
print X;
```

### 2

The following program will terminate with an assertion error. See I/O example below.

```pascal
var nTimes : int := 0;
print "How many times?";
read nTimes;
var x : int;
for x in 0..nTimes-1 do
    print x;
    print " : Hello, World!\n";
end for;
assert (x = nTimes);
```

```
How many times?4
0 : Hello, World!
1 : Hello, World!
2 : Hello, World!
3 : Hello, World!
-----------------------------------
Assertion failed

end for;
assert (x = nTimes);
-----^
```

### 3

The following program computes and outputs the sequence product form 1 to n for an input integer n.

```pascal
print "Give a number";      
var n : int;     
read n;     
var v : int := 1;     
var i : int;     
for i in 1..n do          
    v := v * i;     
end for;     
print "The result is: ";     
print v; 
```

### 4

The following produces the n<sup>th</sup> Fibonacci number.

```pascal
print "Program for calculating the n:th fibonacci number\n";
print "The sequence is assumed to start 1, 1, 2, 3, ...\n";
var n : int;     
print "Enter n: ";
read n;
assert (0 < n);
var a: int;
var b: int := 1;
for n in 1..n do         
    var t: int := b;
    b := a + b;
    a := t;
end for;     
print "Your number is ";
print a;
print "!\n"; 
```
