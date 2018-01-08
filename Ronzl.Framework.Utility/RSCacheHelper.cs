using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Ronzl.Framework.Utility
{
    /// <summary>
    /// Cache助手类
    /// </summary>
    public class RSCacheHelper
    {
        /// <summary>
        /// 判断缓存对象是否存在
        /// </summary>
        /// <param name="strKey">缓存键值名称</param>
        /// <returns>是否存在true 、false</returns>
        public static bool IsExist(string cacheKey)
        {
            return HttpContext.Current.Cache[cacheKey] != null;
        }
        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="cacheKey">缓存主键</param>
        public static object GetCache(string cacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[cacheKey];
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T">T对象（泛型）</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <returns></returns>
        public static T GetCache<T>(string cacheKey)
        {
            object obj = GetCache(cacheKey);
            return obj == null ? default(T) : (T)obj;
        }


        /// <summary>
        /// 创建缓存项的文件依赖
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="cacheValue">object对象</param>
        /// <param name="fileName">文件绝对路径</param>
        public static void InsertFile(string cacheKey, object cacheValue, string fileName)
        {
            //创建缓存依赖项
            CacheDependency dep = new CacheDependency(fileName);
            //创建缓存
            HttpContext.Current.Cache.Insert(cacheKey, cacheValue, dep);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="cacheKey">缓存主键</param>
        /// <param name="cacheValue">缓存值</param>
        public static void SetCache(string cacheKey, object cacheValue)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, cacheValue);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="cacheKey">缓存主键</param>
        /// <param name="cacheValue">缓存值</param>
        /// <param name="timeOut">缓存超时时间</param>
        public static void SetCache(string cacheKey, object cacheValue, TimeSpan timeOut)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, cacheValue, null, DateTime.MaxValue, timeOut, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        }
        /// <summary>
        /// 创建缓存项过期，过期时间(分钟)
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="cacheValue">object对象</param>
        /// <param name="iExpires">过期时间(分钟)</param>
        public static void SetCache(string cacheKey, object cacheValue, int iExpires)
        {
            HttpContext.Current.Cache.Insert(cacheKey, cacheValue, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, iExpires, 0));
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="cacheKey">缓存主键</param>
        /// <param name="cacheValue">缓存值</param>
        /// <param name="iExpires">到期时间</param>
        /// <param name="tExpires">移除缓存时间</param>
        public static void SetCache(string cacheKey, object cacheValue, DateTime iExpires, TimeSpan tExpires)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, cacheValue, null, iExpires, tExpires);
        }


        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        /// <param name="cacheKey">缓存主键</param>
        public static void RemoveCache(string cacheKey)
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            _cache.Remove(cacheKey);
        }
        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                _cache.Remove(CacheEnum.Key.ToString());
            }
        }
    }
}
