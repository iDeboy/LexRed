using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using LexRed.Common;

using BrzozowskiRegex = LexRed.Brzozowski.BrzozowskiRegex;
using ThompsonRegex = LexRed.Thompson.ThompsonRegex;

namespace LexRed.Benchmarks.Benchmarks;

[MemoryDiagnoser(false)]
[ShortRunJob]
[Config(typeof(Config))]
public class AutomatonCreationBenchmark {

    private class Config : ManualConfig {
        public Config() {
            AddJob(Job.Dry);
            // You can add custom tags per each method using Columns
            AddColumn(new StatesColumn());
            AddColumn(new TagColumn("Foo or Bar", name => name.Substring(0, 3)));
            AddColumn(new TagColumn("Number", name => name.Substring(3)));
        }
    }

    /*
     Regex to benchmarks:
           1.- ((a|b|c)*d|(e|f|g)(e|f|g)*h)* i (j|k|l)m(n|o|p(q|r|s)*t)* u
           2.- (a|b)*a(a|b)(a|b)(a|b)(a|b)
     */

    private BrzozowskiRegex _brzozowskiRegex_1 = default!;
    private BrzozowskiRegex _brzozowskiRegex_2 = default!;

    private DFA _brzozowskiDFA_1 = default!;
    private DFA _brzozowskiDFA_2 = default!;

    private ThompsonRegex _thompsonRegex_1 = default!;
    private ThompsonRegex _thompsonRegex_2 = default!;

    private NFA _thompsonNFA_1 = default!;
    private NFA _thompsonNFA_2 = default!;

    private DFA _thompsonDFA_1 = default!;
    private DFA _thompsonDFA_2 = default!;

    [GlobalSetup]
    public void Setup() {

        _brzozowskiRegex_1 = CreateBrzozowskiRegex_1();
        _brzozowskiRegex_2 = CreateBrzozowskiRegex_2();

        _brzozowskiDFA_1 = CreateBrzozowskiDFA_1();
        _brzozowskiDFA_2 = CreateBrzozowskiDFA_2();

        _thompsonRegex_1 = CreateThompsonRegex_1();
        _thompsonRegex_2 = CreateThompsonRegex_2();

        _thompsonNFA_1 = CreateThompsonNFA_1();
        _thompsonNFA_2 = CreateThompsonNFA_2();

        _thompsonDFA_1 = CreateThompsonDFA_1();
        _thompsonDFA_2 = CreateThompsonDFA_1();

        return;
        var aCC = CharClass.CreatePos("a");
        var bCC = CharClass.CreatePos("b");
        var cCC = CharClass.CreatePos("c");
        var dCC = CharClass.CreatePos("d");
        var eCC = CharClass.CreatePos("e");
        var fCC = CharClass.CreatePos("f");
        var gCC = CharClass.CreatePos("g");
        var hCC = CharClass.CreatePos("h");
        var iCC = CharClass.CreatePos("i");
        var jCC = CharClass.CreatePos("j");
        var kCC = CharClass.CreatePos("k");
        var lCC = CharClass.CreatePos("l");
        var mCC = CharClass.CreatePos("m");
        var nCC = CharClass.CreatePos("n");
        var oCC = CharClass.CreatePos("o");
        var pCC = CharClass.CreatePos("p");
        var qCC = CharClass.CreatePos("q");
        var rCC = CharClass.CreatePos("r");
        var sCC = CharClass.CreatePos("s");
        var tCC = CharClass.CreatePos("t");
        var uCC = CharClass.CreatePos("u");

        Brzozowski.CharClassRegex brA = aCC;
        Brzozowski.CharClassRegex brB = bCC;
        Brzozowski.CharClassRegex brC = cCC;
        Brzozowski.CharClassRegex brD = dCC;
        Brzozowski.CharClassRegex brE = eCC;
        Brzozowski.CharClassRegex brF = fCC;
        Brzozowski.CharClassRegex brG = gCC;
        Brzozowski.CharClassRegex brH = hCC;
        Brzozowski.CharClassRegex brI = iCC;
        Brzozowski.CharClassRegex brJ = jCC;
        Brzozowski.CharClassRegex brK = kCC;
        Brzozowski.CharClassRegex brL = lCC;
        Brzozowski.CharClassRegex brM = mCC;
        Brzozowski.CharClassRegex brN = nCC;
        Brzozowski.CharClassRegex brO = oCC;
        Brzozowski.CharClassRegex brP = pCC;
        Brzozowski.CharClassRegex brQ = qCC;
        Brzozowski.CharClassRegex brR = rCC;
        Brzozowski.CharClassRegex brS = sCC;
        Brzozowski.CharClassRegex brT = tCC;
        Brzozowski.CharClassRegex brU = uCC;

        // (e|f|g)
        BrzozowskiRegex brTemp = BrzozowskiRegex.MakeOr(brE, brF, brG);

        // (a|b|c)*d|(e|f|g)(e|f|g)*h)*
        var brGroup1 = BrzozowskiRegex.MakeKleene(BrzozowskiRegex.MakeOr(
            BrzozowskiRegex.MakeConcat(
                BrzozowskiRegex.MakeKleene(BrzozowskiRegex.MakeOr(brA, brB, brC)),
                brD
                ),
            BrzozowskiRegex.MakeConcat(brTemp, BrzozowskiRegex.MakeKleene(brTemp), brH)
            ));

        // (j|k|l)m(n|o|p(q|r|s)*t)*
        var brGroup2 = BrzozowskiRegex.MakeConcat(
            BrzozowskiRegex.MakeOr(brJ, brK, brL),
            brM,
            BrzozowskiRegex.MakeKleene(
                BrzozowskiRegex.MakeOr(
                    brN,
                    brO,
                    BrzozowskiRegex.MakeConcat(
                        brP,
                        BrzozowskiRegex.MakeKleene(BrzozowskiRegex.MakeOr(
                            brQ,
                            brR,
                            brS)),
                        brT)
            )));

        // ((a|b|c)*d|(e|f|g)(e|f|g)*h)* i (j|k|l)m(n|o|p(q|r|s)*t)* u
        _brzozowskiRegex_1 = BrzozowskiRegex.MakeConcat(brGroup1, brI, brGroup2, brU);

        var br_a_or_b = BrzozowskiRegex.MakeOr(brA, brB);
        var br_a_or_b_k = BrzozowskiRegex.MakeKleene(br_a_or_b);

        // (a|b)*a(a|b)(a|b)(a|b)(a|b)
        _brzozowskiRegex_2 = BrzozowskiRegex.MakeConcat(br_a_or_b_k, brA, br_a_or_b, br_a_or_b, br_a_or_b, br_a_or_b);

        Thompson.CharClassRegex thA = aCC;
        Thompson.CharClassRegex thB = bCC;
        Thompson.CharClassRegex thC = cCC;
        Thompson.CharClassRegex thD = dCC;
        Thompson.CharClassRegex thE = eCC;
        Thompson.CharClassRegex thF = fCC;
        Thompson.CharClassRegex thG = gCC;
        Thompson.CharClassRegex thH = hCC;
        Thompson.CharClassRegex thI = iCC;
        Thompson.CharClassRegex thJ = jCC;
        Thompson.CharClassRegex thK = kCC;
        Thompson.CharClassRegex thL = lCC;
        Thompson.CharClassRegex thM = mCC;
        Thompson.CharClassRegex thN = nCC;
        Thompson.CharClassRegex thO = oCC;
        Thompson.CharClassRegex thP = pCC;
        Thompson.CharClassRegex thQ = qCC;
        Thompson.CharClassRegex thR = rCC;
        Thompson.CharClassRegex thS = sCC;
        Thompson.CharClassRegex thT = tCC;
        Thompson.CharClassRegex thU = uCC;

        // (e|f|g)
        ThompsonRegex thTemp = ThompsonRegex.MakeOr(thE, thF, thG);

        // (a|b|c)*d|(e|f|g)(e|f|g)*h)*
        var thGroup1 = ThompsonRegex.MakeKleene(ThompsonRegex.MakeOr(
            ThompsonRegex.MakeConcat(
                ThompsonRegex.MakeKleene(ThompsonRegex.MakeOr(thA, thF, thG)),
                thD
                ),
            ThompsonRegex.MakeConcat(thTemp, ThompsonRegex.MakeKleene(thTemp), thH)
            ));

        // (j|k|l)m(n|o|p(q|r|s)*t)*
        var thGroup2 = ThompsonRegex.MakeConcat(
            ThompsonRegex.MakeOr(thJ, thK, thL),
            thM,
            ThompsonRegex.MakeKleene(
                ThompsonRegex.MakeOr(
                    thN,
                    thO,
                    ThompsonRegex.MakeConcat(
                        thP,
                        ThompsonRegex.MakeKleene(ThompsonRegex.MakeOr(
                            thQ,
                            thR,
                            thS)),
                        thT)
            )));

        // ((a|b|c)*d|(e|f|g)(e|f|g)*h)* i (j|k|l)m(n|o|p(q|r|s)*t)* u
        _thompsonRegex_1 = ThompsonRegex.MakeConcat(thGroup1, thI, thGroup2, thU);

        var th_a_or_b = ThompsonRegex.MakeOr(thA, thB);
        var th_a_or_b_k = ThompsonRegex.MakeKleene(th_a_or_b);

        // (a|b)*a(a|b)(a|b)(a|b)(a|b)
        _thompsonRegex_2 = ThompsonRegex.MakeConcat(th_a_or_b_k, thA, th_a_or_b, th_a_or_b, th_a_or_b, th_a_or_b);

    }

    [Benchmark]
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

    [Benchmark]
    public BrzozowskiRegex CreateBrzozowskiRegex_2() {

        Brzozowski.CharClassRegex a = CharClass.CreatePos("a");
        Brzozowski.CharClassRegex b = CharClass.CreatePos("b");

        var br_a_or_b = BrzozowskiRegex.MakeOr(a, b);
        var br_a_or_b_k = BrzozowskiRegex.MakeKleene(br_a_or_b);

        // (a|b)*a(a|b)(a|b)(a|b)(a|b)
        return BrzozowskiRegex.MakeConcat(br_a_or_b_k, a, br_a_or_b, br_a_or_b, br_a_or_b, br_a_or_b);
    }

    [Benchmark]
    public DFA CreateBrzozowskiDFA_1() {
        return _brzozowskiRegex_1.MakeDFA();
    }

    [Benchmark]
    public DFA CreateBrzozowskiDFA_2() {


        return _brzozowskiRegex_2.MakeDFA();
    }

    [Benchmark]
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

    [Benchmark]
    public ThompsonRegex CreateThompsonRegex_2() {

        Thompson.CharClassRegex a = CharClass.CreatePos("a");
        Thompson.CharClassRegex b = CharClass.CreatePos("b");

        var a_or_b = ThompsonRegex.MakeOr(a, b);
        var a_or_b_k = ThompsonRegex.MakeKleene(a_or_b);

        // (a|b)*a(a|b)(a|b)(a|b)(a|b)
        return ThompsonRegex.MakeConcat(a_or_b_k, a, a_or_b, a_or_b, a_or_b, a_or_b);

    }

    [Benchmark]
    public NFA CreateThompsonNFA_1() {
        return _thompsonRegex_1.MakeNFA();
    }

    [Benchmark]
    public NFA CreateThompsonNFA_2() {
        return _thompsonRegex_2.MakeNFA();
    }

    [Benchmark]
    public DFA CreateThompsonDFA_1() {
        return _thompsonRegex_1.MakeNFA().ToDFA();
    }

    [Benchmark]
    public DFA CreateThompsonDFA_2() {
        return _thompsonRegex_2.MakeNFA().ToDFA();
    }

}
