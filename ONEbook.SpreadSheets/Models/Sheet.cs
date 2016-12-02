namespace ONEbook.SpreadSheets.Models
{
    public interface Sheet
    {
        Row GetRow(int rownum);

        void AppendRows(int destinateRowIndex);

        void SetCellValue(int rowIndex, int colIndex, object value);

        void SetCellRangeValue(int firstRow, int firstCol, int lastRow, int lastCol, string value);

        Cell GetCell(int rowIndex, int colIndex);

        void CreateFreezePane(int colIndex, int rowIndex);
    }
}