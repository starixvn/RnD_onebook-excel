namespace ONEbook.SpreadSheets
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class Mapper<T> where T : class
    {
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed")]
        public IDictionary<string, Func<T, object>> Mappings { get; } = new Dictionary<string, Func<T, object>>();

        public Mapper<T> Map(string columnName, Func<T, object> mapping)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentOutOfRangeException(nameof(columnName), "Column Name cannot be null or empty");
            }

            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping), "Mapping cannot be null or empty");
            }

            if (!Mappings.ContainsKey(columnName))
            {
                Mappings[columnName] = mapping;
            }

            return this;
        }
    }
}