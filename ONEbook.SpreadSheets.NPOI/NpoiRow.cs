namespace ONEbook.SpreadSheets.NPOI
{
    using System;
    using System.Collections.Generic;
    using global::NPOI.SS.UserModel;

    public class NpoiRow : Row
    {
        public NpoiRow(IRow row)
        {
            Row = row;
        }

        public IRow Row { get; }

        public short FirstCellIndex => Row.FirstCellNum;

        public short LastCellIndex => Row.LastCellNum;

        public void CopyCellsFrom(Row row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            for (var i = row.FirstCellIndex; i < row.LastCellIndex; i++)
            {
                var sourceCell = row.GetCell(i);

                if (sourceCell == null)
                {
                    continue;
                }

                var cell = GetCell(i);

                cell.CopyFrom(sourceCell);
            }

            var npoiRow = row as NpoiRow;

            if (npoiRow == null)
            {
                throw new InvalidCastException("NPOI Target Row cannot be null");
            }

            Row.Height = npoiRow.Row.Height;
        }

        public Cell GetCell(int cellnum)
        {
            return Row.GetCell(cellnum) == null
                ? new NpoiCell(Row.CreateCell(cellnum))
                : new NpoiCell(Row.GetCell(cellnum));
        }

        public void SetCellValue(int colIndex, object value)
        {
            var cell = GetCell(colIndex);
            cell.SetCellValue(value);
        }

        public void SetCellValues(IDictionary<int, object> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values), "Cell Values cannot be null");
            }

            foreach (var value in values)
            {
                SetCellValue(value.Key, value.Value);
            }
        }
    }
}