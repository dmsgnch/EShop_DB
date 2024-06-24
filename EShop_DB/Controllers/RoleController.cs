using EShop_DB.Common.Constants;
using EShop_DB.Common.Extensions;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using EShop_DB.Models.MainModels;
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

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdAction)]
    public IActionResult GetRoleById([FromBody] Guid id)
    {
        var result = _dbContext.Roles.FirstOrDefault(r => r.RoleId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(errorInfo: ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(result.ToRoleDto());
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllAction)]
    public IActionResult GetAllRoles()
    {
        List<Role> result = _dbContext.Roles.ToList();

        return Ok(result.Select(u => u.ToRoleDto()).ToList());
    }
}
