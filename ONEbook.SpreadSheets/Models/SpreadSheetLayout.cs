namespace ONEbook.SpreadSheets.Models
{
    using System;

    public class SpreadSheetLayout
    {
        public SpreadSheetLayout(
            int titleIndex = 1,
            int headerStartIndex = 2,
            int bodyStartIndex = 4,
            int freezeRowIndex = 0,
            string freezeColumnName = "",
            int columnStartIndex = 1,
            int resultSheetIndex = 1,
            int templateSheetIndex = 2,
            int oddRowTemplateIndex = 4,
            int evenRowTemplateIndex = 5,
            int totalRowTemplateIndex = 6,
            string noInfomationText = "No information available.")
        {
            FreezeRowIndex = freezeRowIndex - 1;
            FreezeColumnName = freezeColumnName;
            HeaderStartIndex = headerStartIndex - 1;
            BodyStartIndex = bodyStartIndex - 1;
            TitleIndex = titleIndex - 1;
            ColumnStartIndex = columnStartIndex - 1;
            ResultSheetIndex = resultSheetIndex - 1;
            OddRowTemplateIndex = oddRowTemplateIndex - 1;
            EvenRowTemplateIndex = evenRowTemplateIndex - 1;
            TotalRowTemplateIndex = totalRowTemplateIndex - 1;
            TemplateSheetIndex = templateSheetIndex - 1;
            NoInfomationText = noInfomationText;
        }

        public int ResultSheetIndex { get; protected set; }

        public int TemplateSheetIndex { get; protected set; }

        public int TitleIndex { get; protected set; }

        public int ColumnStartIndex { get; protected set; }

        public int HeaderStartIndex { get; protected set; }

        public int BodyStartIndex { get; protected set; }

        public int FreezeRowIndex { get; protected set; }

        public string FreezeColumnName { get; protected set; }

        public int OddRowTemplateIndex { get; protected set; }

        public int EvenRowTemplateIndex { get; protected set; }

        public int TotalRowTemplateIndex { get; protected set; }

        public Row EvenRowTemplate { get; protected set; }

        public Row OddRowTemplate { get; protected set; }

        public Row TotalRowTemplate { get; protected set; }

        public Sheet TemplateSheet { get; protected set; }

        public string NoInfomationText { get; protected set; }

        public Sheet LoadTemplateSheet(Workbook workbook)
        {
            if (workbook == null)
            {
                throw new ArgumentNullException(nameof(workbook), "Workbook cannot be null");
            }

            TemplateSheet = workbook.GetSheetAt(TemplateSheetIndex);
            OddRowTemplate = TemplateSheet.GetRow(OddRowTemplateIndex);
            EvenRowTemplate = TemplateSheet.GetRow(EvenRowTemplateIndex);
            TotalRowTemplate = TemplateSheet.GetRow(TotalRowTemplateIndex);

            return TemplateSheet;
        }

        public SpreadSheetLayout SetTitleIndex(int titleIndex)
        {
            TitleIndex = titleIndex - 1;

            return this;
        }

        public SpreadSheetLayout SetSheetIndex(
            int resultSheetIndex, 
            int templateSheetIndex)
        {
            ResultSheetIndex = resultSheetIndex - 1;
            TemplateSheetIndex = templateSheetIndex - 1;

            return this;
        }

        public SpreadSheetLayout SetFreezePanel(
            int freezeRowIndex, 
            string freezeColumnName)
        {
            FreezeColumnName = freezeColumnName;
            FreezeRowIndex = freezeRowIndex - 1;

            return this;
        }

        public SpreadSheetLayout SetRowsIndex(
            int headerStartIndex = 2, 
            int bodyStartIndex = 4)
        {
            HeaderStartIndex = headerStartIndex - 1;
            BodyStartIndex = bodyStartIndex - 1;

            return this;
        }

        public SpreadSheetLayout SetTemplateRowsIndex(
            int oddRowTemplateIndex = 4, 
            int evenRowTemplateIndex = 5, 
            int totalRowTemplateIndex = 6)
        {
            OddRowTemplateIndex = oddRowTemplateIndex - 1;
            EvenRowTemplateIndex = evenRowTemplateIndex - 1;
            TotalRowTemplateIndex = totalRowTemplateIndex - 1;

            return this;
        }

        public SpreadSheetLayout SetNoInformationText(string noInformationText)
        {
            NoInfomationText = noInformationText;

            return this;
        }
    }
}