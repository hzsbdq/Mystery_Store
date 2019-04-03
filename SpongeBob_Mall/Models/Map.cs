using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpongeBob_Mall.Models
{
    public class Map
    {
        public int MapId { set; get; }//ID（类别信息包含在内）
        public String Name { set; get; }//名称
        public String Rare { set; get; }//物品稀有度
        public String type { set; get; }//物品类型
        public int Place { set; get; }//占用空间
        public int Validity { set; get; }//有效时间（小时）
        public String Picture { set; get; }//照片名称
    }
}