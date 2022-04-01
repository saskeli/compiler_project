# Language implementation

Based on the [language definition](min_pl.md)

Below is the regular definition of the language as implemented (barring bugs).

After the definition, implementation details are discussed with mentions of differences and curiosities 
compared to the language definition. Beyond that are some examples.

## Regular definition

Non-literal tokens are defined by escaping with a backslash for clarity. When regular expression tokens 
are to be considered literals, they are also escaped. For example `\read` means the token while `read` is a 
literal, `\(` means the literal while `(` denotes the start of a grouping.

This definition is purely syntactical, and allows for semantically invalid code. e.g. `var i: integer := "hello";` 
is syntactically valid but semantically a string literal can not be assigned to an integer variable.

Some extraneous whitespace is added for readability.

<pre>
\program     ⭢ program \name; (\procedure | \function)* \block\.

\procedure   ⭢ procedure \name \(\parameters\);(\block;)?

\function    ⭢ function \name \(\parameters\): \type;(\block;)?

\block       ⭢ begin (\statement;)* end

\parameters  ⭢ (\parameter(, \parameter)*)?

\type        ⭢ (\type_id|\array_type)

\statement   ⭢ (\simple_st|\struct_st|\declaration)

\parameter   ⭢ (var)? \name: \type

\type_id     ⭢ (boolean|integer|string|real)

\array_type  ⭢ array\[\expr?\] of (boolean|integer|real)

\simple_st   ⭢ (\assignment|\call|\return|\read|\write|\assert)

\struct_st   ⭢ (\block|\if|\while|\for)

\declaration ⭢ var \name(, \name)*: \type( := \expr)?

\name        ⭢ [a-Z][a-Z0-9_]*

\expr        ⭢ \s_expr( (=|<>|<|<=|>=|>) \s_expr)?

\assignment  ⭢ \name(\[\expr\])? := \expr

\call        ⭢ \name\(\args\)

\return      ⭢ return (\name|\expr)?

\read        ⭢ read\(\name(, \name)*\)

\write       ⭢ write\(\args\)

\assert      ⭢ assert\(\expr\)

\if          ⭢ if \expr then \statement( else \statement)

\while       ⭢ while \expr do \statement

\for         ⭢ for \name in \expr\.\.\expr do \statement

\s_expr      ⭢ (-|+)?\term((-|+|or)\term)?

\args        ⭢ ((\name|\expr)(, (\name|\expr))*)?

\term        ⭢ \factor( (\*|/|%|and) \factor)

\factor      ⭢ (\call|\name|\literal|\(\expr\)|not \factor||\factor\.size)

\literal     ⭢ (\integer|\real|\string)

\integer     ⭢ (-|+)?\d+

\real        ⭢ \d+\.\d+(e(-|+)\d+)?

\string      ⭢ "([^\n"\\]|\\n|\\"|\\t|\\r|\\\\)*"
</pre>

## Significant differences to language definition

### Variable definition

Contrary to the language definition, all non-assigning variable definitions will assign a default value to the variable.

This value can be overridden by doing an immediate assignment. The immediate assignment syntax not defined in the language definition.

```pascal
var i: integer := 7;
var a: array[i] of integer := i;
```

The declared variable `i` will have value 7 and the array `a` will contain seven sevens.

### Procedures and functions

The implementation requires function and procedure defintion before use. 
This means that forward declaration will have to be used in cases where mutual 
recurrence is needed (or out of order declaration in general). The syntax for 
forward declaration is simply to omit the block.

```pascal
program make_one;
procedure div(var a: integer);

procedure sub(var a: integer);
begin
  a := a - 1;
  if a = 1 then return;
  if a % 2 = 0 then
    div(a)
  else
    sub(a);
end;

procedure div(var a: integer);
begin
  a := a / 2;
  if a = 1 then return;
  if a % 2 = 0 then
    div(a)
  else
    sub(a);
end;

begin
  var a: integer;
  writeln("give number to make small");
  read(a);
  if a % 2 = 0 then
    div(a)
  else
    sub(a);
  writeln("Done")
end.
```

### Predefined identifiers (or not)

I hate predefined identifiers with a passion. The things given as predefined identifiers will be 
reserved keywords in this implementation. I dare you to fail me! Fight me!

```pascal
var false: integer := 7;
```

Shall not compile!

### Array (re)allocation

The language definition does not specify how arrays should work.

```pascal
var a: array[] of integer;
```

is syntactically supported but no reallocation is provided for so the array is unusable?

I decided to allow

```pascal
a.size := 10;
```

This will allow reallocation in general and make the previous a usable statement.

### Strings

Strings are not simple. Creating an array of strings is not allowed.

However, since we are now provided indexing and array syntax, string will behave as character arrays.

```pascal
var s: string[10]; // 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
s[0] := 73;
s[1] := 105;
s[2] := 33;
assert(s = "Hi!");
```

is equivalent (to the user) to

```pascal
var s: string := "Hi!"; // 0x48, 0x69, 0x21, 0x00
assert(s = "Hi!");
```
