namespace Sample;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public partial class SampleContext : DbContext
{
    public DbSet<Item> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseMySql(
                @"Server=127.0.0.1;Uid=root;Pwd=root;Database=databaseName",
                MariaDbServerVersion.LatestSupportedServerVersion);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Sample).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

public class Item
{
    public string Key { get; set; }
    public ItemValue ItemValue { get; set; }
}

public class ItemValue
{
    public long Value { get; set; }
    // the behaviors change when having a default date value, still a bug
    public DateTime Date { get; set; }// = DateTime.UtcNow;
}

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> entity)
    {
        entity.HasKey(e => e.Key);

        entity
            .ToTable("items");

        entity
            .OwnsOne(
                e => e.ItemValue,
                q =>
                {
                    q
                        .Property(quotas => quotas.Value)
                        .HasColumnType("bigint(20)")
                        .HasDefaultValue(0);
                    q
                        .Property(quotas => quotas.Date)
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("NOW()");
                });
    }
}

public class Sample
{
    private static void EnsureDatabaseStateIsClean()
    {
        try
        {
            using var context = new SampleContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        catch (Exception)
        {
        }
    }

    private static void CreateItemToValue(string key, long value)
    {
        using (var context = new SampleContext())
        {
            var savedItem = context.Add(new Item()
            {
                Key = key,
                ItemValue = new ItemValue()
                {
                    Value = value,
                }
            });
            context.SaveChanges();

            Console.WriteLine($"creating the value to : {savedItem.Entity.ItemValue.Value}");
        }
    }

    private static void UpdateItemToValue(string key, long value)
    {
        using (var context = new SampleContext())
        {
            var item = context.Items.Find(key);
            item.ItemValue = new ItemValue { Value = value, };
            context.SaveChanges();

            Console.WriteLine($"===={Environment.NewLine}setting the value to : {item.ItemValue.Value}");
        }
    }

    private static void UpdateItemToValueWithoutNewObject(string key, long value)
    {
        using (var context = new SampleContext())
        {
            var item = context.Items.Find(key);
            item.ItemValue.Value = value;
            context.SaveChanges();

            Console.WriteLine($"===={Environment.NewLine}[WITHOUT CREATING A NEW OBJECT] setting the value to : {item.ItemValue.Value}");
        }
    }

    private static void ReadItemToValue(string key)
    {
        using (var context = new SampleContext())
        {
            var item = context.Items.Find(key);
            Console.WriteLine($"the value is now: {item.ItemValue.Value} ({context.Items.Count()} values in db)");
        }
    }

    private const string ITEM_KEY = "key";
    public static void Main(string [] args)
    {
        EnsureDatabaseStateIsClean();

        CreateItemToValue(ITEM_KEY, 666);

        UpdateItemToValue(ITEM_KEY, 0);
        ReadItemToValue(ITEM_KEY);

        UpdateItemToValue(ITEM_KEY, 1);
        ReadItemToValue(ITEM_KEY);

        UpdateItemToValue(ITEM_KEY, 0);
        ReadItemToValue(ITEM_KEY);

        UpdateItemToValueWithoutNewObject(ITEM_KEY, 0);
        ReadItemToValue(ITEM_KEY);
    }
}