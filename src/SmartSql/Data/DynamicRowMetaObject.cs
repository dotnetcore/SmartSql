using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SmartSql.Data
{
    public class DynamicRowMetaObject : DynamicMetaObject
    {
        private static readonly MethodInfo getValueMethod = typeof(IDictionary<string, object>).GetProperty("Item").GetGetMethod();
        private static readonly MethodInfo setValueMethod = typeof(DynamicRow).GetMethod("SetValue", new Type[] { typeof(string), typeof(object) });

        public DynamicRowMetaObject(
           Expression expression,
            BindingRestrictions restrictions
            )
            : base(expression, restrictions)
        {
        }

        public DynamicRowMetaObject(
            Expression expression,
            BindingRestrictions restrictions,
            object value
            )
            : base(expression, restrictions, value)
        {
        }

        private DynamicMetaObject CallMethod(
            MethodInfo method,
            Expression[] parameters
            )
        {
            var callMethod = new DynamicMetaObject(
                Expression.Call(
                    Expression.Convert(Expression, LimitType),
                    method,
                    parameters),
                BindingRestrictions.GetTypeRestriction(Expression, LimitType)
                );
            return callMethod;
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var parameters = new Expression[]
                                 {
                                         Expression.Constant(binder.Name)
                                 };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            var parameters = new Expression[]
                                 {
                                         Expression.Constant(binder.Name)
                                 };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var parameters = new Expression[]
                                 {
                                         Expression.Constant(binder.Name),
                                         value.Expression,
                                 };

            var callMethod = CallMethod(setValueMethod, parameters);

            return callMethod;
        }
    }
}
