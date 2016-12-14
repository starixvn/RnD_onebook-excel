namespace ONEbook.SpreadSheets
{
    public class TemplateInfo
    {
        public TemplateInfo(
            string templatePath,
            string noInfomationText = "No information available.",
            int titleStartIndex = 1,
            int headerStartIndex = 2,
            int bodyStartIndex = 4,
            int bodyEndIndex = 5,
            int freezeRowIndex = 0,
            string freezeColumnName = "",
            string columnStartName = Column.A,
            int sheetIndex = 1,
            int footerEndIndex = 6)
        {
            TemplatePath = templatePath;
            FreezeRowIndex = freezeRowIndex - 1;
            FreezeColumnName = freezeColumnName;
            HeaderStartIndex = headerStartIndex - 1;
            BodyStartIndex = bodyStartIndex - 1;
            BodyEndIndex = bodyEndIndex - 1;
            TitleStartIndex = titleStartIndex - 1;
            ColumnStartName = columnStartName;
            SheetIndex = sheetIndex - 1;
            FooterEndIndex = footerEndIndex - 1;
            NoInfomationText = noInfomationText;
        }

        public string TemplatePath { get; protected set; }

        public int SheetIndex { get; protected set; }

        public int TitleStartIndex { get; protected set; }

        public string ColumnStartName { get; protected set; }

        public int HeaderStartIndex { get; protected set; }

        public int HeaderEndIndex => BodyStartIndex - 1;

        public int BodyStartIndex { get; protected set; }

        public int BodyEndIndex { get; protected set; }

        public int FreezeRowIndex { get; protected set; }

        public string FreezeColumnName { get; protected set; }

        public int OddRowTemplateIndex => BodyStartIndex;

        public int EvenRowTemplateIndex => BodyEndIndex;

        public int FooterStartIndex => BodyEndIndex + 1;

        public int FooterEndIndex { get; protected set; }

        public string NoInfomationText { get; protected set; }

        public TemplateInfo Title(int titleIndex)
        {
            TitleStartIndex = titleIndex - 1;

            return this;
        }

        public TemplateInfo Sheet(int sheetIndex)
        {
            SheetIndex = sheetIndex - 1;

            return this;
        }

        public TemplateInfo FreezePanel(
            int freezeRowIndex,
            string freezeColumnName)
        {
            FreezeColumnName = freezeColumnName;
            FreezeRowIndex = freezeRowIndex - 1;

            return this;
        }

        public TemplateInfo Header(int headerStartIndex = 2)
        {
            HeaderStartIndex = headerStartIndex;

            return this;
        }

        public TemplateInfo Body(
            int bodyStartIndex = 4,
            int bodyEndIndex = 6)
        {
            BodyStartIndex = bodyStartIndex - 1;
            bodyEndIndex = bodyEndIndex - 1;

            return this;
        }

        public TemplateInfo Footer(int footerEndIndex = 6)
        {
            FooterEndIndex = footerEndIndex - 1;

            return this;
        }

        public TemplateInfo NoInformationText(string noInformationText)
        {
            NoInfomationText = noInformationText;

            return this;
        }
    }
}