using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using MongoDB.EntityFrameworkCore.Extensions;
using OrangeButton.Models;
using UniversalMapper.DBModels;

namespace UniversalMapper;

public class UniversalMapperDbContext : DbContext
{
    public UniversalMapperDbContext(DbContextOptions<UniversalMapperDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //ignore additional properties
        modelBuilder.Ignore<IDictionary<string, object>>();
        modelBuilder.Ignore<Dictionary<string, object>>();

        //map class
        modelBuilder.Entity<UUIDMap>().ToCollection("uuid-map");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<UUIDMap> Maps { get; set; }

}