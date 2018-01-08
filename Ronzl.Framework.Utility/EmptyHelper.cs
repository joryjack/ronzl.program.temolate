using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronzl.Framework.Utility
{
    public static class EmptyHelper
    {
        #region 获取Empty属性name&value
        /// <summary>
        /// 获取Empty属性name&value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="empty"></param>
        /// <returns></returns>
        public static Dictionary<string, object> getProperties<T>(T empty)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (empty == null)
            {
                return dic;
            }
            System.Reflection.PropertyInfo[] properties = empty.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return dic;
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(empty, null);
                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    dic.Add(name, value);
                }
                //else
                //{
                //    getProperties(value);
                //}
            }
            return dic;
        }
        #endregion

        #region 递归查询
        /// <summary>
        /// 递归查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allItems">全部对象集合</param>
        /// <param name="currentItem">父级对象</param>
        /// <param name="tempItems">结果集合</param>
        /// <param name="parentIdName">父级ID名称</param>
        /// <param name="idName">自己ID名称</param>
        public static void LoopToAppendChildren<T>(List<T> allItems, T currentItem, List<T> tempItems, string parentIdName = "ParentId", string idName = "Id")
        {
            var value = currentItem.GetType().GetProperty(idName).GetValue(currentItem, null).ToString();


            var subItems = allItems.Where(a => a.GetType().GetProperty(parentIdName).GetValue(a, null).ToString() == currentItem.GetType().GetProperty(idName).GetValue(currentItem, null).ToString()).ToList();

            tempItems.AddRange(subItems);
            if (subItems.Count == 0) return;

            foreach (var subItem in subItems)
            {
                LoopToAppendChildren(allItems, subItem, tempItems, parentIdName, idName);
            }
        }
        #endregion
    }
}
