using SmartSql.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SmartSql.Test.Entities
{
    public class WebMenu
    {
        [Column("MENUID", IsPrimaryKey = true)]
        [DisplayName("菜单ID")]
        public string MenuId { get; set; }

        [Column("MENUHREF")]
        [DisplayName("菜单链接")]
        public string MenuHref { get; set; }

        [Column("MENUNAME")]
        [DisplayName("菜单标题")]
        public string MenuName { get; set; }

        [Column("TARGET")]
        [DisplayName("打开方式")]
        public string Target { get; set; }

        [Column("OWNERID")]
        [DisplayName("上级菜单")]
        public string ParentId { get; set; }

        [Column("RIGHT")]
        [DisplayName("权限")]
        public string Right { get; set; } = "N";

        [Column("ORDERBY")]
        [DisplayName("排序")]
        public int? OrderBy { get; set; } = 999;

    }
}
