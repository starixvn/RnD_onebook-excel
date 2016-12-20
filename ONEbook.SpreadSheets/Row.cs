namespace ONEbook.SpreadSheets
{
    public interface Row
    {
        short FirstCellIndex { get; }

        short LastCellIndex { get; }

        void CopyCellsFrom(Row row);

        void SetCellValue(int colIndex, object value);

        Cell GetCell(int cellnum);
    }
}