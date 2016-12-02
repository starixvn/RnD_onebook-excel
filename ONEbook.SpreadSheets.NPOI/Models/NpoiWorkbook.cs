namespace ONEbook.SpreadSheets.NPOI.Models
{
    using System.IO;
    using global::NPOI.HSSF.UserModel;
    using SpreadSheets.Models;

    public class NpoiWorkbook : Workbook
    {
        public NpoiWorkbook(HSSFWorkbook workbook)
        {
            Workbook = workbook;
        }

        public NpoiWorkbook(string templatePath)
        {
            Load(templatePath);
        }

        public HSSFWorkbook Workbook { get; private set; }

        public Sheet GetSheetAt(int sheetIndex)
        {
            return new NpoiSheet(Workbook.GetSheetAt(sheetIndex));
        }

        public void Load(string templatePath)
        {
            using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                Workbook = new HSSFWorkbook(file);
            }
        }

        public void SetSheetName(int index, string name)
        {
            Workbook.SetSheetName(index, name);
        }

        public byte[] ToBytes()
        {
            using (var memoryStream = new MemoryStream())
            {
                Workbook.Write(memoryStream);

                return memoryStream.GetBuffer();
            }
        }

        public void RemoveSheetAt(int sheetIndex)
        {
            Workbook.RemoveSheetAt(sheetIndex);
        }
    }
}