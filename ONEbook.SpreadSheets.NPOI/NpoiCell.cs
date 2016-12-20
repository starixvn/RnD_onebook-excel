namespace ONEbook.SpreadSheets.NPOI
{
    using System;
    using Fanex.Common;
    using global::NPOI.SS.UserModel;
    using global::NPOI.SS.Util;

    public class NpoiCell : Cell
    {
        public NpoiCell(ICell cell)
        {
            Cell = cell;
        }

        public ICell Cell { get; }

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

        public virtual void CopyFrom(Cell cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException(nameof(cell));
            }

            CopyMergeInfoFrom(cell);

            var npoiTargetCell = cell as NpoiCell;

            Cell.CellComment = npoiTargetCell.Cell.CellComment ?? default(IComment);
            Cell.Hyperlink = npoiTargetCell.Cell.Hyperlink ?? default(IHyperlink);
            Cell.CellStyle = npoiTargetCell.Cell.CellStyle ?? default(ICellStyle);

            Cell.SetCellType(npoiTargetCell.Cell.CellType);

            switch (npoiTargetCell.Cell.CellType)
            {
                case CellType.Boolean:
                    Cell.SetCellValue(npoiTargetCell.Cell.BooleanCellValue);
                    break;

                case CellType.Numeric:
                    Cell.SetCellValue(npoiTargetCell.Cell.NumericCellValue);
                    break;

                case CellType.String:
                    Cell.SetCellValue(npoiTargetCell.Cell.StringCellValue);
                    break;

                case CellType.Error:
                    Cell.SetCellValue(npoiTargetCell.Cell.ErrorCellValue);
                    break;

                case CellType.Formula:
                    Cell.CellFormula = npoiTargetCell.Cell.CellFormula;
                    break;
            }
        }

        public virtual void CopyMergeInfoFrom(Cell cell)
        {
            var npoiCell = cell as NpoiCell;

            if (npoiCell == null)
            {
                throw new InvalidCastException("NPOI Cell cannot be null");
            }

            if (!npoiCell.Cell.IsMergedCell)
            {
                return;
            }

            CellRangeAddress sourceRange = null;

            for (var regionIndex = 0; regionIndex < npoiCell.Cell.Sheet.NumMergedRegions; regionIndex++)
            {
                CellRangeAddress range = npoiCell.Cell.Sheet.GetMergedRegion(regionIndex);

                if (range.FirstColumn == npoiCell.Cell.ColumnIndex
                    && range.FirstRow == npoiCell.Cell.RowIndex)
                {
                    sourceRange = range;
                    break;
                }
            }

            if (sourceRange != null)
            {
                CellRangeAddress newRange = new CellRangeAddress(
                    Cell.RowIndex,
                    Cell.RowIndex + sourceRange.LastRow - sourceRange.FirstRow,
                    Cell.ColumnIndex,
                    Cell.ColumnIndex + sourceRange.LastColumn - sourceRange.FirstColumn);

                Cell.Sheet.AddMergedRegion(newRange);
            }
        }

        public virtual void SetCellValue(object value)
        {
            if (Cell == null)
            {
                return;
            }

            var isNumber = value is double
                || value is float
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