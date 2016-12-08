namespace ONEbook.SpreadSheets
{
    using Models;

    public interface SpreadSheetEngine
    {
        byte[] Export<T>(SpreadSheet<T> spreadSheet) where T : class;
    }
}