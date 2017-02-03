namespace ONEbook.SpreadSheets.NPOI.Tests
{
    using System.IO;
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

        [Test]
        public void ToBytes_Always_ExecuteGetMemoryStream()
        {
            // Acc
            npoiWorkbook.ToBytes();

            // Assert
            npoiWorkbook.Received().GetMemoryStream();
        }

        [Test]
        public void ToBytes_Always_ReturnStreamBuffer()
        {
            // Arrange
            var npoiWorkbookForTesting = new NpoiWorkbookForTesting();
            npoiWorkbookForTesting.Workbook = workbook;

            // Acc
            var actualBuffer = npoiWorkbookForTesting.ToBytes();

            // Assert
            Assert.AreEqual(new byte[] { 1, 2 }, actualBuffer);
        }
    }

    public class NpoiWorkbookForTesting : NpoiWorkbook<object>
    {
        internal override MemoryStream GetMemoryStream()
        {
            var expectedBuffer = new byte[] { 1, 2 };
            var memoryStream = Substitute.ForPartsOf<MemoryStream>();
            memoryStream.GetBuffer().Returns(expectedBuffer);
            return memoryStream;
        }
    }
}