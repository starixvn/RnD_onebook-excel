namespace ONEbook.SpreadSheets
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public abstract class Workbook<T> where T : class
    {
        protected internal virtual SpreadSheet<T> SpreadSheet { get; set; }

        protected internal virtual TemplateInfo TemplateInfo { get; set; }

        protected abstract Workbook<T> Load(string templatePath);

        protected abstract void SetSheetName(int index, string name);

        protected abstract byte[] ToBytes();

        protected abstract Sheet GetSheetAt(int sheetIndex);

        protected abstract void RemoveSheetAt(int sheetIndex);

        public virtual Workbook<T> UseTemplate(TemplateInfo templateInfo)
        {
            SpreadSheet = new SpreadSheet<T>();
            TemplateInfo = templateInfo;

            return Load(TemplateInfo.TemplatePath);
        }

        public virtual Workbook<T> Body(IEnumerable<T> rows, Mapper<T> mapper)
        {
            SpreadSheet.Body(rows, mapper);

            return this;
        }

        public virtual Workbook<T> Title(string title)
        {
            SpreadSheet.Title(title);

            return this;
        }

        public virtual Workbook<T> Sheet(string sheetName)
        {
            SpreadSheet.Sheet(sheetName);

            return this;
        }

        public virtual Workbook<T> Header(Func<string, string> headerMapping)
        {
            SpreadSheet.Header(headerMapping);

            return this;
        }

        public virtual Workbook<T> Footer(IEnumerable<T> rows, Mapper<T> rowMapper)
        {
            SpreadSheet.Footer(rows, rowMapper);

            return this;
        }

        public virtual Workbook<T> Footer(T row, Mapper<T> rowMapper)
        {
            SpreadSheet.Footer(row, rowMapper);
            return this;
        }

        public virtual byte[] ExportToBytes()
        {
            var sheet = LoadRowTemplates();

            SetSheetName(TemplateInfo.SheetIndex, SpreadSheet.SheetName);

            sheet = RenderTitle(sheet);

            sheet = RenderHeader(sheet);

            sheet = RenderBody(sheet);

            sheet = RenderFreezePanels(sheet);

            return ToBytes();
        }

        public virtual void ExportToFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentOutOfRangeException(nameof(filePath), "File Path cannot be null or empty");
            }

            var bytes = ExportToBytes();

            using (FileStream fileStream = File.Create(filePath, bytes.Length))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        protected internal virtual Sheet LoadRowTemplates()
        {
            var sheet = GetSheetAt(TemplateInfo.SheetIndex);

            sheet.OddRowTemplate = sheet.GetRow(TemplateInfo.OddRowTemplateIndex);
            sheet.EvenRowTemplate = sheet.GetRow(TemplateInfo.EvenRowTemplateIndex);

            var totalRowsTemplate = new List<Row>();
            for (int rowIndex = TemplateInfo.FooterStartIndex; rowIndex <= TemplateInfo.FooterEndIndex; rowIndex++)
            {
                totalRowsTemplate.Add(sheet.GetRow(rowIndex));
            }

            sheet.TotalRowsTemplate = totalRowsTemplate;

            return sheet;
        }

        protected internal virtual Sheet RenderHeader(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            if (TemplateInfo.HeaderStartIndex <= TemplateInfo.HeaderEndIndex)
            {
                for (int rowIndex = TemplateInfo.HeaderStartIndex; rowIndex <= TemplateInfo.HeaderEndIndex; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);

                    for (int colIndex = row.FirstCellIndex; colIndex < row.LastCellIndex; colIndex++)
                    {
                        var resourceKey = row.GetCell(colIndex).GetCellValue();

                        if (!string.IsNullOrWhiteSpace(resourceKey.ToString()))
                        {
                            row.SetCellValue(colIndex, SpreadSheet.HeaderMapping(resourceKey.ToString()));
                        }
                    }
                }
            }

            return sheet;
        }

        protected internal virtual Sheet RenderBody(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            sheet = RemoveTemplateRows(sheet);

            if (SpreadSheet.Any())
            {
                sheet = RenderTotalRows(sheet);

                sheet = RenderBodyRows(sheet);
            }
            else
            {
                sheet = RenderNoInfoRow(sheet);
            }

            return sheet;
        }

        protected internal virtual Sheet RenderBodyRows(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            var rowIndex = TemplateInfo.BodyStartIndex;
            var isEven = false;
            var dataLength = SpreadSheet.BodyRows.Count();

            for (var dataIndex = 0; dataIndex < dataLength; dataIndex++, rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                var templateRow = isEven ? sheet.EvenRowTemplate : sheet.OddRowTemplate;
                var cellValuesWithColumnName = Map(SpreadSheet.RowMapper, SpreadSheet.BodyRows.ElementAt(dataIndex));

                row.CopyCellsFrom(templateRow);
                row.SetCellValues(ConvertColumnNameToIndex(cellValuesWithColumnName));
                isEven = !isEven;
            }

            return sheet;
        }

        protected internal virtual IDictionary<string, object> Map(Mapper<T> mapper, T data)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null");
            }

            var resultObjects = new Dictionary<string, object>();

            foreach (var mapping in mapper.Mappings)
            {
                resultObjects.Add(mapping.Key, mapping.Value(data));
            }

            return resultObjects;
        }

        protected internal virtual Sheet RenderNoInfoRow(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            var rowIndex = TemplateInfo.BodyStartIndex;
            var colIndex = Column.GetColIndex(TemplateInfo.ColumnStartName);

            sheet.SetCellRangeValue(
                    rowIndex,
                    colIndex,
                    rowIndex,
                    sheet.OddRowTemplate.LastCellIndex - 1,
                    TemplateInfo.NoInfomationText);

            sheet.GetCell(rowIndex, colIndex).SetCenterHorizontalAlignment();

            return sheet;
        }

        protected internal virtual Sheet RemoveTemplateRows(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            sheet.RemoveRow(sheet.EvenRowTemplate);
            sheet.RemoveRow(sheet.OddRowTemplate);

            foreach (var template in sheet.TotalRowsTemplate)
            {
                sheet.RemoveRow(template);
            }

            return sheet;
        }

        protected internal virtual Sheet RenderTitle(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Result Sheet cannot be null");
            }

            if (TemplateInfo.TitleStartIndex < 0)
            {
                return sheet;
            }

            sheet.SetCellValue(TemplateInfo.TitleStartIndex, Column.GetColIndex(TemplateInfo.ColumnStartName), SpreadSheet.TitleText);

            return sheet;
        }

        protected internal virtual Sheet RenderFreezePanels(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            var freeColumnIndex = string.IsNullOrWhiteSpace(TemplateInfo.FreezeColumnName) ? 0 : Column.GetColIndex(TemplateInfo.FreezeColumnName);
            if (freeColumnIndex > 0 || TemplateInfo.FreezeRowIndex > 0)
            {
                sheet.CreateFreezePane(freeColumnIndex, TemplateInfo.FreezeRowIndex);
            }

            return sheet;
        }

        protected internal virtual Sheet RenderTotalRows(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            if (!SpreadSheet.FooterRows.Any())
            {
                return sheet;
            }

            var dataLength = SpreadSheet.FooterRows.Count();
            var rowIndex = TemplateInfo.BodyStartIndex + SpreadSheet.BodyRows.Count();

            for (var dataIndex = 0; dataIndex < dataLength; dataIndex++, rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                var templateRow = sheet.TotalRowsTemplate.ElementAt(dataIndex);
                var cellValuesWithColumnName = Map(SpreadSheet.FooterRowMapper, SpreadSheet.FooterRows.ElementAt(dataIndex));

                row.CopyCellsFrom(templateRow);
                row.SetCellValues(ConvertColumnNameToIndex(cellValuesWithColumnName));
            }

            return sheet;
        }

        internal IDictionary<int, object> ConvertColumnNameToIndex(IDictionary<string, object> cellValuesWithColumnName)
        {
            var columnValuesWithIndex = new Dictionary<int, object>();
            foreach (var cellValue in cellValuesWithColumnName)
            {
                columnValuesWithIndex.Add(Column.GetColIndex(cellValue.Key), cellValue.Value);
            }

            return columnValuesWithIndex;
        }
    }
}