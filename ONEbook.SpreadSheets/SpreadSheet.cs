namespace ONEbook.SpreadSheets
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

        public string TitleText { get; protected set; }

        public Func<string, string> HeaderMapping { get; protected set; }

        public string SheetName { get; protected set; }

        public IEnumerable<T> BodyRows { get; protected set; }

        public IEnumerable<T> FooterRows { get; protected set; }

        public Mapper<T> RowMapper { get; protected set; }

        public Mapper<T> FooterRowMapper { get; protected set; }

        public bool Any() => BodyRows.Any();

        public SpreadSheet<T> Body(IEnumerable<T> rows, Mapper<T> rowMapping)
        {
            BodyRows = rows;
            RowMapper = rowMapping;

            return this;
        }

        public SpreadSheet<T> Title(string title)
        {
            TitleText = title;

            return this;
        }

        public SpreadSheet<T> Sheet(string sheetName)
        {
            SheetName = sheetName;

            return this;
        }

        public SpreadSheet<T> Header(Func<string, string> headerMapping)
        {
            HeaderMapping = headerMapping;

            return this;
        }

        public SpreadSheet<T> Footer(IEnumerable<T> rows, Mapper<T> rowMapping)
        {
            FooterRows = rows;
            FooterRowMapper = rowMapping;

            return this;
        }

        public SpreadSheet<T> Footer(T row, Mapper<T> rowMapping)
        {
            FooterRows = new List<T> { row };
            FooterRowMapper = rowMapping;

            return this;
        }
    }
}