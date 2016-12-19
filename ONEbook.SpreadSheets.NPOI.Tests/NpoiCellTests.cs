namespace ONEbook.SpreadSheets.NPOI.Tests
{
    using System;
    using global::NPOI.SS.UserModel;
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

        public void SetCellValue_Always_ExecuteSetCellValue()
        {
        }

        #endregion
    }
}