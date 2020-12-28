namespace ONEbook.SpreadSheets.NPOI
{
    using System;
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
                throw new ArgumentNullException(nameof(row), "Row cannot be null");
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

            if (!(row is NpoiRow npoiRow))
            {
                throw new InvalidCastException("NPOI Target Row cannot be null");
            }

            Row.Height = npoiRow.Row.Height;
        }

        public virtual Cell GetCell(int cellnum)
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
    }
}