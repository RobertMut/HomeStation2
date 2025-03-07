using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HomeStation.Application.Common.Virtual;
using HomeStation.Domain.Common.Entities;
using HomeStation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeStation.Application.Tests.Common.Virtual;

[TestFixture]
[TestOf(typeof(Repository<>))]
[ExcludeFromCodeCoverage]
public class RepositoryTest
{
    private DbContextOptions<AirDbContext> options;
    private readonly DateTimeOffset dateTime = DateTimeOffset.Now;
    private int deviceId;

    [SetUp]
    public async Task SetUp()
    {
        options = new DbContextOptionsBuilder<AirDbContext>()
            .UseInMemoryDatabase("AirDb").Options;

        using (AirDbContext context = new AirDbContext(options))
        {
            context.Devices.Add(new Device
            {
                Name = "TEST",
                IsKnown = true
            });
            await context.SaveChangesAsync();

            Device? device = await context.Devices.AsQueryable().FirstOrDefaultAsync(x => x.Name == "TEST");
            deviceId = device.Id;
            
            context.Climate.Add(new Climate
            {
                DeviceId = device.Id,
                Temperature = 1,
                Humidity = 1,
                Pressure = 1,
                Reading = new Reading { Date = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero) }
            });
            context.Climate.Add(new Climate
            {
                DeviceId = device.Id,
                Temperature = 2,
                Humidity = 2,
                Pressure = 2,
                Reading = new Reading { Date = new DateTimeOffset(2000, 1, 2, 1, 1, 1, TimeSpan.Zero) }
            });

            await context.SaveChangesAsync();
        }
    }
    
    [Test]
    public async Task GetShouldReturnExpectedResult()
    {
        using AirDbContext context = new AirDbContext(options);

        var expected =
            new List<Climate>
            {
                new()
                {
                    DeviceId = deviceId,
                    Temperature = 1,
                    Humidity = 1,
                    Pressure = 1,
                    Reading = new Reading { Date = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero) }
                }
            };

        
        Repository<Climate> climateRepo = new(context);
        List<Climate> result = await climateRepo.Get(x => x.Where(c => c.DeviceId == 1)).ToListAsync();

        result[0].Humidity.Should().Be(expected[0].Humidity);
        result[0].Temperature.Should().Be(expected[0].Temperature);
        result[0].Pressure.Should().Be(expected[0].Pressure);
        result[0].Reading.Date.Should().Be(expected[0].Reading.Date);
    }
    
    [Test]
    public async Task GetAllShouldReturn()
    {
        using AirDbContext context = new AirDbContext(options);

        var expected =
            new List<Climate>
            {
                new()
                {
                    DeviceId = deviceId,
                    Temperature = 1,
                    Humidity = 1,
                    Pressure = 1,
                    Reading = new Reading { Date = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero) },
                    Device = new Device()
                    {
                        Id = 1,
                        IsKnown = true,
                        Name = "TEST"
                    }
                },
                new()
                {
                    DeviceId = deviceId,
                    Temperature = 2,
                    Humidity = 2,
                    Pressure = 2,
                    Reading = new Reading { Date = new DateTimeOffset(2000, 1, 2, 1, 1, 1, TimeSpan.Zero) },
                    Device = new Device()
                    {
                        Id = 1,
                        IsKnown = true,
                        Name = "TEST"
                    }
                }
            };

        
        Repository<Climate> climateRepo = new(context);
        List<Climate> result = await climateRepo.GetAll(i => i.Device).ToListAsync();

        result[0].Humidity.Should().Be(expected[0].Humidity);
        result[0].Temperature.Should().Be(expected[0].Temperature);
        result[0].Pressure.Should().Be(expected[0].Pressure);
        result[0].Reading.Date.Should().Be(expected[0].Reading.Date);
        result[1].Humidity.Should().Be(expected[1].Humidity);
        result[1].Temperature.Should().Be(expected[1].Temperature);
        result[1].Pressure.Should().Be(expected[1].Pressure);
        result[1].Reading.Date.Should().Be(expected[1].Reading.Date);
    }
    
    [Test]
    public async Task GetObjectByShouldReturnExpectedResults()
    {
        using AirDbContext context = new AirDbContext(options);

        var expected =
            new List<Climate>
            {
                new()
                {
                    DeviceId = deviceId,
                    Temperature = 1,
                    Humidity = 1,
                    Pressure = 1,
                    Reading = new Reading { Date = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero) },
                    Device = new Device()
                    {
                        Id = 1,
                        IsKnown = true,
                        Name = "TEST"
                    }
                },
                new()
                {
                    DeviceId = deviceId,
                    Temperature = 2,
                    Humidity = 2,
                    Pressure = 2,
                    Reading = new Reading { Date = new DateTimeOffset(2000, 1, 2, 1, 1, 1, TimeSpan.Zero) },
                    Device = new Device()
                    {
                        Id = 1,
                        IsKnown = true,
                        Name = "TEST"
                    }
                }
            };

        
        Repository<Climate> climateRepo = new(context);
        Climate result = await climateRepo.GetObjectBy(x => x.Humidity == 1, i => i.Include(d => d.Device));

        result.Humidity.Should().Be(expected[0].Humidity);
        result.Temperature.Should().Be(expected[0].Temperature);
        result.Pressure.Should().Be(expected[0].Pressure);
        result.Reading.Date.Should().Be(expected[0].Reading.Date);
    }
    
    [Test]
    public async Task GetLastByShouldReturnExpectedResult()
    {
        using AirDbContext context = new AirDbContext(options);

        var expected =
            new List<Climate>
            {
                new()
                {
                    DeviceId = deviceId,
                    Temperature = 1,
                    Humidity = 1,
                    Pressure = 1,
                    Reading = new Reading { Date = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero) },
                    Device = new Device()
                    {
                        Id = 1,
                        IsKnown = true,
                        Name = "TEST"
                    }
                },
                new()
                {
                    DeviceId = deviceId,
                    Temperature = 2,
                    Humidity = 2,
                    Pressure = 2,
                    Reading = new Reading { Date = new DateTimeOffset(2000, 1, 2, 1, 1, 1, TimeSpan.Zero) },
                    Device = new Device()
                    {
                        Id = 1,
                        IsKnown = true,
                        Name = "TEST"
                    }
                }
            };

        
        Repository<Climate> climateRepo = new(context);
        Climate result = climateRepo.GetLastBy(x => x.DeviceId == deviceId,
            o => o.Reading.Date,
            i => i.Include(d => d.Device));

        result.Humidity.Should().Be(expected[1].Humidity);
        result.Temperature.Should().Be(expected[1].Temperature);
        result.Pressure.Should().Be(expected[1].Pressure);
        result.Reading.Date.Should().Be(expected[1].Reading.Date);
    }
    
    [Test]
    public async Task InsertAsyncShouldInsertData()
    {
        DbContextOptions<AirDbContext> insertOptions = new DbContextOptionsBuilder<AirDbContext>()
            .UseInMemoryDatabase("InsertDb").Options;
        DateTimeOffset expectedDate = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero);
        using AirDbContext context = new AirDbContext(insertOptions);
        Repository<Climate> climateRepo = new(context);
        
        Func<Task> f = async () =>
        {
            await climateRepo.InsertAsync(new Climate()
            {
                DeviceId = 1,
                Reading = new Reading() { Date = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero) },
                Humidity = 1,
                Pressure = 1,
                Temperature = 1
            });
            
            await context.SaveChangesAsync();
        };
        
        f.Should().NotThrowAsync();
        
        Climate? dbEntity = await context.Climate.AsQueryable().FirstOrDefaultAsync(x => x.DeviceId == 1 && x.Humidity == 1);

        dbEntity.DeviceId.Should().Be(1);
        dbEntity.Humidity.Should().Be(1);
        dbEntity.Temperature.Should().Be(1);
        dbEntity.Pressure.Should().Be(1);
        dbEntity.Reading.Date.Date.Should().Be(expectedDate.Date);
    }
    
    [Test]
    public async Task UpdateAsyncShouldChangeEntity()
    {
        DbContextOptions<AirDbContext> insertOptions = new DbContextOptionsBuilder<AirDbContext>()
            .UseInMemoryDatabase("UpdateDb").Options;
        DateTimeOffset expectedDate = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero);

        using AirDbContext context = new AirDbContext(insertOptions);
        
        context.Climate.Add(new Climate()
        {
            DeviceId = 1,
            Reading = new Reading() { Date = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero) },
            Humidity = 1,
            Pressure = 1,
            Temperature = 1
        });
            
        await context.SaveChangesAsync();
        
        Climate? item = await context.Climate.AsQueryable().FirstOrDefaultAsync(x => x.DeviceId == 1);
        Repository<Climate> climateRepo = new(context);

        item.Humidity = 5;
        item.Pressure = 22;
        
        Func<Task> f = async () =>
        {
            climateRepo.UpdateAsync(item);
            
            await context.SaveChangesAsync();
        };
        
        f.Should().NotThrowAsync();
        
        Climate? dbEntity = await context.Climate.AsQueryable().FirstOrDefaultAsync(x => x.DeviceId == 1 && x.Humidity == 5);

        dbEntity.DeviceId.Should().Be(1);
        dbEntity.Humidity.Should().Be(5);
        dbEntity.Temperature.Should().Be(1);
        dbEntity.Pressure.Should().Be(22);
        dbEntity.Reading.Date.Date.Should().Be(expectedDate.Date);
    }
    
    [Test]
    public async Task DeleteShouldRemoveEntity()
    {
        DbContextOptions<AirDbContext> insertOptions = new DbContextOptionsBuilder<AirDbContext>()
            .UseInMemoryDatabase("UpdateDb").Options;
        
        using AirDbContext context = new AirDbContext(insertOptions);
        
        context.Climate.Add(new Climate()
        {
            DeviceId = 1,
            Reading = new Reading() { Date = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero) },
            Humidity = 1,
            Pressure = 1,
            Temperature = 1
        });
            
        await context.SaveChangesAsync();
        
        Climate? item = await context.Climate.AsQueryable().FirstOrDefaultAsync(x => x.DeviceId == 1);
        Repository<Climate> climateRepo = new(context);
        
        Func<Task> f = async () =>
        {
            climateRepo.Delete(item);
            
            await context.SaveChangesAsync();
        };
        
        f.Should().NotThrowAsync();
        
        Climate? dbEntity = await context.Climate.AsQueryable().FirstOrDefaultAsync(x => x.DeviceId == 1);

        dbEntity.Should().BeNull();
    }
}