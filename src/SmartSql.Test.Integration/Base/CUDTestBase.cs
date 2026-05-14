using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Test.Entities;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.Base;

public abstract class CUDTestBase : IntegrationTestBase
{
    protected CUDTestBase(IDbTestFixture fixture) : base(fixture) { }

    private AllPrimitive InsertReturnIdImpl(out long id)
    {
        var entity = new AllPrimitive { String = "Insert", DateTime = DateTime.Now };
        id = SqlMapper.Insert<AllPrimitive, long>(entity);
        return entity;
    }

    [Fact]
    public void Should_GetEntity_When_InsertedById()
    {
        InsertReturnIdImpl(out long id);
        var entity = SqlMapper.GetById<AllPrimitive, long>(id);
        entity.Should().NotBeNull();
    }

    [Fact]
    public void Should_GetTrackedEntity_When_PropertyTrackEnabled()
    {
        InsertReturnIdImpl(out long id);
        var entity = SqlMapper.GetById<AllPrimitive, long>(id, enablePropertyChangedTrack: true);
        entity.Should().NotBeNull();
    }

    [Fact]
    public void Should_AffectRows_When_InsertEntity()
    {
        var recordsAffected = SqlMapper.Insert(new AllPrimitive { String = "Insert", DateTime = DateTime.Now });
        recordsAffected.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_ReturnId_When_InsertWithId()
    {
        var entity = InsertReturnIdImpl(out long id);
        id.Should().BeGreaterThan(0);
        entity.Id.Should().Be(id);
    }

    [Fact]
    public void Should_AffectRows_When_UpdateEntity()
    {
        InsertReturnIdImpl(out long id);
        var recordsAffected = SqlMapper.Update(new AllPrimitive
        {
            Id = id, String = "Update", Boolean = true, DateTime = DateTime.Now
        });
        recordsAffected.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_AffectRows_When_DyUpdateWithAnonymousObject()
    {
        InsertReturnIdImpl(out long id);
        var recordsAffected = SqlMapper.DyUpdate<AllPrimitive>(new { Id = id, Boolean = true });
        recordsAffected.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_AffectRows_When_DyUpdateWithDictionary()
    {
        InsertReturnIdImpl(out long id);
        var recordsAffected = SqlMapper.DyUpdate<AllPrimitive>(new Dictionary<string, object>
        {
            ["Id"] = id, ["Boolean"] = true
        });
        recordsAffected.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_AffectRows_When_DeleteById()
    {
        InsertReturnIdImpl(out long id);
        var recordsAffected = SqlMapper.DeleteById<AllPrimitive, long>(id);
        recordsAffected.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_DeleteAll_When_DeleteMany()
    {
        InsertReturnIdImpl(out long id0);
        InsertReturnIdImpl(out long id1);
        InsertReturnIdImpl(out long id2);
        var recordsAffected = SqlMapper.DeleteMany<AllPrimitive, long>([id0, id1, id2]);
        recordsAffected.Should().Be(3);
    }

    [Fact]
    public void Should_UpdateEntity_When_PropertyChangedTracked()
    {
        InsertReturnIdImpl(out long id);
        var entity = SqlMapper.GetById<AllPrimitive, long>(id, enablePropertyChangedTrack: true);
        entity.String = "Updated";
        SqlMapper.Update(entity);
    }
}
