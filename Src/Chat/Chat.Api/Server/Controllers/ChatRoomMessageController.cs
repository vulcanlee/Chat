using AutoMapper;
using Business.Factories;
using CommonDomain.Models;
using CommonDomainLayer.Enums;
using CommonDomainLayer.Magics;
using DataTransferObject.Dtos;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Chat.Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class ChatRoomMessageController : ControllerBase
{
    private readonly IChatRoomMessageService ChatRoomMessageService;
    private readonly IMapper mapper;

    #region 建構式
    public ChatRoomMessageController(IChatRoomMessageService ChatRoomMessageService,
        IMapper mapper)
    {
        this.ChatRoomMessageService = ChatRoomMessageService;
        this.mapper = mapper;
    }
    #endregion

    #region C 新增
    [HttpPost]
    public async Task<ActionResult<APIResult<ChatRoomMessageDto>>> Post([FromBody] ChatRoomMessageDto data)
    {
        APIResult<ChatRoomMessageDto> apiResult;

        #region 驗證 DTO 物件的資料一致性
        if (!ModelState.IsValid)
        {
            apiResult = APIResultFactory.Build<ChatRoomMessageDto>(false, StatusCodes.Status400BadRequest,
                  "傳送過來的資料有問題", payload: data);
            return BadRequest(apiResult);
        }
        #endregion


        #region 新增記錄前的紀錄完整性檢查
        VerifyRecordResult<ChatRoomMessageDto> verify = await ChatRoomMessageService.BeforeAddCheckAsync(data);
        if (verify.Success == false)
        {
            apiResult = APIResultFactory.Build<ChatRoomMessageDto>(false, StatusCodes.Status400BadRequest,
                  verify.Message, payload: data);
            return BadRequest(apiResult);
        }
        #endregion

        var verifyRecordResult = await ChatRoomMessageService.AddAsync(data);
        if (verifyRecordResult.Success)
        {
            apiResult = APIResultFactory.Build<ChatRoomMessageDto>(true, StatusCodes.Status201Created,
                "", payload: null);
        }
        else
        {
            apiResult = APIResultFactory.Build<ChatRoomMessageDto>(false, StatusCodes.Status200OK,
                "無法新增紀錄", payload: data);
        }
        return Ok(apiResult);
    }
    #endregion

    #region R 查詢
    [HttpGet]
    public async Task<ActionResult<APIResult<List<ChatRoomMessageDto>>>> Get()
    {
        APIResult<List<ChatRoomMessageDto>> apiResult;

        #region 建立查詢物件
        DataRequest dataRequest = new DataRequest()
        {
            Skip = 0,
            Take = 0,
            Search = "",
            Sorted = null,
        };
        #endregion

        var records = await ChatRoomMessageService.GetAsync(dataRequest);
        var result = mapper.Map<List<ChatRoomMessageDto>>(records.Result);
        apiResult = APIResultFactory.Build<List<ChatRoomMessageDto>>(true, StatusCodes.Status200OK,
            "", payload: result);
        return Ok(apiResult);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<APIResult<ChatRoomMessageDto>>> Get([FromRoute] int id)
    {
        APIResult<ChatRoomMessageDto> apiResult;
        var record = await ChatRoomMessageService.GetAsync(id);
        if (record != null)
        {
            apiResult = APIResultFactory.Build<ChatRoomMessageDto>(true, StatusCodes.Status200OK,
                "", payload: record);
        return Ok(apiResult);
        }
        else
        {
            apiResult = APIResultFactory.Build<ChatRoomMessageDto>(false, StatusCodes.Status400BadRequest,
                "沒有任何符合資料存在", payload: null);
        return BadRequest(apiResult);
        }

    }
    #endregion

    #region U 更新
    [HttpPut("{id}")]
    public async Task<ActionResult<APIResult<ChatRoomMessageDto>>> Put([FromRoute] int id, [FromBody] ChatRoomMessageDto data)
    {
        APIResult<ChatRoomMessageDto> apiResult;

        #region 驗證 DTO 物件的資料一致性
        if (!ModelState.IsValid)
        {
            apiResult = APIResultFactory.Build<ChatRoomMessageDto>(false, StatusCodes.Status400BadRequest,
                "傳送過來的資料有問題", payload: data);
            return BadRequest(apiResult);
        }
        #endregion

        var record = await ChatRoomMessageService.GetAsync(id);
        if (record != null)
        {
            #region 修改記錄前的紀錄完整性檢查
            VerifyRecordResult<ChatRoomMessageDto> verify = await ChatRoomMessageService.BeforeUpdateCheckAsync(record);
            if (verify.Success == false)
            {
                apiResult = APIResultFactory.Build<ChatRoomMessageDto>(false, StatusCodes.Status400BadRequest,
                      verify.Message, payload: record);
                return BadRequest(apiResult);
            }
            #endregion

            data.Id= id;
            var verifyRecordResult = await ChatRoomMessageService.UpdateAsync(data);
            if (verifyRecordResult.Success)
            {
                apiResult = APIResultFactory.Build<ChatRoomMessageDto>(true, StatusCodes.Status202Accepted,
                    "", payload: null);
                return Ok(apiResult);
            }
            else
            {
                apiResult = APIResultFactory.Build<ChatRoomMessageDto>(false, StatusCodes.Status400BadRequest,
                    "無法修改紀錄", payload: record);
                return BadRequest(apiResult);
            }
        }
        else
        {
            apiResult = APIResultFactory.Build<ChatRoomMessageDto>(false, StatusCodes.Status400BadRequest,
                "沒有任何符合資料存在", payload: data);
            return BadRequest(apiResult);
        }
    }
    #endregion

    #region D 刪除
    [HttpDelete("{id}")]
    public async Task<ActionResult<APIResult<object>>> Delete([FromRoute] int id)
    {
        APIResult<object> apiResult;
        var record = await ChatRoomMessageService.GetAsync(id);
        if (record != null)
        {
            #region 刪除記錄前的紀錄完整性檢查
            VerifyRecordResult<ChatRoomMessageDto> verify = await ChatRoomMessageService.BeforeDeleteCheckAsync(record);
            if (verify.Success == false)
            {
                apiResult = APIResultFactory.Build<object>(false, StatusCodes.Status400BadRequest,
                      verify.Message, payload: null);
                return BadRequest(apiResult);
            }
            #endregion

            var verifyRecordResult = await ChatRoomMessageService.DeleteAsync(id);
            if (verifyRecordResult.Success)
            {
                apiResult = APIResultFactory.Build<object>(true, StatusCodes.Status202Accepted,
                    "", payload: null);
                return Ok(apiResult);
            }
            else
            {
                apiResult = APIResultFactory.Build<object>(false, StatusCodes.Status400BadRequest,
                    MagicObject.CannotDeleteRecord, payload: null);
                return BadRequest(apiResult);
            }
        }
        else
        {
            apiResult = APIResultFactory.Build<object>(false, StatusCodes.Status400BadRequest,
                "沒有任何符合資料存在", payload: null);
            return BadRequest(apiResult);
        }
    }
    #endregion

}
