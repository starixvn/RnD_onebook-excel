namespace ONEbook.SpreadSheets.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class WorkbookTests
    {
        Workbook<object> workBook;

        [SetUp]
        public void SetUp()
        {
            workBook = Substitute.ForPartsOf<Workbook<object>>();
        } 
    }
}
