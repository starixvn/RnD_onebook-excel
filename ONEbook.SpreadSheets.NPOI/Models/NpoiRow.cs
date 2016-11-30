namespace ONEbook.SpreadSheets.NPOI.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::NPOI.SS.UserModel;
    using SpreadSheets.Models;

    public class NpoiRow : Row
    {
        public NpoiRow(IRow row, Sheet sheet)
        {
            Row = row;
            Sheet = sheet;
        }

        public IRow Row { get; }

        public Sheet Sheet { get; }

        public short FirstCellNum => Row.FirstCellNum;

        public short Height
        {
            get
            {
                return Row.Height;
            }

            set
            {
                Row.Height = value;
            }
        }

        public short LastCellNum => Row.LastCellNum;

        public void CopyCellsTo(Row targetRow)
        {
            if (targetRow == null)
            {
                throw new ArgumentNullException("targetRow");
            }

            var firstCellNum = FirstCellNum;
            var lastCellNum = Math.Min(LastCellNum, int.MaxValue);

            for (var i = firstCellNum; i < lastCellNum; i++)
            {
                var sourceCell = GetCell(i);

                if (sourceCell == null)
                {
                    continue;
                }

                var targetCell = targetRow.GetCell(i);

                sourceCell.CopyTo(targetCell);
            }

            var npoiTargetRow = targetRow as NpoiRow;

            Row.Height = npoiTargetRow.Height;
        }

        public Cell GetCell(int cellnum)
        {
            return Row.GetCell(cellnum) == null
                ? new NpoiCell(Row.CreateCell(cellnum), Sheet)
                : new NpoiCell(Row.GetCell(cellnum), Sheet);
        }

        public void SetCellValue(int colIndex, object value)
        {
            var cell = GetCell(colIndex);
            cell.SetCellValue(value);
        }

        public void SetCellValues(int colIndex, IEnumerable<object> values)
        {
            SetCellValues(colIndex, values.ToArray());
        }

        public void SetCellValues(int colIndex, params object[] values)
        {
            foreach (var value in values)
            {
                SetCellValue(colIndex++, value);
            }
        }

        public void SetCellValues(IDictionary<int, object> values)
        {
            foreach (var value in values)
            {
                SetCellValue(value.Key, value.Value);
            }
        }
    }
}