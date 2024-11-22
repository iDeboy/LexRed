using LexRed.Common;
using LexRed.Thompson;
using System.Diagnostics;
using System.Runtime.CompilerServices;

//CharClassRegex a = CharClass.CreatePos("a");
//CharClassRegex b = CharClass.CreatePos("b");

//// ab
//var concat = ThompsonRegex.MakeConcat(a, b);

//// a|ab
//var union = ThompsonRegex.MakeOr(a, concat, a);

//// (a|ab)*
//var kleene = ThompsonRegex.MakeKleene(union);

//var nfa = kleene.MakeNFA();
//var dfa = nfa.ToDFA();

//var r = nfa.IsMatch("aba");

CharClassRegex a = CharClass.CreatePos("a");
CharClassRegex b = CharClass.CreatePos("b");

var a_or_b = ThompsonRegex.MakeOr(a, b);

var a_or_b_k = ThompsonRegex.MakeKleene(a_or_b);

// (a|b)*a(a|b)(a|b)(a|b)(a|b)
var re = ThompsonRegex.MakeConcat(a_or_b_k, a, a_or_b, a_or_b, a_or_b, a_or_b);

var sw = Stopwatch.StartNew();
var nfa = re.MakeNFA();
sw.Stop();
Console.WriteLine($"Make NFA: {sw.ElapsedMilliseconds}ms");

sw.Restart();
var dfa = nfa.ToDFA();
Use(dfa);
sw.Stop();
Console.WriteLine($"Make DFA: {sw.ElapsedMilliseconds}ms");

sw.Restart();
Use(nfa.IsMatch("abbbbaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
sw.Stop();
Console.WriteLine($"IsMatch NFA: {sw.ElapsedMilliseconds}ms");

sw.Restart();
Use(dfa.IsMatch("abbbbaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
sw.Stop();
Console.WriteLine($"IsMatch DFA: {sw.ElapsedMilliseconds}ms");

return 0;

[MethodImpl(MethodImplOptions.NoInlining)]
static void Use<T>(T value) { }