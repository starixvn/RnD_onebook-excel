namespace ONEbook.SpreadSheets
{
    public interface SpreadSheetEngine
    {
        byte[] Export<T>(SpreadSheet<T> spreadSheet, Workbook workbook) where T : class;
    }
}