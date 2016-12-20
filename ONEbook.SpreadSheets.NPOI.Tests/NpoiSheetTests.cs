namespace ONEbook.SpreadSheets.NPOI.Tests
{
    using global::NPOI.SS.UserModel;
    using global::NPOI.SS.Util;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class NpoiSheetTests
    {
        private ISheet sheet;
        private NpoiSheet npoiSheet;

        [SetUp]
        public void SetUp()
        {
            sheet = Substitute.For<ISheet>();
            npoiSheet = Substitute.ForPartsOf<NpoiSheet>(sheet);
        }

        [Test]
        public void AppendRows_NewRowIndexIsLowerThanLastRow_DontExecuteCreateRow()
        {
            // Arrange
            sheet.LastRowNum.Returns(20);

            // Act
            npoiSheet.AppendRows(10);

            // Assert
            sheet.DidNotReceiveWithAnyArgs().CreateRow(0);
        }

        [Test]
        public void AppendRows_Always_ExecuteCreateRow()
        {
            // Arrange
            sheet.LastRowNum.Returns(0);

            // Act
            npoiSheet.AppendRows(10);

            // Assert
            sheet.ReceivedWithAnyArgs(10).CreateRow(0);
        }

        [Test]
        public void CreateFreezePanel_Always_ExecuteCreateFreezePane()
        {
            // Act
            npoiSheet.CreateFreezePanel(10, 10);

            // Assert
            sheet.Received().CreateFreezePane(10, 10);
        }

        [Test]
        public void RemoveRow_Always_ExecuteRemoveRow()
        {
            // Arrange
            var row = Substitute.For<NpoiRow>(Substitute.For<IRow>());

            // Act
            npoiSheet.RemoveRow(row);

            // Assert
            sheet.Received().RemoveRow(row.Row);
        }

        [Test]
        public void SetCellRangeValue_Always_ExecuteAppendRows()
        {
            // Act
            npoiSheet.SetCellRangeValue(1, 1, 15, 1, "value");

            // Assert
            npoiSheet.Received().AppendRows(15);
        }

        [Test]
        public void SetCellRangeValue_Always_ExecuteAddMergedRegion()
        {
            // Act
            npoiSheet.SetCellRangeValue(1, 1, 15, 1, "value");

            // Assert
            sheet.Received().AddMergedRegion(Arg.Is<CellRangeAddress>(
                    range => range.FirstRow == 1
                            && range.FirstColumn == 1
                            && range.LastRow == 15
                            && range.LastColumn == 1));
        }

        [Test]
        public void SetCellRangeValue_Always_ExecuteSetCellValue()
        {
            // Act
            npoiSheet.SetCellRangeValue(1, 1, 15, 1, "value");

            // Assert
            npoiSheet.Received().SetCellValue(1, 1, "value");
        }
    }
}