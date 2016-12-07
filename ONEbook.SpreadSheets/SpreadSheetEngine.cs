namespace ONEbook.SpreadSheets
{
    using Models;

    public interface SpreadSheetEngine
    {
        byte[] Export<T>(SpreadSheet<T> spreadSheet, Workbook workbook) where T : class;
    }
}