namespace ONEbook.SpreadSheets.NPOI.Tests
{
    using System;
    using global::NPOI.SS.UserModel;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class NpoiRowTests
    {
        private Cell cell;
        private NpoiRow npoiRow;
        private IRow row;
        private IRow sourceRow;
        private NpoiRow npoiSourceRow;
        private Cell sourceCell;

        [SetUp]
        public void SetUp()
        {
            cell = Substitute.For<Cell>();
            row = Substitute.For<IRow>();
            npoiRow = Substitute.ForPartsOf<NpoiRow>(row);

            sourceCell = Substitute.For<Cell>();
            sourceRow = Substitute.For<IRow>();
            npoiSourceRow = Substitute.ForPartsOf<NpoiRow>(sourceRow);
        }

        #region Copy Cells From Row

        [Test]
        public void CopyCellsFrom_SourceRowIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => npoiRow.CopyCellsFrom(null));
        }

        [Test]
        public void CopyCellsFrom_Always_CopyFromCellsFromSourceRow()
        {
            // Arrange
            npoiRow.GetCell(Arg.Any<int>()).Returns(cell);
            npoiSourceRow.Row.FirstCellNum.Returns((short)0);
            npoiSourceRow.Row.LastCellNum.Returns((short)20);

            // Act
            npoiRow.CopyCellsFrom(npoiSourceRow);

            // Assert
            cell.ReceivedWithAnyArgs(20).CopyFrom(null);
        }

        [Test]
        public void CopyCellsFrom_AfterCopyCells_SetRowHeight()
        {
            // Arrange
            npoiSourceRow.Row.Height.Returns((short)100);

            // Act
            npoiRow.CopyCellsFrom(npoiSourceRow);

            // Assert
            Assert.AreEqual(npoiRow.Row.Height, 100);
        }

        #endregion Copy Cells From Row

        [Test]
        public void SetCellValue_Always_CallSetCellValue()
        {
            // Arrange
            var colIndex = 1;
            var value = "value";
            npoiRow.GetCell(Arg.Any<int>()).Returns(cell);

            // Act
            npoiRow.SetCellValue(colIndex, value);

            // Assert
            cell.Received().SetCellValue(value);
        }
    }
}