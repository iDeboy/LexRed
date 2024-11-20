namespace LexRed.Thompson;

public enum ThompsonRegexKind {

    // Constant Expressions
    EmptySet,
    EmptyString,
    AnyChar,

    // Dynamic Expressions
    CharClass,
    Concatenation,
    Union,
    Kleene,

    // Constant Expression
    AllChars,
}
