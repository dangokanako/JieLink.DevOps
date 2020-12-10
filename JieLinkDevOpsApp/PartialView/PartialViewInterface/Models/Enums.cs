﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Models
{
    public enum enumProductType
    {
        /// <summary>
        /// 运维工具
        /// </summary>
        DevOps = 0,
        /// <summary>
        /// Jielink中心
        /// </summary>
        JSOCT2016 = 1,
    }

    public enum enumToolType
    {
        /// <summary>
        /// 数据库备份
        /// </summary>
        MySqlBackUp = 0,

        /// <summary>
        /// 一键升级
        /// </summary>
        OneKeyUpdate = 1,

    }
}
