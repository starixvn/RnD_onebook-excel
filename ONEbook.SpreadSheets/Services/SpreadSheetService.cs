namespace ONEbook.SpreadSheets.Services
{
    using System;
    using System.Collections.Generic;
    using Models;

    public interface SpreadSheetService
    {
        byte[] Export<T>(
            SpreadSheet<T> spreadSheet,
            Workbook workbook,
            Func<T, int, IDictionary<int, object>> rowMapping,
            Func<T, int, IDictionary<int, object>> totalRowMapping = null) where T : class;
    }
}