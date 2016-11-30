namespace ONEbook.SpreadSheets.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class SpreadSheet<T> where T : class
    {
        public SpreadSheet(
            IEnumerable<T> dataSource,
            T totalData,
            string title,
            string sheetName,
            IDictionary<string, string> headerTexts,
            SpreadSheetLayout spreadSheetLayout)
        {
            DataSource = dataSource ?? Enumerable.Empty<T>();
            TotalData = totalData;
            Title = title;
            SheetName = sheetName;
            HeaderTexts = headerTexts;
            Layout = spreadSheetLayout;
        }

        public string Title { get; }

        public IDictionary<string, string> HeaderTexts { get; }

        public string SheetName { get; }

        public IEnumerable<T> DataSource { get; }

        public T TotalData { get; }

        public SpreadSheetLayout Layout { get; }

        public bool Any() => DataSource.Any();
    }
}