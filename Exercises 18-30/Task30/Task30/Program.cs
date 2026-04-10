using System;
using System.Globalization;

namespace Task30MyString
{
    public sealed class MyString
    {
        private readonly char[] value;
        private readonly int length;

        public MyString()
        {
            value = new char[0];
            length = 0;
        }

        public MyString(char[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            this.value = new char[value.Length];
            Array.Copy(value, this.value, value.Length);
            length = value.Length;
        }

        public MyString(MyString original)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            value = new char[original.length];
            Array.Copy(original.value, value, original.length);
            length = original.length;
        }

        public int Length()
        {
            return length;
        }

        public char CharAt(int index)
        {
            if (index < 0 || index >= length)
                throw new IndexOutOfRangeException("Индекс выходит за границы строки.");

            return value[index];
        }

        public MyString Substring(int beginIndex, int endIndex)
        {
            if (beginIndex < 0 || endIndex < 0 || beginIndex > endIndex || endIndex > length)
                throw new IndexOutOfRangeException("Некорректные границы подстроки.");

            char[] arr = new char[endIndex - beginIndex];
            Array.Copy(value, beginIndex, arr, 0, arr.Length);
            return new MyString(arr);
        }

        public MyString Concat(MyString str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            char[] arr = new char[length + str.length];
            Array.Copy(value, 0, arr, 0, length);
            Array.Copy(str.value, 0, arr, length, str.length);
            return new MyString(arr);
        }

        public bool Equals(MyString str)
        {
            if (ReferenceEquals(str, null))
                return false;

            if (length != str.length)
                return false;

            for (int i = 0; i < length; i++)
            {
                if (value[i] != str.value[i])
                    return false;
            }

            return true;
        }

        public bool EqualsIgnoreCase(MyString str)
        {
            if (ReferenceEquals(str, null))
                return false;

            if (length != str.length)
                return false;

            for (int i = 0; i < length; i++)
            {
                if (char.ToLowerInvariant(value[i]) != char.ToLowerInvariant(str.value[i]))
                    return false;
            }

            return true;
        }

        public MyString ToLowerCase()
        {
            char[] arr = new char[length];

            for (int i = 0; i < length; i++)
                arr[i] = char.ToLowerInvariant(value[i]);

            return new MyString(arr);
        }

        public MyString ToUpperCase()
        {
            char[] arr = new char[length];

            for (int i = 0; i < length; i++)
                arr[i] = char.ToUpperInvariant(value[i]);

            return new MyString(arr);
        }

        public MyString Trim()
        {
            int start = 0;
            int end = length - 1;

            while (start < length && value[start] == ' ')
                start++;

            while (end >= start && value[end] == ' ')
                end--;

            if (start > end)
                return new MyString();

            return Substring(start, end + 1);
        }

        public MyString Replace(char oldChar, char newChar)
        {
            char[] arr = new char[length];

            for (int i = 0; i < length; i++)
                arr[i] = value[i] == oldChar ? newChar : value[i];

            return new MyString(arr);
        }

        public bool Contains(MyString substr)
        {
            return IndexOf(substr) >= 0;
        }

        public int IndexOf(MyString substr)
        {
            if (substr == null)
                throw new ArgumentNullException(nameof(substr));

            if (substr.length == 0)
                return 0;

            if (substr.length > length)
                return -1;

            for (int i = 0; i <= length - substr.length; i++)
            {
                bool ok = true;

                for (int j = 0; j < substr.length; j++)
                {
                    if (value[i + j] != substr.value[j])
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok)
                    return i;
            }

            return -1;
        }

        public MyString[] Split(char delimiter)
        {
            int parts = 1;
            for (int i = 0; i < length; i++)
            {
                if (value[i] == delimiter)
                    parts++;
            }

            MyString[] result = new MyString[parts];
            int start = 0;
            int index = 0;

            for (int i = 0; i < length; i++)
            {
                if (value[i] == delimiter)
                {
                    result[index++] = Substring(start, i);
                    start = i + 1;
                }
            }

            result[index] = Substring(start, length);
            return result;
        }

        public bool StartsWith(MyString prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));

            if (prefix.length > length)
                return false;

            for (int i = 0; i < prefix.length; i++)
            {
                if (value[i] != prefix.value[i])
                    return false;
            }

            return true;
        }

        public bool EndsWith(MyString suffix)
        {
            if (suffix == null)
                throw new ArgumentNullException(nameof(suffix));

            if (suffix.length > length)
                return false;

            int shift = length - suffix.length;

            for (int i = 0; i < suffix.length; i++)
            {
                if (value[shift + i] != suffix.value[i])
                    return false;
            }

            return true;
        }

        public MyString Reverse()
        {
            char[] arr = new char[length];

            for (int i = 0; i < length; i++)
                arr[i] = value[length - 1 - i];

            return new MyString(arr);
        }

        public static MyString ValueOf(int i)
        {
            return new MyString(i.ToString(CultureInfo.InvariantCulture).ToCharArray());
        }

        public static MyString ValueOf(double d)
        {
            return new MyString(d.ToString(CultureInfo.InvariantCulture).ToCharArray());
        }

        public static MyString ValueOf(bool b)
        {
            return new MyString((b ? "true" : "false").ToCharArray());
        }

        public override string ToString()
        {
            return new string(value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MyString);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                for (int i = 0; i < length; i++)
                    hash = hash * 31 + value[i];
                return hash;
            }
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            var s1 = new MyString("  RUSSIAN GOIDA  ".ToCharArray());
            var s2 = new MyString("russian goida".ToCharArray());
            var s3 = new MyString("Goida".ToCharArray());

            Console.WriteLine("Исходная строка: [" + s1 + "]");
            Console.WriteLine("Length = " + s1.Length());
            Console.WriteLine("CharAt(2) = " + s1.CharAt(2));
            Console.WriteLine("Trim = [" + s1.Trim() + "]");
            Console.WriteLine("ToLowerCase = " + s1.ToLowerCase());
            Console.WriteLine("ToUpperCase = " + s1.ToUpperCase());
            Console.WriteLine("Replace(l, x) = " + s1.Replace('l', 'x'));
            Console.WriteLine("Contains(World) = " + s1.Contains(s3));
            Console.WriteLine("IndexOf(World) = " + s1.IndexOf(s3));
            Console.WriteLine("Substring(2, 7) = " + s1.Substring(2, 7));
            Console.WriteLine("Concat = " + s1.Trim().Concat(new MyString("!!!".ToCharArray())));
            Console.WriteLine("Equals = " + s1.Equals(s2));
            Console.WriteLine("EqualsIgnoreCase = " + s1.Trim().EqualsIgnoreCase(s2));
            Console.WriteLine("StartsWith(Hello) = " + s1.Trim().StartsWith(new MyString("Hello".ToCharArray())));
            Console.WriteLine("EndsWith(World) = " + s1.Trim().EndsWith(new MyString("World".ToCharArray())));
            Console.WriteLine("Reverse = " + s1.Trim().Reverse());

            MyString[] parts = new MyString("one,two,,four".ToCharArray()).Split(',');
            Console.WriteLine("Split:");
            for (int i = 0; i < parts.Length; i++)
                Console.WriteLine("[" + parts[i] + "]");

            Console.WriteLine("ValueOf(int) = " + MyString.ValueOf(12345));
            Console.WriteLine("ValueOf(double) = " + MyString.ValueOf(3.14159));
            Console.WriteLine("ValueOf(bool) = " + MyString.ValueOf(true));
        }
    }
}