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

            FillTitle(sheet);

            FillHeader(sheet);

            FillBody(sheet);

            FillFreezePanels(sheet);

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

        protected internal virtual void FillHeader(Sheet sheet)
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
        }

        protected internal virtual void FillBody(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            RemoveTemplateRows(sheet);

            if (SpreadSheet.Any())
            {
                FillTotalRows(sheet);
                FillBodyRows(sheet);
            }
            else
            {
                FillNoInfoRow(sheet);
            }
        }

        protected internal virtual void FillBodyRows(Sheet sheet)
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

                row.CopyCellsFrom(templateRow);

                FillCells(row, SpreadSheet.BodyRows.ElementAt(dataIndex), SpreadSheet.RowMapper.Mappings);

                isEven = !isEven;
            }
        }

        protected internal virtual void FillNoInfoRow(Sheet sheet)
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
        }

        protected internal virtual void RemoveTemplateRows(Sheet sheet)
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
        }

        protected internal virtual void FillTitle(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Result Sheet cannot be null");
            }

            if (TemplateInfo.TitleStartIndex < 0)
            {
                return;
            }

            sheet.SetCellValue(TemplateInfo.TitleStartIndex, Column.GetColIndex(TemplateInfo.ColumnStartName), SpreadSheet.TitleText);
        }

        protected internal virtual void FillFreezePanels(Sheet sheet)
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
        }

        protected internal virtual void FillTotalRows(Sheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet), "Sheet cannot be null");
            }

            if (!SpreadSheet.FooterRows.Any())
            {
                return;
            }

            var dataLength = SpreadSheet.FooterRows.Count();
            var rowIndex = TemplateInfo.BodyStartIndex + SpreadSheet.BodyRows.Count();

            for (var dataIndex = 0; dataIndex < dataLength; dataIndex++, rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                var templateRow = sheet.TotalRowsTemplate.ElementAt(dataIndex);

                row.CopyCellsFrom(templateRow);

                FillCells(row, SpreadSheet.FooterRows.ElementAt(dataIndex), SpreadSheet.FooterRowMapper.Mappings);
            }
        }

        protected internal virtual void FillCells(Row row, T data, IDictionary<string, Func<T, object>> mappings)
        {
            if (!mappings.Keys.Any())
            {
                return;
            }

            foreach (var mapping in mappings)
            {
                row.SetCellValue(Column.GetColIndex(mapping.Key), mapping.Value(data));
            }
        }
    }
}