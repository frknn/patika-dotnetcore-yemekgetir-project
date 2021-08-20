using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using YemekGetir.Entities;

namespace YemekGetir.DBOperations
{
  public class DataGenerator
  {
    public static void Initialize(IServiceProvider serviceProvider)
    {
      using (var context = new YemekGetirDbContext(serviceProvider.GetRequiredService<DbContextOptions<YemekGetirDbContext>>()))
      {
        if (context.Restaurants.Any())
        {
          return;
        }

        var product1 = new Product() { Name = "Cheeseburger", Price = 35 };
        var product2 = new Product() { Name = "Hamburger", Price = 32 };
        var product3 = new Product() { Name = "Pizza", Price = 30 };
        var product4 = new Product() { Name = "Suşi", Price = 55 };
        var product5 = new Product() { Name = "Steak", Price = 45 };
        var product6 = new Product() { Name = "Çıtır Tavuk", Price = 25 };
        var product7 = new Product() { Name = "Kahvaltı Tabağı", Price = 25 };

        var address1 = new Address() { Country = "Turkey", City = "Karabük", District = "Merkez", Line1 = "Addres Line 1", Line2 = "Addres Line 2" };
        var address2 = new Address() { Country = "Turkey", City = "İstanbul", District = "Merkez", Line1 = "Addres Line 1", Line2 = "Addres Line 2" };
        var address3 = new Address() { Country = "Turkey", City = "Ankara", District = "Merkez", Line1 = "Addres Line 1", Line2 = "Addres Line 2" };
        var address4 = new Address() { Country = "Turkey", City = "İzmir", District = "Merkez", Line1 = "Addres Line 1", Line2 = "Addres Line 2" };
        var address5 = new Address() { Country = "Turkey", City = "Bursa", District = "Merkez", Line1 = "Addres Line 1", Line2 = "Addres Line 2" };
        var address6 = new Address() { Country = "Turkey", City = "Antalya", District = "Merkez", Line1 = "Addres Line 1", Line2 = "Addres Line 2" };
        var address7 = new Address() { Country = "Turkey", City = "Eskişehir", District = "Merkez", Line1 = "Addres Line 1", Line2 = "Addres Line 2" };
        var address8 = new Address() { Country = "Turkey", City = "Kocaeli", District = "Merkez", Line1 = "Addres Line 1", Line2 = "Addres Line 2" };

        var user1 = new User() { FirstName = "Furkan", LastName = "Setbaşı", Email = "furkan@example.com", Password = BCrypt.Net.BCrypt.HashPassword("furkan123"), Address = address7, PhoneNumber = "5554443322" };
        var user2 = new User() { FirstName = "Hakan", LastName = "Setbaşı", Email = "hakan@example.com", Password = BCrypt.Net.BCrypt.HashPassword("hakan123"), Address = address8, PhoneNumber = "5556667788" };

        var restaurant1 = new Restaurant() { Name = "Hamburgerci", CategoryId = 1, Address = address1, Email = "hamburgerci@example.com", Password = BCrypt.Net.BCrypt.HashPassword("hamburgerci123"), Products = new List<Product> { product1, product2 } };
        var restaurant2 = new Restaurant() { Name = "Pizzacı", CategoryId = 2, Address = address2, Email = "pizzaci@example.com", Password = BCrypt.Net.BCrypt.HashPassword("pizzaci123"), Products = new List<Product> { product3 } };
        var restaurant3 = new Restaurant() { Name = "Çin Restoranı", CategoryId = 3, Address = address3, Email = "cin_restorani@example.com", Password = BCrypt.Net.BCrypt.HashPassword("cinrestorani123"), Products = new List<Product> { product4 } };
        var restaurant4 = new Restaurant() { Name = "Steakhouse", CategoryId = 4, Address = address4, Email = "steakhouse@example.com", Password = BCrypt.Net.BCrypt.HashPassword("steakhouse123"), Products = new List<Product> { product5 } };
        var restaurant5 = new Restaurant() { Name = "Tavukçu", CategoryId = 5, Address = address5, Email = "tavukcu@example.com", Password = BCrypt.Net.BCrypt.HashPassword("tavukcu123"), Products = new List<Product> { product6 } };
        var restaurant6 = new Restaurant() { Name = "Kahvaltıcı", CategoryId = 6, Address = address6, Email = "kahvaltici@example.com", Password = BCrypt.Net.BCrypt.HashPassword("kahvaltici123"), Products = new List<Product> { product7 } };

        var cart1 = new Cart() { User = user1 };
        var cart2 = new Cart() { User = user2 };

        context.Products.AddRange(product1, product2, product3, product4, product5, product6, product7);
        context.Addresses.AddRange(address1, address2, address3, address4, address5, address6, address7, address8);
        context.Users.AddRange(user1, user2);
        context.Restaurants.AddRange(restaurant1, restaurant2, restaurant3, restaurant4, restaurant5, restaurant6);
        context.Carts.AddRange(cart1, cart2);

        context.SaveChanges();
      }
    }
  }
}
