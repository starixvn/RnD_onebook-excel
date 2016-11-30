namespace ONEbook.SpreadSheets.NPOI.Models
{
    using global::NPOI.SS.UserModel;
    using global::NPOI.SS.Util;
    using SpreadSheets.Models;

    public class NpoiSheet : Sheet
    {
        public NpoiSheet(ISheet sheet)
        {
            Sheet = sheet;
        }

        public ISheet Sheet { get; }

        public void AppendRows(int destinateRowIndex)
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

        public void CreateFreezePane(int colSplit, int rowSplit)
        {
            Sheet.CreateFreezePane(colSplit, rowSplit);
        }

        public Cell GetCell(int rowIndex, int colIndex)
        {
            return new NpoiCell(Sheet.GetRow(rowIndex).GetCell(colIndex), this);
        }

        public Row GetRow(int rownum)
        {
            return Sheet.GetRow(rownum) == null
                ? new NpoiRow(Sheet.CreateRow(rownum), this)
                : new NpoiRow(Sheet.GetRow(rownum), this);
        }

        public void SetCellRangeValue(int firstRow, int firstCol, int lastRow, int lastCol, string value)
        {
            AppendRows(lastRow);

            var cellRange = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
            Sheet.AddMergedRegion(cellRange);
            SetCellValue(firstRow, firstCol, value);
        }

        public void SetCellValue(int rowIndex, int colIndex, object value)
        {
            var row = GetRow(rowIndex);
            row.SetCellValue(colIndex, value);
        }
    }
}