namespace ONEbook.SpreadSheets.Models
{
    public interface Workbook
    {
        Workbook Load(string templatePath);

        void SetSheetName(int index, string name);

        byte[] ToBytes();

        Sheet GetSheetAt(int sheetIndex);

        void RemoveSheetAt(int sheetIndex);
    }
}