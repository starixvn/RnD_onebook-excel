namespace ONEbook.SpreadSheets
{
    public class SpreadSheetLayout
    {
        public SpreadSheetLayout(
            string noInfomationText = "No information available.",
            int titleIndex = 1,
            int headerStartIndex = 2,
            int bodyStartIndex = 4,
            int freezeRowIndex = 0,
            string freezeColumnName = "",
            int columnStartIndex = 1,
            int resultSheetIndex = 1,
            int oddRowTemplateIndex = 4,
            int evenRowTemplateIndex = 5,
            int footerRowsTemplateStartIndex = 6,
            int footerRowsTemplateEndIndex = 6)
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
            FooterRowsTemplateStartIndex = footerRowsTemplateStartIndex - 1;
            FooterRowsTemplateEndIndex = footerRowsTemplateEndIndex - 1;
            NoInfomationText = noInfomationText;
        }

        public int ResultSheetIndex { get; protected set; }

        public int TitleIndex { get; protected set; }

        public int ColumnStartIndex { get; protected set; }

        public int HeaderStartIndex { get; protected set; }

        public int HeaderEndIndex => BodyStartIndex - 1;

        public int BodyStartIndex { get; protected set; }

        public int FreezeRowIndex { get; protected set; }

        public string FreezeColumnName { get; protected set; }

        public int OddRowTemplateIndex { get; protected set; }

        public int EvenRowTemplateIndex { get; protected set; }

        public int FooterRowsTemplateStartIndex { get; protected set; }

        public int FooterRowsTemplateEndIndex { get; protected set; }

        public string NoInfomationText { get; protected set; }

        public SpreadSheetLayout SetTitleIndex(int titleIndex)
        {
            TitleIndex = titleIndex - 1;

            return this;
        }

        public SpreadSheetLayout SetSheetIndex(int resultSheetIndex)
        {
            ResultSheetIndex = resultSheetIndex - 1;

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
            int footerRowsTemplateStartIndex = 6,
            int footerRowsTemplateEndIndex = 6)
        {
            OddRowTemplateIndex = oddRowTemplateIndex - 1;
            EvenRowTemplateIndex = evenRowTemplateIndex - 1;
            FooterRowsTemplateStartIndex = footerRowsTemplateStartIndex - 1;
            FooterRowsTemplateEndIndex = footerRowsTemplateEndIndex - 1;

            return this;
        }

        public SpreadSheetLayout SetNoInformationText(string noInformationText)
        {
            NoInfomationText = noInformationText;

            return this;
        }
    }
}