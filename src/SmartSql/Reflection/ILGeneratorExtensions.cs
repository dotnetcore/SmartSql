using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace System.Reflection.Emit
{
    public static class ILGeneratorExtensions
    {
        public static void Pop(this ILGenerator ilGen)
        {
            ilGen.Emit(OpCodes.Pop);
        }

        public static void Dup(this ILGenerator ilGen)
        {
            ilGen.Emit(OpCodes.Dup);
        }

        public static void Call(this ILGenerator ilGen, MethodInfo methodInfo)
        {
            ilGen.Emit(OpCodes.Call, methodInfo);
        }

        public static void Call(this ILGenerator ilGen, ConstructorInfo ctorInfo)
        {
            ilGen.Emit(OpCodes.Call, ctorInfo);
        }

        public static void Callvirt(this ILGenerator ilGen, MethodInfo methodInfo)
        {
            ilGen.Emit(OpCodes.Callvirt, methodInfo);
        }

        public static void LoadType(this ILGenerator ilGen, Type type)
        {
            ilGen.Emit(OpCodes.Ldtoken, type);
            ilGen.Emit(OpCodes.Call, SmartSql.Reflection.TypeConstants.TypeType.Method.GetTypeFromHandle);
        }

        public static void New(this ILGenerator ilGen, ConstructorInfo ctorInfo)
        {
            ilGen.Emit(OpCodes.Newobj, ctorInfo);
        }

        public static void Return(this ILGenerator ilGen)
        {
            ilGen.Emit(OpCodes.Ret);
        }

        public static void LoadNull(this ILGenerator ilGen)
        {
            ilGen.Emit(OpCodes.Ldnull);
        }

        public static void IfTrue(this ILGenerator ilGen, Label target)
        {
            ilGen.Emit(OpCodes.Brtrue, target);
        }

        public static void IfFalse(this ILGenerator ilGen, Label target)
        {
            ilGen.Emit(OpCodes.Brfalse, target);
        }

        public static void IfTrueS(this ILGenerator ilGen, Label target)
        {
            ilGen.Emit(OpCodes.Brtrue_S, target);
        }

        public static void IfFalseS(this ILGenerator ilGen, Label target)
        {
            ilGen.Emit(OpCodes.Brfalse_S, target);
        }

        public static void LoadString(this ILGenerator ilGen, string value)
        {
            ilGen.Emit(OpCodes.Ldstr, value);
        }

        public static void Unbox(this ILGenerator ilGen, Type type)
        {
            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }

            ilGen.Emit(OpCodes.Unbox, type);
        }

        public static void Box(this ILGenerator ilGen, Type type)
        {
            ilGen.Emit(OpCodes.Box, type);
        }

        public static void LoadValueIndirect(this ILGenerator ilGen, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                    ilGen.Emit(OpCodes.Ldind_I1);
                    break;
                case TypeCode.Boolean:
                case TypeCode.SByte:
                    ilGen.Emit(OpCodes.Ldind_U1);
                    break;
                case TypeCode.Int16:
                    ilGen.Emit(OpCodes.Ldind_I2);
                    break;
                case TypeCode.Char:
                case TypeCode.UInt16:
                    ilGen.Emit(OpCodes.Ldind_U2);
                    break;
                case TypeCode.Int32:
                    ilGen.Emit(OpCodes.Ldind_I4);
                    break;
                case TypeCode.UInt32:
                    ilGen.Emit(OpCodes.Ldind_U4);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    ilGen.Emit(OpCodes.Ldind_I8);
                    break;
                case TypeCode.Single:
                    ilGen.Emit(OpCodes.Ldind_R4);
                    break;
                case TypeCode.Double:
                    ilGen.Emit(OpCodes.Ldind_R8);
                    break;
                default:
                    if (type.IsValueType)
                    {
                        ilGen.Emit(OpCodes.Ldobj, type);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Ldind_Ref);
                    }

                    break;
            }
        }


        public static void StoreValueIndirect(this ILGenerator ilGen, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    ilGen.Emit(OpCodes.Stind_I1);
                    break;
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    ilGen.Emit(OpCodes.Stind_I2);
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    ilGen.Emit(OpCodes.Stind_I4);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    ilGen.Emit(OpCodes.Stind_I8);
                    break;
                case TypeCode.Single:
                    ilGen.Emit(OpCodes.Stind_R4);
                    break;
                case TypeCode.Double:
                    ilGen.Emit(OpCodes.Stind_R8);
                    break;
                default:
                    if (type.IsValueType)
                    {
                        ilGen.Emit(OpCodes.Stobj, type);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Stind_Ref);
                    }

                    break;
            }
        }

        public static void Add(this ILGenerator ilGen)
        {
            ilGen.Emit(OpCodes.Add);
        }

        public static void LoadInt32(this ILGenerator ilGen, int value)
        {
            switch (value)
            {
                case -1:
                    ilGen.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    ilGen.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    ilGen.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    ilGen.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    ilGen.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    ilGen.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    ilGen.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    ilGen.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    ilGen.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    ilGen.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                    {
                        ilGen.Emit(OpCodes.Ldc_I4_S, (sbyte) value);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Ldc_I4, value);
                    }

                    break;
            }
        }

        public static void LoadArg(this ILGenerator ilGen, int index)
        {
            if (index < 0 || index >= short.MaxValue) throw new ArgumentNullException(nameof(index));
            switch (index)
            {
                case 0:
                    ilGen.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    ilGen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    ilGen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    ilGen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= byte.MaxValue)
                    {
                        ilGen.Emit(OpCodes.Ldarg_S, (byte) index);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Ldarg, (short) index);
                    }

                    break;
            }
        }

        /// <summary>
        /// Ldloc
        /// </summary>
        /// <param name="ilGen"></param>
        /// <param name="index"></param>
        public static void LoadLocalVar(this ILGenerator ilGen, int index)
        {
            if (index < 0 || index >= short.MaxValue) throw new ArgumentNullException(nameof(index));
            switch (index)
            {
                case 0:
                    ilGen.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    ilGen.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    ilGen.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    ilGen.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (index <= 255)
                    {
                        ilGen.Emit(OpCodes.Ldloc_S, (byte) index);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Ldloc, (short) index);
                    }

                    break;
            }
        }

        public static void StoreLocalVar(this ILGenerator ilGen, int index)
        {
            if (index < 0 || index >= short.MaxValue) throw new ArgumentNullException(nameof(index));
            switch (index)
            {
                case 0:
                    ilGen.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    ilGen.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    ilGen.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    ilGen.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    if (index <= 255)
                    {
                        ilGen.Emit(OpCodes.Stloc_S, (byte) index);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Stloc, (short) index);
                    }

                    break;
            }
        }

        public static void StoreElement(this ILGenerator ilGen, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    ilGen.Emit(OpCodes.Stelem_I1);
                    break;
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    ilGen.Emit(OpCodes.Stelem_I2);
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    ilGen.Emit(OpCodes.Stelem_I4);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    ilGen.Emit(OpCodes.Stelem_I8);
                    break;
                case TypeCode.Single:
                    ilGen.Emit(OpCodes.Stelem_R4);
                    break;
                case TypeCode.Double:
                    ilGen.Emit(OpCodes.Stelem_R8);
                    break;
                default:
                    if (type.IsValueType)
                    {
                        ilGen.Emit(OpCodes.Stelem, type);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Stelem_Ref);
                    }

                    break;
            }
        }

        public static void LoadElement(this ILGenerator il, Type type)
        {
            if (!type.IsValueType)
            {
                il.Emit(OpCodes.Ldelem_Ref);
            }
            else
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                    case TypeCode.SByte:
                        il.Emit(OpCodes.Ldelem_I1);
                        break;
                    case TypeCode.Byte:
                        il.Emit(OpCodes.Ldelem_U1);
                        break;
                    case TypeCode.Int16:
                        il.Emit(OpCodes.Ldelem_I2);
                        break;
                    case TypeCode.Char:
                    case TypeCode.UInt16:
                        il.Emit(OpCodes.Ldelem_U2);
                        break;
                    case TypeCode.Int32:
                        il.Emit(OpCodes.Ldelem_I4);
                        break;
                    case TypeCode.UInt32:
                        il.Emit(OpCodes.Ldelem_U4);
                        break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        il.Emit(OpCodes.Ldelem_I8);
                        break;
                    case TypeCode.Single:
                        il.Emit(OpCodes.Ldelem_R4);
                        break;
                    case TypeCode.Double:
                        il.Emit(OpCodes.Ldelem_R8);
                        break;
                    default:
                        il.Emit(OpCodes.Ldelem, type);
                        break;
                }
            }
        }

        public static void FieldGet(this ILGenerator ilGen, FieldInfo fi)
        {
            ilGen.Emit(fi.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, fi);
        }

        public static void FieldSet(this ILGenerator ilGen, FieldInfo fi)
        {
            ilGen.Emit(fi.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, fi);
        }
    }
}