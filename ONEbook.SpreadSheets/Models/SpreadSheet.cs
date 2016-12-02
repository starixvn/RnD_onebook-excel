namespace ONEbook.SpreadSheets.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SpreadSheet<T> where T : class
    {
        public SpreadSheet(
            IEnumerable<T> bodyRows,
            Func<T, int, IDictionary<int, object>> rowMapping,
            string title,
            string sheetName,
            IDictionary<string, string> headerTexts,
            SpreadSheetLayout layout,
            IEnumerable<T> footerRows = null,
            Func<T, int, IDictionary<int, object>> footerRowMapping = null)
        {
            BodyRows = bodyRows ?? Enumerable.Empty<T>();
            FooterRows = footerRows ?? Enumerable.Empty<T>();
            Title = title;
            SheetName = sheetName;
            HeaderTexts = headerTexts;
            Layout = layout;
            RowMapping = rowMapping;
            FooterRowMapping = footerRowMapping ?? rowMapping;
        }

        public string Title { get; }

        public IDictionary<string, string> HeaderTexts { get; }

        public string SheetName { get; }

        public IEnumerable<T> BodyRows { get; }

        public IEnumerable<T> FooterRows { get; }

        public SpreadSheetLayout Layout { get; }

        public Func<T, int, IDictionary<int, object>> RowMapping { get; }

        public Func<T, int, IDictionary<int, object>> FooterRowMapping { get; }

        public bool Any() => BodyRows.Any();
    }
}