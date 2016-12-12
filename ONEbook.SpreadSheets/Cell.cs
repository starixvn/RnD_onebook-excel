namespace ONEbook.SpreadSheets
{
    public interface Cell
    {
        int ColumnIndex { get; }

        int RowIndex { get; }

        object GetCellValue();

        void SetCellValue(object value);

        void SetCenterHorizontalAlignment();

        void CopyFrom(Cell cell);
    }
}