using EShop_DB.Common.Constants;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.DbModels.SecondaryModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.RecipientContr)]
public class RecipientController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity = "Recipient";

    public RecipientController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreatePath)]
    public IActionResult AddRecipient([FromBody] Recipient recipient)
    {
        if (_dbContext.Recipients.Any(r => r.PhoneNumber.Equals(recipient.PhoneNumber)))
        {
            return BadRequest(new LambdaResponse(ErrorMessages.RecipientMessages.AlreadyExistsPhone));
        }

        if (!recipient.RecipientId.Equals(Guid.Empty))
        {
            if (_dbContext.Recipients.Any(r => r.RecipientId.Equals(recipient.RecipientId)))
            {
                return BadRequest(
                    new LambdaResponse(ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, recipient.RecipientId)));
            }
        }
        else
        {
            recipient.RecipientId = Guid.NewGuid();
        }

        _dbContext.Recipients.Add(recipient);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteControllerPath)]
    public IActionResult DeleteRecipient([FromRoute] Guid id)
    {
        var result = _dbContext.Recipients.FirstOrDefault(r => r.RecipientId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        _dbContext.Recipients.Remove(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdatePath)]
    public IActionResult UpdateRecipient([FromBody] Recipient recipient)
    {
        var result = _dbContext.Recipients.FirstOrDefault(r => r.RecipientId.Equals(recipient.RecipientId));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, recipient.RecipientId)));
        }

        result.RecipientId = recipient.RecipientId;
        result.PhoneNumber = recipient.PhoneNumber;
        result.Name = recipient.Name;
        result.LastName = recipient.LastName;
        result.Patronymic = recipient.Patronymic;
        result.OrderId = recipient.OrderId;
        result.Order = recipient.Order;
        result.UserId = recipient.UserId;
        result.User = recipient.User;

        _dbContext.Recipients.Update(result);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdControllerPath)]
    public IActionResult GetRecipientById([FromRoute] Guid id)
    {
        var result = _dbContext.Recipients.FirstOrDefault(r => r.RecipientId.Equals(id));

        if (result is null)
        {
            return BadRequest(new LambdaResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(result);
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllPath)]
    public IActionResult GetAllRecipients()
    {
        List<Recipient> result = _dbContext.Recipients.ToList();

        return Ok(result);
    }
}