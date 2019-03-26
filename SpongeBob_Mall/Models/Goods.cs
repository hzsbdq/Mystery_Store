using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpongeBob_Mall.Models
{
    public class Goods
    {
        public int GoodsId { set; get; }//ID
        public int UserID { set; get; }//所有者ID
        public int MapID { set; get; }//图鉴ID
        public int Amount { set; get; }//数量
        public int Price { set; get; }//价格
        public DateTime GetDate { set; get; }//获得日期
        public DateTime ShelvesDate { set; get; }//物品上架日期
        public int CollectionTag { set; get; }//收藏标记
        public int State { set; get; }//物品状态
        public int Location { set; get; }//物品在背包的位置

        public virtual User User { set; get; }
        public virtual Map Map { set; get; }

    }
}