# Compiler Project

Project work for compilers course. C# interpreter for the "Mini-PL" toy programming language.

This course focuses on the front end of compilers, including lexical, syntactic and semantic analysis.

A further course called code generation, will focus on the back end of compilers with an extended version of the language.

## Quick guide

```
  -v, --verbose        (Default: false) Enable output of diagnostic messages

  -d, --debug          (Default: 0) Enable detailed diagnostic messages

  -m, --multi-error    (Default: false) Report multiple errors during single run.

  --help               Display this help screen.

  --version            Display version information.

  file (pos. 0)        Required. input Mini-PL source code file. Required
```

The interpreter takes one required positional argument, that is the path to a Mini-PL source code file.

In addition to the required argument some others may be specified:
* The `-v` switch will enable some information on the running of the interpreter
* Valid input values for the `-d` switch are 0, 1 and 2. Even 1 will likely flood the standard output to an annoying extent and 2 will definitely do so.
* The default behaviour of the interpreter is to terminate on the first encountered error and report. This can be overridden by providing the `-m` switch. When set, the lexer and parser will try to continue on after error states in case more errors are found. This typically works very badly and a massive amount of false positives will be reported due to previous errors. In any case, errors encountered while parsing the input will prevent the program from being run.

To run the code in file `fib.mpl` for example, one would simply run `mpl.exe fib.mpl`.

## Documentation

### [Given language defnition](doc/language_definition.md)

### [Language as implemented for the interpreter](doc/language_implementation.md)
