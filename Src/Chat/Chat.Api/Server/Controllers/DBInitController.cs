using Business.Factories;
using Business.Helpers;
using CommonDomainLayer.Configurations;
using CommonDomainLayer.Magics;
using DataTransferObject.Dtos;
using DomainData.Models;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using System.Security.Claims;

namespace Chat.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class DBInitController : ControllerBase
    {
        private readonly DatabaseInitService databaseInitService;

        public DBInitController(DatabaseInitService databaseInitService)
        {
            this.databaseInitService = databaseInitService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<APIResult<object>>>
            Get()
        {
            APIResult<object> apiResult;
            await Task.Yield();
            await databaseInitService.InitDBAsync(x=>
            { });

            apiResult = APIResultFactory.Build<object>(true, StatusCodes.Status200OK,
               "", payload: "");
            return Ok(apiResult);

        }

    }
}
