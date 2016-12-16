namespace ONEbook.SpreadSheets.NPOI
{
    using System.IO;
    using global::NPOI.HSSF.UserModel;

    public class NpoiWorkbook<T> : Workbook<T> where T : class
    {
        public HSSFWorkbook Workbook { get; private set; }

        protected override Sheet GetSheetAt(int sheetIndex)
        {
            return new NpoiSheet(Workbook.GetSheetAt(sheetIndex));
        }

        protected override Workbook<T> Load(string templatePath)
        {
            using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                Workbook = new HSSFWorkbook(file);
            }

            return this;
        }

        protected override void FillSheetName(int index, string name)
        {
            Workbook.SetSheetName(index, name);
        }

        protected override byte[] ToBytes()
        {
            using (var memoryStream = new MemoryStream())
            {
                Workbook.Write(memoryStream);

                return memoryStream.GetBuffer();
            }
        }

        protected override void RemoveSheetAt(int sheetIndex)
        {
            Workbook.RemoveSheetAt(sheetIndex);
        }
    }
}