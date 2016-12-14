namespace ONEbook.SpreadSheets
{
    using System;
    using System.Collections.Concurrent;

    public static class Column
    {
        public const string A = "A";
        public const string B = "B";
        public const string C = "C";
        public const string D = "D";
        public const string E = "E";
        public const string F = "F";
        public const string G = "G";
        public const string H = "H";
        public const string I = "I";
        public const string J = "J";
        public const string K = "K";
        public const string L = "L";
        public const string M = "M";
        public const string N = "N";
        public const string O = "O";
        public const string P = "P";
        public const string Q = "Q";
        public const string R = "R";
        public const string S = "S";
        public const string T = "T";
        public const string U = "U";
        public const string V = "V";
        public const string W = "W";
        public const string X = "X";
        public const string Y = "Y";
        public const string Z = "Z";
        public const string AA = A + A;
        public const string AB = A + B;
        public const string AC = A + C;
        public const string AD = A + D;
        public const string AE = A + E;
        public const string AF = A + F;

        private static readonly ConcurrentDictionary<string, int> ColumnNameToIndexMapping = new ConcurrentDictionary<string, int>();

        public static int GetColIndex(string colName)
        {
            if (string.IsNullOrWhiteSpace(colName))
            {
                throw new ArgumentOutOfRangeException(nameof(colName), "Column Name cannot be null or white space.");
            }

            if (ColumnNameToIndexMapping.ContainsKey(colName))
            {
                return ColumnNameToIndexMapping[colName];
            }

            int colIndex = -1;
            int pow = 1;
            for (int i = colName.Length - 1; i >= 0; i--)
            {
                colIndex += (colName[i] - 'A' + 1) * pow;
                pow *= 26;
            }

            if (!ColumnNameToIndexMapping.ContainsKey(colName))
            {
                ColumnNameToIndexMapping.TryAdd(colName, colIndex);
            }

            return colIndex;
        }
    }
}