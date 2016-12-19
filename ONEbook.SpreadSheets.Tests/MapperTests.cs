namespace ONEbook.SpreadSheets.Tests
{
    using System;
    using System.Collections.Generic;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MapperTests
    {
        private Mapper<object> mapper;
        private CompareLogic compareLogic;

        [SetUp]
        public void SetUp()
        {
            mapper = Substitute.ForPartsOf<Mapper<object>>();
            compareLogic = new CompareLogic();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Map_ColumnNameIsNullOrWhiteSpace_ThrowArgumentOutOfRangeException(string columnName)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => mapper.Map(columnName, data => data));
        }

        [Test]
        public void Map_MappingIsNull_ThrowArgumentNullException()
        {
            // Arrange
            Func<object, object> mapping = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => mapper.Map(Column.A, mapping));
        }

        [Test]
        public void Map_ColumnMappingFuncIsDefined_ThrowArgumentException()
        {
            // Arrange
            mapper.Mappings.Returns(new Dictionary<string, Func<object, object>>
            {
                { Column.A, data => data }
            });

            // Act & Assert
            Assert.Throws<ArgumentException>(() => mapper.Map(Column.A, data => data));
        }

        [Test]
        public void Map_ColumnMappingFuncIsNotDefined_AddMappingFunc()
        {
            // Arrange
            Func<object, object> mapping = data => data;

            // Act
            mapper.Map(Column.A, mapping);

            // Assert
            Assert.IsTrue(compareLogic.Compare(mapping, mapper.Mappings[Column.A]).AreEqual);
        }
    }
}