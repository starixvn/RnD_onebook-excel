namespace ONEbook.SpreadSheets.NPOI.Models
{
    using System;
    using Fanex.Common;
    using global::NPOI.SS.UserModel;
    using global::NPOI.SS.Util;
    using ONEbook.SpreadSheets.Models;

    public class NpoiCell : Cell
    {
        public NpoiCell(ICell cell, Sheet sheet)
        {
            Cell = cell;
            Sheet = sheet;
        }

        public ICell Cell { get; }

        public Sheet Sheet { get; }

        public int ColumnIndex
        {
            get
            {
                return Cell.ColumnIndex;
            }
        }

        public int RowIndex
        {
            get
            {
                return Cell.RowIndex;
            }
        }

        public void CopyTo(Cell targetCell)
        {
            if (targetCell == null)
            {
                throw new ArgumentNullException("targetCell");
            }

            this.CopyMergeInfoTo(targetCell);

            var npoiTargetCell = targetCell as NpoiCell;

            npoiTargetCell.Cell.CellComment = Cell.CellComment ?? default(IComment);
            npoiTargetCell.Cell.Hyperlink = Cell.Hyperlink ?? default(IHyperlink);
            npoiTargetCell.Cell.CellStyle = Cell.CellStyle ?? default(ICellStyle);

            npoiTargetCell.Cell.SetCellType(Cell.CellType);

            switch (Cell.CellType)
            {
                case CellType.Boolean:
                    npoiTargetCell.SetCellValue(Cell.BooleanCellValue);
                    break;

                case CellType.Numeric:
                    npoiTargetCell.SetCellValue(Cell.NumericCellValue);
                    break;

                case CellType.String:
                    npoiTargetCell.SetCellValue(Cell.StringCellValue);
                    break;

                case CellType.Error:
                    npoiTargetCell.SetCellValue(Cell.ErrorCellValue);
                    break;

                case CellType.Formula:
                    npoiTargetCell.Cell.CellFormula = Cell.CellFormula;
                    break;
            }
        }

        public void CopyMergeInfoTo(Cell targetCell)
        {
            if (!Cell.IsMergedCell)
            {
                return;
            }

            if (targetCell == null)
            {
                throw new ArgumentNullException("targetCell");
            }

            CellRangeAddress sourceRange = null;

            for (var i = 0; i < Cell.Sheet.NumMergedRegions; i++)
            {
                CellRangeAddress range = Cell.Sheet.GetMergedRegion(i);

                if (range.FirstColumn == Cell.ColumnIndex
                    && range.FirstRow == Cell.RowIndex)
                {
                    sourceRange = range;
                    break;
                }
            }

            if (sourceRange != null)
            {
                CellRangeAddress newRange = new CellRangeAddress(
                    targetCell.RowIndex,
                    targetCell.RowIndex + sourceRange.LastRow - sourceRange.FirstRow,
                    targetCell.ColumnIndex,
                    targetCell.ColumnIndex + sourceRange.LastColumn - sourceRange.FirstColumn);

                var npoiCell = targetCell as NpoiCell;

                npoiCell.Cell.Sheet.AddMergedRegion(newRange);
            }
        }

        public void SetCellValue(object value)
        {
            if (Cell == null)
            {
                return;
            }

            var isNumber = value is double
                || value is int
                || value is long
                || value is decimal;

            if (isNumber)
            {
                Cell.SetCellValue(Converter.To<double>(value));
            }
            else if (value is string)
            {
                Cell.SetCellValue(Converter.ToString2(value));
            }
            else if (value is bool)
            {
                Cell.SetCellValue(Converter.To<bool>(value));
            }
            else if (value is DateTime)
            {
                Cell.SetCellValue(Converter.To<DateTime>(value));
            }
            else if (value == null)
            {
                Cell.SetCellValue("0");
            }
            else if (value is IRichTextString)
            {
                Cell.SetCellValue(Converter.To<IRichTextString>(value));
            }
            else
            {
                Cell.SetCellValue("Undefined");
            }
        }

        public void SetCenterHorizontalAlignment()
        {
            Cell.CellStyle.Alignment = HorizontalAlignment.Center;
        }

        public object GetCellValue()
        {
            switch (Cell.CellType)
            {
                case CellType.Boolean:
                    return Cell.BooleanCellValue;

                case CellType.Numeric:
                    return Cell.NumericCellValue;

                case CellType.String:
                    return Cell.StringCellValue;

                case CellType.Error:
                    return Cell.ErrorCellValue;

                case CellType.Formula:
                    return Cell.CellFormula;

                default:
                    return string.Empty;
            }
        }
    }
}