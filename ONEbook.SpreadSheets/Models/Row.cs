namespace ONEbook.SpreadSheets.Models
{
    using System.Collections.Generic;

    public interface Row
    {
        short FirstCellIndex { get; }

        short LastCellIndex { get; }

        void CopyCellsFrom(Row row);

        void SetCellValues(IDictionary<int, object> values);

        void SetCellValue(int colIndex, object value);

        Cell GetCell(int cellnum);
    }
}