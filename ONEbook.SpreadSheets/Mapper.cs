namespace ONEbook.SpreadSheets
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class Mapper<T> where T : class
    {
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed")]
        IDictionary<string, Func<T, object>> mappings { get; } = new Dictionary<string, Func<T, object>>();

        public virtual Mapper<T> Map(string columnName, Func<T, object> mapping)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentOutOfRangeException(nameof(columnName), "Column Name cannot be null or empty");
            }

            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping), "Mapping cannot be null or empty");
            }

            if (mappings.ContainsKey(columnName))
            {
                throw new ArgumentException($"Column {columnName} already defined mapping function.");
            }

            mappings[columnName] = mapping;

            return this;
        }

        public virtual IDictionary<string, object> GetResults(T data)
        {
            var resultObjects = new Dictionary<string, object>();

            foreach (var mapping in mappings)
            {
                resultObjects.Add(mapping.Key, mapping.Value(data));
            }

            return resultObjects;
        }

    }
}