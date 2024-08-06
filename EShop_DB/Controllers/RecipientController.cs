using EShop_DB.Common.Constants;
using EShop_DB.Common.Extensions;
using EShop_DB.Data;
using Microsoft.AspNetCore.Mvc;
using EShop_DB.Models.SecondaryModels;
using SharedLibrary.Models.DtoModels.SecondaryModels;
using SharedLibrary.Responses;
using SharedLibrary.Routes;

namespace EShop_DB.Controllers;

[ApiController, Route(ApiRoutesDb.Controllers.RecipientContr)]
public class RecipientController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _entity;

    public RecipientController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        
        _entity = this.GetControllerName();
    }

    [HttpPost, Route(ApiRoutesDb.UniversalActions.CreateAction)]
    public IActionResult AddRecipient([FromBody] RecipientDTO recipientDto)
    {
        var recipient = recipientDto.ToRecipient();
        
        if (_dbContext.Recipients.Any(r => r.PhoneNumber.Equals(recipient.PhoneNumber)))
        {
            return BadRequest(new UniversalResponse(errorInfo: ErrorMessages.RecipientMessages.AlreadyExistsPhone));
        }

        if (!recipient.RecipientId.Equals(Guid.Empty))
        {
            if (_dbContext.Recipients.Any(r => r.RecipientId.Equals(recipient.RecipientId)))
            {
                return BadRequest(
                    new UniversalResponse(errorInfo: ErrorMessages.UniversalMessages.AlreadyExistsId(_entity, recipient.RecipientId)));
            }
        }
        else
        {
            recipient.RecipientId = Guid.NewGuid();
        }

        _dbContext.Recipients.Add(recipient);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse<RecipientDTO>(responseObject: recipient.ToRecipientDto(), info: SuccessMessages.UniversalResponse.Created(_entity)));
    }

    [HttpDelete, Route(ApiRoutesDb.UniversalActions.DeleteAction)]
    public IActionResult DeleteRecipient([FromBody] Guid id)
    {
        var result = _dbContext.Recipients.FirstOrDefault(r => r.RecipientId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        _dbContext.Recipients.Remove(result);
        _dbContext.SaveChanges();

        return Ok(new UniversalResponse(info: SuccessMessages.UniversalResponse.Deleted(_entity)));
    }

    [HttpPut, Route(ApiRoutesDb.UniversalActions.UpdateAction)]
    public IActionResult UpdateRecipient([FromBody] RecipientDTO recipientDto)
    {
        var recipient = recipientDto.ToRecipient();
        
        var result = _dbContext.Recipients.FirstOrDefault(r => r.RecipientId.Equals(recipient.RecipientId));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, recipient.RecipientId)));
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

        return Ok(new UniversalResponse<RecipientDTO>(responseObject: recipient.ToRecipientDto(), info: SuccessMessages.UniversalResponse.Updated(_entity)));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetByIdAction)]
    public IActionResult GetRecipientById([FromBody] Guid id)
    {
        var result = _dbContext.Recipients.FirstOrDefault(r => r.RecipientId.Equals(id));

        if (result is null)
        {
            return BadRequest(new UniversalResponse(ErrorMessages.UniversalMessages.NotFoundWithId(_entity, id)));
        }

        return Ok(new UniversalResponse<RecipientDTO>(responseObject: result.ToRecipientDto()));
    }

    [HttpGet, Route(ApiRoutesDb.UniversalActions.GetAllAction)]
    public IActionResult GetAllRecipients()
    {
        List<Recipient> result = _dbContext.Recipients.ToList();

        return Ok(new UniversalResponse<List<RecipientDTO>>(responseObject: result.Select(u => u.ToRecipientDto()).ToList()));
    }
}