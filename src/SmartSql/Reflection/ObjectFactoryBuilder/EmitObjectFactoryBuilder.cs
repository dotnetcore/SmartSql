using SmartSql.Reflection.TypeConstants;
using System;
using System.Reflection.Emit;
using SmartSql.CUD;

namespace SmartSql.Reflection.ObjectFactoryBuilder
{
    public class EmitObjectFactoryBuilder : AbstractObjectFactoryBuilder
    {
        public static readonly IObjectFactoryBuilder Instance = new EmitObjectFactoryBuilder();

        public override Func<object[], object> Build(Type targetType, Type[] ctorArgTypes)
        {
            var dyMethodName = $"{targetType.Name}_{Guid.NewGuid():N}";

            DynamicMethod dyMethod = new DynamicMethod(dyMethodName, targetType, new Type[] {CommonType.ObjectArray},
                targetType, true);
            var ilGen = dyMethod.GetILGenerator();

            #region Init Arg

            for (int i = 0; i < ctorArgTypes.Length; i++)
            {
                Type argType = ctorArgTypes[i];
                ilGen.LoadArg(0);
                ilGen.LoadInt32(i);
                ilGen.LoadElement(CommonType.Object);
                if (argType.IsValueType)
                {
                    ilGen.Unbox(argType);
                    ilGen.LoadValueIndirect(argType);
                }
            }

            #endregion

            var targetCtor = GetConstructor(targetType, ctorArgTypes);
            ilGen.New(targetCtor);
            ilGen.Return();
            return (Func<object[], object>) dyMethod.CreateDelegate(typeof(Func<object[], object>));
        }
    }
}