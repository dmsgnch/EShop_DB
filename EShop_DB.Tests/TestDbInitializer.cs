using EShop_DB.Data;
using EShop_DB.Models.MainModels;
using EShop_DB.Models.SecondaryModels;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models.Enums;
using SharedLibrary.Routes;

namespace EShop_DB.Tests;

public static class TestDbInitializer
{
     public static void TestSeed(ApplicationDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        #region Initialize with test data

        Guid idForAnonymousOrder = Guid.NewGuid();

            if (!context.Sellers.Any())
            {
                context.Sellers.AddRange(new Seller[]
                {
                    new Seller(
                            companyName: "TradeWave",
                            contactNumber: "0970001111",
                            emailAddress: "tradeWave@gmail.com",
                            companyDescription: "Own production",
                            additionNumber: "0970001110", imageUrl: "StaticFiles/CompanyLogo.png")
                        { SellerId = OwnSellerData.Id },
                    new Seller(
                            companyName: "Apple",
                            contactNumber: "0970000100",
                            emailAddress: "SEmail1@gmail.com",
                            companyDescription: "A company that for over a decade has been in the business of selling1",
                            additionNumber: "0970000110", imageUrl: "StaticFiles/apple_logo.png")
                        { SellerId = new Guid("0C6AB3FC-374F-41F3-B2B3-2BD3E765C3E4") },
                    new Seller(
                            companyName: "Samsung",
                            contactNumber: "0970000200",
                            emailAddress: "SEmail2@gmail.com",
                            companyDescription: "A company that for over a decade has been in the business of selling2",
                            additionNumber: "0970000220", imageUrl: "StaticFiles/samsung_logo.png")
                        { SellerId = new Guid("77F3F87B-36C7-4460-9A60-153C0B3B0764") }
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
                        pricePerUnit: 600m,
                        weightInGrams: 150,
                        inStock: 300,
                        context.Sellers.First(s => s.CompanyName.Equals("Apple")).SellerId)
                    {
                        ImageUrl = "StaticFiles/iPhone_15.png"
                    },
                    new Product(
                        name: "Galaxy A55",
                        description: "Nice thing)",
                        pricePerUnit: 500m,
                        weightInGrams: 200,
                        inStock: 300,
                        context.Sellers.First(s => s.CompanyName.Equals("Samsung")).SellerId)
                    {
                        ImageUrl = "StaticFiles/samsung_A55.png"
                    },
                    new Product(
                        name: "MyPhone 10",
                        description: "Own company product",
                        pricePerUnit: 400m,
                        weightInGrams: 250,
                        inStock: 300,
                        context.Sellers.First(s => s.CompanyName.Equals("TradeWave")).SellerId)
                    {
                        ImageUrl = "StaticFiles/noname_phone.png"
                    },
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
                    new Role(RoleTag.Seller),
                });

                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(new User[]
                {
                    new User("Ivan", "Ivanov", "r9X/o0T/7e+aYvhUDRUre6BV1wuz7iin",
                        "ISYt5iHhmvYglEvtnU5SxLYWeWBKZQvT", "0910000001",
                        "email1@gmail.com", "Ivanovich", 
                        context.Roles.First(r => r.RoleTag == RoleTag.Customer).RoleId) 
                        { UserId = new Guid("301bf403-6e71-45fa-a2ec-49e80dbdf9d9") },
                    new User("Peter", "Simpson", "49afmaFDMZXw9qLkP4m2igfuP2WoAN5H",
                            "5Xn+THuUhypPeBCINM4aR8W2upPFGoCf", "0910000002",
                            "email2@gmail.com", 
                            roleId: context.Roles.First(r => r.RoleTag == RoleTag.Customer).RoleId)
                        { UserId = new Guid("914ee195-0d3c-460a-801a-f9bdfa34f7e4") },
                    new User("Mike", "Tyson", "7hbDqQcwuuP93r0EyxratECL4aOQ6IBC",
                        "VxV/v9Fnmq2duNDMwcoAM1qdBw4ZFalG", "0910000003",
                        "email3@gmail.com", roleId: context.Roles.First(r => r.RoleTag == RoleTag.Manager).RoleId,
                        patronymic: "Patronymic3")
                    {
                        UserId = new Guid("9b4f7c7e-b855-4a35-b375-cd1e35112ccd"),
                        SellerId = OwnSellerData.Id,
                    },
                    new User("John", "Doe", "MOW7jtvri2tEw2aaiQb1aykdWAMHvYhZ",
                        "W+HxwNlPVaGbMOQC63QJsMjYXXJkw+lW", "0910000004",
                        "email4@gmail.com", roleId: context.Roles.First(r => r.RoleTag == RoleTag.Seller).RoleId)
                    {
                        UserId = new Guid("69436fd8-50b5-4273-b261-8497cf2b1e82"),
                        SellerId = context.Sellers.First(s => s.CompanyName.Equals("Apple")).SellerId,
                    },
                });

                context.SaveChanges();
            }

            #endregion

            #region Orders seeding

            if (!context.Orders.Any())
            {
                context.Orders.AddRange(new Order[]
                {
                    new Order(context.Users.First(u => u.PhoneNumber.Equals("0910000001")).UserId),
                    new Order(context.Users.First(u => u.PhoneNumber.Equals("0910000002")).UserId),
                    new Order(anonymousToken: idForAnonymousOrder)
                });

                context.SaveChanges();
            }

            if (!context.OrderItems.Any())
            {
                context.OrderItems.AddRange(new OrderItem[]
                {
                    new OrderItem(context.Orders.First(o => o.AnonymousToken.Equals(idForAnonymousOrder)).OrderId,
                        context.Products.First(p => p.Name.Equals("Galaxy A55")).ProductId),
                    new OrderItem(context.Orders.Include(o => o.User)
                            .First(o => o.User.PhoneNumber.Equals("0910000001")).OrderId,
                        context.Products.First(p =>
                            p.Name.Equals("iPhone 15")).ProductId),
                    new OrderItem(context.Orders.Include(o => o.User)
                                      .FirstOrDefault(o => o.User.PhoneNumber.Equals("0910000002")).OrderId,
                        context.Products.First(p => p.Name.Equals("Galaxy A55")).ProductId),
                });

                context.SaveChanges();
            }

            if (!context.DeliveryAddresses.Any())
            {
                context.DeliveryAddresses.AddRange(new DeliveryAddress[]
                {
                    new DeliveryAddress("Kharkiv", "Otakara Yarosha", "2",
                        orderId: context.Orders.Include(o => o.User).First(o => o.User.PhoneNumber.Equals("0910000001")).OrderId),

                    new DeliveryAddress("Kyiv", "Tarasa Shevchenka", "10", "121", "5",
                        orderId: context.Orders.Include(o => o.User).First(o => o.User.PhoneNumber.Equals("0910000002")).OrderId),

                    new DeliveryAddress("Lviv", "Heroiv ATO", "6",
                        userId: context.Users.First(o => o.PhoneNumber.Equals("0910000001")).UserId),
                });

                context.SaveChanges();
            }

            if (!context.Recipients.Any())
            {
                context.Recipients.AddRange(new Recipient[]
                {
                    new Recipient("RName1", "RLastName1", "0950000001", "RPatronymic1",
                        orderId: context.Orders.First(o => o.User.PhoneNumber.Equals("0910000001")).OrderId),

                    new Recipient("RName2", "RLastName2", "0950000002", "RPatronymic2",
                        orderId: context.Orders.First(o => o.User.PhoneNumber.Equals("0910000002")).OrderId),

                    new Recipient("RName3", "RLastName3", "0950000003", "RPatronymic3",
                        userId: context.Users.First(o => o.PhoneNumber.Equals("0910000001")).UserId),
                });

                context.SaveChanges();
            }

            #endregion
    }
}