namespace ONEbook.SpreadSheets.Models
{
    using System.Collections.Generic;

    public interface Row
    {
        short FirstCellNum { get; }

        short LastCellNum { get; }

        void CopyCellsTo(Row targetRow);

        void SetCellValues(IDictionary<int, object> values);

        void SetCellValues(int colIndex, IEnumerable<object> values);

        void SetCellValue(int colIndex, object value);

        Cell GetCell(int cellnum);
    }
}