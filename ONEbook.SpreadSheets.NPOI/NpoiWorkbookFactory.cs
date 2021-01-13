namespace ONEbook.SpreadSheets.NPOI
{
    public interface INpoiWorkbookFactory
    {
        Workbook<T> Create<T>() where T : class;
    }

    public class NpoiWorkbookFactory : INpoiWorkbookFactory
    {
        public Workbook<T> Create<T>() where T : class
        {
            return new NpoiWorkbook<T>();
        }
    }
}
