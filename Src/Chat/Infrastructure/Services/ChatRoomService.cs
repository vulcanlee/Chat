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

public class ChatRoomService : IChatRoomService
{
    private readonly ChatDBContext context;

    public IMapper Mapper { get; }

    public ChatRoomService(ChatDBContext context, IMapper mapper)
    {
        this.context = context;
        Mapper = mapper;
    }

    public async Task<DataRequestResult<ChatRoomDto>> GetAsync(DataRequest dataRequest)
    {
        List<ChatRoomDto> data = new List<ChatRoomDto>();
        DataRequestResult<ChatRoomDto> result = new DataRequestResult<ChatRoomDto>();
        var DataSource = context.ChatRoom
            .AsNoTracking()
            .AsQueryable();

        #region 進行搜尋動作
        if (!string.IsNullOrWhiteSpace(dataRequest.Search))
        {
            DataSource = DataSource
            .Where(x => x.Name.Contains(dataRequest.Search));
        }
        #endregion

        #region 進行排序動作
        if (dataRequest.Sorted != null)
        {
            SortCondition CurrentSortCondition = dataRequest.Sorted;
            switch (CurrentSortCondition.Id)
            {
                case (int)ChatRoomSortEnum.NameDescending:
                    DataSource = DataSource.OrderByDescending(x => x.Name);
                    break;
                case (int)ChatRoomSortEnum.NameAscending:
                    DataSource = DataSource.OrderBy(x => x.Name);
                    break;
                case (int)ChatRoomSortEnum.UpdateAtDescending:
                    DataSource = DataSource.OrderByDescending(x => x.UpdateAt);
                    break;
                case (int)ChatRoomSortEnum.UpdateAtAscending:
                    DataSource = DataSource.OrderBy(x => x.UpdateAt);
                    break;
                default:
                    DataSource = DataSource.OrderBy(x => x.Id);
                    break;
            }
        }
        #endregion

        #region 進行分頁
        // 取得記錄總數量，將要用於分頁元件面板使用
        result.Count = DataSource.Cast<ChatRoom>().Count();
        DataSource = DataSource.Skip(dataRequest.Skip);
        if (dataRequest.Take != 0)
        {
            DataSource = DataSource.Take(dataRequest.Take);
        }
        #endregion

        #region 在這裡進行取得資料與與額外屬性初始化
        List<ChatRoomDto> adapterModelObjects =
            Mapper.Map<List<ChatRoomDto>>(DataSource);

        foreach (var adapterModelItem in adapterModelObjects)
        {
            await OhterDependencyData(adapterModelItem);
        }
        #endregion

        result.Result = adapterModelObjects;
        await Task.Yield();
        return result;
    }

    public async Task<ChatRoomDto> GetAsync(int id)

    {
        ChatRoom item = await context.ChatRoom
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return null;
        }
        ChatRoomDto result = Mapper.Map<ChatRoomDto>(item);
        await OhterDependencyData(result);
        return result;
    }

    public async Task<VerifyRecordResult<ChatRoomDto>> AddAsync(ChatRoomDto paraObject)
    {
        ChatRoom itemParameter = Mapper.Map<ChatRoom>(paraObject);
        CleanTrackingHelper.Clean<ChatRoom>(context);
        try
        {
            itemParameter.CreateAt = DateTime.Now;
            itemParameter.UpdateAt = DateTime.Now;
            await context.ChatRoom
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
            paraObject = Mapper.Map<ChatRoomDto>(itemParameter);
            return VerifyRecordResultFactory.Build<ChatRoomDto>(paraObject);
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<ChatRoomDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<ChatRoom>(context);
        }
    }

    public async Task<VerifyRecordResult<ChatRoomDto>> UpdateAsync(ChatRoomDto paraObject)
    {
        try
        {
            ChatRoom itemData = Mapper.Map<ChatRoom>(paraObject);
            CleanTrackingHelper.Clean<ChatRoom>(context);
            ChatRoom item = await context.ChatRoom
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build<ChatRoomDto>(MagicObject.CannotFindRecordForUpdate, null);
            }
            else
            {
                itemData.UpdateAt = DateTime.Now;

                CleanTrackingHelper.Clean<ChatRoom>(context);
                context.Entry(itemData).State = EntityState.Modified;
                await context.SaveChangesAsync();
                paraObject = Mapper.Map<ChatRoomDto>(itemData);
                return VerifyRecordResultFactory.Build<ChatRoomDto>(paraObject);
            }
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<ChatRoomDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<ChatRoom>(context);
        }
    }

    public async Task<VerifyRecordResult<ChatRoomDto>> DeleteAsync(int id)
    {
        try
        {

            CleanTrackingHelper.Clean<ChatRoom>(context);
            ChatRoom item = await context.ChatRoom
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build<ChatRoomDto>(MagicObject.CannotDeleteRecord);
            }
            else
            {
                CleanTrackingHelper.Clean<ChatRoom>(context);
                context.Entry(item).State = EntityState.Deleted;
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<ChatRoom>(context);
                var deleteObject = Mapper.Map<ChatRoomDto>(item);
                return VerifyRecordResultFactory.Build<ChatRoomDto>(deleteObject);
            }
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<ChatRoomDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<ChatRoom>(context);
        }
    }
    public async Task<VerifyRecordResult<ChatRoomDto>> BeforeAddCheckAsync(ChatRoomDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<ChatRoomDto>(paraObject);
    }

    public async Task<VerifyRecordResult<ChatRoomDto>> BeforeUpdateCheckAsync(ChatRoomDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<ChatRoomDto>(paraObject);
    }

    public async Task<VerifyRecordResult<ChatRoomDto>> BeforeDeleteCheckAsync(ChatRoomDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<ChatRoomDto>(paraObject);
    }
    async Task OhterDependencyData(ChatRoomDto data)
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
