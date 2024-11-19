namespace LexRed.Brzozowski;
public enum BrzozowskiRegexKind {

    // Constant Expressions
    EmptySet,
    EmptyString,
    AnyChar,

    // Dynamic Expressions
    CharClass,
    Concatenation,
    Disjunction,
    Conjunction,
    Complement,
    Kleene,

    // Constant Expression
    AllChars,
}
