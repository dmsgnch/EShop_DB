using EShop_DB.Controllers;
using EShop_DB.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models.DtoModels.MainModels;
using SharedLibrary.Responses;

namespace EShop_DB.Tests;

public class UserControllerTests
{
    private readonly ApplicationDbContext _dbContext;

    public UserControllerTests()
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=TestLocalDatabase.db");
        var dbContext = new ApplicationDbContext(builder.Options);

        dbContext.ChangeTracker.LazyLoadingEnabled = false;
        
        TestDbInitializer.TestSeed(dbContext);

        _dbContext = dbContext;
    }

    [Fact]
    public void GetAllUsers_ReturnOkResult_WithListOfUsersIncludedEntities()
    {
        // Arrange
        _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        var testUser2Id = new Guid("914ee195-0d3c-460a-801a-f9bdfa34f7e4");
        var testUser3Id = new Guid("9b4f7c7e-b855-4a35-b375-cd1e35112ccd");

        // Act
        var result = new UserController(_dbContext).GetAllUsers();

        // Assert
        result.Should().NotBeNull();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UniversalResponse<List<UserDTO>>>(okResult.Value);

        if (returnValue.ResponseObject.Count > 0)
        {
            foreach (var user in returnValue.ResponseObject)
            {
                user.RoleDto.Should().NotBeNull();
                if (user.SellerDtoId is not null)
                {
                    user.SellerDto.Should().NotBeNull();
                }

                if (user.UserDtoId.Equals(testUser2Id))
                {
                    Assert.NotNull(user.OrdersDto);
                    Assert.NotEmpty(user.OrdersDto);
                }

                if (user.UserDtoId.Equals(testUser3Id))
                {
                    Assert.True(user.OrdersDto is null || user.OrdersDto.Count.Equals(0));
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
        var resultUser2 = new UserController(_dbContext).GetUserById(testUser2Id);
        var resultUser3 = new UserController(_dbContext).GetUserById(testUser3Id);

        // Assert
        resultUser2.Should().NotBeNull();
        resultUser3.Should().NotBeNull();

        var okResultUser2 = Assert.IsType<OkObjectResult>(resultUser2);
        var okResultUser3 = Assert.IsType<OkObjectResult>(resultUser3);
        var user2 = Assert.IsType<UniversalResponse<UserDTO>>(okResultUser2.Value).ResponseObject;
        var user3 = Assert.IsType<UniversalResponse<UserDTO>>(okResultUser3.Value).ResponseObject;

        user2.RoleDto.Should().NotBeNull();
        user3.RoleDto.Should().NotBeNull();

        user2.SellerDtoId.Should().BeNull();
        user2.SellerDto.Should().BeNull();

        user2.OrdersDto.Should().HaveCount(1);

        user3.SellerDtoId.Should().NotBeNull();
        user3.SellerDto.Should().NotBeNull();

        Assert.True(user3.OrdersDto is null || user3.OrdersDto.Count.Equals(0));
    }

    [Fact]
    public void EditUser_ReturnOkResult_WithUserIncludedEntities()
    {
        // Arrange
        var testUser2Id = new Guid("914ee195-0d3c-460a-801a-f9bdfa34f7e4");
        var testUser3Id = new Guid("9b4f7c7e-b855-4a35-b375-cd1e35112ccd");

        // Act
        var resultUser2 = new UserController(_dbContext).GetUserById(testUser2Id);
        var resultUser3 = new UserController(_dbContext).GetUserById(testUser3Id);

        // Assert
        resultUser2.Should().NotBeNull();
        resultUser3.Should().NotBeNull();

        var okResultUser2 = Assert.IsType<OkObjectResult>(resultUser2);
        var okResultUser3 = Assert.IsType<OkObjectResult>(resultUser3);
        var user2 = Assert.IsType<UniversalResponse<UserDTO>>(okResultUser2.Value).ResponseObject;
        var user3 = Assert.IsType<UniversalResponse<UserDTO>>(okResultUser3.Value).ResponseObject;

        user2.RoleDto.Should().NotBeNull();
        user3.RoleDto.Should().NotBeNull();

        user2.SellerDtoId.Should().BeNull();
        user2.SellerDto.Should().BeNull();

        user2.OrdersDto.Should().HaveCount(1);

        user3.SellerDtoId.Should().NotBeNull();
        user3.SellerDto.Should().NotBeNull();

        Assert.True(user3.OrdersDto is null || user3.OrdersDto.Count.Equals(0));
    }
}