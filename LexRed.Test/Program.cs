using LexRed.Brzozowski;

// TODO: Make real tests with a specielized library
CharClass a = CharClass.CreatePos("a");
CharClass b = CharClass.CreatePos("b");
CharClass c = CharClass.CreatePos("c");

CharClassRegex aRe = a;
CharClassRegex bRe = b;
CharClassRegex cRe = c;

var ab = BrzozowskiRegex.MakeConcat(aRe, bRe);
var ac = BrzozowskiRegex.MakeConcat(aRe, cRe);

var a_or_b = BrzozowskiRegex.MakeOr(aRe, aRe, bRe);

var or = BrzozowskiRegex.MakeOr(ab, ac, BrzozowskiRegex.EmptyString, a_or_b);

var dfa = or.MakeDFA();

return 0;