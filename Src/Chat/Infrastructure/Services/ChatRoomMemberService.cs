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

public class ChatRoomMemberService : IChatRoomMemberService
{
    private readonly ChatDBContext context;

    public IMapper Mapper { get; }

    public ChatRoomMemberService(ChatDBContext context, IMapper mapper)
    {
        this.context = context;
        Mapper = mapper;
    }

    public async Task<DataRequestResult<ChatRoomMemberDto>> GetAsync(DataRequest dataRequest)
    {
        List<ChatRoomMemberDto> data = new List<ChatRoomMemberDto>();
        DataRequestResult<ChatRoomMemberDto> result = new DataRequestResult<ChatRoomMemberDto>();
        var DataSource = context.ChatRoomMember
            .AsNoTracking()
            .AsQueryable();

        #region 進行搜尋動作
        if (!string.IsNullOrWhiteSpace(dataRequest.Search))
        {
            DataSource = DataSource
                .Include(x => x.User)
                .Where(x => x.User.Name.Contains(dataRequest.Search) ||
                x.User.Account.Contains(dataRequest.Search) ||
                x.User.PhoneNumber.Contains(dataRequest.Search));
        }
        #endregion

        #region 進行排序動作
        //if (dataRequest.Sorted != null)
        //{
        //    SortCondition CurrentSortCondition = dataRequest.Sorted;
        //    switch (CurrentSortCondition.Id)
        //    {
        //        case (int)ChatRoomMemberSortEnum.NameDescending:
        //            DataSource = DataSource.OrderByDescending(x => x.Name);
        //            break;
        //        case (int)ChatRoomMemberSortEnum.NameAscending:
        //            DataSource = DataSource.OrderBy(x => x.Name);
        //            break;
        //        case (int)ChatRoomMemberSortEnum.UpdateAtDescending:
        //            DataSource = DataSource.OrderByDescending(x => x.UpdateAt);
        //            break;
        //        case (int)ChatRoomMemberSortEnum.UpdateAtAscending:
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
        result.Count = DataSource.Cast<ChatRoomMember>().Count();
        DataSource = DataSource.Skip(dataRequest.Skip);
        if (dataRequest.Take != 0)
        {
            DataSource = DataSource.Take(dataRequest.Take);
        }
        #endregion

        #region 在這裡進行取得資料與與額外屬性初始化
        List<ChatRoomMemberDto> adapterModelObjects =
            Mapper.Map<List<ChatRoomMemberDto>>(DataSource);

        foreach (var adapterModelItem in adapterModelObjects)
        {
            await OhterDependencyData(adapterModelItem);
        }
        #endregion

        result.Result = adapterModelObjects;
        await Task.Yield();
        return result;
    }

    public async Task<ChatRoomMemberDto> GetAsync(int id)

    {
        ChatRoomMember item = await context.ChatRoomMember
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return null;
        }
        ChatRoomMemberDto result = Mapper.Map<ChatRoomMemberDto>(item);
        await OhterDependencyData(result);
        return result;
    }

    public async Task<VerifyRecordResult<ChatRoomMemberDto>> AddAsync(ChatRoomMemberDto paraObject)
    {
        ChatRoomMember itemParameter = Mapper.Map<ChatRoomMember>(paraObject);
        CleanTrackingHelper.Clean<ChatRoomMember>(context);
        try
        {
            itemParameter.CreateAt = DateTime.Now;
            await context.ChatRoomMember
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
            paraObject = Mapper.Map<ChatRoomMemberDto>(itemParameter);
            return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(paraObject);
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<ChatRoomMember>(context);
        }
    }

    public async Task<VerifyRecordResult<ChatRoomMemberDto>> UpdateAsync(ChatRoomMemberDto paraObject)
    {
        try
        {
            ChatRoomMember itemData = Mapper.Map<ChatRoomMember>(paraObject);
            CleanTrackingHelper.Clean<ChatRoomMember>(context);
            ChatRoomMember item = await context.ChatRoomMember
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(MagicObject.CannotFindRecordForUpdate, null);
            }
            else
            {
                CleanTrackingHelper.Clean<ChatRoomMember>(context);
                context.Entry(itemData).State = EntityState.Modified;
                await context.SaveChangesAsync();
                paraObject = Mapper.Map<ChatRoomMemberDto>(itemData);
                return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(paraObject);
            }
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<ChatRoomMember>(context);
        }
    }

    public async Task<VerifyRecordResult<ChatRoomMemberDto>> DeleteAsync(int id)
    {
        try
        {

            CleanTrackingHelper.Clean<ChatRoomMember>(context);
            ChatRoomMember item = await context.ChatRoomMember
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(MagicObject.CannotDeleteRecord);
            }
            else
            {
                CleanTrackingHelper.Clean<ChatRoomMember>(context);
                context.Entry(item).State = EntityState.Deleted;
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<ChatRoomMember>(context);
                var deleteObject = Mapper.Map<ChatRoomMemberDto>(item);
                return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(deleteObject);
            }
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<ChatRoomMember>(context);
        }
    }
    public async Task<VerifyRecordResult<ChatRoomMemberDto>> BeforeAddCheckAsync(ChatRoomMemberDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(paraObject);
    }

    public async Task<VerifyRecordResult<ChatRoomMemberDto>> BeforeUpdateCheckAsync(ChatRoomMemberDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(paraObject);
    }

    public async Task<VerifyRecordResult<ChatRoomMemberDto>> BeforeDeleteCheckAsync(ChatRoomMemberDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<ChatRoomMemberDto>(paraObject);
    }
    async Task OhterDependencyData(ChatRoomMemberDto data)
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
