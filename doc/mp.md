
# Syntax of Mini-Pascal (Spring 2020)

Mini-Pascal is a simplified (and slightly modified) subset of Pascal. Generally, the meaning of the features of Mini-Pascal programs are similar to their semantics in other common imperative languages, such as C. 

1. A Mini-Pascal program consist of series of functions and procedures, and a main block. The subroutines may call each other and may be (mutually) recursive. Within the same scope (procedure, function, or block), identifiers must be unique but it is OK to redefine a name in an *inner* scope. 

2. A **var** parameter is passed by *reference*, i.e. its address is passed, and inside the subroutine the parameter name acts as a synonym for the variable given as an argument. A called procedure or function can freely read and write a variable that the caller passed in the argument list. 

3. Mini-Pascal includes a C-style *assert* statement. If an assertion fails the system prints out a diagnostic message and halts execution. 

4. The Mini-Pascal operation *a.size* only applies to values of type **array of** *T* (where *T* is a simple type). There are only one-dimensional arrays. Array types are compatible only if they have the same element type. Arrays' indices begin with zero. The compatibility of array indices and array sizes is checked at run time (usually). 

5. By default, variables in Pascal are not initialized (with zero or otherwise); so they may initially contain rubbish (random) values. 

6. A Mini-Pascal program can print numbers and strings via the predefined special routines *read* and *writeln*. The stream-style input operation *read* makes conversion of values from their text representation to appropriate internal numerical (binary) representation. 

7. Pascal is a case non-sensitive language, which means you can write the names of variables, functions and procedures in either case. 

8. The Mini-Pascal *multiline comments* are enclosed within curly brackets and asterisks as follows: "{* . . . *}". 

9. Note that the names *Boolean*, *false*, *integer*, *read*, *real*, *size*, *string*, *true*, *writeln* are treated in Mini-Pascal as "predefined identifiers", i.e., it is allowed to use them as regular identifiers in Mini-Pascal programs. 

The arithmetic operator symbols '+', '-', '*', and '/' represent the following functions, where T is either "*integer*" or "*real*".   

```
"+" : (T, T) -> T            // addition   
"-" : (T, T) -> T            // subtraction     
"*" : (T, T) -> T            // multiplication     
"/" : (T, T) -> T            // division 
```

The operator '%' represents integer modulo operation. The operator '+' also represents string concatenation:   

```
"%" : (integer, integer) -> integer           // integer modulo   
"+" : (string, string) -> string              // string concatenation 
```

The operators "**and**", "**or**", and "**not**" represent Boolean operations:   

```
"or"  : (Boolean, Boolean) -> Boolean         // logical or  
"and" : (Boolean, Boolean) -> Boolean         // logical and  
"not" : (Boolean) -> Boolean                  // logical not
```
The relational operators "=", "<>", "<", "<=", ">=", ">" are overloaded to represent the comparisons between two values of the same type, with the obvious meanings. They can be applied to values of the types *int*, *real*, *string*, *Boolean*.

## Context-free syntax notation for Mini-Pascal

The syntax definition is given in so-called **Extended Backus-Naur** form (EBNF). In the following Mini-Pascal grammar, the use of curly brackets "{ ... }" means 0, 1, or more repetitions of the enclosed items. Parentheses may be used to group together a sequence of related symbols. Brackets ("[" "]") may be used to enclose optional parts (i.e., zero or one occurrence). Reserved keywords are marked bold (as "**bold**"). Operators, separators, and other single or multiple character tokens are enclosed within quotes (as ":="). Note that the syntax given below also specifies the precedence of operators (via productions defined at different hierarchical levels). 

## Context-free grammar

<pre>
&lt;<em>program</em>&gt;              ::= "<b>program</b>" &lt;<em>id</em>&gt; ";" { &lt;<em>procedure</em>&gt; | &lt;<em>function</em>&gt; } &lt;<em>main-block</em>&gt; "." 
&lt;<em>procedure</em>&gt;            ::= "<b>procedure</b>" &lt;<em>id</em>&gt; "(" &lt;<em>parameters</em>&gt; ")" ";" &lt;<em>block</em>&gt; ";"
&lt;<em>function</em>&gt;             ::= "<b>function</b>" &lt;<em>id</em>&lt; "(" &lt;<em>parameters</em>&gt; ")" ":" &lt;<em>type</em>&gt; ";" &lt;<em>block</em>&gt; ";" 
&lt;<em>var-declaration</em>&gt;      ::= "<b>var</b>" &lt;<em>id</em>&gt; { "," &lt;<em>id</em>&gt; } ":" &lt;<em>type</em>&gt;
&lt;<em>parameters</em>&gt;           ::= [ "<b>var</b>" ] &lt;<em>id</em>&gt; ":" &lt;<em>type</em>&gt; { "," [ "<b>var</b>" ] &lt;<em>id</em>&gt; ":" &lt;<em>type</em>&gt; } | &lt;<em>empty</em>&gt;
&lt;<em>type</em>&gt;                 ::= &lt;<em>simple type</em>&gt; | &lt;<em>array type</em>&gt; 
&lt;<em>array type</em>&gt;           ::= "<b>array</b>" "[" [&lt;<em>integer expr</em>&gt;] "]" "<b>of</b>" &lt;<em>simple type</em>&gt;
&lt;<em>simple type</em>&gt;          ::= &lt;<em>type id</em>&gt;
&lt;<em>block</em>&gt;                ::= "<b>begin</b>" &lt;<em>statement</em>&gt; { ";" &lt;<em>statement</em>&gt; } [ ";" ] "<b>end</b>"
&lt;<em>statement</em>&gt;            ::= &lt;<em>simple statement</em>&gt; | &lt<em>structured statement</em>&gt; | &lt;<em>var-declaration</em>&gt;
&lt;<em>empty</em>&gt;                ::= 
&lt;<em>simple statement</em>&gt;     ::= &lt;<em>assignment statement</em>&gt; | &lt;<em>call</em>&gt; | &lt;<em>return statement</em>&gt; 
                           | &lt;<em>read statement</em>&gt; | &lt;<em>write statement</em>&gt; | &lt;<em>assert statement</em>&gt;
&lt;<em>assignment statement</em>&gt; ::= &lt;<em>variable</em>&gt; ":=" &lt;<em>expr</em>&gt; 
&lt;<em>call</em>&gt;                 ::= &lt;<em>id</em>&gt; "(" &lt;<em>arguments</em>&gt; ")" 
&lt;<em>arguments</em>&gt;            ::= &lt;<em>expr</em>&gt; { "," &lt;<em>expr</em>&gt; } | &lt;<em>empty</em>&gt; 
&lt;<em>return statement</em>&gt;     ::= "<b>return</b>" [ &lt;<em>expr</em>&gt; ] 
&lt;<em>read statement</em>&gt;       ::= "<b>read</b>" "(" &lt;<em>variable</em>&gt; { "," &lt;<em>variable</em>&gt; } ")" 
&lt;<em>write statement</em>&gt;      ::= "<b>writeln</b>" "(" &lt;<em>arguments</em>&gt; ")" 
&lt;<em>assert statement</em>&gt;     ::= "<b>assert</b>" "(" &lt;<em>Boolean expr</em>&gt; ")" 
&lt;<em>structured statement</em>&gt; ::= &lt;<em>block</em>&gt; | &lt;<em>if statement</em>&gt; | &lt;<em>while statement</em>&gt; 
&lt;<em>if statement</em>&gt;         ::= "<b>if</b>" &lt;<em>Boolean expr</em>&gt; "<b>then</b>" &lt;<em>statement</em>&gt; 
                           | "<b>if</b>" &lt;<em>Boolean expr</em>&gt; "<b>then</b>" &lt;<em>statement</em>&gt; "<b>else</b>" &lt;<em>statement</em>&gt; 
&lt;<em>while statement</em>&gt;      ::= "<b>while</b>" &lt;<em>Boolean expr</em>&gt; "<b>do</b>" &lt;<em>statement</em> &gt;
&lt;<em>expr</em>&gt;                 ::= &lt;<em>simple expr</em>&gt; | &lt;<em>simple expr</em>&gt; &lt;<em>relational operator</em>&gt; &lt;<em>simple expr</em>&gt; 
&lt;<em>simple expr</em>&gt;          ::= [ &lt;<em>sign</em>&gt; ] &le;<em>term</em>&gt; { &lt;<em>adding operator</em>&gt; &lt;<em>term</em>&gt; } 
&lt;<em>term</em>&gt;                 ::= &lt;<em>factor</em>&gt; { &lt;<em>multiplying operator</em>&gt; &lt;<em>factor</em>&gt; } 
&lt;<em>factor</em>&gt;               ::= &lt;<em>call</em>&gt; | &lt;<em>variable</em>&gt; | &lt;<em>literal</em>&gt; | "(" &lt;<em>expr</em>&gt; ")" 
                           | "<b>not</b>" &lt;<em>factor</em>&gt; | &lt;<em>factor</em>&gt; "." "<b>size</b>" 
&lt;<em>variable</em>&gt;             ::= &lt;<em>variable id</em>&gt; [ "[" &lt;<em>integer expr</em>&gt; "]" ] 
&lt;<em>relational operator</em>&gt;  ::= "=" | "&lt;&gt;" | "&lt;" | "&lt;=" | "&gt;=" | "&gt;" 
&lt;<em>sign</em>&gt;                 ::= "+" | "-" 
&lt;<em>negation</em>&gt;             ::= "<b>not</b>" 
&lt;<em>adding operator</em>&gt;      ::= "+" | "-" | "<b>or</b>" 
&lt;<em>multiplying operator</em>&gt; ::= "*" | "/" | "%" | "<b>and</b>"
</pre>

## Lexical grammar

<pre>
&lt;<em>id</em>&gt;              ::= &lt;<em>letter</em>&gt; { &lt;<em>letter</em>&gt; | &lt;<em>digit</em>&gt; | "_" } 
&lt;<em>literal</em>&gt;         ::= &lt;<em>integer literal</em>&gt; | &lt;<em>real literal</em>&gt; | &lt;<em>string literal</em>&gt; 
&lt;<em>integer literal</em>&gt; ::= &lt;<em>digits</em>&gt; 
&lt;<em>digits</em>&gt;          ::= &lt;<em>digit</em>&gt; { &lt;<em>digit</em>&gt; } 
&lt;<em>real literal</em>&gt;    ::= &lt;<em>digits</em>&gt; "." &lt;<em>digits</em>&gt; [ "<b>e</b>" [ &lt;<em>sign</em>&gt; ] &lt;<em>digits</em>&gt;] 
&lt;<em>string literal</em>&gt;  ::= "\"" { &lt;<em> a char or escape char</em>&gt; } "\"" 
&lt;<em>letter</em>&gt;          ::= a | b | c | d | e | f | g | h | i | j | k | l | m | n | o 
                    | p | q | r | s | t | u | v | w | x | y | z | A | B | C | D 
                    | E | F | G | H | I | J | K | L | M | N | O | P | Q | R | S 
                    | T | U | V | W | X | Y | Z 
&lt;<em>digit</em>&gt;           ::= 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 
&lt;<em>special symbol or keyword</em> ::= 
                      "+" | "-" | "*" | "%" | "=" | "&lt;&gt;" | "&lt;" | "&gt;" | "&lt;=" | "&gt;=" | "(" | ")" 
                    | "[" | "]" | ":=" | "." | "," | ";" | ":" | "<b>or</b>" | "<b>and</b>" | "<b>not</b>" | "<b>if</b>" 
                    | "<b>then</b>" | "<b>else</b>" | "<b>of</b>" | "<b>while</b>" | "<b>do</b>" | "<b>begin</b>" | "<b>end</b>" | "<b>var</b>" 
                    | "<b>array</b>" | "<b>procedure</b>" | "<b>function</b>" | "<b>program</b>" | "<b>assert</b>" | "<b>return</b>" 
&lt;<em>predefined id</em>&gt;   ::= "Boolean" | "false" | "integer" | "read" | "real" | "size" | "string" 
                    | "true" | "writeln" 
</pre>
