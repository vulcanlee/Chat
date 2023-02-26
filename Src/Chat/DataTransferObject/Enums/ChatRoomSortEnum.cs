using DataTransferObject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObject.Enums
{
    public enum ChatRoomSortEnum
    {
        NameAscending,
        NameDescending,
        UpdateAtAscending,
        UpdateAtDescending,
    }
    public class ChatRoomSort
    {
        public static void Initialization(List<SortCondition> SortConditions)
        {
            SortConditions.Clear();
            SortConditions.Add(new SortCondition()
            {
                Id = (int)ChatRoomSortEnum.NameAscending,
                Title = "名稱 遞增"
            });
            SortConditions.Add(new SortCondition()
            {
                Id = (int)ChatRoomSortEnum.NameDescending,
                Title = "名稱 遞減"
            });
            SortConditions.Add(new SortCondition()
            {
                Id = (int)ChatRoomSortEnum.UpdateAtAscending,
                Title = "更新時間 遞增"
            });
            SortConditions.Add(new SortCondition()
            {
                Id = (int)ChatRoomSortEnum.UpdateAtDescending,
                Title = "更新時間 遞減"
            });
        }
    }
}
