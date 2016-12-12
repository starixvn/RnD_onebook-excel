namespace ONEbook.SpreadSheets
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class SpreadSheetEngineImpl : SpreadSheetEngine
    {
        private static readonly ConcurrentDictionary<string, int> ColumnNameToIndexMapping = new ConcurrentDictionary<string, int>();

        public virtual byte[] Export<T>(SpreadSheet<T> spreadSheet, Workbook workbook) where T : class
        {
            if (spreadSheet == null)
            {
                throw new ArgumentNullException(nameof(spreadSheet), "SpreadSheet cannot be null");
            }

            var layout = spreadSheet.Layout;
            var sheet = LoadRowTemplates(layout, workbook);

            workbook.SetSheetName(layout.ResultSheetIndex, spreadSheet.SheetName);

            sheet = RenderTitle(
                sheet,
                layout.ColumnStartIndex,
                layout.TitleIndex,
                spreadSheet.Title);

            sheet = RenderHeader(
                sheet,
                spreadSheet.Layout.HeaderStartIndex,
                spreadSheet.Layout.HeaderEndIndex,
                spreadSheet.HeaderTexts);

            sheet = RenderBody(sheet, spreadSheet);

            sheet = RenderFreezePanels(sheet, layout.FreezeRowIndex, layout.FreezeColumnName);

            return workbook.ToBytes();
        }

        protected virtual Sheet LoadRowTemplates(SpreadSheetLayout layout, Workbook workbook)
        {
            var resultSheet = workbook.GetSheetAt(layout.ResultSheetIndex);
            resultSheet.OddRowTemplate = resultSheet.GetRow(layout.OddRowTemplateIndex);
            resultSheet.EvenRowTemplate = resultSheet.GetRow(layout.EvenRowTemplateIndex);

            var totalRowsTemplate = new List<Row>();
            for (int rowIndex = layout.FooterRowsTemplateStartIndex; rowIndex <= layout.FooterRowsTemplateEndIndex; rowIndex++)
            {
                totalRowsTemplate.Add(resultSheet.GetRow(rowIndex));
            }

            resultSheet.TotalRowsTemplate = totalRowsTemplate;

            return resultSheet;
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

            sheet = RemoveTemplateRows(sheet);

            if (spreadSheet.Any())
            {
                sheet = RenderTotalRows(
                       sheet,
                       spreadSheet.FooterRows,
                       spreadSheet.BodyRows.Count() + rowIndex,
                       spreadSheet.FooterRowMapping);

                sheet = RenderBodyRows(sheet, spreadSheet.BodyRows, rowIndex, spreadSheet.RowMapping);
            }
            else
            {
                sheet = RenderNoInfoRow(sheet, rowIndex, columnStartIndex, layout);
            }

            return sheet;
        }

        protected virtual Sheet RenderBodyRows<T>(
            Sheet sheet,
            IEnumerable<T> bodyRows,
            int rowIndex,
            Func<T, int, IDictionary<string, object>> rowMapping) where T : class
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            var isEven = false;
            var dataLength = bodyRows.Count();

            for (var dataIndex = 0; dataIndex < dataLength; dataIndex++, rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                var templateRow = isEven ? sheet.EvenRowTemplate : sheet.OddRowTemplate;
                var cellValuesWithColumnName = rowMapping(bodyRows.ElementAt(dataIndex), rowIndex);

                row.CopyCellsFrom(templateRow);
                row.SetCellValues(ConvertColumnNameToIndex(cellValuesWithColumnName));
                isEven = !isEven;
            }

            return sheet;
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
                    sheet.OddRowTemplate.LastCellIndex - 1,
                    layout.NoInfomationText);

            sheet.GetCell(rowIndex, colIndex).SetCenterHorizontalAlignment();

            return sheet;
        }

        protected virtual Sheet RemoveTemplateRows(Sheet sheet)
        {
            sheet.RemoveRow(sheet.EvenRowTemplate);
            sheet.RemoveRow(sheet.OddRowTemplate);

            foreach (var template in sheet.TotalRowsTemplate)
            {
                sheet.RemoveRow(template);
            }

            return sheet;
        }

        protected virtual Sheet RenderTitle(
            Sheet sheet,
            int titleColIndex,
            int titleRowIndex,
            string title)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Result Sheet cannot be null");
            }

            var titleRow = sheet.GetRow(titleRowIndex);

            sheet.SetCellValue(titleRowIndex, titleColIndex, title);

            return sheet;
        }

        protected virtual Sheet RenderFreezePanels(Sheet sheet, int freezeRowIndex, string freeColumnName)
        {
            var freeColumnIndex = string.IsNullOrWhiteSpace(freeColumnName) ? 0 : GetColIndex(freeColumnName);
            if (freeColumnIndex > 0 || freezeRowIndex > 0)
            {
                sheet.CreateFreezePane(freeColumnIndex, freezeRowIndex);
            }

            return sheet;
        }

        protected virtual Sheet RenderTotalRows<T>(
            Sheet sheet,
            IEnumerable<T> totalData,
            int rowIndex,
            Func<T, int, IDictionary<string, object>> rowMapping) where T : class
        {
            if (!totalData.Any())
            {
                return sheet;
            }

            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Result Sheet cannot be null");
            }

            var dataLength = totalData.Count();

            for (var dataIndex = 0; dataIndex < dataLength; dataIndex++, rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                var templateRow = sheet.TotalRowsTemplate.ElementAt(dataIndex);
                var cellValuesWithColumnName = rowMapping(totalData.ElementAt(dataIndex), rowIndex);

                row.CopyCellsFrom(templateRow);
                row.SetCellValues(ConvertColumnNameToIndex(cellValuesWithColumnName));
            }

            return sheet;
        }

        private int GetColIndex(string colName)
        {
            if (ColumnNameToIndexMapping.ContainsKey(colName))
            {
                return ColumnNameToIndexMapping[colName];
            }

            int colIndex = -1;
            int pow = 1;
            for (int i = colName.Length - 1; i >= 0; i--)
            {
                colIndex += (colName[i] - 'A' + 1) * pow;
                pow *= 26;
            }

            if (!ColumnNameToIndexMapping.ContainsKey(colName))
            {
                ColumnNameToIndexMapping.TryAdd(colName, colIndex);
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