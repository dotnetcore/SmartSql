using System;
using System.Collections.Generic;
using SmartSql.Exceptions;
using SmartSql.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace SmartSql.Test.Unit.Reflection
{
    public class PropertyTokenizerTest
    {
        private readonly ITestOutputHelper _outputHelper;

        public PropertyTokenizerTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void Access()
        {
            var propertyTokenizer = new PropertyTokenizer("User");
            var current = propertyTokenizer.Current;
            Assert.Equal("User", current.Name);
            Assert.Null(current.Index);
            Assert.Null(current.Children);

            Assert.False(propertyTokenizer.MoveNext());
        }

        [Fact]
        public void AccessNest1()
        {
            var propertyTokenizer = new PropertyTokenizer("User.Name");
            
            var current = propertyTokenizer.Current;
            Assert.Equal("User", current.Name);
            Assert.Null(current.Index);
            Assert.Equal("Name", current.Children);

            propertyTokenizer.MoveNext();
            current = propertyTokenizer.Current;
            Assert.Equal("Name", current.Name);
            Assert.Null(current.Index);
            Assert.Null(current.Children);

            Assert.False(propertyTokenizer.MoveNext());
        }

        [Fact]
        public void AccessNest2()
        {
            var propertyTokenizer = new PropertyTokenizer("User.Info.Id");
            
            var current = propertyTokenizer.Current;
            Assert.Equal("User", current.Name);
            Assert.Null(current.Index);
            Assert.Equal("Info.Id", current.Children);

            propertyTokenizer.MoveNext();
            current = propertyTokenizer.Current;
            Assert.Equal("Info", current.Name);
            Assert.Null(current.Index);
            Assert.Equal("Id", current.Children);

            propertyTokenizer.MoveNext();
            current = propertyTokenizer.Current;
            Assert.Equal("Id", current.Name);
            Assert.Null(current.Index);
            Assert.Null(current.Children);

            Assert.False(propertyTokenizer.MoveNext());
        }

        [Fact]
        public void IndexAccess()
        {
            var propertyTokenizer = new PropertyTokenizer("Items[0]");

            var current = propertyTokenizer.Current;
            Assert.Equal("Items", current.Name);
            Assert.Equal("0", current.Index);
            Assert.Null(current.Children);

            Assert.False(propertyTokenizer.MoveNext());
        }

        [Fact]
        public void IndexAccessNest1()
        {
            var propertyTokenizer = new PropertyTokenizer("Order.Items[0]");

            var current = propertyTokenizer.Current;
            Assert.Equal("Order", current.Name);
            Assert.Null(current.Index);
            Assert.Equal("Items[0]", current.Children);

            propertyTokenizer.MoveNext();
            current = propertyTokenizer.Current;
            Assert.Equal("Items", current.Name);
            Assert.Equal("0", current.Index);
            Assert.Null(current.Children);

            Assert.False(propertyTokenizer.MoveNext());
        }

        [Fact]
        public void IndexAccessString()
        {
            var propertyTokenizer = new PropertyTokenizer("Items[Name]");
            var current = propertyTokenizer.Current;
            Assert.Equal("Items", current.Name);
            Assert.Equal("Name",current.Index);
            Assert.Null(current.Children);
            
        }
    }
}