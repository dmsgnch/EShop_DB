using EShop_DB.Data;
using EShop_DB.Models.Enums;
using EShop_DB.Models.MainModels;
using EShop_DB.Models.SecondaryModels;
using Microsoft.EntityFrameworkCore;

namespace EShop_DB.Data;

public class AppDbInitializer
{
    public static void Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>() ??
                          throw new Exception(nameof(ApplicationDbContext) + " service was not found");

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();


            #region Initialize with test data

            Guid idForAnonymousOrder = Guid.NewGuid();

            if (!context.Sellers.Any())
            {
                context.Sellers.AddRange(new Seller[]
                {
                    new Seller(
                        companyName: "Apple",
                        contactNumber: "0970000100",
                        emailAddress: "SEmail1@gmail.com",
                        companyDescription: "A company that for over a decade has been in the business of selling1",
                        additionNumber: "0970000110"),
                    new Seller(
                        companyName: "Samsung",
                        contactNumber: "0970000200",
                        emailAddress: "SEmail2@gmail.com",
                        companyDescription: "A company that for over a decade has been in the business of selling2",
                        additionNumber: "0970000220")
                });

                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                context.Products.AddRange(new Product[]
                {
                    new Product(
                        name: "iPhone 15",
                        description: "Model that much better than previous models",
                        pricePerUnit: 30000m,
                        weightInGrams: 150,
                        seller: context.Sellers.First(s => s.CompanyName.Equals("Apple"))),
                    new Product(
                        "Galaxy A50",
                        "Nice thing)",
                        10000m,
                        200,
                        context.Sellers.First(s => s.CompanyName.Equals("Samsung"))),
                });

                context.SaveChanges();
            }

            #endregion

            #region Users seeding

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(new Role[]
                {
                    new Role(RoleTag.Customer),
                    new Role(RoleTag.Manager),
                    new Role(RoleTag.AdvManager),
                });

                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(new User[]
                {
                    new User("Name1", "LastName1", "password1", "0910000001",
                        "email1@gmail.com", context.Roles.First(r => r.RoleTag == RoleTag.Customer),
                        "Patronymic1"),
                    new User("Name2", "LastName2", "password2", "0910000002",
                        "email2@gmail.com", context.Roles.First(r => r.RoleTag == RoleTag.Customer),
                        "Patronymic2"),
                    new User("Name3", "LastName3", "password3", "0910000003",
                        "email3@gmail.com", context.Roles.First(r => r.RoleTag == RoleTag.Manager),
                        "Patronymic3"),
                    new User("Name4", "LastName4", "password4", "0910000004",
                        "email4@gmail.com", context.Roles.First(r => r.RoleTag == RoleTag.AdvManager),
                        "Patronymic4"),
                });

                context.SaveChanges();
            }

            #endregion

            #region Orders seeding

            if (!context.Orders.Any())
            {
                context.Orders.AddRange(new Order[]
                {
                    new Order(context.Users.First(u => u.PhoneNumber.Equals("0910000001"))),
                    new Order(context.Users.First(u => u.PhoneNumber.Equals("0910000002"))),
                    new Order(anonymousToken: idForAnonymousOrder)
                });

                context.SaveChanges();
            }

            if (!context.OrderItems.Any())
            {
                context.OrderItems.AddRange(new OrderItem[]
                {
                    new OrderItem(context.Orders.First(o => o.AnonymousToken.Equals(idForAnonymousOrder)),
                        context.Products.First(p => p.Name.Equals("Galaxy A50"))),
                    new OrderItem(context.Orders.Include(o => o.User)
                            .First(o => o.User.PhoneNumber.Equals("0910000001")),
                        context.Products.First(p =>
                            p.Name.Equals("iPhone 15"))),
                    new OrderItem(context.Orders.Include(o => o.User)
                                      .FirstOrDefault(o => o.User.PhoneNumber.Equals("0910000002")) ??
                                  throw new Exception("Cant find user with the number: 0910000002"),
                        context.Products.First(p => p.Name.Equals("Galaxy A50"))),
                });

                context.SaveChanges();
            }

            if (!context.DeliveryAddresses.Any())
            {
                context.DeliveryAddresses.AddRange(new DeliveryAddress[]
                {
                    new DeliveryAddress("Kharkiv", "Otakara Yarosha", "2",
                        order: context.Orders.Include(o => o.User).First(o => o.User.PhoneNumber.Equals("0910000001"))),

                    new DeliveryAddress("Kyiv", "Tarasa Shevchenka", "10", "121", "5",
                        order: context.Orders.Include(o => o.User).First(o => o.User.PhoneNumber.Equals("0910000002"))),

                    new DeliveryAddress("Lviv", "Heroiv ATO", "6",
                        user: context.Users.First(o => o.PhoneNumber.Equals("0910000001"))),
                });

                context.SaveChanges();
            }

            if (!context.Recipients.Any())
            {
                context.Recipients.AddRange(new Recipient[]
                {
                    new Recipient("RName1", "RLastName1", "0950000001", "RPatronymic1",
                        order: context.Orders.First(o => o.User.PhoneNumber.Equals("0910000001"))),

                    new Recipient("RName2", "RLastName2", "0950000002", "RPatronymic2",
                        order: context.Orders.First(o => o.User.PhoneNumber.Equals("0910000002"))),

                    new Recipient("RName3", "RLastName3", "0950000003", "RPatronymic3",
                        user: context.Users.First(o => o.PhoneNumber.Equals("0910000001"))),
                });

                context.SaveChanges();
            }

            #endregion
        }
    }
}