using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.DbModels.MainModels;
using SharedLibrary.Models.DtoModels.MainModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.RoleContr)]
public class RoleController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Role";

    public RoleController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdControllerPath)]
    public IActionResult GetRoleById([FromRoute] Guid id)
    {
        var result = _dbContext.Roles.FirstOrDefault(r => r.RoleId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(result);
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllPath)]
    public IActionResult GetAllRoles()
    {
        List<Role> result = _dbContext.Roles.ToList();

        return Ok(result);
    }
}
