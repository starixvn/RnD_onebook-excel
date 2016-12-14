namespace ONEbook.SpreadSheets
{
    using System.Collections.Generic;

    public interface Sheet
    {
        Row EvenRowTemplate { get; set; }

        Row OddRowTemplate { get; set; }

        IEnumerable<Row> TotalRowsTemplate { get; set; }

        Row GetRow(int rowIndex);

        void AppendRows(int destinateRowIndex);

        void SetCellValue(int rowIndex, int colIndex, object value);

        void SetCellRangeValue(int firstRow, int firstCol, int lastRow, int lastCol, string value);

        Cell GetCell(int rowIndex, int colIndex);

        void CreateFreezePane(int colIndex, int rowIndex);

        void RemoveRow(Row row);
    }
}