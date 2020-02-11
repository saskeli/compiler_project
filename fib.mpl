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