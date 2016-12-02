namespace ONEbook.SpreadSheets.Services
{
    using Models;

    public interface SpreadSheetService
    {
        byte[] Export<T>(SpreadSheet<T> spreadSheet, Workbook workbook) where T : class;
    }
}