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
public class ChatRoomMemberController : ControllerBase
{
    private readonly IChatRoomMemberService ChatRoomMemberService;
    private readonly IMapper mapper;

    #region 建構式
    public ChatRoomMemberController(IChatRoomMemberService ChatRoomMemberService,
        IMapper mapper)
    {
        this.ChatRoomMemberService = ChatRoomMemberService;
        this.mapper = mapper;
    }
    #endregion

    #region C 新增
    [HttpPost]
    public async Task<ActionResult<APIResult<ChatRoomMemberDto>>> Post([FromBody] ChatRoomMemberDto data)
    {
        APIResult<ChatRoomMemberDto> apiResult;

        #region 驗證 DTO 物件的資料一致性
        if (!ModelState.IsValid)
        {
            apiResult = APIResultFactory.Build<ChatRoomMemberDto>(false, StatusCodes.Status400BadRequest,
                  "傳送過來的資料有問題", payload: data);
            return BadRequest(apiResult);
        }
        #endregion


        #region 新增記錄前的紀錄完整性檢查
        VerifyRecordResult<ChatRoomMemberDto> verify = await ChatRoomMemberService.BeforeAddCheckAsync(data);
        if (verify.Success == false)
        {
            apiResult = APIResultFactory.Build<ChatRoomMemberDto>(false, StatusCodes.Status400BadRequest,
                  verify.Message, payload: data);
            return BadRequest(apiResult);
        }
        #endregion

        var verifyRecordResult = await ChatRoomMemberService.AddAsync(data);
        if (verifyRecordResult.Success)
        {
            apiResult = APIResultFactory.Build<ChatRoomMemberDto>(true, StatusCodes.Status201Created,
                "", payload: verifyRecordResult.Result);
        }
        else
        {
            apiResult = APIResultFactory.Build<ChatRoomMemberDto>(false, StatusCodes.Status200OK,
                "無法新增紀錄", payload: data);
        }
        return Ok(apiResult);
    }
    #endregion

    #region R 查詢
    [HttpGet]
    public async Task<ActionResult<APIResult<List<ChatRoomMemberDto>>>> Get()
    {
        APIResult<List<ChatRoomMemberDto>> apiResult;

        #region 建立查詢物件
        DataRequest dataRequest = new DataRequest()
        {
            Skip = 0,
            Take = 0,
            Search = "",
            Sorted = null,
        };
        #endregion

        var records = await ChatRoomMemberService.GetAsync(dataRequest);
        var result = mapper.Map<List<ChatRoomMemberDto>>(records.Result);
        apiResult = APIResultFactory.Build<List<ChatRoomMemberDto>>(true, StatusCodes.Status200OK,
            "", payload: result);
        return Ok(apiResult);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<APIResult<ChatRoomMemberDto>>> Get([FromRoute] int id)
    {
        APIResult<ChatRoomMemberDto> apiResult;
        var record = await ChatRoomMemberService.GetAsync(id);
        if (record != null)
        {
            apiResult = APIResultFactory.Build<ChatRoomMemberDto>(true, StatusCodes.Status200OK,
                "", payload: record);
        return Ok(apiResult);
        }
        else
        {
            apiResult = APIResultFactory.Build<ChatRoomMemberDto>(false, StatusCodes.Status400BadRequest,
                "沒有任何符合資料存在", payload: null);
        return BadRequest(apiResult);
        }

    }
    #endregion

    #region U 更新
    [HttpPut("{id}")]
    public async Task<ActionResult<APIResult<ChatRoomMemberDto>>> Put([FromRoute] int id, [FromBody] ChatRoomMemberDto data)
    {
        APIResult<ChatRoomMemberDto> apiResult;

        #region 驗證 DTO 物件的資料一致性
        if (!ModelState.IsValid)
        {
            apiResult = APIResultFactory.Build<ChatRoomMemberDto>(false, StatusCodes.Status400BadRequest,
                "傳送過來的資料有問題", payload: data);
            return BadRequest(apiResult);
        }
        #endregion

        var record = await ChatRoomMemberService.GetAsync(id);
        if (record != null)
        {
            #region 修改記錄前的紀錄完整性檢查
            VerifyRecordResult<ChatRoomMemberDto> verify = await ChatRoomMemberService.BeforeUpdateCheckAsync(record);
            if (verify.Success == false)
            {
                apiResult = APIResultFactory.Build<ChatRoomMemberDto>(false, StatusCodes.Status400BadRequest,
                      verify.Message, payload: record);
                return BadRequest(apiResult);
            }
            #endregion

            data.Id= id;
            var verifyRecordResult = await ChatRoomMemberService.UpdateAsync(data);
            if (verifyRecordResult.Success)
            {
                apiResult = APIResultFactory.Build<ChatRoomMemberDto>(true, StatusCodes.Status202Accepted,
                    "", payload: null);
                return Ok(apiResult);
            }
            else
            {
                apiResult = APIResultFactory.Build<ChatRoomMemberDto>(false, StatusCodes.Status400BadRequest,
                    "無法修改紀錄", payload: record);
                return BadRequest(apiResult);
            }
        }
        else
        {
            apiResult = APIResultFactory.Build<ChatRoomMemberDto>(false, StatusCodes.Status400BadRequest,
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
        var record = await ChatRoomMemberService.GetAsync(id);
        if (record != null)
        {
            #region 刪除記錄前的紀錄完整性檢查
            VerifyRecordResult<ChatRoomMemberDto> verify = await ChatRoomMemberService.BeforeDeleteCheckAsync(record);
            if (verify.Success == false)
            {
                apiResult = APIResultFactory.Build<object>(false, StatusCodes.Status400BadRequest,
                      verify.Message, payload: null);
                return BadRequest(apiResult);
            }
            #endregion

            var verifyRecordResult = await ChatRoomMemberService.DeleteAsync(id);
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
