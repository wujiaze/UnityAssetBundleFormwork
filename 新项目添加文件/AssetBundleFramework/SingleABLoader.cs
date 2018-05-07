/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *              框架主流程：
 *              第2层：WWW 加载 AB 
 *      Description:
 *              功能:
 *              
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
    public class SingleABLoader : System.IDisposable
    {
        // 引用类：资源加载类
        private AssetLoader _assetLoader;
        // 委托
        private DelLoadComplete _LoadCompleteHandle;
        // 需要下载的 AB 名称
        private string _ABname;
        // AB 的下载路径 URL
        private string _ABurl;


        public SingleABLoader(string abName,DelLoadComplete loadComplete)
        {
            _assetLoader = null;
            _ABname = abName;
            // 委托定义
            _LoadCompleteHandle = loadComplete;
            // 下载路径
            _ABurl =PathTools.GetABwwwPath()+ "/" + _ABname;
        }

        /// <summary>
        /// 下载 AB 资源包
        /// </summary>
        /// <returns></returns>
         public IEnumerator LoadAssetBundle()
        {
            using (WWW www =new WWW(_ABurl))
            {
                yield return www;
                // 下载完成
                if (www.progress >= 1)
                {
                    // 获取 AB 实例
                    AssetBundle ab = www.assetBundle;
                    if (ab == null)
                    {
                        Debug.LogError(GetType()+ "/LoadAssetBundle() / www 下载出错 URL: "+ _ABurl + "错误信息 "+www.error);
                    }
                    else
                    {
                        // 实例化引用类
                        _assetLoader = new AssetLoader(ab);
                        // AB 下载完毕 ，调用委托
                        if (_LoadCompleteHandle!=null)
                        {
                            _LoadCompleteHandle(_ABname);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 加载 AB 包中的资源
        /// </summary>
        /// <param name="assetName">指定的资源名称</param>
        /// <param name="isCache">是否缓存</param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string assetName,bool isCache)
        {
            if (_assetLoader != null)
            {
                return _assetLoader.LoadAsset(assetName, isCache);
            }
            Debug.LogError(GetType() + "/LoadAsset() / _assetLoader 为空");
            return null;
        }

        /// <summary>
        /// 卸载指定资源
        /// </summary>
        /// <param name="asset">指定的资源</param>
        /// <returns></returns>
        public bool UnLoadAsset(UnityEngine.Object asset)
        {
            if (_assetLoader == null)
            {
                Debug.LogError(GetType() + "/UnLoadAsset() /_assetLoader 为空");
                return false;
            }
            else
            { 
                return _assetLoader.UnLoadAsset(asset); 
            }
        }

        /// <summary>
        /// 释放当前 AB 包的 内存镜像（基于WWW类下载）
        /// </summary>
        public void Dispose()
        {
            if (_assetLoader == null)
            {
                Debug.LogError(GetType() + "/Dispose() /_assetLoader 为null");
            }
            else
            {
                _assetLoader.Dispose();
                _assetLoader = null;
            }
        }

        /// <summary>
        /// 释放当前 AB 包的 内存镜像 （基于WWW类下载）以及 内存对象
        /// </summary>
        public void DisposeAll()
        {
            if (_assetLoader == null)
            {
                Debug.LogError(GetType() + "/DisposeAll() /_assetLoader 为null");
            }
            else
            {
                _assetLoader.DisposeAll();
                _assetLoader = null;
            }
        }

        /// <summary>
        /// 查询当前 AB 包中所有资源
        /// </summary>
        /// <returns></returns>
        public string[] RetriveAllAssetName()
        {
            if (_assetLoader == null)
            {
                Debug.LogError(GetType() + "/RetriveAllAssetName() /_assetLoader 为null");
                return null;
            }
            else
            {
                return _assetLoader.RetriveAllAssetName();
            }
        }





    }
}
