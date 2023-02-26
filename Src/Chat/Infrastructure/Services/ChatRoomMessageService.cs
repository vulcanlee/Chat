using AutoMapper;
using Business.Factories;
using CommonDomain.Models;
using CommonDomainLayer.Enums;
using CommonDomainLayer.Magics;
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

public class ChatRoomMessageService : IChatRoomMessageService
{
    private readonly ChatDBContext context;

    public IMapper Mapper { get; }

    public ChatRoomMessageService(ChatDBContext context, IMapper mapper)
    {
        this.context = context;
        Mapper = mapper;
    }

    public async Task<DataRequestResult<ChatRoomMessageDto>> GetAsync(DataRequest dataRequest)
    {
        List<ChatRoomMessageDto> data = new List<ChatRoomMessageDto>();
        DataRequestResult<ChatRoomMessageDto> result = new DataRequestResult<ChatRoomMessageDto>();
        var DataSource = context.ChatRoomMessage
            .AsNoTracking()
            .AsQueryable();

        #region 進行搜尋動作
        if (!string.IsNullOrWhiteSpace(dataRequest.Search))
        {
            DataSource = DataSource
            .Where(x => x.Content.Contains(dataRequest.Search));
        }
        #endregion

        #region 進行排序動作
        DataSource = DataSource.OrderByDescending(x => x.UpdateAt);
        //if (dataRequest.Sorted != null)
        //{
        //    SortCondition CurrentSortCondition = dataRequest.Sorted;
        //    switch (CurrentSortCondition.Id)
        //    {
        //        case (int)ChatRoomMessageSortEnum.NameDescending:
        //            DataSource = DataSource.OrderByDescending(x => x.Name);
        //            break;
        //        case (int)ChatRoomMessageSortEnum.NameAscending:
        //            DataSource = DataSource.OrderBy(x => x.Name);
        //            break;
        //        case (int)ChatRoomMessageSortEnum.UpdateAtDescending:
        //            DataSource = DataSource.OrderByDescending(x => x.UpdateAt);
        //            break;
        //        case (int)ChatRoomMessageSortEnum.UpdateAtAscending:
        //            DataSource = DataSource.OrderBy(x => x.UpdateAt);
        //            break;
        //        default:
        //            DataSource = DataSource.OrderBy(x => x.Id);
        //            break;
        //    }
        //}
        #endregion

        #region 進行分頁
        // 取得記錄總數量，將要用於分頁元件面板使用
        result.Count = DataSource.Cast<ChatRoomMessage>().Count();
        DataSource = DataSource.Skip(dataRequest.Skip);
        if (dataRequest.Take != 0)
        {
            DataSource = DataSource.Take(dataRequest.Take);
        }
        #endregion

        #region 在這裡進行取得資料與與額外屬性初始化
        List<ChatRoomMessageDto> adapterModelObjects =
            Mapper.Map<List<ChatRoomMessageDto>>(DataSource);

        foreach (var adapterModelItem in adapterModelObjects)
        {
            await OhterDependencyData(adapterModelItem);
        }
        #endregion

        result.Result = adapterModelObjects;
        await Task.Yield();
        return result;
    }

    public async Task<ChatRoomMessageDto> GetAsync(int id)

    {
        ChatRoomMessage item = await context.ChatRoomMessage
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return null;
        }
        ChatRoomMessageDto result = Mapper.Map<ChatRoomMessageDto>(item);
        await OhterDependencyData(result);
        return result;
    }

    public async Task<VerifyRecordResult<ChatRoomMessageDto>> AddAsync(ChatRoomMessageDto paraObject)
    {
        ChatRoomMessage itemParameter = Mapper.Map<ChatRoomMessage>(paraObject);
        CleanTrackingHelper.Clean<ChatRoomMessage>(context);
        try
        {
            itemParameter.CreateAt = DateTime.Now;
            itemParameter.UpdateAt = DateTime.Now;
            await context.ChatRoomMessage
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
            paraObject = Mapper.Map<ChatRoomMessageDto>(itemParameter);
            return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(paraObject);
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<ChatRoomMessage>(context);
        }
    }

    public async Task<VerifyRecordResult<ChatRoomMessageDto>> UpdateAsync(ChatRoomMessageDto paraObject)
    {
        try
        {
            ChatRoomMessage itemData = Mapper.Map<ChatRoomMessage>(paraObject);
            CleanTrackingHelper.Clean<ChatRoomMessage>(context);
            ChatRoomMessage item = await context.ChatRoomMessage
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(MagicObject.CannotFindRecordForUpdate, null);
            }
            else
            {
                itemData.UpdateAt = DateTime.Now;

                CleanTrackingHelper.Clean<ChatRoomMessage>(context);
                context.Entry(itemData).State = EntityState.Modified;
                await context.SaveChangesAsync();
                paraObject = Mapper.Map<ChatRoomMessageDto>(itemData);
                return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(paraObject);
            }
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<ChatRoomMessage>(context);
        }
    }

    public async Task<VerifyRecordResult<ChatRoomMessageDto>> DeleteAsync(int id)
    {
        try
        {

            CleanTrackingHelper.Clean<ChatRoomMessage>(context);
            ChatRoomMessage item = await context.ChatRoomMessage
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(MagicObject.CannotDeleteRecord);
            }
            else
            {
                CleanTrackingHelper.Clean<ChatRoomMessage>(context);
                context.Entry(item).State = EntityState.Deleted;
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<ChatRoomMessage>(context);
                var deleteObject = Mapper.Map<ChatRoomMessageDto>(item);
                return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(deleteObject);
            }
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<ChatRoomMessage>(context);
        }
    }
    public async Task<VerifyRecordResult<ChatRoomMessageDto>> BeforeAddCheckAsync(ChatRoomMessageDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(paraObject);
    }

    public async Task<VerifyRecordResult<ChatRoomMessageDto>> BeforeUpdateCheckAsync(ChatRoomMessageDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(paraObject);
    }

    public async Task<VerifyRecordResult<ChatRoomMessageDto>> BeforeDeleteCheckAsync(ChatRoomMessageDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<ChatRoomMessageDto>(paraObject);
    }
    async Task OhterDependencyData(ChatRoomMessageDto data)
    {
        try
        {
        }
        catch (Exception)
        {
        }
        return;
    }
}
