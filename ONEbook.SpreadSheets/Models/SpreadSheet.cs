namespace ONEbook.SpreadSheets.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SpreadSheet<T> where T : class
    {
        public SpreadSheet()
        {
            BodyRows = Enumerable.Empty<T>();
            FooterRows = Enumerable.Empty<T>();
        }

        public string Title { get; protected set; }

        public IDictionary<string, string> HeaderTexts { get; protected set; }

        public string SheetName { get; protected set; }

        public IEnumerable<T> BodyRows { get; protected set; }

        public IEnumerable<T> FooterRows { get; protected set; }

        public SpreadSheetLayout Layout { get; protected set; }

        public Func<T, int, IDictionary<string, object>> RowMapping { get; protected set; }

        public Func<T, int, IDictionary<string, object>> FooterRowMapping { get; protected set; }

        public bool Any() => BodyRows.Any();

        public SpreadSheet<T> AddBody(IEnumerable<T> rows, Func<T, int, IDictionary<string, object>> rowMapping)
        {
            BodyRows = rows;
            RowMapping = rowMapping;

            return this;
        }

        public SpreadSheet<T> AddTitle(string title)
        {
            Title = title;

            return this;
        }

        public SpreadSheet<T> AddSheetName(string sheetName)
        {
            SheetName = sheetName;

            return this;
        }

        public SpreadSheet<T> AddHeaderTexts(IDictionary<string, string> headerTexts)
        {
            HeaderTexts = headerTexts;

            return this;
        }

        public SpreadSheet<T> AddFooter(IEnumerable<T> rows, Func<T, int, IDictionary<string, object>> rowMapping)
        {
            FooterRows = rows;
            FooterRowMapping = rowMapping;

            return this;
        }

        public SpreadSheet<T> AddFooter(T row, Func<T, int, IDictionary<string, object>> rowMapping)
        {
            FooterRows = new List<T> { row };
            FooterRowMapping = rowMapping;

            return this;
        }

        public SpreadSheet<T> SetLayout(SpreadSheetLayout layout)
        {
            Layout = layout;

            return this;
        }
    }
}