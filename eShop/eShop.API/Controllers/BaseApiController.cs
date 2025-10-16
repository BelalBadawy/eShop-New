
using eShop.API.Helpers;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace eStoreCA.API.Infrastructure
{


    [Route("/api/v{version:apiVersion}/[controller]/")]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class BaseApiController : ControllerBase
    {

        private ISender _sender = null;
        public ISender Sender => _sender ??= HttpContext.RequestServices.GetService<ISender>();

        //private IConfiguration _configuration;
        //protected IConfiguration _ConfigurationApp
        //{
        //    get
        //    {
        //        if (_configuration == null)
        //        {
        //            _configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        //        }

        //        return _configuration;
        //    }
        //}


        //private IMediator _mediator;
        //protected IMediator _Mediator
        //{
        //    get
        //    {
        //        if (_mediator == null)
        //        {
        //            _mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
        //        }

        //        return _mediator;
        //    }
        //}

        //private ICurrentUserService _currentUserService;
        //protected ICurrentUserService _CurrentUserService
        //{
        //    get
        //    {
        //        if (_currentUserService == null)
        //        {
        //            _currentUserService = HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();
        //        }

        //        return _currentUserService;
        //    }
        //}


        //private IMapper _mapper;
        //protected IMapper _Mapper
        //{
        //    get
        //    {
        //        if (_mapper == null)
        //        {
        //            _mapper = HttpContext.RequestServices.GetRequiredService<IMapper>();
        //        }

        //        return _mapper;
        //    }
        //}

        //private ICacheService _cache;
        //protected ICacheService _Cache
        //{
        //    get
        //    {
        //        if (_cache == null)
        //        {
        //            _cache = HttpContext.RequestServices.GetRequiredService<ICacheService>();
        //        }
        //        return _cache;
        //    }
        //}

        public BaseApiController()
        {

        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //protected IActionResult ReturnActionResult(object result = null, bool succeeded = false, List<string> errors = null, string message = null, object data = null)
        //{
        //    if (bool.Parse(_ConfigurationApp["ReturnCustomResult"]))
        //    {
        //        if (result != null)
        //        {
        //            return Ok(result);
        //        }
        //        else
        //        {
        //            if (errors != null)
        //            {
        //                return Ok(new MyAppResponse<int>(errors: errors));
        //            }
        //            else
        //            {
        //                return Ok(new MyAppResponse<int>(message: message));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (result != null)
        //        {
        //            if (succeeded)
        //            {
        //                return Ok(data);
        //            }
        //            else
        //            {
        //                if (errors != null && errors.Any())
        //                {
        //                    return BadRequest(errors);
        //                }
        //                else
        //                {
        //                    return BadRequest(message);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (errors != null)
        //            {
        //                return BadRequest(string.Join(",", errors));
        //            }
        //            else
        //            {
        //                return BadRequest(message);
        //            }
        //        }
        //    }
        //}


        //public ObjectResult ActionResult<T>(IResponseWrapper response, Exception exception = null)
        //{
        //    if (response == null && exception == null)
        //    {
        //        return new BadRequestObjectResult(SD.ErrorOccurred);
        //    }

        //    if (exception != null)
        //    {
        //        if (exception is ForbiddenAccessException)
        //        {
        //            return ActionResult(new ResponseWrapper(message: exception.Message, statusCode: HttpStatusCode.Unauthorized));
        //        }
        //        else if (exception is BadRequestException)
        //        {
        //            return ActionResult(new MyAppResponse<bool>(message: exception.Message, statusCode: HttpStatusCode.BadRequest));
        //        }
        //        else if (exception is NotFoundException)
        //        {
        //            return ActionResult(new MyAppResponse<bool>(message: exception.Message, statusCode: HttpStatusCode.NotFound));
        //        }
        //        else
        //        {
        //            return new BadRequestObjectResult(SD.ErrorOccurred);
        //        }
        //    }

        //    switch (response.StatusCode)
        //    {
        //        case HttpStatusCode.OK:
        //            return new OkObjectResult(response);
        //        case HttpStatusCode.Created:
        //            return new CreatedResult(string.Empty, response);
        //        case HttpStatusCode.Unauthorized:
        //            return new UnauthorizedObjectResult(response);
        //        case HttpStatusCode.BadRequest:
        //            return new BadRequestObjectResult(response);
        //        case HttpStatusCode.NotFound:
        //            return new NotFoundObjectResult(response);
        //        case HttpStatusCode.Accepted:
        //            return new AcceptedResult(string.Empty, response);
        //        case HttpStatusCode.UnprocessableEntity:
        //            return new UnprocessableEntityObjectResult(response);
        //        default:
        //            return new BadRequestObjectResult(response);
        //    }
        //}

        

    }
}
