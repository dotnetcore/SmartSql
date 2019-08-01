using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration.Tags.TagBuilders;
using SmartSql.Exceptions;

namespace SmartSql.Configuration.Tags
{
    public class TagBuilderFactory : ITagBuilderFactory
    {
        private readonly IDictionary<String, ITagBuilder> _tagBuilderMap = new Dictionary<String, ITagBuilder>();

        public TagBuilderFactory()
        {
            _tagBuilderMap.Add(nameof(Dynamic), new DynamicBuilder());
            _tagBuilderMap.Add(nameof(Env), new EnvBuilder());
            _tagBuilderMap.Add(nameof(For), new ForBuilder());
            _tagBuilderMap.Add(nameof(Include), new IncludeBuilder());
            _tagBuilderMap.Add(nameof(IsEmpty), new IsEmptyBuilder());
            _tagBuilderMap.Add(nameof(IsEqual), new IsEqualBuilder());
            _tagBuilderMap.Add(nameof(IsFalse), new IsFalseBuilder());
            _tagBuilderMap.Add(nameof(IsGreaterEqual), new IsGreaterEqualBuilder());
            _tagBuilderMap.Add(nameof(IsGreaterThan), new IsGreaterThanBuilder());
            _tagBuilderMap.Add(nameof(IsLessEqual), new IsLessEqualBuilder());
            _tagBuilderMap.Add(nameof(IsLessThan), new IsLessThanBuilder());
            _tagBuilderMap.Add(nameof(IsNotEmpty), new IsNotEmptyBuilder());
            _tagBuilderMap.Add(nameof(IsNotEqual), new IsNotEqualBuilder());
            _tagBuilderMap.Add(nameof(IsNotNull), new IsNotNullBuilder());
            _tagBuilderMap.Add(nameof(IsNull), new IsNullBuilder());
            _tagBuilderMap.Add(nameof(IsProperty), new IsPropertyBuilder());
            _tagBuilderMap.Add(nameof(IsNotProperty), new IsNotPropertyBuilder());
            _tagBuilderMap.Add(nameof(IsTrue), new IsTrueBuilder());
            _tagBuilderMap.Add(nameof(Placeholder), new PlaceholderBuilder());
            _tagBuilderMap.Add(nameof(Range), new RangeBuilder());
            _tagBuilderMap.Add(nameof(Set), new SetBuilder());
            _tagBuilderMap.Add("#text", new SqlTextBuilder());
            _tagBuilderMap.Add("#cdata-section", new SqlTextBuilder());
            _tagBuilderMap.Add(nameof(Switch), new SwitchBuilder());
            _tagBuilderMap.Add(nameof(Switch.Case), new SwitchCaseBuilder());
            _tagBuilderMap.Add(nameof(Switch.Default), new SwitchDefaultBuilder());
            _tagBuilderMap.Add(nameof(Where), new WhereBuilder());
            _tagBuilderMap.Add(nameof(IdGenerator), new IdGeneratorBuilder());
            _tagBuilderMap.Add(nameof(OrderBy), new OrderByBuilder());
            _tagBuilderMap.Add(nameof(Now), new NowBuilder());
            _tagBuilderMap.Add(nameof(UUID), new UUIDBuilder());
        }

        public ITagBuilder Get(string nodeName)
        {
            if (!_tagBuilderMap.ContainsKey(nodeName))
            {
                throw new SmartSqlException($"Not support Node.Name:{nodeName}!");
            }

            return _tagBuilderMap[nodeName];
        }

        public void Register(string nodeName, ITagBuilder tagBuilder)
        {
            _tagBuilderMap.Add(nodeName, tagBuilder);
        }
    }
}