namespace ONEbook.SpreadSheets.NPOI
{
    using System.IO;
    using global::NPOI.HSSF.UserModel;
    using global::NPOI.SS.UserModel;

    public class NpoiWorkbook<T> : Workbook<T> where T : class
    {
        public IWorkbook Workbook { get; internal set; }

        protected override Sheet GetSheetAt(int sheetIndex)
        {
            return new NpoiSheet(Workbook.GetSheetAt(sheetIndex));
        }

        protected override Workbook<T> Load(string templatePath)
        {
            using (FileStream file = GetFileStream(templatePath))
            {
                Workbook = new HSSFWorkbook(file);
            }

            return this;
        }

        internal virtual FileStream GetFileStream(string templatePath)
        {
            return new FileStream(templatePath, FileMode.Open, FileAccess.Read);
        }

        protected override void FillSheetName(int index, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Workbook.SetSheetName(index, name);
            }
        }

        protected override byte[] ToBytes()
        {
            using (var memoryStream = GetMemoryStream())
            {
                Workbook.Write(memoryStream);

                return memoryStream.GetBuffer();
            }
        }

        internal virtual MemoryStream GetMemoryStream()
        {
            return new MemoryStream();
        }

        protected override void RemoveSheetAt(int sheetIndex)
        {
            Workbook.RemoveSheetAt(sheetIndex);
        }
    }
}