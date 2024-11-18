using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace LexRed.Brzozowski;
public class DisjunctionRegex : BrzozowskiRegex {
    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.Disjunction;

    public override bool IsNullable { get; }

    internal readonly BrzozowskiRegex[] _body;

    internal DisjunctionRegex(params Span<BrzozowskiRegex> body) {

        Debug.Assert(!body.IsEmpty);

        var distinct = body.Distinct();
        IsNullable = distinct[0].IsNullable;

        for (int i = 1; i < distinct.Length && !IsNullable; ++i) {

            IsNullable = IsNullable || distinct[i].IsNullable;
        }

        _body = distinct.ToArray();

    }

    public override CharClass[] Classy() {

        var pool = ArrayPool<CharClass[]>.Shared.Rent(_body.Length);

        CharClass[] result = _body[0].Classy();

        for (int i = 1; i < _body.Length; ++i) {
            result = _body[i].Classy();
        }

        ArrayPool<CharClass[]>.Shared.Return(pool);
    }

    public override BrzozowskiRegex Derive(char ch) {
        throw new NotImplementedException();
    }

    private protected override int CompareToCore(BrzozowskiRegex other) {

        if (other is not DisjunctionRegex disjuctionRegex) throw new ArgumentException("Compared regex is not CharClassRegex");

        if (_body.Length > disjuctionRegex._body.Length) return -1;

        if (_body.Length < disjuctionRegex._body.Length) return 1;

        int length = _body.Length;

        for (int i = 0; i < length; ++i) {

            var compare = _body[i].CompareTo(disjuctionRegex._body[i]);

            if (compare is not 0) return compare;

        }

        return 0;
    }
}
