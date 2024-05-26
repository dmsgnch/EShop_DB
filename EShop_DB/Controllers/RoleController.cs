using EShop_DB.Common.Constants;
using EShop_DB.Data;
using EShop_DB.Models.MainModels;
using EShop_DB.Common.Constants.Routes;
using Microsoft.AspNetCore.Mvc;

namespace EShop_DB.Controllers;

[ApiController, Route("role")]
public class RoleController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Role";

    public RoleController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet, Route(ApiRoutes.Universal.GetById)]
    public IActionResult GetRoleById([FromRoute] Guid id)
    {
        var result = _dbContext.Roles.FirstOrDefault(r => r.RoleId.Equals(id));

        if (result is null)
        {
            return BadRequest(ErrorMessages.Universal.NotFoundWithId(_entity, id));
        }

        return Ok(result);
    }

    [HttpGet, Route(ApiRoutes.Universal.GetAll)]
    public IActionResult GetAllRoles()
    {
        List<Role> result = _dbContext.Roles.ToList();

        return Ok(result);
    }
}
