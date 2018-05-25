/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *           框架主流程：
 *           第一层：AB资源加载类
 *      Description:
 *              功能:
 *                  1、管理和加载指定的AB资源
 *                  2、加载具有缓存功能的资源，带选用参数
 *                  3、卸载、释放 AB 资源
 *                  4、查看当前 AB 资源
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *           
 */

using System;
using System.Collections;
using UnityEngine;
namespace AssetBundleFormWork
{
    // 继承 IDisposable 用来实现 功能3 ,也可以自己写方法
    public class AssetLoader : IDisposable
    {
        // 当前AB 资源
        private AssetBundle _CurrentAB = null;
        // 缓存容器集合
        private Hashtable _Ht = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ab">指定的 AB 资源</param>
        public AssetLoader(AssetBundle ab)
        {
            if (ab != null)
            {
                _CurrentAB = ab;
                _Ht = new Hashtable();
            }
            else
            {
                Debug.LogError(GetType()+ "/构造函数/AssetBundle 参数为空");
            }
        }

        /// <summary>
        /// 加载当前包中指定的资源
        /// </summary>
        /// <param name="assetName">资源的名称</param>
        /// <param name="isCache">是否开启缓存</param>
        public UnityEngine.Object MyLoadAsset(string assetName,bool isCache =false)
        {
            return LoadResources<UnityEngine.Object>(assetName, isCache);

        }

        /// <summary>
        /// 加载当前包中指定的资源(泛型)
        /// </summary>
        /// <param name="assetName">资源的名称</param>
        /// <param name="isCache">是否开启缓存</param>
        public T MyLoadAsset<T>(string assetName, bool isCache = false) where T : UnityEngine.Object
        {
            return LoadResources<T>(assetName, isCache);
        }

        /// <summary>
        /// 加载当前包中指定的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName">资源的名称</param>
        /// <param name="isCache">是否开启缓存</param>
        /// <returns></returns>
        public T LoadResources<T>(string assetName, bool isCache) where T : UnityEngine.Object
        {
            // 是否缓存集合已存在
            if (_Ht.Contains(assetName))
            {
                return  _Ht[assetName] as T;
            }
            // 加载资源
            T tmpResource = _CurrentAB.LoadAsset<T>(assetName);
            // 是否加入缓存集合
            if (isCache && tmpResource!=null)
            {
                _Ht.Add(assetName, tmpResource);
            }
            else if (tmpResource == null) 
            {
                Debug.LogError(GetType()+ "/LoadResources<T> 参数 assetName 资源不存在");
            }
            return tmpResource;
        }

        /// <summary>
        /// 卸载指定资源（内存中卸载）
        /// </summary>
        /// <param name="asset">资源</param>
        /// <returns></returns>
        public bool UnLoadAsset(UnityEngine.Object asset)
        {
            if (asset == null)
            {
                Debug.LogError(GetType()+ "/UnLoadAsset() /参数 asset 为空");
                return false;
            }
            else
            {
                Resources.UnloadAsset(asset);
                return true;
            }
        }

        /// <summary>
        /// 释放当前 AB 包的 内存镜像（基于WWW类下载）
        /// </summary>
        public void Dispose()
        {
           _CurrentAB.Unload(false);
        }
        /// <summary>
        /// 释放当前 AB 包的 内存镜像 （基于WWW类下载）以及 内存对象
        /// </summary>
        public void DisposeAll()
        {
            _CurrentAB.Unload(true);
            _Ht.Clear();
            _Ht = null;
        }

        /// <summary>
        /// 查询当前 AB 包中的所有资源
        /// </summary>
        /// <returns></returns>
        public string[] RetriveAllAssetName()
        {
            return _CurrentAB.GetAllAssetNames();
        }

    }
}
