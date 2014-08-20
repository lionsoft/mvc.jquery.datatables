﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mvc.JQuery.Datatables;
using NUnit.Framework;

namespace Mvc.JQuery.Datatables.Tests
{
    public class FilterTests
    {
        private DataTablesParam dataTablesParam;
        private IQueryable<SomeModel> queryable;
        private Tuple<int, ColInfo>[] columns;

        [SetUp]
        public void Setup()
        {
            queryable = new List<SomeModel>()
            {
                new SomeModel()
                {
                    Category = 1,
                    DisplayName = "Cheddar",
                    Id = 123,
                    Scale = 123.456d,
                    Discounted = true
                }
            }.AsQueryable();

            dataTablesParam = new DataTablesParam();
            columns = DataTablesTypeInfo<SomeModel>.Properties.Select((p, i) =>
                Tuple.Create(i, new ColInfo(p.Item1.Name, p.Item1.PropertyType))).ToArray();
            dataTablesParam.sSearchColumns = new List<string>(columns.Select(c => null as string));
            dataTablesParam.bSearchable = new List<bool>(columns.Select(c => true));

        }


        [TestCase("asdf", typeof(string), false)] //contains, not a match
        [TestCase("Cheddar", typeof(string), true)] //contains, is match
        [TestCase("^Ched", typeof(string), true)] //startswith, is match
        [TestCase("^Cheddar", typeof(string), true)] //startswith query, is match
        [TestCase("^Cheddar$", typeof(string), true)] //exact query, is match
        [TestCase("^True$", typeof(bool), true)] //exact query, is match
        [TestCase("^False$", typeof(bool), false)] //exact query, isnt match
        [TestCase("True", typeof(bool), true)] //exact query, is match
        [TestCase("False", typeof(bool), false)] //exact query, isnt match
        [TestCase("^123$", typeof(int), true)] //exact query, is match
        [TestCase("^456$", typeof(int), false)] //exact query, isnt match
        [TestCase("123", typeof(int), true)] //query, is match
        [TestCase("456", typeof(int), false)] //query, isnt match
        public void SearchQueryTests(string searchString, Type colType, bool returnsResult)
        {
            var col = columns.First(c => c.Item2.Type == colType).Item1;

            dataTablesParam.sSearchColumns[col] = searchString;
            var result = new DataTablesResult<SomeModel>(queryable, dataTablesParam);

            var data = result.LegacyData;
            Assert.AreEqual(returnsResult, data.iTotalDisplayRecords > 0);
        }

    }
}
