namespace ONEbook.SpreadSheets.NPOI.Tests
{
    using System;
    using global::NPOI.SS.UserModel;
    using global::NPOI.SS.Util;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class NpoiCellTests
    {
        private ICell cell;
        private NpoiCell npoiCell;

        private ICell sourceCell;
        private NpoiCell sourceNpoiCell;

        [SetUp]
        public void SetUp()
        {
            cell = Substitute.For<ICell>();
            sourceCell = Substitute.For<ICell>();
            sourceNpoiCell = Substitute.For<NpoiCell>(sourceCell);
            npoiCell = Substitute.ForPartsOf<NpoiCell>(cell);
        }

        #region Copy From

        [Test]
        public void CopyFrom_SourceCellIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => npoiCell.CopyFrom(null));
        }

        [Test]
        public void CopyFrom_Always_CopyMergeInfoFromSourceCell()
        {
            // Act
            npoiCell.CopyFrom(sourceNpoiCell);

            // Assert
            npoiCell.Received().CopyMergeInfoFrom(sourceNpoiCell);
        }

        [Test]
        public void CopyFrom_Always_CopyCellType()
        {
            // Act
            npoiCell.CopyFrom(sourceNpoiCell);

            // Assert
            cell.Received().SetCellType(sourceNpoiCell.Cell.CellType);
        }

        [Test]
        public void CopyFrom_Always_CopyBooleanValue()
        {
            // Arrange
            sourceNpoiCell.Cell.CellType.Returns(CellType.Boolean);

            // Act
            npoiCell.CopyFrom(sourceNpoiCell);

            // Assert
            cell.Received().SetCellValue(sourceNpoiCell.Cell.BooleanCellValue);
        }

        [Test]
        public void CopyFrom_Always_CopyNumericValue()
        {
            // Arrange
            sourceNpoiCell.Cell.CellType.Returns(CellType.Numeric);

            // Act
            npoiCell.CopyFrom(sourceNpoiCell);

            // Assert
            cell.Received().SetCellValue(sourceNpoiCell.Cell.NumericCellValue);
        }

        [Test]
        public void CopyFrom_Always_CopyStringValue()
        {
            // Arrange
            sourceNpoiCell.Cell.CellType.Returns(CellType.String);

            // Act
            npoiCell.CopyFrom(sourceNpoiCell);

            // Assert
            cell.Received().SetCellValue(sourceNpoiCell.Cell.StringCellValue);
        }

        [Test]
        public void CopyFrom_Always_CopyErrorValue()
        {
            // Arrange
            sourceNpoiCell.Cell.CellType.Returns(CellType.Error);

            // Act
            npoiCell.CopyFrom(sourceNpoiCell);

            // Assert
            cell.Received().SetCellValue(sourceNpoiCell.Cell.ErrorCellValue);
        }

        [Test]
        public void CopyFrom_Always_CopyCellFormula()
        {
            // Arrange
            sourceNpoiCell.Cell.CellType.Returns(CellType.Formula);

            // Act
            npoiCell.CopyFrom(sourceNpoiCell);

            // Assert
            Assert.AreEqual(sourceNpoiCell.Cell.CellFormula, npoiCell.Cell.CellFormula);
        }

        #endregion Copy From

        #region

        [TestCase(1, 1)]
        [TestCase(2.1, 2.1)]
        [TestCase(0.5, 0.5)]
        [TestCase(0.5f, 0.5)]
        [TestCase(0.5d, 0.5)]
        [TestCase(20000000, 20000000)]
        public void SetCellValue_Always_ExecuteSetCellNumberValue(object value, double expectedSetValue)
        {
            // Act
            npoiCell.SetCellValue(value);

            // Assert
            cell.Received().SetCellValue(expectedSetValue);
        }

        [TestCase("1", "1")]
        [TestCase("34343434aadsdasd", "34343434aadsdasd")]
        public void SetCellValue_Always_ExecuteSetCellStringValue(object value, string expectedSetValue)
        {
            // Act
            npoiCell.SetCellValue(value);

            // Assert
            cell.Received().SetCellValue(expectedSetValue);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void SetCellValue_Always_ExecuteSetCellBoolValue(object value, bool expectedSetValue)
        {
            // Act
            npoiCell.SetCellValue(value);

            // Assert
            cell.Received().SetCellValue(expectedSetValue);
        }

        [Test]
        public void SetCellValue_ValueIsNull_SetZeroStringToCellValue()
        {
            // Act
            npoiCell.SetCellValue(null);

            // Assert
            cell.Received().SetCellValue("0");
        }

        [Test]
        public void SetCellValue_ValueIsDateTime_SetCellDateTimeValue()
        {
            var date = new DateTime(1989, 5, 28);

            // Act
            npoiCell.SetCellValue(date);

            // Assert
            cell.Received().SetCellValue(date);
        }

        [Test]
        public void SetCellValue_ValueTypeIsNotSupport_SetUndefinedValue()
        {
            var undefined = new object();

            // Act
            npoiCell.SetCellValue(undefined);

            // Assert
            cell.Received().SetCellValue("Undefined");
        }

        #endregion

        #region Get Cell Value

        [Test]
        public void GetCellValue_TypeIsBoolean_ReturnBooleanValue()
        {
            // Arrange
            cell.CellType.Returns(CellType.Boolean);
            cell.BooleanCellValue.Returns(true);

            // Act
            var actualResult = npoiCell.GetCellValue();

            // Assert
            Assert.IsTrue((bool)actualResult);
        }

        [Test]
        public void GetCellValue_TypeIsNumeric_ReturnNumericValue()
        {
            // Arrange
            var expectedResult = 2000;
            cell.CellType.Returns(CellType.Numeric);
            cell.NumericCellValue.Returns(expectedResult);

            // Act
            var actualResult = npoiCell.GetCellValue();

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCellValue_TypeIsString_ReturnStringValue()
        {
            // Arrange
            var expectedResult = "34343434";
            cell.CellType.Returns(CellType.String);
            cell.StringCellValue.Returns(expectedResult);

            // Act
            var actualResult = npoiCell.GetCellValue();

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCellValue_TypeIsError_ReturnErrorValue()
        {
            // Arrange
            byte expectedResult = 125;
            cell.CellType.Returns(CellType.Error);
            cell.ErrorCellValue.Returns(expectedResult);

            // Act
            var actualResult = npoiCell.GetCellValue();

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCellValue_TypeIsFormula_ReturnFormula()
        {
            // Arrange
            string expectedResult = "sum(a:b)";
            cell.CellType.Returns(CellType.Formula);
            cell.CellFormula.Returns(expectedResult);

            // Act
            var actualResult = npoiCell.GetCellValue();

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCellValue_TypeIsUnknown_ReturnEmpty()
        {
            // Arrange
            cell.CellType.Returns(CellType.Unknown);

            // Act
            var actualResult = npoiCell.GetCellValue();

            // Assert
            Assert.AreEqual(string.Empty, actualResult);
        }

        #endregion

        #region Set Center Horizontal Alignment

        [Test]
        public void SetCenterHorizontalAlignment_Always_SetCenterAlignment()
        {
            // Act
            npoiCell.SetCenterHorizontalAlignment();

            // Assert
            Assert.AreEqual(HorizontalAlignment.Center, cell.CellStyle.Alignment);
        }

        #endregion

        #region Copy Merge Info From

        [Test]
        public void CopyMergeInfoFrom_SourceCellIsNotNpoiCell_ThrowInvalidCastException()
        {
            Assert.Throws<InvalidCastException>(() => npoiCell.CopyMergeInfoFrom(Substitute.For<Cell>()));
        }

        [Test]
        public void CopyMergeInfoFrom_SourceCellIsMergedCell_Return()
        {
            // Arrange
            sourceCell.IsMergedCell.Returns(false);

            // Act
            npoiCell.CopyMergeInfoFrom(sourceNpoiCell);

            // Assert
            npoiCell.Cell.Sheet.DidNotReceiveWithAnyArgs().GetMergedRegion(1);
        }

        [Test]
        public void CopyMergeInfoFrom_CannotFindSuitableMerge_DontExecuteAddMergedRegion()
        {
            // Arrange
            sourceCell.IsMergedCell.Returns(true);

            // Act
            npoiCell.CopyMergeInfoFrom(sourceNpoiCell);

            // Assert
            npoiCell.Cell.Sheet.DidNotReceiveWithAnyArgs().AddMergedRegion(null);
        }

        [Test]
        public void CopyMergeInfoFrom_SourceCellHasBeenMerged_CopyMergeInfo()
        {
            // Arrange
            var colIndex = 0;
            var rowIndex = 1;

            var mergeRange = new CellRangeAddress(rowIndex, rowIndex, colIndex, 4);
            sourceCell.IsMergedCell.Returns(true);
            sourceCell.Sheet.NumMergedRegions.Returns(1);
            sourceCell.Sheet.GetMergedRegion(Arg.Any<int>()).Returns(mergeRange);

            sourceCell.ColumnIndex.Returns(colIndex);
            sourceCell.RowIndex.Returns(rowIndex);

            cell.ColumnIndex.Returns(colIndex);
            cell.RowIndex.Returns(rowIndex);

            // Act
            npoiCell.CopyMergeInfoFrom(sourceNpoiCell);

            // Assert
            npoiCell.Cell.Sheet.Received().AddMergedRegion(Arg.Is<CellRangeAddress>(
                actualMergeRange => actualMergeRange.FirstRow == rowIndex
                                && actualMergeRange.LastRow == rowIndex
                                && actualMergeRange.FirstColumn == colIndex
                                && actualMergeRange.LastColumn == 4));
        }

        #endregion
    }
}