
using LexRed.Common;
using LexRed.Thompson;
using System.ComponentModel;

CharClassRegex a = CharClass.CreatePos("a");
CharClassRegex b = CharClass.CreatePos("b");

// ab
var concat = ThompsonRegex.MakeConcat(a, b);

// a|ab
var union = ThompsonRegex.MakeOr(a, concat, a);

// (a|ab)*
var kleene = ThompsonRegex.MakeKleene(union);

var nfa = kleene.MakeNFA();
var dfa = nfa.ToDFA();

var r = nfa.IsMatch("aba");



return 0;