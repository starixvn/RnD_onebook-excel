namespace ONEbook.SpreadSheets.NPOI
{
    using System.Collections.Generic;
    using global::NPOI.SS.UserModel;
    using global::NPOI.SS.Util;

    public class NpoiSheet : Sheet
    {
        public NpoiSheet(ISheet sheet)
        {
            Sheet = sheet;
        }

        public Row EvenRowTemplate { get; set; }

        public Row OddRowTemplate { get; set; }

        public ISheet Sheet { get; }

        public IEnumerable<Row> FooterRowsTemplate { get; set; }

        public virtual void AppendRows(int destinateRowIndex)
        {
            if (destinateRowIndex <= Sheet.LastRowNum)
            {
                return;
            }

            for (var i = Sheet.LastRowNum + 1; i <= destinateRowIndex; i++)
            {
                Sheet.CreateRow(i);
            }
        }

        public void CreateFreezePanel(int colIndex, int rowIndex)
        {
            Sheet.CreateFreezePane(colIndex, rowIndex);
        }

        public Cell GetCell(int rowIndex, int colIndex)
        {
            return new NpoiCell(Sheet.GetRow(rowIndex).GetCell(colIndex));
        }

        public Row GetRow(int rowIndex)
        {
            return Sheet.GetRow(rowIndex) == null
                ? new NpoiRow(Sheet.CreateRow(rowIndex))
                : new NpoiRow(Sheet.GetRow(rowIndex));
        }

        public void RemoveRow(Row row)
        {
            var npoiRow = row as NpoiRow;

            if (npoiRow != null)
            {
                Sheet.RemoveRow(npoiRow.Row);
            }
        }

        public void SetCellRangeValue(int firstRow, int firstCol, int lastRow, int lastCol, string value)
        {
            AppendRows(lastRow);

            var cellRange = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
            Sheet.AddMergedRegion(cellRange);
            SetCellValue(firstRow, firstCol, value);
        }

        public virtual void SetCellValue(int rowIndex, int colIndex, object value)
        {
            var row = GetRow(rowIndex);
            row.SetCellValue(colIndex, value);
        }
    }
}