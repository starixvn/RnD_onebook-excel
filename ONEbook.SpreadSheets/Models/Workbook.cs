namespace ONEbook.SpreadSheets.Models
{
    public interface Workbook
    {
        void Load(string templatePath);

        void SetSheetName(int sheetIx, string name);

        byte[] ToBytes();

        Sheet GetSheetAt(int sheetIndex);

        void RemoveSheetAt(int sheetIndex);
    }
}