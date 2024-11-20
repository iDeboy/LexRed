
using LexRed.Common;
using LexRed.Thompson;

CharClassRegex a = CharClass.CreatePos("a");

var union = new UnionRegex(a, a);

return 0;