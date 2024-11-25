using BenchmarkDotNet.Attributes;
using LexRed.Brzozowski;
using LexRed.Common;
using LexRed.Thompson;

namespace LexRed.Benchmarks.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser(false)]
[LongRunJob]
public class MatchBenchmark {

    /*
     Regex to benchmarks:
           1.- ((a|b|c)*d|(e|f|g)(e|f|g)*h)*i(j|k|l)m(n|o|p(q|r|s)*t)*u
           2.- (a|b)*a(a|b)(a|b)(a|b)(a|b)
     */

    private const string FirstRegex = "((a|b|c)*d|(e|f|g)(e|f|g)*h)*i(j|k|l)m(n|o|p(q|r|s)*t)*u";
    private const string SecondRegex = "(a|b)*a(a|b)(a|b)(a|b)(a|b)";

    private const string FirstInput = "abcdbdegfehikmnpsqrtu";
    private const string SecondInput = "bbaababa";

    private BrzozowskiRegex _brzozowskiRegex_1 = default!;
    private BrzozowskiRegex _brzozowskiRegex_2 = default!;

    private DFA _brzozowskiDFA_1 = default!;
    private DFA _brzozowskiDFA_2 = default!;

    private NFA _thompsonNFA_1 = default!;
    private NFA _thompsonNFA_2 = default!;

    private DFA _thompsonDFA_1 = default!;
    private DFA _thompsonDFA_2 = default!;

    [Params(FirstRegex, SecondRegex)]
    public string Regex = null!;

    [Params(FirstInput, SecondInput)]
    public string Input = null!;
    [GlobalSetup]
    public void Setup() {

        _brzozowskiRegex_1 = CreateBrzozowskiRegex_1();
        _brzozowskiRegex_2 = CreateBrzozowskiRegex_2();

        _brzozowskiDFA_1 = _brzozowskiRegex_1.MakeDFA();
        _brzozowskiDFA_2 = _brzozowskiRegex_2.MakeDFA();

        var thompsonRegex_1 = CreateThompsonRegex_1();
        var thompsonRegex_2 = CreateThompsonRegex_2();

        _thompsonNFA_1 = thompsonRegex_1.MakeNFA();
        _thompsonNFA_2 = thompsonRegex_2.MakeNFA();

        _thompsonDFA_1 = _thompsonNFA_1.ToDFA();
        _thompsonDFA_2 = _thompsonNFA_2.ToDFA();

#if DEBUG
        Regex = FirstRegex;
        Input = FirstInput;
        Console.WriteLine($"{Regex} include {Input}: {Brzozowski_Regex_IsMatch()}");
        Console.WriteLine($"{Regex} include {Input}: {Brzozowski_DFA_IsMatch()}");
        Console.WriteLine($"{Regex} include {Input}: {Thompson_NFA_IsMatch()}");
        Console.WriteLine($"{Regex} include {Input}: {Thompson_DFA_IsMatch()}");

        Regex = SecondRegex;
        Input = SecondInput;
        Console.WriteLine($"{Regex} include {Input}: {Brzozowski_Regex_IsMatch()}");
        Console.WriteLine($"{Regex} include {Input}: {Brzozowski_DFA_IsMatch()}");
        Console.WriteLine($"{Regex} include {Input}: {Thompson_NFA_IsMatch()}");
        Console.WriteLine($"{Regex} include {Input}: {Thompson_DFA_IsMatch()}");
#endif
    }

    [Benchmark]
    public bool Brzozowski_Regex_IsMatch() {

        if (Regex is FirstRegex)
            return _brzozowskiRegex_1.IsMatch(Input);
        else if (Regex is SecondRegex)
            return _brzozowskiRegex_2.IsMatch(Input);

        return false;
    }

    [Benchmark]
    public bool Brzozowski_DFA_IsMatch() {

        if (Regex is FirstRegex)
            return _brzozowskiDFA_1.IsMatch(Input);
        else if (Regex is SecondRegex)
            return _brzozowskiDFA_2.IsMatch(Input);

        return false;
    }

    [Benchmark]
    public bool Thompson_NFA_IsMatch() {

        if (Regex is FirstRegex)
            return _thompsonNFA_1.IsMatch(Input);
        else if (Regex is SecondRegex)
            return _thompsonNFA_2.IsMatch(Input);

        return false;
    }

    [Benchmark]
    public bool Thompson_DFA_IsMatch() {

        if (Regex is FirstRegex)
            return _thompsonDFA_1.IsMatch(Input);
        else if (Regex is SecondRegex)
            return _thompsonDFA_2.IsMatch(Input);

        return false;
    }

    private BrzozowskiRegex CreateBrzozowskiRegex_1() {

        Brzozowski.CharClassRegex a = CharClass.CreatePos("a");
        Brzozowski.CharClassRegex b = CharClass.CreatePos("b");
        Brzozowski.CharClassRegex c = CharClass.CreatePos("c");
        Brzozowski.CharClassRegex d = CharClass.CreatePos("d");
        Brzozowski.CharClassRegex e = CharClass.CreatePos("e");
        Brzozowski.CharClassRegex f = CharClass.CreatePos("f");
        Brzozowski.CharClassRegex g = CharClass.CreatePos("g");
        Brzozowski.CharClassRegex h = CharClass.CreatePos("h");
        Brzozowski.CharClassRegex i = CharClass.CreatePos("i");
        Brzozowski.CharClassRegex j = CharClass.CreatePos("j");
        Brzozowski.CharClassRegex k = CharClass.CreatePos("k");
        Brzozowski.CharClassRegex l = CharClass.CreatePos("l");
        Brzozowski.CharClassRegex m = CharClass.CreatePos("m");
        Brzozowski.CharClassRegex n = CharClass.CreatePos("n");
        Brzozowski.CharClassRegex o = CharClass.CreatePos("o");
        Brzozowski.CharClassRegex p = CharClass.CreatePos("p");
        Brzozowski.CharClassRegex q = CharClass.CreatePos("q");
        Brzozowski.CharClassRegex r = CharClass.CreatePos("r");
        Brzozowski.CharClassRegex s = CharClass.CreatePos("s");
        Brzozowski.CharClassRegex t = CharClass.CreatePos("t");
        Brzozowski.CharClassRegex u = CharClass.CreatePos("u");


        // (e|f|g)
        BrzozowskiRegex temp = BrzozowskiRegex.MakeOr(e, f, g);

        // (a|b|c)*d|(e|f|g)(e|f|g)*h)*
        var brGroup1 = BrzozowskiRegex.MakeKleene(BrzozowskiRegex.MakeOr(
            BrzozowskiRegex.MakeConcat(
                BrzozowskiRegex.MakeKleene(BrzozowskiRegex.MakeOr(a, b, c)),
                d
                ),
            BrzozowskiRegex.MakeConcat(temp, BrzozowskiRegex.MakeKleene(temp), h)
            ));

        // (j|k|l)m(n|o|p(q|r|s)*t)*
        var brGroup2 = BrzozowskiRegex.MakeConcat(
            BrzozowskiRegex.MakeOr(j, k, l),
            m,
            BrzozowskiRegex.MakeKleene(
                BrzozowskiRegex.MakeOr(
                    n,
                    o,
                    BrzozowskiRegex.MakeConcat(
                        p,
                        BrzozowskiRegex.MakeKleene(BrzozowskiRegex.MakeOr(
                            q,
                            r,
                            s)),
                        t)
            )));

        // ((a|b|c)*d|(e|f|g)(e|f|g)*h)* i (j|k|l)m(n|o|p(q|r|s)*t)* u
        return BrzozowskiRegex.MakeConcat(brGroup1, i, brGroup2, u);
    }
    private BrzozowskiRegex CreateBrzozowskiRegex_2() {

        Brzozowski.CharClassRegex a = CharClass.CreatePos("a");
        Brzozowski.CharClassRegex b = CharClass.CreatePos("b");

        var br_a_or_b = BrzozowskiRegex.MakeOr(a, b);
        var br_a_or_b_k = BrzozowskiRegex.MakeKleene(br_a_or_b);

        // (a|b)*a(a|b)(a|b)(a|b)(a|b)
        return BrzozowskiRegex.MakeConcat(br_a_or_b_k, a, br_a_or_b, br_a_or_b, br_a_or_b, br_a_or_b);
    }

    private ThompsonRegex CreateThompsonRegex_1() {

        Thompson.CharClassRegex a = CharClass.CreatePos("a");
        Thompson.CharClassRegex b = CharClass.CreatePos("b");
        Thompson.CharClassRegex c = CharClass.CreatePos("c");
        Thompson.CharClassRegex d = CharClass.CreatePos("d");
        Thompson.CharClassRegex e = CharClass.CreatePos("e");
        Thompson.CharClassRegex f = CharClass.CreatePos("f");
        Thompson.CharClassRegex g = CharClass.CreatePos("g");
        Thompson.CharClassRegex h = CharClass.CreatePos("h");
        Thompson.CharClassRegex i = CharClass.CreatePos("i");
        Thompson.CharClassRegex j = CharClass.CreatePos("j");
        Thompson.CharClassRegex k = CharClass.CreatePos("k");
        Thompson.CharClassRegex l = CharClass.CreatePos("l");
        Thompson.CharClassRegex m = CharClass.CreatePos("m");
        Thompson.CharClassRegex n = CharClass.CreatePos("n");
        Thompson.CharClassRegex o = CharClass.CreatePos("o");
        Thompson.CharClassRegex p = CharClass.CreatePos("p");
        Thompson.CharClassRegex q = CharClass.CreatePos("q");
        Thompson.CharClassRegex r = CharClass.CreatePos("r");
        Thompson.CharClassRegex s = CharClass.CreatePos("s");
        Thompson.CharClassRegex t = CharClass.CreatePos("t");
        Thompson.CharClassRegex u = CharClass.CreatePos("u");

        // (e|f|g)
        ThompsonRegex temp = ThompsonRegex.MakeOr(e, f, g);

        // (a|b|c)*d|(e|f|g)(e|f|g)*h)*
        var group1 = ThompsonRegex.MakeKleene(ThompsonRegex.MakeOr(
            ThompsonRegex.MakeConcat(
                ThompsonRegex.MakeKleene(ThompsonRegex.MakeOr(a, b, c)),
                d
                ),
            ThompsonRegex.MakeConcat(temp, ThompsonRegex.MakeKleene(temp), h)
            ));

        // (j|k|l)m(n|o|p(q|r|s)*t)*
        var group2 = ThompsonRegex.MakeConcat(
            ThompsonRegex.MakeOr(j, k, l),
            m,
            ThompsonRegex.MakeKleene(
                ThompsonRegex.MakeOr(
                    n,
                    o,
                    ThompsonRegex.MakeConcat(
                        p,
                        ThompsonRegex.MakeKleene(ThompsonRegex.MakeOr(
                            q,
                            r,
                            s)),
                        t)
            )));

        // ((a|b|c)*d|(e|f|g)(e|f|g)*h)* i (j|k|l)m(n|o|p(q|r|s)*t)* u
        return ThompsonRegex.MakeConcat(group1, i, group2, u);
    }
    private ThompsonRegex CreateThompsonRegex_2() {

        Thompson.CharClassRegex a = CharClass.CreatePos("a");
        Thompson.CharClassRegex b = CharClass.CreatePos("b");

        var a_or_b = ThompsonRegex.MakeOr(a, b);
        var a_or_b_k = ThompsonRegex.MakeKleene(a_or_b);

        // (a|b)*a(a|b)(a|b)(a|b)(a|b)
        return ThompsonRegex.MakeConcat(a_or_b_k, a, a_or_b, a_or_b, a_or_b, a_or_b);

    }


}
