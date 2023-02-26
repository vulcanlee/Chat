using AutoMapper;
using Business.Factories;
using CommonDomain.Models;
using CommonDomainLayer.Enums;
using DataTransferObject.Dtos;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Chat.Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class ExceptionRecordController : ControllerBase
{
    private readonly IExceptionRecordService ExceptionRecordService;
    private readonly IMapper mapper;

    #region 建構式
    public ExceptionRecordController(IExceptionRecordService ExceptionRecordService,
        IMapper mapper)
    {
        this.ExceptionRecordService = ExceptionRecordService;
        this.mapper = mapper;
    }
    #endregion

    #region C 新增
    [HttpPost]
    public async Task<ActionResult<APIResult<ExceptionRecordDto>>> Post([FromBody] ExceptionRecordDto data)
    {
        APIResult<ExceptionRecordDto> apiResult;

        #region 驗證 DTO 物件的資料一致性
        if (!ModelState.IsValid)
        {
            apiResult = APIResultFactory.Build<ExceptionRecordDto>(false, StatusCodes.Status400BadRequest,
                  "傳送過來的資料有問題", payload: data);
            return BadRequest(apiResult);
        }
        #endregion


        #region 新增記錄前的紀錄完整性檢查
        VerifyRecordResult<ExceptionRecordDto> verify = await ExceptionRecordService.BeforeAddCheckAsync(data);
        if (verify.Success == false)
        {
            apiResult = APIResultFactory.Build<ExceptionRecordDto>(false, StatusCodes.Status400BadRequest,
                  verify.Message, payload: data);
            return BadRequest(apiResult);
        }
        #endregion

        var verifyRecordResult = await ExceptionRecordService.AddAsync(data);
        if (verifyRecordResult.Success)
        {
            apiResult = APIResultFactory.Build<ExceptionRecordDto>(true, StatusCodes.Status201Created,
                "", payload: null);
        }
        else
        {
            apiResult = APIResultFactory.Build<ExceptionRecordDto>(false, StatusCodes.Status200OK,
                "無法新增紀錄", payload: data);
        }
        return Ok(apiResult);
    }

    [Route("Collection")]
    [HttpPost]
    public async Task<ActionResult<APIResult<List<ExceptionRecordDto>>>> Post([FromBody] List<ExceptionRecordDto> datas)
    {
        APIResult<List<ExceptionRecordDto>> apiResult = new();

        #region 驗證 DTO 物件的資料一致性
        #endregion

        foreach (var record in datas)
        {

            #region 新增記錄前的紀錄完整性檢查
            VerifyRecordResult<ExceptionRecordDto> verify = await ExceptionRecordService.BeforeAddCheckAsync(record);
            #endregion

            var verifyRecordResult = await ExceptionRecordService.AddAsync(record);
            if (verifyRecordResult.Success)
            {
                apiResult = APIResultFactory.Build<List<ExceptionRecordDto>>(true, StatusCodes.Status201Created,
                    "", payload: null);
            }
            else
            {
                apiResult = APIResultFactory.Build<List<ExceptionRecordDto>>(false, StatusCodes.Status200OK,
                    "無法新增紀錄", payload: null);
            }
        }

        apiResult = APIResultFactory.Build<List<ExceptionRecordDto>>(true, StatusCodes.Status201Created,
            "", payload: datas);

        return Ok(apiResult);
    }
    #endregion

    #region R 查詢
    [HttpGet]
    public async Task<ActionResult<APIResult<List<ExceptionRecordDto>>>> Get()
    {
        APIResult<List<ExceptionRecordDto>> apiResult;

        #region 建立查詢物件
        DataRequest dataRequest = new DataRequest()
        {
            Skip = 0,
            Take = 0,
            Search = "",
            Sorted = null,
        };
        #endregion

        var records = await ExceptionRecordService.GetAsync(dataRequest);
        var result = mapper.Map<List<ExceptionRecordDto>>(records.Result);
        apiResult = APIResultFactory.Build<List<ExceptionRecordDto>>(true, StatusCodes.Status200OK,
            "", payload: result);
        return Ok(apiResult);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<APIResult<ExceptionRecordDto>>> Get([FromRoute] int id)
    {
        APIResult<ExceptionRecordDto> apiResult;
        var record = await ExceptionRecordService.GetAsync(id);
        if (record != null)
        {
            apiResult = APIResultFactory.Build<ExceptionRecordDto>(true, StatusCodes.Status200OK,
                "", payload: record);
        return Ok(apiResult);
        }
        else
        {
            apiResult = APIResultFactory.Build<ExceptionRecordDto>(false, StatusCodes.Status400BadRequest,
                "沒有任何符合資料存在", payload: null);
        return BadRequest(apiResult);
        }

    }
    #endregion

    #region U 更新
    [HttpPut("{id}")]
    public async Task<ActionResult<APIResult<ExceptionRecordDto>>> Put([FromRoute] int id, [FromBody] ExceptionRecordDto data)
    {
        APIResult<ExceptionRecordDto> apiResult;

        #region 驗證 DTO 物件的資料一致性
        if (!ModelState.IsValid)
        {
            apiResult = APIResultFactory.Build<ExceptionRecordDto>(false, StatusCodes.Status400BadRequest,
                "傳送過來的資料有問題", payload: data);
            return BadRequest(apiResult);
        }
        #endregion

        var record = await ExceptionRecordService.GetAsync(id);
        if (record != null)
        {
            #region 修改記錄前的紀錄完整性檢查
            VerifyRecordResult<ExceptionRecordDto> verify = await ExceptionRecordService.BeforeUpdateCheckAsync(record);
            if (verify.Success == false)
            {
                apiResult = APIResultFactory.Build<ExceptionRecordDto>(false, StatusCodes.Status400BadRequest,
                      verify.Message, payload: record);
                return BadRequest(apiResult);
            }
            #endregion

            data.Id= id;
            var verifyRecordResult = await ExceptionRecordService.UpdateAsync(data);
            if (verifyRecordResult.Success)
            {
                apiResult = APIResultFactory.Build<ExceptionRecordDto>(true, StatusCodes.Status202Accepted,
                    "", payload: null);
                return Ok(apiResult);
            }
            else
            {
                apiResult = APIResultFactory.Build<ExceptionRecordDto>(false, StatusCodes.Status400BadRequest,
                    "無法修改紀錄", payload: record);
                return BadRequest(apiResult);
            }
        }
        else
        {
            apiResult = APIResultFactory.Build<ExceptionRecordDto>(false, StatusCodes.Status400BadRequest,
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
        var record = await ExceptionRecordService.GetAsync(id);
        if (record != null)
        {
            #region 刪除記錄前的紀錄完整性檢查
            VerifyRecordResult<ExceptionRecordDto> verify = await ExceptionRecordService.BeforeDeleteCheckAsync(record);
            if (verify.Success == false)
            {
                apiResult = APIResultFactory.Build<object>(false, StatusCodes.Status400BadRequest,
                      verify.Message, payload: null);
                return BadRequest(apiResult);
            }
            #endregion

            var verifyRecordResult = await ExceptionRecordService.DeleteAsync(id);
            if (verifyRecordResult.Success)
            {
                apiResult = APIResultFactory.Build<object>(true, StatusCodes.Status202Accepted,
                    "", payload: null);
                return Ok(apiResult);
            }
            else
            {
                apiResult = APIResultFactory.Build<object>(false, StatusCodes.Status400BadRequest,
                    "無法刪除紀錄", payload: null);
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
