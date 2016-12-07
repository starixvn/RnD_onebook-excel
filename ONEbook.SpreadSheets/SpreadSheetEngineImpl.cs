namespace ONEbook.SpreadSheets
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    public class SpreadSheetEngineImpl : SpreadSheetEngine
    {
        static readonly ConcurrentDictionary<string, int> columnNameToIndex = new ConcurrentDictionary<string, int>();

        public virtual byte[] Export<T>(
            SpreadSheet<T> spreadSheet,
            Workbook workbook) where T : class
        {
            if (spreadSheet == null)
            {
                throw new ArgumentNullException(nameof(spreadSheet), "SpreadSheet cannot be null");
            }

            if (workbook == null)
            {
                throw new ArgumentNullException(nameof(workbook), "Workbook cannot be null");
            }

            var layout = spreadSheet.Layout;
            var templateSheet = layout.LoadTemplateSheet(workbook);
            var resultSheet = workbook.GetSheetAt(layout.ResultSheetIndex);

            workbook.SetSheetName(layout.ResultSheetIndex, spreadSheet.SheetName);

            resultSheet = RenderTitle(
                resultSheet,
                templateSheet,
                layout.ColumnStartIndex,
                layout.TitleIndex,
                spreadSheet.Title);

            resultSheet = RenderHeader(
                resultSheet,
                spreadSheet.Layout.HeaderStartIndex,
                spreadSheet.Layout.BodyStartIndex - 1,
                spreadSheet.HeaderTexts);

            resultSheet = RenderBody(resultSheet, spreadSheet);

            resultSheet = RenderFreePanel(resultSheet, layout.FreezeRowIndex, layout.FreezeColumnName);

            workbook.RemoveSheetAt(layout.TemplateSheetIndex);

            return workbook.ToBytes();
        }

        protected virtual Sheet RenderHeader(
            Sheet resultSheet,
            int headerStartIndex,
            int headerEndIndex,
            IDictionary<string, string> headerTexts)
        {
            if (resultSheet == null)
            {
                throw new ArgumentNullException(nameof(resultSheet), "Result Sheet cannot be null");
            }

            if (headerStartIndex <= headerEndIndex)
            {
                for (int rowIndex = headerStartIndex; rowIndex <= headerEndIndex; rowIndex++)
                {
                    var row = resultSheet.GetRow(rowIndex);

                    for (int colIndex = row.FirstCellIndex; colIndex < row.LastCellIndex; colIndex++)
                    {
                        var resourceKey = row.GetCell(colIndex).GetCellValue();

                        if (headerTexts.ContainsKey(resourceKey.ToString()))
                        {
                            row.SetCellValue(colIndex, headerTexts[resourceKey.ToString()]);
                        }
                    }
                }
            }

            return resultSheet;
        }

        protected virtual Sheet RenderBody<T>(
            Sheet sheet,
            SpreadSheet<T> spreadSheet) where T : class
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            if (spreadSheet == null)
            {
                throw new ArgumentNullException(nameof(spreadSheet), "Spread Sheet cannot be null");
            }

            var layout = spreadSheet.Layout;
            var rowIndex = layout.BodyStartIndex;
            var columnStartIndex = layout.ColumnStartIndex;

            if (spreadSheet.Any())
            {
                rowIndex = RenderBodyRows(sheet, layout, spreadSheet.BodyRows, rowIndex, spreadSheet.RowMapping);

                sheet = RenderTotalRows(sheet, layout.TemplateSheet, spreadSheet.FooterRows, layout.TotalRowTemplateIndex, rowIndex, spreadSheet.FooterRowMapping);
            }
            else
            {
                sheet = RenderNoInfoRow(sheet, rowIndex, columnStartIndex, layout);
            }

            return sheet;
        }

        protected virtual int RenderBodyRows<T>(
            Sheet sheet,
            SpreadSheetLayout layout,
            IEnumerable<T> bodyRows,
            int rowIndex,
            Func<T, int, IDictionary<string, object>> rowMapping) where T : class
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout), "Layout cannot be null");
            }

            var isEven = false;
            var dataLength = bodyRows.Count();

            for (var dataIndex = 0; dataIndex < dataLength; dataIndex++, rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                var templateRow = isEven ? layout.EvenRowTemplate : layout.OddRowTemplate;
                var cellValuesWithColumnName = rowMapping(bodyRows.ElementAt(dataIndex), rowIndex);

                row.CopyCellsFrom(templateRow);
                row.SetCellValues(ConvertColumnNameToIndex(cellValuesWithColumnName));
                isEven = !isEven;
            }

            return rowIndex;
        }

        protected virtual Sheet RenderNoInfoRow(
            Sheet sheet,
            int rowIndex,
            int colIndex,
            SpreadSheetLayout layout)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout), "Layout cannot be null");
            }

            sheet.SetCellRangeValue(
                    rowIndex,
                    colIndex,
                    rowIndex,
                    layout.OddRowTemplate.LastCellIndex - 1,
                    layout.NoInfomationText);

            sheet.GetCell(rowIndex, colIndex).SetCenterHorizontalAlignment();

            return sheet;
        }

        protected virtual Sheet RenderTitle(
            Sheet resultSheet,
            Sheet templateSheet,
            int titleColIndex,
            int titleRowIndex,
            string title)
        {
            if (resultSheet == null)
            {
                throw new ArgumentNullException(nameof(resultSheet), "Result Sheet cannot be null");
            }

            if (templateSheet == null)
            {
                throw new ArgumentNullException(nameof(templateSheet), "Template Sheet cannot be null");
            }

            var titleRow = resultSheet.GetRow(titleRowIndex);
            var templateTitleRow = templateSheet.GetRow(titleRowIndex);

            titleRow.CopyCellsFrom(templateTitleRow);

            resultSheet.SetCellValue(titleRowIndex, titleColIndex, title);

            return resultSheet;
        }

        protected virtual Sheet RenderFreePanel(Sheet sheet, int freezeRowIndex, string freeColumnName)
        {
            var freeColumnIndex = string.IsNullOrWhiteSpace(freeColumnName) ? 0 : GetColIndex(freeColumnName);
            if (freeColumnIndex > 0 || freezeRowIndex > 0)
            {
                sheet.CreateFreezePane(freeColumnIndex, freezeRowIndex);
            }

            return sheet;
        }

        protected virtual Sheet RenderTotalRows<T>(
            Sheet resultSheet,
            Sheet templateSheet,
            IEnumerable<T> totalData,
            int templateRowIndex,
            int rowIndex,
            Func<T, int, IDictionary<string, object>> rowMapping) where T : class
        {
            if (!totalData.Any())
            {
                return resultSheet;
            }

            if (resultSheet == null)
            {
                throw new ArgumentNullException(nameof(resultSheet), "Result Sheet cannot be null");
            }

            if (templateSheet == null)
            {
                throw new ArgumentNullException(nameof(templateSheet), "Template Sheet cannot be null");
            }

            var dataLength = totalData.Count();

            for (var dataIndex = 0; dataIndex < dataLength; dataIndex++, rowIndex++, templateRowIndex++)
            {
                var row = resultSheet.GetRow(rowIndex);
                var templateRow = templateSheet.GetRow(templateRowIndex);
                var cellValuesWithColumnName = rowMapping(totalData.ElementAt(dataIndex), rowIndex);

                row.CopyCellsFrom(templateRow);
                row.SetCellValues(ConvertColumnNameToIndex(cellValuesWithColumnName));
            }

            return resultSheet;
        }

        private int GetColIndex(string colName)
        {
            if(columnNameToIndex.ContainsKey(colName))
            {
                return columnNameToIndex[colName];
            }

            int colIndex = 0;
            int pow = 1;
            for (int i = colName.Length - 1; i >= 0; i--)
            {
                colIndex += (colName[i] - 'A') * pow;
                pow *= 26;
            }

            if(!columnNameToIndex.ContainsKey(colName))
            {
                columnNameToIndex.TryAdd(colName, colIndex);
            }

            return colIndex;
        }

        private IDictionary<int, object> ConvertColumnNameToIndex(IDictionary<string, object> cellValuesWithColumnName)
        {
            var columnValuesWithIndex = new Dictionary<int, object>();
            foreach (var cellValue in cellValuesWithColumnName)
            {
                columnValuesWithIndex.Add(GetColIndex(cellValue.Key), cellValue.Value);
            }

            return columnValuesWithIndex;
        }

    }
}