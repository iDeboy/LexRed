﻿using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace LexRed.Common;
public static class Extensions {

    public static char GetNextChar(this char ch)
        => ch.GetNextChar(1);

    public static char GetNextChar(this char ch, int i)
        => (char)(ch + i);

    public static char GetPrevChar(this char ch)
        => ch.GetPrevChar(1);

    public static char GetPrevChar(this char ch, int i)
        => (char)(ch - i);

    public static StringBuilder AppendIf(this StringBuilder sb, bool condition, char value) {

        if (condition) sb.Append(value);

        return sb;
    }

    public static StringBuilder AppendIf(this StringBuilder sb, bool condition, ReadOnlySpan<char> value) {

        if (condition) sb.Append(value);

        return sb;
    }

    public static bool Contains<T>(this Span<T> source, ReadOnlySpan<T> span)
        => Contains((ReadOnlySpan<T>)source, span);

    public static bool Contains<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> span) {

        if (span.IsEmpty) return true;
        if (source.Length < span.Length) return false;

        for (int i = 0; i <= source.Length - span.Length; i++) {

            if (source.Slice(i, span.Length).SequenceEqual(span))
                return true;

        }

        return false;
    }

    public static Span<T> Distinct<T>(this Span<T> span) {

        if (span.IsEmpty || span.Length is 1) return span;

        span.Sort();

        // Quitar duplicados
        int j = 1;

        if (typeof(T).IsValueType) {

            for (int i = 1; i < span.Length; ++i) {

                if (Comparer<T>.Default.Compare(span[i], span[i - 1]) != 0)
                    span[j++] = span[i];

            }
        }
        else {

            var defaultComparer = Comparer<T>.Default;

            for (int i = 1; i < span.Length; ++i) {

                if (defaultComparer.Compare(span[i], span[i - 1]) != 0)
                    span[j++] = span[i];

            }

        }

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            span[j..].Clear();

        return span[..j];
    }

    public static T[] Intersect<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second) where T : IEquatable<T> {

        var arrayToReturn = ArrayPool<T>.Shared.Rent(Math.Min(first.Length, second.Length));

        Span<T> span = arrayToReturn;
        int i = 0;

        foreach (ref readonly var item in first) {

            if (second.Contains(item)) span[i++] = item;

        }

        var intersection = span[..i].Distinct().ToArray();

        ArrayPool<T>.Shared.Return(arrayToReturn, RuntimeHelpers.IsReferenceOrContainsReferences<T>());

        return intersection;
    }

    public static T[] Except<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second) where T : IEquatable<T> {

        var arrayToReturn = ArrayPool<T>.Shared.Rent(first.Length);

        Span<T> span = arrayToReturn;
        int i = 0;

        foreach (ref readonly var item in first) {

            if (!second.Contains(item)) span[i++] = item;

        }

        var except = span[..i].Distinct().ToArray();

        ArrayPool<T>.Shared.Return(arrayToReturn, RuntimeHelpers.IsReferenceOrContainsReferences<T>());

        return except;
    }

    public static T[] Union<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second) where T : IEquatable<T> {

        var arrayToReturn = ArrayPool<T>.Shared.Rent(first.Length + second.Length);

        Span<T> span = arrayToReturn;
        int i = 0;

        first.CopyTo(span);
        i += first.Length;
        second.CopyTo(span[first.Length..]);
        i += second.Length;

        var union = span[..i].Distinct().ToArray();

        ArrayPool<T>.Shared.Return(arrayToReturn, RuntimeHelpers.IsReferenceOrContainsReferences<T>());

        return union;
    }

    public static CharClass[] AllPairs(this ReadOnlySpan<CharClass> charClasses, ReadOnlySpan<CharClass> otherCharClasses) {

        int length = charClasses.Length * otherCharClasses.Length;

        var pool = ArrayPool<CharClass>.Shared.Rent(length);

        Span<CharClass> span = pool.AsSpan(0, length);

        for (int j = 0; j < charClasses.Length; ++j) {

            CharClass charClass = charClasses[j];

            for (int i = 0; i < otherCharClasses.Length; ++i) {

                CharClass otherCharClass = otherCharClasses[i];

                span[i + otherCharClasses.Length * j] = charClass.Intersection(otherCharClass);
            }

        }

        var result = span.Distinct().TrimStart(CharClass.None).ToArray();

        ArrayPool<CharClass>.Shared.Return(pool);

        return result;
    }

}
