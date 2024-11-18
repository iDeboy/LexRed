namespace LexRed.Brzozowski;
public enum BrzozowskiRegexKind {

    // Constant Expressions
    EmptySet,
    EmptyString,
    AnyChar,
    AllChars,

    // Dynamic Expressions
    CharClass,
    Concatenation,
    Disjunction,
    Conjunction,
    Complement,
    Kleene,

}
