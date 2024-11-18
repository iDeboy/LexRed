namespace LexRed.Brzozowski {
    public class CharClassRegex : BrzozowskiRegex {
        public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.CharClass;

        public override bool IsNullable => false;

        public CharClass CharClass { get; init; }

        public override CharClass[] Classy() {
            return [CharClass.CreateOposite(), CharClass];
        }

        public override BrzozowskiRegex Derive(char ch) {

            if (CharClass.Contains(ch)) return EmptyString;

            return EmptySet;
        }

        private protected override int CompareToCore(BrzozowskiRegex other) {

            if (other is not CharClassRegex charClassRegex) throw new ArgumentException("Compared regex is not CharClassRegex");

            return CharClass.CompareTo(charClassRegex.CharClass);
        }

        public static implicit operator CharClassRegex(CharClass charClass) {
            return new CharClassRegex { CharClass = charClass };
        }
    }
}
