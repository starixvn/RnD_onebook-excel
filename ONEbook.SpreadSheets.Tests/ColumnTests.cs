namespace ONEbook.SpreadSheets.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ColumnTests
    {
        private static readonly object[] getColIndexTestCaseSource =
            {
                new object[] { Column.A, 0 },
                new object[] { Column.B, 1 },
                new object[] { Column.C, 2 },
                new object[] { Column.D, 3 },
                new object[] { Column.E, 4 },
                new object[] { Column.F, 5 },
                new object[] { Column.G, 6 },
                new object[] { Column.H, 7 },
                new object[] { Column.I, 8 },
                new object[] { Column.J, 9 },
                new object[] { Column.K, 10 },
                new object[] { Column.L, 11 },
                new object[] { Column.M, 12 },
                new object[] { Column.N, 13 },
                new object[] { Column.O, 14 },
                new object[] { Column.P, 15 },
                new object[] { Column.Q, 16 },
                new object[] { Column.R, 17 },
                new object[] { Column.S, 18 },
                new object[] { Column.T, 19 },
                new object[] { Column.U, 20 },
                new object[] { Column.V, 21 },
                new object[] { Column.W, 22 },
                new object[] { Column.X, 23 },
                new object[] { Column.Y, 24 },
                new object[] { Column.Z, 25 },
                new object[] { Column.AA, 26 },
                new object[] { Column.AB, 27 },
                new object[] { Column.AC, 28 },
                new object[] { Column.AD, 29 },
                new object[] { Column.AE, 30 },
                new object[] { Column.AF, 31 },
                new object[] { Column.AG, 32 },
                new object[] { Column.AH, 33 },
                new object[] { Column.AI, 34 },
                new object[] { Column.AJ, 35 },
                new object[] { Column.AK, 36 },
                new object[] { Column.AL, 37 },
                new object[] { Column.AM, 38 },
                new object[] { Column.AN, 39 },
                new object[] { Column.AO, 40 },
                new object[] { Column.AP, 41 },
                new object[] { Column.AQ, 42 },
                new object[] { Column.AR, 43 },
                new object[] { Column.AS, 44 },
                new object[] { Column.AT, 45 },
                new object[] { Column.AU, 46 },
                new object[] { Column.AV, 47 },
                new object[] { Column.AW, 48 },
                new object[] { Column.AX, 49 },
                new object[] { Column.AY, 50 },
                new object[] { Column.AZ, 51 }
            };

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetColIndex_ColumnNameIsNullOrWhiteSpace_ArgumentOutOfRangeException(string columnName)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Column.GetColIndex(columnName));
        }

        [TestCaseSource("getColIndexTestCaseSource")]
        public void GetColIndex_InputColumName_ReturnColumnIndex(string columnName, int expectedColumnIndex)
        {
            // Act
            var actualColumnIndex = Column.GetColIndex(columnName);

            // Assert
            Assert.AreEqual(expectedColumnIndex, actualColumnIndex);
        }
    }
}