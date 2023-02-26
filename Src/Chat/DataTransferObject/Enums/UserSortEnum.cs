using DataTransferObject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObject.Enums
{
    public enum UserSortEnum
    {
        NameAscending,
        NameDescending,
        UpdateAtAscending,
        UpdateAtDescending,
    }
    public class UserSort
    {
        public static void Initialization(List<SortCondition> SortConditions)
        {
            SortConditions.Clear();
            SortConditions.Add(new SortCondition()
            {
                Id = (int)UserSortEnum.NameAscending,
                Title = "名稱 遞增"
            });
            SortConditions.Add(new SortCondition()
            {
                Id = (int)UserSortEnum.NameDescending,
                Title = "名稱 遞減"
            });
            SortConditions.Add(new SortCondition()
            {
                Id = (int)UserSortEnum.UpdateAtAscending,
                Title = "更新時間 遞增"
            });
            SortConditions.Add(new SortCondition()
            {
                Id = (int)UserSortEnum.UpdateAtDescending,
                Title = "更新時間 遞減"
            });
        }
    }
}
