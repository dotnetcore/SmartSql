using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql
{
    public abstract class ResultContext
    {
        public bool End { get; set; }
        public abstract Type ResultType { get; }
        public abstract bool IsList { get; }
        public bool FromCache { get; protected set; }
        public abstract void SetData(object data, bool fromCache = false);
        public abstract object GetData();
    }

    public class SingleResultContext<TResult> : ResultContext
    {
        public override Type ResultType => typeof(TResult);
        public override bool IsList => false;
        public TResult Data { get; private set; }
        public override object GetData() => Data;

        public override void SetData(object data, bool fromCache = false)
        {
            End = true;
            FromCache = fromCache;
            Data = (TResult) data;
        }
    }

    public class ListResultContext<TResult> : ResultContext
    {
        public override Type ResultType => typeof(IList<TResult>);
        public override bool IsList => true;
        public IList<TResult> Data { get; private set; }
        public override object GetData() => Data;

        public override void SetData(object data, bool fromCache = false)
        {
            End = true;
            FromCache = fromCache;
            Data = (IList<TResult>) data;
        }
    }
}