using SpongeBob_Mall.Controllers.XieBaoWang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpongeBob_Mall.GameExecute
{
    //随机矿物种类，随机数量
    public class Mining
    {
        //初级挖矿

        //中级挖矿

        //高级挖矿
        
        public static async System.Threading.Tasks.Task<BagController> MiningAsync()
        {
            BagController bag = new BagController();
            await bag.GetGoods(1);
            return null;
        }
    }
}