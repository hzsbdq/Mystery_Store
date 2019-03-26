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
        public int Rare { set; get; }//物品稀有度
        public int Place { set; get; }//占用空间
        public int Validity { set; get; }//有效时间（小时）
        public String Picture { set; get; }//照片名称
    }
}