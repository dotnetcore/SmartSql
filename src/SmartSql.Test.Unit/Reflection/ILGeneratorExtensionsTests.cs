using System;
using System.Reflection;
using System.Reflection.Emit;
using FluentAssertions;
using Xunit;

namespace SmartSql.Test.Unit.Reflection;

public class ILGeneratorExtensionsTests
{
    #region LoadInt32

    [Fact]
    public void Should_LoadInt32_When_ValueIsZero()
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            il.LoadInt32(0);
            il.Return();
        });

        var result = method();

        result.Should().Be(0);
    }

    [Fact]
    public void Should_LoadInt32_When_ValueIsOne()
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            il.LoadInt32(1);
            il.Return();
        });

        var result = method();

        result.Should().Be(1);
    }

    [Fact]
    public void Should_LoadInt32_When_ValueIsNegativeOne()
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            il.LoadInt32(-1);
            il.Return();
        });

        var result = method();

        result.Should().Be(-1);
    }

    [Fact]
    public void Should_LoadInt32_When_ValueIsShortForm()
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            il.LoadInt32(42);
            il.Return();
        });

        var result = method();

        result.Should().Be(42);
    }

    [Fact]
    public void Should_LoadInt32_When_ValueIsFullForm()
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            il.LoadInt32(500);
            il.Return();
        });

        var result = method();

        result.Should().Be(500);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void Should_LoadInt32_When_OptimizedValues(int value)
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            il.LoadInt32(value);
            il.Return();
        });

        var result = method();

        result.Should().Be(value);
    }

    #endregion

    #region LoadString

    [Fact]
    public void Should_LoadString_When_ValueProvided()
    {
        var method = CreateDynamicMethod<Func<string>>(il =>
        {
            il.LoadString("Hello, SmartSql!");
            il.Return();
        });

        var result = method();

        result.Should().Be("Hello, SmartSql!");
    }

    [Fact]
    public void Should_LoadString_When_EmptyString()
    {
        var method = CreateDynamicMethod<Func<string>>(il =>
        {
            il.LoadString(string.Empty);
            il.Return();
        });

        var result = method();

        result.Should().BeEmpty();
    }

    #endregion

    #region LoadNull

    [Fact]
    public void Should_LoadNull_When_Called()
    {
        var method = CreateDynamicMethod<Func<object>>(il =>
        {
            il.LoadNull();
            il.Return();
        });

        var result = method();

        result.Should().BeNull();
    }

    #endregion

    #region New

    [Fact]
    public void Should_NewObject_When_DefaultConstructor()
    {
        var ctor = typeof(TestEntity).GetConstructor(Type.EmptyTypes);
        var method = CreateDynamicMethod<Func<TestEntity>>(il =>
        {
            il.New(ctor);
            il.Return();
        });

        var result = method();

        result.Should().NotBeNull();
        result.Value.Should().Be(0);
    }

    [Fact]
    public void Should_NewObject_When_ParameterizedConstructor()
    {
        var ctor = typeof(TestEntity).GetConstructor(new[] { typeof(int) });
        var method = CreateDynamicMethod<Func<int, TestEntity>>(il =>
        {
            il.LoadArg(0);
            il.New(ctor);
            il.Return();
        });

        var result = method(42);

        result.Should().NotBeNull();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Should_NewObject_When_UsingStringConstructor()
    {
        var ctor = typeof(TestEntity).GetConstructor(new[] { typeof(string) });
        var method = CreateDynamicMethod<Func<string, TestEntity>>(il =>
        {
            il.LoadArg(0);
            il.New(ctor);
            il.Return();
        });

        var result = method("TestName");

        result.Should().NotBeNull();
        result.Name.Should().Be("TestName");
    }

    #endregion

    #region Call

    [Fact]
    public void Should_CallStaticMethod_When_MethodInfo()
    {
        var concatMethod = typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });
        var method = CreateDynamicMethod<Func<string>>(il =>
        {
            il.LoadString("Hello");
            il.LoadString(" World");
            il.Call(concatMethod);
            il.Return();
        });

        var result = method();

        result.Should().Be("Hello World");
    }

    [Fact]
    public void Should_CallInstanceMethod_When_VirtualCall()
    {
        var toStringMethod = typeof(object).GetMethod(nameof(object.ToString));
        var method = CreateDynamicMethod<Func<string>>(il =>
        {
            il.LoadString("test");
            il.Callvirt(toStringMethod);
            il.Return();
        });

        var result = method();

        result.Should().Be("test");
    }

    #endregion

    #region Return

    [Fact]
    public void Should_Return_When_ReturnCalled()
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            il.LoadInt32(123);
            il.Return();
        });

        var result = method();

        result.Should().Be(123);
    }

    #endregion

    #region Box / Unbox

    [Fact]
    public void Should_Box_When_ValueTypeToInt32()
    {
        var method = CreateDynamicMethod<Func<object>>(il =>
        {
            il.LoadInt32(99);
            il.Box(typeof(int));
            il.Return();
        });

        var result = method();

        result.Should().Be(99);
        result.Should().BeOfType<int>();
    }

    [Fact]
    public void Should_Unbox_When_ObjectToValueType()
    {
        var method = CreateDynamicMethod<Func<object, int>>(il =>
        {
            il.LoadArg(0);
            il.Unbox(typeof(int));
            il.Emit(OpCodes.Ldobj, typeof(int));
            il.Return();
        });

        var result = method(42);

        result.Should().Be(42);
    }

    [Fact]
    public void Should_Unbox_When_EnumType()
    {
        var method = CreateDynamicMethod<Func<object, TestEnum>>(il =>
        {
            il.LoadArg(0);
            il.Unbox(typeof(TestEnum));
            il.Emit(OpCodes.Ldobj, typeof(TestEnum));
            il.Return();
        });

        var result = method(TestEnum.ValueB);

        result.Should().Be(TestEnum.ValueB);
    }

    #endregion

    #region LoadArg

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Should_LoadArg_When_ShortFormIndex(int index)
    {
        var method = new DynamicMethod("LoadArgTest", typeof(int),
            new[] { typeof(int), typeof(int), typeof(int), typeof(int) },
            restrictedSkipVisibility: true);
        var il = method.GetILGenerator();
        il.LoadArg(index);
        il.Return();

        var invoke = (Func<int, int, int, int, int>)method.CreateDelegate(typeof(Func<int, int, int, int, int>));
        var result = invoke(10, 20, 30, 40);

        result.Should().Be((index + 1) * 10);
    }

    [Fact]
    public void Should_LoadArg_When_IndexGreaterThanThree()
    {
        var method = new DynamicMethod("LoadArgTest5", typeof(int),
            new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) },
            restrictedSkipVisibility: true);
        var il = method.GetILGenerator();
        il.LoadArg(4);
        il.Return();

        var invoke = (Func<int, int, int, int, int, int>)method.CreateDelegate(typeof(Func<int, int, int, int, int, int>));
        var result = invoke(10, 20, 30, 40, 50);

        result.Should().Be(50);
    }

    [Fact]
    public void Should_Throw_When_LoadArgWithNegativeIndex()
    {
        var method = new DynamicMethod("LoadArgNegative", typeof(void), Type.EmptyTypes, restrictedSkipVisibility: true);
        var il = method.GetILGenerator();

        var act = () => il.LoadArg(-1);

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region LoadLocalVar / StoreLocalVar

    [Fact]
    public void Should_StoreAndLoadLocalVar_When_IndexZero()
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            var local = il.DeclareLocal(typeof(int));
            il.LoadInt32(77);
            il.StoreLocalVar(local.LocalIndex);
            il.LoadLocalVar(local.LocalIndex);
            il.Return();
        });

        var result = method();

        result.Should().Be(77);
    }

    [Fact]
    public void Should_StoreAndLoadLocalVar_When_HighIndex()
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            var local = il.DeclareLocal(typeof(int));
            il.LoadInt32(256);
            il.StoreLocalVar(local.LocalIndex);
            il.LoadLocalVar(local.LocalIndex);
            il.Return();
        });

        var result = method();

        result.Should().Be(256);
    }

    [Fact]
    public void Should_Throw_When_StoreLocalVarWithNegativeIndex()
    {
        var method = new DynamicMethod("StoreLocalNegative", typeof(void), Type.EmptyTypes, restrictedSkipVisibility: true);
        var il = method.GetILGenerator();

        var act = () => il.StoreLocalVar(-1);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_Throw_When_LoadLocalVarWithNegativeIndex()
    {
        var method = new DynamicMethod("LoadLocalNegative", typeof(void), Type.EmptyTypes, restrictedSkipVisibility: true);
        var il = method.GetILGenerator();

        var act = () => il.LoadLocalVar(-1);

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region StoreElement / LoadElement

    [Fact]
    public void Should_StoreElement_When_Int32Array()
    {
        var method = CreateDynamicMethod<Func<int[]>>(il =>
        {
            il.LoadInt32(3);
            il.Emit(OpCodes.Newarr, typeof(int));
            il.Dup();
            il.LoadInt32(0);
            il.LoadInt32(10);
            il.StoreElement(typeof(int));
            il.Dup();
            il.LoadInt32(1);
            il.LoadInt32(20);
            il.StoreElement(typeof(int));
            il.Dup();
            il.LoadInt32(2);
            il.LoadInt32(30);
            il.StoreElement(typeof(int));
            il.Return();
        });

        var result = method();

        result.Should().Equal(10, 20, 30);
    }

    [Fact]
    public void Should_LoadElement_When_Int32Array()
    {
        var method = CreateDynamicMethod<Func<int[], int>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(0);
            il.LoadElement(typeof(int));
            il.Return();
        });

        var result = method(new[] { 42, 43, 44 });

        result.Should().Be(42);
    }

    [Fact]
    public void Should_StoreElement_When_StringArray()
    {
        var method = CreateDynamicMethod<Func<string[]>>(il =>
        {
            il.LoadInt32(2);
            il.Emit(OpCodes.Newarr, typeof(string));
            il.Dup();
            il.LoadInt32(0);
            il.LoadString("first");
            il.StoreElement(typeof(string));
            il.Dup();
            il.LoadInt32(1);
            il.LoadString("second");
            il.StoreElement(typeof(string));
            il.Return();
        });

        var result = method();

        result.Should().Equal("first", "second");
    }

    [Fact]
    public void Should_LoadElement_When_StringArray()
    {
        var method = CreateDynamicMethod<Func<string[], string>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(1);
            il.LoadElement(typeof(string));
            il.Return();
        });

        var result = method(new[] { "a", "b", "c" });

        result.Should().Be("b");
    }

    [Fact]
    public void Should_StoreElement_When_DoubleArray()
    {
        var method = CreateDynamicMethod<Func<double[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(double));
            il.Dup();
            il.LoadInt32(0);
            il.Emit(OpCodes.Ldc_R8, 3.14);
            il.StoreElement(typeof(double));
            il.Return();
        });

        var result = method();

        result[0].Should().BeApproximately(3.14, 0.001);
    }

    [Fact]
    public void Should_LoadElement_When_ByteArray()
    {
        var method = CreateDynamicMethod<Func<byte[], byte>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(0);
            il.LoadElement(typeof(byte));
            il.Return();
        });

        var result = method(new byte[] { 200, 100 });

        result.Should().Be(200);
    }

    [Fact]
    public void Should_StoreElement_When_Int64Array()
    {
        var method = CreateDynamicMethod<Func<long[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(long));
            il.Dup();
            il.LoadInt32(0);
            il.Emit(OpCodes.Ldc_I8, 999L);
            il.StoreElement(typeof(long));
            il.Return();
        });

        var result = method();

        result[0].Should().Be(999L);
    }

    [Fact]
    public void Should_StoreElement_When_SingleArray()
    {
        var method = CreateDynamicMethod<Func<float[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(float));
            il.Dup();
            il.LoadInt32(0);
            il.Emit(OpCodes.Ldc_R4, 2.5f);
            il.StoreElement(typeof(float));
            il.Return();
        });

        var result = method();

        result[0].Should().BeApproximately(2.5f, 0.001f);
    }

    [Fact]
    public void Should_StoreElement_When_StructValueType()
    {
        var parseMethod = typeof(DateTime).GetMethod(nameof(DateTime.Parse), new[] { typeof(string) });
        var method = CreateDynamicMethod<Func<DateTime[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(DateTime));
            il.Dup();
            il.LoadInt32(0);
            il.LoadString("2024-01-01");
            il.Call(parseMethod);
            il.StoreElement(typeof(DateTime));
            il.Return();
        });

        var result = method();

        result[0].Year.Should().Be(2024);
    }

    #endregion

    #region Add

    [Fact]
    public void Should_Add_When_TwoIntegers()
    {
        var method = CreateDynamicMethod<Func<int, int, int>>(il =>
        {
            il.LoadArg(0);
            il.LoadArg(1);
            il.Add();
            il.Return();
        });

        var result = method(10, 20);

        result.Should().Be(30);
    }

    #endregion

    #region LoadValueIndirect / StoreValueIndirect

    [Fact]
    public void Should_StoreAndLoadValueIndirect_When_Int32()
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            var local = il.DeclareLocal(typeof(int));
            il.LoadInt32(42);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(int));
            il.Return();
        });

        var result = method();

        result.Should().Be(42);
    }

    [Fact]
    public void Should_StoreValueIndirect_When_DoubleType()
    {
        var method = CreateDynamicMethod<Func<double>>(il =>
        {
            var local = il.DeclareLocal(typeof(double));
            il.Emit(OpCodes.Ldc_R8, 3.14);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.Emit(OpCodes.Ldc_R8, 2.71);
            il.StoreValueIndirect(typeof(double));
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(double));
            il.Return();
        });

        var result = method();

        result.Should().BeApproximately(2.71, 0.001);
    }

    [Fact]
    public void Should_StoreValueIndirect_When_Int64Type()
    {
        var method = CreateDynamicMethod<Func<long>>(il =>
        {
            var local = il.DeclareLocal(typeof(long));
            il.Emit(OpCodes.Ldc_I8, 100L);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.Emit(OpCodes.Ldc_I8, 200L);
            il.StoreValueIndirect(typeof(long));
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(long));
            il.Return();
        });

        var result = method();

        result.Should().Be(200L);
    }

    [Fact]
    public void Should_LoadValueIndirect_When_BooleanType()
    {
        var method = CreateDynamicMethod<Func<bool>>(il =>
        {
            var local = il.DeclareLocal(typeof(bool));
            il.LoadInt32(1);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(bool));
            il.Return();
        });

        var result = method();

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_LoadValueIndirect_When_ReferenceType()
    {
        var method = CreateDynamicMethod<Func<string>>(il =>
        {
            var local = il.DeclareLocal(typeof(string));
            il.LoadString("indirect");
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(string));
            il.Return();
        });

        var result = method();

        result.Should().Be("indirect");
    }

    [Fact]
    public void Should_StoreValueIndirect_When_SingleType()
    {
        var method = CreateDynamicMethod<Func<float>>(il =>
        {
            var local = il.DeclareLocal(typeof(float));
            il.Emit(OpCodes.Ldc_R4, 1.5f);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.Emit(OpCodes.Ldc_R4, 2.5f);
            il.StoreValueIndirect(typeof(float));
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(float));
            il.Return();
        });

        var result = method();

        result.Should().BeApproximately(2.5f, 0.001f);
    }

    #endregion

    #region LoadType

    [Fact]
    public void Should_LoadType_When_Int32Type()
    {
        var method = CreateDynamicMethod<Func<Type>>(il =>
        {
            il.LoadType(typeof(int));
            il.Return();
        });

        var result = method();

        result.Should().Be(typeof(int));
    }

    [Fact]
    public void Should_LoadType_When_StringType()
    {
        var method = CreateDynamicMethod<Func<Type>>(il =>
        {
            il.LoadType(typeof(string));
            il.Return();
        });

        var result = method();

        result.Should().Be(typeof(string));
    }

    #endregion

    #region FieldGet / FieldSet

    [Fact]
    public void Should_FieldGet_When_StaticField()
    {
        var fi = typeof(StaticFieldHolder).GetField(nameof(StaticFieldHolder.StaticValue));
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            il.FieldGet(fi);
            il.Return();
        });

        var result = method();

        result.Should().Be(StaticFieldHolder.StaticValue);
    }

    [Fact]
    public void Should_FieldSet_When_StaticField()
    {
        var fi = typeof(StaticFieldHolder).GetField(nameof(StaticFieldHolder.MutableValue));
        var original = StaticFieldHolder.MutableValue;
        try
        {
            var method = CreateDynamicMethod<Func<int, int>>(il =>
            {
                il.LoadArg(0);
                il.FieldSet(fi);
                il.FieldGet(fi);
                il.Return();
            });

            var result = method(999);

            result.Should().Be(999);
        }
        finally
        {
            StaticFieldHolder.MutableValue = original;
        }
    }

    [Fact]
    public void Should_FieldGet_When_InstanceField()
    {
        var fi = typeof(TestEntity).GetField(nameof(TestEntity.PublicField));
        var ctor = typeof(TestEntity).GetConstructor(new[] { typeof(int) });
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            il.LoadInt32(0);
            il.New(ctor);
            il.FieldGet(fi);
            il.Return();
        });

        var result = method();

        result.Should().Be(0);
    }

    #endregion

    #region IfTrue / IfFalse / IfTrueS / IfFalseS

    [Fact]
    public void Should_IfTrue_When_Branching()
    {
        var method = CreateDynamicMethod<Func<bool, string>>(il =>
        {
            var label = il.DefineLabel();
            il.LoadArg(0);
            il.IfTrue(label);
            il.LoadString("false");
            il.Return();
            il.MarkLabel(label);
            il.LoadString("true");
            il.Return();
        });

        method(true).Should().Be("true");
        method(false).Should().Be("false");
    }

    [Fact]
    public void Should_IfFalse_When_Branching()
    {
        var method = CreateDynamicMethod<Func<bool, string>>(il =>
        {
            var label = il.DefineLabel();
            il.LoadArg(0);
            il.IfFalse(label);
            il.LoadString("true");
            il.Return();
            il.MarkLabel(label);
            il.LoadString("false");
            il.Return();
        });

        method(true).Should().Be("true");
        method(false).Should().Be("false");
    }

    [Fact]
    public void Should_IfTrueS_When_ShortBranch()
    {
        var method = CreateDynamicMethod<Func<bool, int>>(il =>
        {
            var label = il.DefineLabel();
            il.LoadArg(0);
            il.IfTrueS(label);
            il.LoadInt32(0);
            il.Return();
            il.MarkLabel(label);
            il.LoadInt32(1);
            il.Return();
        });

        method(true).Should().Be(1);
        method(false).Should().Be(0);
    }

    [Fact]
    public void Should_IfFalseS_When_ShortBranch()
    {
        var method = CreateDynamicMethod<Func<bool, int>>(il =>
        {
            var label = il.DefineLabel();
            il.LoadArg(0);
            il.IfFalseS(label);
            il.LoadInt32(1);
            il.Return();
            il.MarkLabel(label);
            il.LoadInt32(0);
            il.Return();
        });

        method(true).Should().Be(1);
        method(false).Should().Be(0);
    }

    #endregion

    #region Pop / Dup

    [Fact]
    public void Should_Pop_When_ValueOnStack()
    {
        var method = CreateDynamicMethod<Func<int>>(il =>
        {
            il.LoadInt32(42);
            il.Pop();
            il.LoadInt32(99);
            il.Return();
        });

        var result = method();

        result.Should().Be(99);
    }

    [Fact]
    public void Should_Dup_When_ValueOnStack()
    {
        var method = CreateDynamicMethod<Func<int[]>>(il =>
        {
            il.LoadInt32(2);
            il.Emit(OpCodes.Newarr, typeof(int));
            il.Dup();
            il.LoadInt32(0);
            il.LoadInt32(7);
            il.StoreElement(typeof(int));
            il.Dup();
            il.LoadInt32(1);
            il.LoadInt32(7);
            il.StoreElement(typeof(int));
            il.Return();
        });

        var result = method();

        result.Should().Equal(7, 7);
    }

    #endregion

    #region LoadElement - all type branches

    [Fact]
    public void Should_LoadElement_When_Int16Array()
    {
        var method = CreateDynamicMethod<Func<short[], short>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(0);
            il.LoadElement(typeof(short));
            il.Return();
        });

        var result = method(new short[] { 100, 200 });

        result.Should().Be(100);
    }

    [Fact]
    public void Should_LoadElement_When_UInt16Array()
    {
        var method = CreateDynamicMethod<Func<ushort[], ushort>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(0);
            il.LoadElement(typeof(ushort));
            il.Return();
        });

        var result = method(new ushort[] { 500, 600 });

        result.Should().Be(500);
    }

    [Fact]
    public void Should_LoadElement_When_UInt32Array()
    {
        var method = CreateDynamicMethod<Func<uint[], uint>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(0);
            il.LoadElement(typeof(uint));
            il.Return();
        });

        var result = method(new uint[] { 1000, 2000 });

        result.Should().Be(1000);
    }

    [Fact]
    public void Should_LoadElement_When_Int64Array()
    {
        var method = CreateDynamicMethod<Func<long[], long>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(0);
            il.LoadElement(typeof(long));
            il.Return();
        });

        var result = method(new long[] { 9999L, 8888L });

        result.Should().Be(9999L);
    }

    [Fact]
    public void Should_LoadElement_When_SingleArray()
    {
        var method = CreateDynamicMethod<Func<float[], float>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(0);
            il.LoadElement(typeof(float));
            il.Return();
        });

        var result = method(new float[] { 1.1f, 2.2f });

        result.Should().BeApproximately(1.1f, 0.001f);
    }

    [Fact]
    public void Should_LoadElement_When_DoubleArray()
    {
        var method = CreateDynamicMethod<Func<double[], double>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(0);
            il.LoadElement(typeof(double));
            il.Return();
        });

        var result = method(new double[] { 3.14, 2.71 });

        result.Should().BeApproximately(3.14, 0.001);
    }

    [Fact]
    public void Should_LoadElement_When_SByteArray()
    {
        var method = CreateDynamicMethod<Func<sbyte[], sbyte>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(0);
            il.LoadElement(typeof(sbyte));
            il.Return();
        });

        var result = method(new sbyte[] { -10, 20 });

        result.Should().Be(-10);
    }

    [Fact]
    public void Should_LoadElement_When_UInt64Array()
    {
        var method = CreateDynamicMethod<Func<ulong[], ulong>>(il =>
        {
            il.LoadArg(0);
            il.LoadInt32(0);
            il.LoadElement(typeof(ulong));
            il.Return();
        });

        var result = method(new ulong[] { 999UL, 888UL });

        result.Should().Be(999UL);
    }

    #endregion

    #region StoreValueIndirect - all type branches

    [Fact]
    public void Should_StoreValueIndirect_When_ByteType()
    {
        var method = CreateDynamicMethod<Func<byte>>(il =>
        {
            var local = il.DeclareLocal(typeof(byte));
            il.LoadInt32(200);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadInt32(250);
            il.StoreValueIndirect(typeof(byte));
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(byte));
            il.Return();
        });

        var result = method();

        result.Should().Be(250);
    }

    [Fact]
    public void Should_StoreValueIndirect_When_SByteType()
    {
        var method = CreateDynamicMethod<Func<sbyte>>(il =>
        {
            var local = il.DeclareLocal(typeof(sbyte));
            il.LoadInt32(-50);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadInt32(-60);
            il.StoreValueIndirect(typeof(sbyte));
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(sbyte));
            il.Return();
        });

        var result = method();

        result.Should().Be((sbyte)-60);
    }

    [Fact]
    public void Should_StoreValueIndirect_When_Int16Type()
    {
        var method = CreateDynamicMethod<Func<short>>(il =>
        {
            var local = il.DeclareLocal(typeof(short));
            il.LoadInt32(1000);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadInt32(2000);
            il.StoreValueIndirect(typeof(short));
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(short));
            il.Return();
        });

        var result = method();

        result.Should().Be((short)2000);
    }

    [Fact]
    public void Should_StoreValueIndirect_When_CharType()
    {
        var method = CreateDynamicMethod<Func<char>>(il =>
        {
            var local = il.DeclareLocal(typeof(char));
            il.LoadInt32('A');
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadInt32('B');
            il.StoreValueIndirect(typeof(char));
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(char));
            il.Return();
        });

        var result = method();

        result.Should().Be('B');
    }

    [Fact]
    public void Should_StoreValueIndirect_When_UInt32Type()
    {
        var method = CreateDynamicMethod<Func<uint>>(il =>
        {
            var local = il.DeclareLocal(typeof(uint));
            il.LoadInt32(100000);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadInt32(200000);
            il.StoreValueIndirect(typeof(uint));
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(uint));
            il.Return();
        });

        var result = method();

        result.Should().Be(200000u);
    }

    [Fact]
    public void Should_StoreValueIndirect_When_StructValueType()
    {
        var method = CreateDynamicMethod<Func<DateTime>>(il =>
        {
            var local = il.DeclareLocal(typeof(DateTime));
            var parseMethod = typeof(DateTime).GetMethod(nameof(DateTime.Parse), new[] { typeof(string) });
            il.LoadString("2024-06-01");
            il.Call(parseMethod);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadString("2025-07-01");
            il.Call(parseMethod);
            il.StoreValueIndirect(typeof(DateTime));
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(DateTime));
            il.Return();
        });

        var result = method();

        result.Year.Should().Be(2025);
    }

    #endregion

    #region StoreElement - additional type branches

    [Fact]
    public void Should_StoreElement_When_Int16Array()
    {
        var method = CreateDynamicMethod<Func<short[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(short));
            il.Dup();
            il.LoadInt32(0);
            il.LoadInt32(500);
            il.StoreElement(typeof(short));
            il.Return();
        });

        var result = method();

        result[0].Should().Be(500);
    }

    [Fact]
    public void Should_StoreElement_When_CharArray()
    {
        var method = CreateDynamicMethod<Func<char[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(char));
            il.Dup();
            il.LoadInt32(0);
            il.LoadInt32('Z');
            il.StoreElement(typeof(char));
            il.Return();
        });

        var result = method();

        result[0].Should().Be('Z');
    }

    [Fact]
    public void Should_StoreElement_When_BooleanArray()
    {
        var method = CreateDynamicMethod<Func<bool[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(bool));
            il.Dup();
            il.LoadInt32(0);
            il.LoadInt32(1);
            il.StoreElement(typeof(bool));
            il.Return();
        });

        var result = method();

        result[0].Should().BeTrue();
    }

    [Fact]
    public void Should_StoreElement_When_SByteArray()
    {
        var method = CreateDynamicMethod<Func<sbyte[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(sbyte));
            il.Dup();
            il.LoadInt32(0);
            il.LoadInt32(-42);
            il.StoreElement(typeof(sbyte));
            il.Return();
        });

        var result = method();

        result[0].Should().Be(-42);
    }

    [Fact]
    public void Should_StoreElement_When_ByteArray()
    {
        var method = CreateDynamicMethod<Func<byte[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(byte));
            il.Dup();
            il.LoadInt32(0);
            il.LoadInt32(255);
            il.StoreElement(typeof(byte));
            il.Return();
        });

        var result = method();

        result[0].Should().Be(255);
    }

    [Fact]
    public void Should_StoreElement_When_UInt32Array()
    {
        var method = CreateDynamicMethod<Func<uint[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(uint));
            il.Dup();
            il.LoadInt32(0);
            il.LoadInt32(12345);
            il.StoreElement(typeof(uint));
            il.Return();
        });

        var result = method();

        result[0].Should().Be(12345u);
    }

    [Fact]
    public void Should_StoreElement_When_UInt64Array()
    {
        var method = CreateDynamicMethod<Func<ulong[]>>(il =>
        {
            il.LoadInt32(1);
            il.Emit(OpCodes.Newarr, typeof(ulong));
            il.Dup();
            il.LoadInt32(0);
            il.Emit(OpCodes.Ldc_I8, 12345L);
            il.StoreElement(typeof(ulong));
            il.Return();
        });

        var result = method();

        result[0].Should().Be(12345ul);
    }

    #endregion

    #region LoadValueIndirect - additional type branches

    [Fact]
    public void Should_LoadValueIndirect_When_ByteType()
    {
        var method = CreateDynamicMethod<Func<byte>>(il =>
        {
            var local = il.DeclareLocal(typeof(byte));
            il.LoadInt32(250);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(byte));
            il.Return();
        });

        var result = method();

        result.Should().Be(250);
    }

    [Fact]
    public void Should_LoadValueIndirect_When_Int16Type()
    {
        var method = CreateDynamicMethod<Func<short>>(il =>
        {
            var local = il.DeclareLocal(typeof(short));
            il.LoadInt32(32000);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(short));
            il.Return();
        });

        var result = method();

        result.Should().Be(32000);
    }

    [Fact]
    public void Should_LoadValueIndirect_When_CharType()
    {
        var method = CreateDynamicMethod<Func<char>>(il =>
        {
            var local = il.DeclareLocal(typeof(char));
            il.LoadInt32('X');
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(char));
            il.Return();
        });

        var result = method();

        result.Should().Be('X');
    }

    [Fact]
    public void Should_LoadValueIndirect_When_UInt32Type()
    {
        var method = CreateDynamicMethod<Func<uint>>(il =>
        {
            var local = il.DeclareLocal(typeof(uint));
            il.LoadInt32(300000);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(uint));
            il.Return();
        });

        var result = method();

        result.Should().Be(300000u);
    }

    [Fact]
    public void Should_LoadValueIndirect_When_StructValueType()
    {
        var method = CreateDynamicMethod<Func<DateTime>>(il =>
        {
            var local = il.DeclareLocal(typeof(DateTime));
            var parseMethod = typeof(DateTime).GetMethod(nameof(DateTime.Parse), new[] { typeof(string) });
            il.LoadString("2024-03-15");
            il.Call(parseMethod);
            il.StoreLocalVar(local.LocalIndex);
            il.Emit(OpCodes.Ldloca_S, local);
            il.LoadValueIndirect(typeof(DateTime));
            il.Return();
        });

        var result = method();

        result.Year.Should().Be(2024);
        result.Month.Should().Be(3);
    }

    #endregion

    #region Helper Types

    private class TestEntity
    {
        public int PublicField;
        public int Value;
        public string Name;

        public TestEntity() { }

        public TestEntity(int value) { Value = value; }

        public TestEntity(string name) { Name = name; }
    }

    private static class StaticFieldHolder
    {
        public static int StaticValue = 42;
        public static int MutableValue = 100;
    }

    private enum TestEnum
    {
        ValueA,
        ValueB,
        ValueC
    }

    #endregion

    #region Helper

    private static TDelegate CreateDynamicMethod<TDelegate>(Action<ILGenerator> emitBody)
        where TDelegate : Delegate
    {
        var invokeMethod = typeof(TDelegate).GetMethod("Invoke");
        var parameters = invokeMethod.GetParameters();
        var paramTypes = new Type[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            paramTypes[i] = parameters[i].ParameterType;
        }

        var method = new DynamicMethod(
            "TestMethod_" + Guid.NewGuid().ToString("N"),
            invokeMethod.ReturnType,
            paramTypes,
            restrictedSkipVisibility: true);
        var il = method.GetILGenerator();
        emitBody(il);
        return (TDelegate)method.CreateDelegate(typeof(TDelegate));
    }

    #endregion
}
