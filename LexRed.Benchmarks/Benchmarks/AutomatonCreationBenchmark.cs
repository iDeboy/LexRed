using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using LexRed.Common;

using BrzozowskiRegex = LexRed.Brzozowski.BrzozowskiRegex;
using ThompsonRegex = LexRed.Thompson.ThompsonRegex;

namespace LexRed.Benchmarks.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser(false)]
[HideColumns(Column.InvocationCount, Column.UnrollFactor, Column.Job)]
[LongRunJob]
public class AutomatonCreationBenchmark {

    /*
     Regex to benchmarks:
           1.- ((a|b|c)*d|(e|f|g)(e|f|g)*h)*i(j|k|l)m(n|o|p(q|r|s)*t)*u
           2.- (a|b)*a(a|b)(a|b)(a|b)(a|b)
     */

    private const string FirstRegex = "((a|b|c)*d|(e|f|g)(e|f|g)*h)*i(j|k|l)m(n|o|p(q|r|s)*t)*u";
    private const string SecondRegex = "(a|b)*a(a|b)(a|b)(a|b)(a|b)";

    private BrzozowskiRegex _brzozowskiRegex_1 = default!;
    private BrzozowskiRegex _brzozowskiRegex_2 = default!;

    private ThompsonRegex _thompsonRegex_1 = default!;
    private ThompsonRegex _thompsonRegex_2 = default!;

    [Params(FirstRegex, SecondRegex)]
    public string Regex = null!;

    [GlobalSetup]
    public void Setup() {

        _brzozowskiRegex_1 = CreateBrzozowskiRegex_1();
        _brzozowskiRegex_2 = CreateBrzozowskiRegex_2();

        _thompsonRegex_1 = CreateThompsonRegex_1();
        _thompsonRegex_2 = CreateThompsonRegex_2();

    }

    [Benchmark]
    public BrzozowskiRegex CreateBrzozowskiRegex() {

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

        if (Regex is FirstRegex) {

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
            return BrzozowskiRegex.MakeConcat(brGroup1, t, brGroup2, u);

        }
        else if (Regex is SecondRegex) {

            var br_a_or_b = BrzozowskiRegex.MakeOr(a, b);
            var br_a_or_b_k = BrzozowskiRegex.MakeKleene(br_a_or_b);

            // (a|b)*a(a|b)(a|b)(a|b)(a|b)
            return BrzozowskiRegex.MakeConcat(br_a_or_b_k, a, br_a_or_b, br_a_or_b, br_a_or_b, br_a_or_b);

        }

        return BrzozowskiRegex.EmptySet;

    }

    //[Benchmark]
    public BrzozowskiRegex CreateBrzozowskiRegex_1() {

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
        return BrzozowskiRegex.MakeConcat(brGroup1, t, brGroup2, u);
    }

    //[Benchmark]
    public BrzozowskiRegex CreateBrzozowskiRegex_2() {

        Brzozowski.CharClassRegex a = CharClass.CreatePos("a");
        Brzozowski.CharClassRegex b = CharClass.CreatePos("b");

        var br_a_or_b = BrzozowskiRegex.MakeOr(a, b);
        var br_a_or_b_k = BrzozowskiRegex.MakeKleene(br_a_or_b);

        // (a|b)*a(a|b)(a|b)(a|b)(a|b)
        return BrzozowskiRegex.MakeConcat(br_a_or_b_k, a, br_a_or_b, br_a_or_b, br_a_or_b, br_a_or_b);
    }

    [Benchmark]
    public DFA CreateBrzozowskiDFA() {

        if (Regex is FirstRegex) {
            _brzozowskiRegex_1.MakeDFA();
        }
        else if (Regex is SecondRegex) {
            _brzozowskiRegex_2.MakeDFA();
        }

        return null!;
    }

    //[Benchmark]
    public DFA CreateBrzozowskiDFA_1() {
        return _brzozowskiRegex_1.MakeDFA();
    }

    //[Benchmark]
    public DFA CreateBrzozowskiDFA_2() {
        return _brzozowskiRegex_2.MakeDFA();
    }

    [Benchmark]
    public ThompsonRegex CreateThompsonRegex() {

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


        if (Regex is FirstRegex) {

            // (e|f|g)
            ThompsonRegex temp = ThompsonRegex.MakeOr(e, f, g);

            // (a|b|c)*d|(e|f|g)(e|f|g)*h)*
            var group1 = ThompsonRegex.MakeKleene(ThompsonRegex.MakeOr(
                ThompsonRegex.MakeConcat(
                    ThompsonRegex.MakeKleene(ThompsonRegex.MakeOr(a, f, g)),
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
        else if (Regex is SecondRegex) {
            var a_or_b = ThompsonRegex.MakeOr(a, b);
            var a_or_b_k = ThompsonRegex.MakeKleene(a_or_b);

            // (a|b)*a(a|b)(a|b)(a|b)(a|b)
            return ThompsonRegex.MakeConcat(a_or_b_k, a, a_or_b, a_or_b, a_or_b, a_or_b);

        }

        return null!;
    }

    //[Benchmark]
    public ThompsonRegex CreateThompsonRegex_1() {

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
                ThompsonRegex.MakeKleene(ThompsonRegex.MakeOr(a, f, g)),
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

    //[Benchmark]
    public ThompsonRegex CreateThompsonRegex_2() {

        Thompson.CharClassRegex a = CharClass.CreatePos("a");
        Thompson.CharClassRegex b = CharClass.CreatePos("b");

        var a_or_b = ThompsonRegex.MakeOr(a, b);
        var a_or_b_k = ThompsonRegex.MakeKleene(a_or_b);

        // (a|b)*a(a|b)(a|b)(a|b)(a|b)
        return ThompsonRegex.MakeConcat(a_or_b_k, a, a_or_b, a_or_b, a_or_b, a_or_b);

    }

    [Benchmark]
    public NFA CreateThompsonNFA() {

        if (Regex is FirstRegex) {
            _thompsonRegex_1.MakeNFA();
        }
        else if (Regex is SecondRegex) {
            _thompsonRegex_2.MakeNFA();
        }

        return null!;
    }

    //[Benchmark]
    public NFA CreateThompsonNFA_1() {
        return _thompsonRegex_1.MakeNFA();
    }

    //[Benchmark]
    public NFA CreateThompsonNFA_2() {
        return _thompsonRegex_2.MakeNFA();
    }

    [Benchmark]
    public DFA CreateThompsonDFA() {

        if (Regex is FirstRegex) {
            return _thompsonRegex_1.MakeNFA().ToDFA();
        }
        else if (Regex is SecondRegex) {
            return _thompsonRegex_2.MakeNFA().ToDFA();
        }

        return null!;
    }

    //[Benchmark]
    public DFA CreateThompsonDFA_1() {
        return _thompsonRegex_1.MakeNFA().ToDFA();
    }

    //[Benchmark]
    public DFA CreateThompsonDFA_2() {
        return _thompsonRegex_2.MakeNFA().ToDFA();
    }

}
