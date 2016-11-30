namespace ONEbook.SpreadSheets.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    public class SpreadSheetServiceImpl : SpreadSheetService
    {
        public virtual byte[] Export<T>(
            SpreadSheet<T> spreadSheet,
            Workbook workbook,
            Func<T, int, IDictionary<int, object>> rowMapping,
            Func<T, int, IDictionary<int, object>> totalRowMapping = null) where T : class
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
                templateSheet,
                spreadSheet.Layout.HeaderStartIndex,
                spreadSheet.Layout.BodyStartIndex - 1,
                spreadSheet.HeaderTexts);

            resultSheet = RenderBody(resultSheet, spreadSheet, rowMapping, totalRowMapping);

            resultSheet = RenderFreePanel(resultSheet, layout.FreezeRowIndex, layout.FreezeColumnIndex);

            workbook.RemoveSheetAt(layout.TemplateSheetIndex);

            return workbook.ToBytes();
        }

        protected virtual Sheet RenderHeader(
            Sheet sheet,
            Sheet templateSheet,
            int headerStartIndex,
            int headerEndIndex,
            IDictionary<string, string> headerTexts)
        {
            if (headerStartIndex <= headerEndIndex)
            {
                for (int i = headerStartIndex; i <= headerEndIndex; i++)
                {
                    var row = sheet.GetRow(i);
                    var templateRow = templateSheet.GetRow(i);

                    templateRow.CopyCellsTo(row);

                    for (int j = templateRow.FirstCellNum; j < templateRow.LastCellNum; j++)
                    {
                        var templateCell = templateRow.GetCell(j);
                        var resourceKey = templateCell.GetCellValue();

                        if (headerTexts.ContainsKey(resourceKey.ToString()))
                        {
                            var currentCell = row.GetCell(j);
                            currentCell.SetCellValue(headerTexts[resourceKey.ToString()]);
                        }
                    }
                }
            }

            return sheet;
        }

        protected virtual Sheet RenderBody<T>(
            Sheet sheet,
            SpreadSheet<T> spreadSheet,
            Func<T, int, IDictionary<int, object>> rowMapping,
            Func<T, int, IDictionary<int, object>> totalRowMapping = null) where T : class
        {
            var layout = spreadSheet.Layout;
            var rowIndex = layout.BodyStartIndex;
            var columnStartIndex = layout.ColumnStartIndex;
            var dataSource = spreadSheet.DataSource;

            if (spreadSheet.Any())
            {
                var isEven = false;
                var dataLength = dataSource.Count();

                for (var i = 0; i < dataLength; i++, rowIndex++)
                {
                    FillRow(
                        sheet.GetRow(rowIndex),
                        dataSource.ElementAt(i),
                        i,
                        rowMapping,
                        isEven ? layout.EvenRowTemplate : layout.OddRowTemplate);
                    isEven = !isEven;
                }

                sheet = RenderTotalRows(sheet, spreadSheet, rowIndex, totalRowMapping ?? rowMapping);
            }
            else
            {
                sheet.SetCellRangeValue(
                    rowIndex,
                    columnStartIndex,
                    rowIndex,
                    columnStartIndex + layout.OddRowTemplate.LastCellNum - 1,
                    layout.NoInfomationText);
                sheet.GetCell(rowIndex, columnStartIndex).SetCenterHorizontalAlignment();
            }

            return sheet;
        }

        protected virtual Sheet RenderTitle(
            Sheet resultSheet,
            Sheet templateSheet,
            int titleColIndex,
            int titleRowIndex,
            string title)
        {
            var titleRow = resultSheet.GetRow(titleRowIndex);
            var templateTitleRow = templateSheet.GetRow(titleRowIndex);

            templateTitleRow.CopyCellsTo(titleRow);

            resultSheet.SetCellValue(titleRowIndex, titleColIndex, title);

            return resultSheet;
        }

        protected virtual Row FillRow<T>(
            Row row,
            T genericData,
            int dataIndex,
            Func<T, int, IDictionary<int, object>> rowMapping,
            Row template)
        {
            var rowTemplate = template;
            rowTemplate.CopyCellsTo(row);

            row.SetCellValues(rowMapping(genericData, dataIndex));

            return row;
        }

        protected virtual Sheet RenderFreePanel(Sheet sheet, int freezeRowIndex, int freeColumnIndex)
        {
            if (freeColumnIndex > 0 || freezeRowIndex > 0)
            {
                sheet.CreateFreezePane(freeColumnIndex, freezeRowIndex);
            }

            return sheet;
        }

        protected virtual Sheet RenderTotalRows<T>(
            Sheet sheet,
            SpreadSheet<T> spreadSheet,
            int rowIndex,
            Func<T, int, IDictionary<int, object>> rowMapping) where T : class
        {
            if (spreadSheet.TotalData == null)
            {
                return sheet;
            }

            var layout = spreadSheet.Layout;
            var row = sheet.GetRow(rowIndex);

            layout.TotalRowTemplate.CopyCellsTo(row);

            row.SetCellValues(rowMapping(spreadSheet.TotalData, rowIndex));

            return sheet;
        }
    }
}