namespace ONEbook.SpreadSheets.Models
{
    public class SpreadSheetLayout
    {
        public SpreadSheetLayout(
            int titleIndex = 0,
            int headerStartIndex = 1,
            int bodyStartIndex = 3,
            int freezeRowIndex = 0,
            int freezeColumnIndex = 0,
            int columnStartIndex = 0,
            int resultSheetIndex = 0,
            int templateSheetIndex = 1,
            int oddRowTemplateIndex = 3,
            int evenRowTemplateIndex = 4,
            int totalRowTemplateIndex = 5,
            string noInfomationText = "No information available.")
        {
            FreezeRowIndex = freezeRowIndex;
            FreezeColumnIndex = freezeColumnIndex;
            HeaderStartIndex = headerStartIndex;
            BodyStartIndex = bodyStartIndex;
            TitleIndex = titleIndex;
            ColumnStartIndex = columnStartIndex;
            ResultSheetIndex = resultSheetIndex;
            OddRowTemplateIndex = oddRowTemplateIndex;
            EvenRowTemplateIndex = evenRowTemplateIndex;
            TotalRowTemplateIndex = totalRowTemplateIndex;
            TemplateSheetIndex = templateSheetIndex;
            NoInfomationText = noInfomationText;
        }

        public int ResultSheetIndex { get; }

        public int TemplateSheetIndex { get; }

        public int TitleIndex { get; }

        public int ColumnStartIndex { get; }

        public int HeaderStartIndex { get; }

        public int BodyStartIndex { get; }

        public int FreezeRowIndex { get; }

        public int FreezeColumnIndex { get; }

        public int OddRowTemplateIndex { get; }

        public int EvenRowTemplateIndex { get; }

        public int TotalRowTemplateIndex { get; }

        public Row EvenRowTemplate { get; private set; }

        public Row OddRowTemplate { get; private set; }

        public Row TotalRowTemplate { get; private set; }

        public Sheet TemplateSheet { get; private set; }

        public string NoInfomationText { get; }

        public Sheet LoadTemplateSheet(Workbook workbook)
        {
            TemplateSheet = workbook.GetSheetAt(TemplateSheetIndex);
            OddRowTemplate = TemplateSheet.GetRow(OddRowTemplateIndex);
            EvenRowTemplate = TemplateSheet.GetRow(EvenRowTemplateIndex);
            TotalRowTemplate = TemplateSheet.GetRow(TotalRowTemplateIndex);

            return TemplateSheet;
        }
    }
}