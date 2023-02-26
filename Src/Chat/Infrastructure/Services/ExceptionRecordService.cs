using AutoMapper;
using Business.Factories;
using CommonDomain.Models;
using CommonDomainLayer.Enums;
using DataTransferObject.Dtos;
using DataTransferObject.Enums;
using DomainData.Models;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class ExceptionRecordService : IExceptionRecordService
{
    private readonly ChatDBContext context;

    public IMapper Mapper { get; }

    public ExceptionRecordService(ChatDBContext context, IMapper mapper)
    {
        this.context = context;
        Mapper = mapper;
    }

    public async Task<DataRequestResult<ExceptionRecordDto>> GetAsync(DataRequest dataRequest)
    {
        List<ExceptionRecordDto> data = new List<ExceptionRecordDto>();
        DataRequestResult<ExceptionRecordDto> result = new DataRequestResult<ExceptionRecordDto>();
        var DataSource = context.ExceptionRecord
            .AsNoTracking()
            .Include(x => x.User)
            .AsQueryable();
        #region 進行搜尋動作
        if (!string.IsNullOrWhiteSpace(dataRequest.Search))
        {
            DataSource = DataSource
            .Where(x => x.Message.Contains(dataRequest.Search) ||
            x.CallStack.Contains(dataRequest.Search));
        }
        #endregion

        #region 進行排序動作
        if (dataRequest.Sorted != null)
        {
            SortCondition CurrentSortCondition = dataRequest.Sorted;
            switch (CurrentSortCondition.Id)
            {
                case (int)ExceptionRecordSortEnum.MessageDescending:
                    DataSource = DataSource.OrderByDescending(x => x.Message);
                    break;
                case (int)ExceptionRecordSortEnum.MessageAscending:
                    DataSource = DataSource.OrderBy(x => x.Message);
                    break;
                case (int)ExceptionRecordSortEnum.ExceptionTimeDescending:
                    DataSource = DataSource.OrderByDescending(x => x.ExceptionTime);
                    break;
                case (int)ExceptionRecordSortEnum.ExceptionTimeAscending:
                    DataSource = DataSource.OrderBy(x => x.ExceptionTime);
                    break;
                default:
                    DataSource = DataSource.OrderBy(x => x.Id);
                    break;
            }
        }
        #endregion

        #region 進行分頁
        // 取得記錄總數量，將要用於分頁元件面板使用
        result.Count = DataSource.Cast<ExceptionRecord>().Count();
        DataSource = DataSource.Skip(dataRequest.Skip);
        if (dataRequest.Take != 0)
        {
            DataSource = DataSource.Take(dataRequest.Take);
        }
        #endregion

        #region 在這裡進行取得資料與與額外屬性初始化
        List<ExceptionRecordDto> adapterModelObjects =
            Mapper.Map<List<ExceptionRecordDto>>(DataSource);

        foreach (var adapterModelItem in adapterModelObjects)
        {
            await OhterDependencyData(adapterModelItem);
        }
        #endregion

        result.Result = adapterModelObjects;
        await Task.Yield();
        return result;
    }

    public async Task<ExceptionRecordDto> GetAsync(int id)

    {
        ExceptionRecord item = await context.ExceptionRecord
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return null;
        }
        ExceptionRecordDto result = Mapper.Map<ExceptionRecordDto>(item);
        await OhterDependencyData(result);
        return result;
    }

    public async Task<VerifyRecordResult> AddAsync(ExceptionRecordDto paraObject)
    {
        ExceptionRecord itemParameter = Mapper.Map<ExceptionRecord>(paraObject);
        CleanTrackingHelper.Clean<ExceptionRecord>(context);
        try
        {
            await context.ExceptionRecord
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        CleanTrackingHelper.Clean<ExceptionRecord>(context);
        return VerifyRecordResultFactory.Build(true);
    }

    public async Task<VerifyRecordResult> UpdateAsync(ExceptionRecordDto paraObject)
    {
        ExceptionRecord itemData = Mapper.Map<ExceptionRecord>(paraObject);
        CleanTrackingHelper.Clean<ExceptionRecord>(context);
        ExceptionRecord item = await context.ExceptionRecord
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (item == null)
        {
            return VerifyRecordResultFactory.Build(false, "無法修改紀錄");
        }
        else
        {
            CleanTrackingHelper.Clean<ExceptionRecord>(context);
            context.Entry(itemData).State = EntityState.Modified;
            await context.SaveChangesAsync();
            CleanTrackingHelper.Clean<ExceptionRecord>(context);
            return VerifyRecordResultFactory.Build(true);
        }
    }

    public async Task<VerifyRecordResult> DeleteAsync(int id)
    {
        CleanTrackingHelper.Clean<ExceptionRecord>(context);
        ExceptionRecord item = await context.ExceptionRecord
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return VerifyRecordResultFactory.Build(false, "無法刪除紀錄");
        }
        else
        {
            CleanTrackingHelper.Clean<ExceptionRecord>(context);
            context.Entry(item).State = EntityState.Deleted;
            await context.SaveChangesAsync();
            CleanTrackingHelper.Clean<ExceptionRecord>(context);
            return VerifyRecordResultFactory.Build(true);
        }
    }
    public async Task<VerifyRecordResult> BeforeAddCheckAsync(ExceptionRecordDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build(true);
    }

    public async Task<VerifyRecordResult> BeforeUpdateCheckAsync(ExceptionRecordDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build(true);
    }
    public async Task<VerifyRecordResult> BeforeDeleteCheckAsync(ExceptionRecordDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build(true);
    }
    async Task OhterDependencyData(ExceptionRecordDto data)
    {
        try
        {
            if (data.UserId != null)
            {
                CleanTrackingHelper.Clean<User>(context);
                var user = await context.User
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == data.UserId);
                if (user != null)
                {
                    data.UserName = user.Name;
                }
            }
        }
        catch (Exception)
        {
        }
        return;
    }
}
