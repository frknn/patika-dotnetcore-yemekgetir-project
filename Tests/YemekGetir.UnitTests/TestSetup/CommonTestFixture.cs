using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YemekGetir.DBOperations;
using YemekGetir.Common;

namespace TestSetup
{
  public class CommonTestFixture
  {
    public YemekGetirDbContext Context { get; set; }
    public IMapper Mapper { get; set; }
    public IConfiguration Configuration { get; set; }

    public CommonTestFixture()
    {
      var options = new DbContextOptionsBuilder<YemekGetirDbContext>().UseInMemoryDatabase(databaseName: "YemekGetirTestDB").Options;
      Context = new YemekGetirDbContext(options);
      Context.Database.EnsureCreated();

      Mapper = new MapperConfiguration(config => { config.AddProfile<MappingProfile>(); }).CreateMapper();

      Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }
  }
}