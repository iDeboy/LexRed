using LexRed.Brzozowski;
using LexRed.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// TODO: Make real tests with a specielized library
//CharClass a = CharClass.CreatePos("a");
//CharClass b = CharClass.CreatePos("b");
//CharClass c = CharClass.CreatePos("c");

//CharClassRegex aRe = a;
//CharClassRegex bRe = b;
//CharClassRegex cRe = c;

//var ab = BrzozowskiRegex.MakeConcat(aRe, bRe);
//var ac = BrzozowskiRegex.MakeConcat(aRe, cRe);

//var a_or_b = BrzozowskiRegex.MakeOr(aRe, aRe, bRe);

//var or = BrzozowskiRegex.MakeOr(ab, ac, BrzozowskiRegex.EmptyString, a_or_b);

//var dfa = or.MakeDFA();

//var isMatch = dfa.IsMatch("a");


CharClassRegex a = CharClass.CreatePos("a");
CharClassRegex b = CharClass.CreatePos("b");

var a_or_b = BrzozowskiRegex.MakeOr(a, b);
var a_or_b_k = BrzozowskiRegex.MakeKleene(a_or_b);

// (a|b)*a(a|b)(a|b)(a|b)(a|b)
var re = BrzozowskiRegex.MakeConcat(a_or_b_k, a, a_or_b, a_or_b, a_or_b, a_or_b);

var sw = Stopwatch.StartNew();

var dfa = re.MakeDFA();

Use(dfa);
Use(dfa.IsMatch("aaaba"));

sw.Stop();

Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds}ms");

return 0;

[MethodImpl(MethodImplOptions.NoInlining)]
static void Use<T>(T value) => Console.WriteLine(value);