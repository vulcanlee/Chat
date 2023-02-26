using AutoMapper;
using Business.Factories;
using CommonDomain.Models;
using CommonDomainLayer.Magics;
using DataTransferObject.Dtos;
using DataTransferObject.Enums;
using DomainData.Models;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ChatDBContext context;

    public IMapper Mapper { get; }

    public UserService(ChatDBContext context, IMapper mapper)
    {
        this.context = context;
        Mapper = mapper;
    }

    public async Task<DataRequestResult<UserDto>> GetAsync(DataRequest dataRequest)
    {
        List<UserDto> data = new List<UserDto>();
        DataRequestResult<UserDto> result = new DataRequestResult<UserDto>();
        var DataSource = context.User
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
                case (int)UserSortEnum.NameDescending:
                    DataSource = DataSource.OrderByDescending(x => x.Name);
                    break;
                case (int)UserSortEnum.NameAscending:
                    DataSource = DataSource.OrderBy(x => x.Name);
                    break;
                case (int)UserSortEnum.UpdateAtDescending:
                    DataSource = DataSource.OrderByDescending(x => x.UpdateAt);
                    break;
                case (int)UserSortEnum.UpdateAtAscending:
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
        result.Count = DataSource.Cast<User>().Count();
        DataSource = DataSource.Skip(dataRequest.Skip);
        if (dataRequest.Take != 0)
        {
            DataSource = DataSource.Take(dataRequest.Take);
        }
        #endregion

        #region 在這裡進行取得資料與與額外屬性初始化
        List<UserDto> adapterModelObjects =
            Mapper.Map<List<UserDto>>(DataSource);

        foreach (var adapterModelItem in adapterModelObjects)
        {
            await OhterDependencyData(adapterModelItem);
        }
        #endregion

        result.Result = adapterModelObjects;
        await Task.Yield();
        return result;
    }

    public async Task<UserDto> GetAsync(int id)

    {
        User item = await context.User
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return null;
        }
        UserDto result = Mapper.Map<UserDto>(item);
        await OhterDependencyData(result);
        return result;
    }

    public async Task<VerifyRecordResult<UserDto>> AddAsync(UserDto paraObject)
    {
        User itemParameter = Mapper.Map<User>(paraObject);
        CleanTrackingHelper.Clean<User>(context);
        try
        {
            itemParameter.CreateAt = DateTime.Now;
            itemParameter.UpdateAt = DateTime.Now;
            await context.User
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
            paraObject = Mapper.Map<UserDto>(itemParameter);
            return VerifyRecordResultFactory.Build<UserDto>(paraObject);
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<UserDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<User>(context);
        }
    }

    public async Task<VerifyRecordResult<UserDto>> UpdateAsync(UserDto paraObject)
    {
        try
        {
            User itemData = Mapper.Map<User>(paraObject);
            CleanTrackingHelper.Clean<User>(context);
            User item = await context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build<UserDto>(MagicObject.CannotFindRecordForUpdate, null);
            }
            else
            {
                itemData.UpdateAt = DateTime.Now;

                CleanTrackingHelper.Clean<User>(context);
                context.Entry(itemData).State = EntityState.Modified;
                await context.SaveChangesAsync();
                paraObject = Mapper.Map<UserDto>(itemData);
                return VerifyRecordResultFactory.Build<UserDto>(paraObject);
            }
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<UserDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<User>(context);
        }
    }

    public async Task<VerifyRecordResult<UserDto>> DeleteAsync(int id)
    {
        try
        {

            CleanTrackingHelper.Clean<User>(context);
            User item = await context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build<UserDto>(MagicObject.CannotDeleteRecord);
            }
            else
            {
                CleanTrackingHelper.Clean<User>(context);
                context.Entry(item).State = EntityState.Deleted;
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<User>(context);
                var deleteObject = Mapper.Map<UserDto>(item);
                return VerifyRecordResultFactory.Build<UserDto>(deleteObject);
            }
        }
        catch (Exception ex)
        {
            return VerifyRecordResultFactory.Build<UserDto>(MagicObject.HasException, ex);
        }
        finally
        {
            CleanTrackingHelper.Clean<User>(context);
        }
    }
    public async Task<VerifyRecordResult<UserDto>> BeforeAddCheckAsync(UserDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<UserDto>(paraObject);
    }

    public async Task<VerifyRecordResult<UserDto>> BeforeUpdateCheckAsync(UserDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<UserDto>(paraObject);
    }

    public async Task<VerifyRecordResult<UserDto>> BeforeDeleteCheckAsync(UserDto paraObject)
    {
        await Task.Yield();
        return VerifyRecordResultFactory.Build<UserDto>(paraObject);
    }
    async Task OhterDependencyData(UserDto data)
    {
        try
        {
        }
        catch (Exception)
        {
        }
        return;
    }

    public async Task<(User, string)>
        CheckUserAsync(string account, string password)
    {
        var checkUser = await context.User
            .FirstOrDefaultAsync(x =>x.Account.ToLower() == account.ToLower());

        await Task.Yield();

        if (checkUser != null)
        {
            string passwordWithSalt = PasswordHelper
                .GetPasswordSHA(checkUser.Salt + MagicObject.PasswordSaltPostfix, password);
            if(passwordWithSalt!=checkUser.Password)
            {
                return (null, "帳號或密碼不正確");
            }

        }
        else
        {
            return (null, "帳號或密碼不正確");
        }
        return (checkUser, "");
    }
}
