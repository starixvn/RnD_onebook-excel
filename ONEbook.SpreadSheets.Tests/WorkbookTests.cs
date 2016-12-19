namespace ONEbook.SpreadSheets.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class WorkbookTests
    {
        private Workbook<object> workbook;
        private TemplateInfo templateInfo;
        private SpreadSheet<object> spreadSheet;
        private Sheet sheet;
        private Row row;
        private Cell cell;
        private CompareLogic compareLogic;

        [SetUp]
        public void SetUp()
        {
            workbook = Substitute.ForPartsOf<Workbook<object>>();
            templateInfo = new TemplateInfo("template.xls");
            spreadSheet = new SpreadSheet<object>();
            sheet = Substitute.For<Sheet>();
            row = Substitute.For<Row>();
            cell = Substitute.For<Cell>();
            compareLogic = new CompareLogic();

            workbook.TemplateInfo.Returns(templateInfo);
            workbook.SpreadSheet.Returns(spreadSheet);
        }

        [Test]
        public void LoadTemplateSheet_Always_ExecuteGetSheetAt()
        {
            // Arrange
            templateInfo.Sheet(1);

            // Act
            workbook.LoadTemplateSheet();

            // Assert
            workbook.Received().GetSheetAt(0);
        }

        #region Load templates

        [Test]
        public void LoadTemplateSheet_Always_ExecuteGetOddRowTemplate()
        {
            // Arrange
            templateInfo.Body(bodyStartIndex: 4, bodyEndIndex: 6);
            workbook.GetSheetAt(Arg.Any<int>()).Returns(sheet);

            // Act
            workbook.LoadTemplateSheet();

            // Assert
            sheet.Received().GetRow(templateInfo.BodyStartIndex);
        }

        [Test]
        public void LoadTemplateSheet_Always_ExecuteGetEvenRowTemplate()
        {
            // Arrange
            templateInfo.Body(bodyStartIndex: 4, bodyEndIndex: 6);
            workbook.GetSheetAt(Arg.Any<int>()).Returns(sheet);

            // Act
            workbook.LoadTemplateSheet();

            // Assert
            sheet.Received().GetRow(templateInfo.BodyStartIndex + 1);
        }

        [Test]
        public void LoadTemplateSheet_Always_SetFooterRowsTemplate()
        {
            // Arrange
            templateInfo.Body(bodyStartIndex: 4, bodyEndIndex: 6);
            templateInfo.Footer(footerEndIndex: 7);
            workbook.GetSheetAt(Arg.Any<int>()).Returns(sheet);

            // Act
            workbook.LoadTemplateSheet();

            // Assert
            Assert.AreEqual(2, sheet.FooterRowsTemplate.Count());
        }

        #endregion Load templates

        #region Fill Title

        [Test]
        public void FillTitle_SheetIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => workbook.FillTitle(null));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void FillTitle_TitleStartIndexIsNegativeOrZero_NotExecuteSetCellValue(int titleIndex)
        {
            // Arrange
            templateInfo.Title(titleIndex);
            spreadSheet.Title("Title Text");

            // Act
            workbook.FillTitle(sheet);

            // Assert
            sheet.DidNotReceive().SetCellValue(templateInfo.TitleStartIndex, 0, spreadSheet.TitleText);
        }

        [Test]
        public void FillTitle_TitleStartIndexIsGreaterThanZero_ExecuteSetCellValue()
        {
            // Arrange
            templateInfo.Title(1);
            spreadSheet.Title("Title Text");

            // Act
            workbook.FillTitle(sheet);

            // Assert
            sheet.Received().SetCellValue(templateInfo.TitleStartIndex, 0, spreadSheet.TitleText);
        }

        #endregion Fill Title

        #region FillFreezePanels

        [Test]
        public void FillFreezePanels_SheetIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => workbook.FillFreezePanels(null));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void FillFreezePanels_FreezeColumnNameIsNullOrWhiteSpace_NotExecuteCreateFreezePane(string freezeColumnName)
        {
            // Arrange
            templateInfo.FreezePanel(1, freezeColumnName);

            // Act
            workbook.FillFreezePanels(sheet);

            // Assert
            sheet.DidNotReceive().CreateFreezePanel(0, templateInfo.FreezeRowIndex);
        }

        [TestCase(2, Column.A)]
        [TestCase(1, Column.B)]
        [TestCase(2, Column.B)]
        public void FillFreezePanels_ColumnNameOrRowIndexIsGreaterThanZero_ExecuteCreateFreezePanel(int rowIndex, string columnName)
        {
            // Arrange
            templateInfo.FreezePanel(rowIndex, columnName);

            // Act
            workbook.FillFreezePanels(sheet);

            // Assert
            sheet.Received().CreateFreezePanel(Column.GetColIndex(columnName), templateInfo.FreezeRowIndex);
        }

        #endregion FillFreezePanels

        #region Fill Header

        [Test]
        public void FillHeader_SheetIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => workbook.FillHeader(null));
        }

        [Test]
        public void FillHeader_HeaderStartIndexIsGreaterThanHeaderEndIndex_DontFillHeader()
        {
            // Arrange
            templateInfo.Header(headerStartIndex: 2);
            templateInfo.Body(bodyStartIndex: 2);

            // Act
            workbook.FillHeader(sheet);

            // Assert
            sheet.DidNotReceive().GetRow(Arg.Any<int>());
        }

        [Test]
        public void FillHeader_ReadHeaderTemplateCell_ToGetResourceKey()
        {
            // Arrange
            templateInfo.Header(headerStartIndex: 2);
            templateInfo.Body(bodyStartIndex: 4);

            sheet.GetRow(Arg.Any<int>()).Returns(row);

            row.FirstCellIndex.Returns((short)0);
            row.LastCellIndex.Returns((short)2);
            row.GetCell(Arg.Any<int>()).Returns(cell);

            cell.GetCellValue().Returns(string.Empty);

            // Act
            workbook.FillHeader(sheet);

            // Assert
            cell.Received().GetCellValue();
        }

        [Test]
        public void FillHeader_GotResourceKey_ExecuteGetHeaderTextFunction()
        {
            // Arrange
            templateInfo.Header(headerStartIndex: 2);
            templateInfo.Body(bodyStartIndex: 4);

            sheet.GetRow(Arg.Any<int>()).Returns(row);

            row.FirstCellIndex.Returns((short)0);
            row.LastCellIndex.Returns((short)2);
            row.GetCell(Arg.Any<int>()).Returns(cell);

            var resourceKey = "resourceKey";
            cell.GetCellValue().Returns(resourceKey);
            Func<string, string> getResourceFunc = Substitute.For<Func<string, string>>();
            spreadSheet.Header(getResourceFunc);

            // Act
            workbook.FillHeader(sheet);

            // Assert
            getResourceFunc.Received().Invoke(resourceKey);
        }

        #endregion Fill Header

        #region Fill Body

        [Test]
        public void FillBody_SheetIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => workbook.FillBody(null));
        }

        [Test]
        public void FillBody_Always_RemoveTemplateRows()
        {
            // Act
            workbook.FillBody(sheet);

            // Assert
            workbook.Received().RemoveTemplateRows(sheet);
        }

        [Test]
        public void FillBody_SpreadSheetHasData_ExecuteFillTotalRowsAndFillBodyRows()
        {
            // Arrange
            spreadSheet.Body(new List<object> { new object() }, Substitute.For<Mapper<object>>());

            // Act
            workbook.FillBody(sheet);

            // Assert
            workbook.Received().FillBodyRows(sheet);
            workbook.Received().FillTotalRows(sheet);
        }

        [Test]
        public void FillBody_SpreadSheetHasNoData_ExecuteFillFillNoInfoRow()
        {
            // Arrange
            spreadSheet.Body(Enumerable.Empty<object>(), Substitute.For<Mapper<object>>());

            // Act
            workbook.FillBody(sheet);

            // Assert
            workbook.Received().FillNoInfoRow(sheet);
        }

        #endregion Fill Body

        #region Fill Body Rows

        [Test]
        public void FillBodyRows_SheetIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => workbook.FillBodyRows(null));
        }

        [Test]
        public void FillBodyRows_Always_CopyFormatFromTemplateRow()
        {
            // Arrange
            var data = new List<object> { new object() };
            spreadSheet.Body(data, Substitute.For<Mapper<object>>());

            var templateRow = Substitute.For<Row>();
            sheet.EvenRowTemplate.Returns(templateRow);
            sheet.OddRowTemplate.Returns(templateRow);

            sheet.GetRow(Arg.Any<int>()).Returns(row);

            // Act
            workbook.FillBodyRows(sheet);

            // Assert
            row.Received().CopyCellsFrom(templateRow);
        }

        [Test]
        public void FillBodyRows_Always_ExecuteFillCells()
        {
            // Arrange
            var data = new List<object> { new object() };
            var rowMapper = Substitute.For<Mapper<object>>();
            spreadSheet.Body(data, rowMapper);

            var templateRow = Substitute.For<Row>();
            sheet.EvenRowTemplate.Returns(templateRow);
            sheet.OddRowTemplate.Returns(templateRow);

            sheet.GetRow(Arg.Any<int>()).Returns(row);

            // Act
            workbook.FillBodyRows(sheet);

            // Assert
            workbook.Received().FillCells(row, data.ElementAt(0), rowMapper.Mappings);
        }

        #endregion Fill Body Rows

        #region Fill No Info Row

        [Test]
        public void FillNoInfoRow_SheetIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => workbook.FillNoInfoRow(null));
        }

        [Test]
        public void FillNoInfoRow_Always_FillNoInformationText()
        {
            // Arrange
            var noInfoText = "No Info Text";
            templateInfo.NoInformationText(noInfoText);

            row.LastCellIndex.Returns((short)10);
            sheet.OddRowTemplate.Returns(row);

            // Act
            workbook.FillNoInfoRow(sheet);

            // Assert
            sheet.Received().SetCellRangeValue(
                templateInfo.BodyStartIndex,
                Column.GetColIndex(templateInfo.ColumnStartName),
                templateInfo.BodyStartIndex,
                9,
                noInfoText);
        }

        [Test]
        public void FillNoInfoRow_Always_SetCenterHorizontalAlignment_ForInformationText()
        {
            // Arrange
            sheet.GetCell(Arg.Any<int>(), Arg.Any<int>()).Returns(cell);

            // Act
            workbook.FillNoInfoRow(sheet);

            // Assert
            cell.Received().SetCenterHorizontalAlignment();
        }

        #endregion Fill No Info Row

        #region Remove Template Rows

        [Test]
        public void RemoveTemplateRows_SheetIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => workbook.RemoveTemplateRows(null));
        }

        [Test]
        public void RemoveTemplateRows_Always_RemoveTemplateRows()
        {
            // Arrange
            var evenTempalateRow = Substitute.For<Row>();
            var oddTemplateRow = Substitute.For<Row>();
            var footerRowTemplate = Substitute.For<Row>();

            sheet.EvenRowTemplate.Returns(evenTempalateRow);
            sheet.OddRowTemplate.Returns(oddTemplateRow);
            sheet.FooterRowsTemplate.Returns(new List<Row> { footerRowTemplate });

            // Act
            workbook.RemoveTemplateRows(sheet);

            // Assert
            sheet.Received().RemoveRow(evenTempalateRow);
            sheet.Received().RemoveRow(oddTemplateRow);
            sheet.Received().RemoveRow(footerRowTemplate);
        }

        #endregion Remove Template Rows

        #region Fill Total Rows

        [Test]
        public void FillTotalRows_SheetIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => workbook.FillTotalRows(null));
        }

        [Test]
        public void FillTotalRows_FooterRowsIsEmpty_DontFillFooterRows()
        {
            // Arrange
            sheet.GetCell(Arg.Any<int>(), Arg.Any<int>()).Returns(cell);

            // Act
            workbook.FillTotalRows(sheet);

            // Assert
            sheet.DidNotReceiveWithAnyArgs().GetRow(Arg.Any<int>());
        }

        [Test]
        public void FillTotalRows_Always_CopyFormatFromTemplateRow()
        {
            // Arrange
            var data = new List<object> { new object(), new object() };
            spreadSheet.Footer(data, Substitute.For<Mapper<object>>());

            var templateRow1 = Substitute.For<Row>();
            var templateRow2 = Substitute.For<Row>();
            sheet.FooterRowsTemplate.Returns(new List<Row> { templateRow1, templateRow2 });

            sheet.GetRow(Arg.Any<int>()).Returns(row);

            // Act
            workbook.FillTotalRows(sheet);

            // Assert
            row.Received().CopyCellsFrom(templateRow1);
            row.Received().CopyCellsFrom(templateRow2);
        }

        [Test]
        public void FillTotalRows_Always_ExecuteFillCells()
        {
            // Arrange
            var data = new List<object> { new object(), new object() };
            spreadSheet.Footer(data, Substitute.For<Mapper<object>>());
            var rowMapper = Substitute.For<Mapper<object>>();

            var templateRow1 = Substitute.For<Row>();
            var templateRow2 = Substitute.For<Row>();
            sheet.FooterRowsTemplate.Returns(new List<Row> { templateRow1, templateRow2 });

            sheet.GetRow(Arg.Any<int>()).Returns(row);

            // Act
            workbook.FillTotalRows(sheet);

            // Assert
            workbook.ReceivedWithAnyArgs(2).FillCells(row, Arg.Any<object>(), rowMapper.Mappings);
        }

        #endregion Fill Total Rows

        #region Fill Cells

        [Test]
        public void FillCells_MappingsHasNoData_DontFillCellValue()
        {
            // Arrange
            var mappings = new Dictionary<string, Func<object, object>>();

            // Act
            workbook.FillCells(row, new object(), mappings);

            // Assert
            row.DidNotReceiveWithAnyArgs().SetCellValue(0, 1);
        }

        [Test]
        public void FillCells_MappingsHasData_SetCellValue()
        {
            // Arrange
            var mappings = new Dictionary<string, Func<object, object>>
            {
                { "a", (a) => a },
                { "b", (b) => b },
            };

            // Act
            workbook.FillCells(row, new object(), mappings);

            // Assert
            row.ReceivedWithAnyArgs(2).SetCellValue(1, 2);
        }

        #endregion Fill Cells

        #region Export To Bytes

        [Test]
        public void ExportToBytes_Always_CallByOrder()
        {
            // Acc
            workbook.ExportToBytes();

            // Assert
            Received.InOrder(() =>
            {
                workbook.LoadTemplateSheet();
                workbook.FillSheetName(templateInfo.SheetIndex, spreadSheet.SheetName);
                workbook.FillTitle(Arg.Any<Sheet>());
                workbook.FillHeader(Arg.Any<Sheet>());
                workbook.FillBody(Arg.Any<Sheet>());
                workbook.FillFreezePanels(Arg.Any<Sheet>());
                workbook.ToBytes();
            });
        }

        #endregion Export To Bytes

        #region Assign data

        [Test]
        public void UseTemplate_Always_SetTemplateInfo()
        {
            // Arrange
            var workbookStub = Substitute.ForPartsOf<Workbook<object>>();
            var templateInfoStub = new TemplateInfo("template-path");

            // Act
            workbookStub.UseTemplate(templateInfoStub);

            // Assert
            Assert.IsTrue(compareLogic.Compare(templateInfoStub, workbookStub.TemplateInfo).AreEqual);
        }

        [Test]
        public void UseTemplate_Always_ExecuteLoadTemplate()
        {
            // Act
            workbook.UseTemplate(templateInfo);

            // Assert
            workbook.Received().Load(templateInfo.TemplatePath);
        }

        [Test]
        public void Body_Always_SetSpreadSheetBody()
        {
            // Arrange
            var rowMapper = new Mapper<object>();
            var bodyRows = new List<object> { new object() };
            workbook.SpreadSheet.Returns(Substitute.For<SpreadSheet<object>>());

            // Act
            workbook.Body(bodyRows, rowMapper);

            // Assert
            workbook.SpreadSheet.Received().Body(bodyRows, rowMapper);
        }

        [Test]
        public void Title_Always_SetSpreadSheetTitle()
        {
            // Arrange
            var titleText = "titleText";
            workbook.SpreadSheet.Returns(Substitute.For<SpreadSheet<object>>());

            // Act
            workbook.Title(titleText);

            // Assert
            workbook.SpreadSheet.Received().Title(titleText);
        }

        [Test]
        public void Sheet_Always_SetSpreadSheetSheet()
        {
            // Arrange
            var sheetName = "sheetName";
            workbook.SpreadSheet.Returns(Substitute.For<SpreadSheet<object>>());

            // Act
            workbook.Sheet(sheetName);

            // Assert
            workbook.SpreadSheet.Received().Sheet(sheetName);
        }

        [Test]
        public void Header_Always_SetSpreadSheetHeader()
        {
            // Arrange
            Func<string, string> headerMapping = (a) => a.ToString();
            workbook.SpreadSheet.Returns(Substitute.For<SpreadSheet<object>>());

            // Act
            workbook.Header(headerMapping);

            // Assert
            workbook.SpreadSheet.Received().Header(headerMapping);
        }

        [Test]
        public void Footer_Always_SetSpreadSheetMultipleFooterRows()
        {
            // Arrange
            var footerMapper = new Mapper<object>();
            var footerRows = new List<object> { new object() };
            workbook.SpreadSheet.Returns(Substitute.For<SpreadSheet<object>>());

            // Act
            workbook.Footer(footerRows, footerMapper);

            // Assert
            workbook.SpreadSheet.Received().Footer(footerRows, footerMapper);
        }

        [Test]
        public void Footer_Always_SetSpreadSheetSingleFooterRow()
        {
            // Arrange
            var footerMapper = new Mapper<object>();
            var footerRow = new object();
            workbook.SpreadSheet.Returns(Substitute.For<SpreadSheet<object>>());

            // Act
            workbook.Footer(footerRow, footerMapper);

            // Assert
            workbook.SpreadSheet.Received().Footer(footerRow, footerMapper);
        }

        #endregion Assign data
    }
}