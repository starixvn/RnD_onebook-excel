namespace ONEbook.SpreadSheets.NPOI.Tests
{
    using global::NPOI.SS.UserModel;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class NpoiWorkbookTests
    {
        private NpoiWorkbook<object> npoiWorkbook;
        private IWorkbook workbook;

        [SetUp]
        public void SetUp()
        {
            workbook = Substitute.For<IWorkbook>();
            npoiWorkbook = Substitute.ForPartsOf<NpoiWorkbook<object>>();
            npoiWorkbook.Workbook = workbook;
        }

        [Test]
        public void FillSheetName_Always_ExecuteSetSheetName()
        {
            // Acc
            npoiWorkbook.FillSheetName(1, "sheet name");

            // Assert
            workbook.Received().SetSheetName(1, "sheet name");
        }

        [Test]
        public void RemoveSheetAt_Always_ExecuteRemoveSheetAt()
        {
            // Acc
            npoiWorkbook.RemoveSheetAt(1);

            // Assert
            workbook.Received().RemoveSheetAt(1);
        }
    }
}