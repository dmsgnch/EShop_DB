using EShop_DB.Controllers;
using EShop_DB.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedLibrary.Models.DbModels.MainModels;
using SharedLibrary.Models.DtoModels.MainModels;
using SharedLibrary.Responses;

namespace EShop_DB.Tests;

public class UserControllerTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite("Data Source=TestLocalDatabase.db");
        _dbContext = new ApplicationDbContext(builder.Options);

        AppDbInitializer.TestSeed(_dbContext);

        _controller = new UserController(_dbContext);
    }

    [Fact]
    public void GetAllUsers_ReturnOkResult_WithListOfUsersIncludedEntities()
    {
        // Arrange
        var testUser2Id = new Guid("914ee195-0d3c-460a-801a-f9bdfa34f7e4");
        var testUser3Id = new Guid("9b4f7c7e-b855-4a35-b375-cd1e35112ccd");

        // Act
        var result = _controller.GetAllUsers();

        // Assert
        result.Should().NotBeNull();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<LambdaResponse<List<User>>>(okResult.Value);

        if (returnValue.ResponseObject.Count > 0)
        {
            foreach (var user in returnValue.ResponseObject)
            {
                user.Role.Should().NotBeNull();
                if (user.SellerId is not null)
                {
                    user.Seller.Should().NotBeNull();
                }

                if (user.UserId.Equals(testUser2Id))
                {
                    Assert.NotNull(user.Orders);
                    Assert.NotEmpty(user.Orders);
                }

                if (user.UserId.Equals(testUser3Id))
                {
                    Assert.True(user.Orders is null || user.Orders.Count.Equals(0));
                }
            }
        }
    }

    [Fact]
    public void GetUserById_ReturnOkResult_WithUserIncludedEntities()
    {
        // Arrange
        var testUser2Id = new Guid("914ee195-0d3c-460a-801a-f9bdfa34f7e4");
        var testUser3Id = new Guid("9b4f7c7e-b855-4a35-b375-cd1e35112ccd");

        // Act
        var resultUser2 = _controller.GetUserById(testUser2Id);
        var resultUser3 = _controller.GetUserById(testUser3Id);

        // Assert
        resultUser2.Should().NotBeNull();
        resultUser3.Should().NotBeNull();

        var okResultUser2 = Assert.IsType<OkObjectResult>(resultUser2);
        var okResultUser3 = Assert.IsType<OkObjectResult>(resultUser3);
        var user2 = Assert.IsType<LambdaResponse<User>>(okResultUser2.Value).ResponseObject;
        var user3 = Assert.IsType<LambdaResponse<User>>(okResultUser3.Value).ResponseObject;

        user2.Role.Should().NotBeNull();
        user3.Role.Should().NotBeNull();

        user2.SellerId.Should().BeNull();
        user2.Seller.Should().BeNull();

        user2.Orders.Should().HaveCount(1);
        
        user3.SellerId.Should().NotBeNull();
        user3.Seller.Should().NotBeNull();

        Assert.True(user3.Orders is null || user3.Orders.Count.Equals(0));
    }
    
    [Fact]
    public void EditUser_ReturnOkResult_WithUserIncludedEntities()
    {
        // Arrange
        var testUser2Id = new Guid("914ee195-0d3c-460a-801a-f9bdfa34f7e4");
        var testUser3Id = new Guid("9b4f7c7e-b855-4a35-b375-cd1e35112ccd");

        // Act
        var resultUser2 = _controller.GetUserById(testUser2Id);
        var resultUser3 = _controller.GetUserById(testUser3Id);

        // Assert
        resultUser2.Should().NotBeNull();
        resultUser3.Should().NotBeNull();

        var okResultUser2 = Assert.IsType<OkObjectResult>(resultUser2);
        var okResultUser3 = Assert.IsType<OkObjectResult>(resultUser3);
        var user2 = Assert.IsType<LambdaResponse<User>>(okResultUser2.Value).ResponseObject;
        var user3 = Assert.IsType<LambdaResponse<User>>(okResultUser3.Value).ResponseObject;

        user2.Role.Should().NotBeNull();
        user3.Role.Should().NotBeNull();

        user2.SellerId.Should().BeNull();
        user2.Seller.Should().BeNull();

        user2.Orders.Should().HaveCount(1);
        
        user3.SellerId.Should().NotBeNull();
        user3.Seller.Should().NotBeNull();

        Assert.True(user3.Orders is null || user3.Orders.Count.Equals(0));
    }
}