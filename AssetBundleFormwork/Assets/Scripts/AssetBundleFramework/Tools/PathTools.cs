/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *          路径工具类
 *      Description:
 *              功能:包含本框架中所有的路径常量、路径方法
 *              
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *           
 */

using System;
using UnityEngine;
namespace AssetBundleFormWork
{
    public class PathTools
    {
        /* 路径常量 */
        // 意味着所有的资源文件都要放在这个目录下，当然可以修改成自己的文件名
        public const string AB_RESOURCES = "Res";

        /* 路径方法 */
        /// <summary>
        /// 得到 AB 资源的输入目录
        /// </summary>
        /// <returns></returns>
        public static string GetABResourcesPath()
        {
            return Application.dataPath + "/" + AB_RESOURCES;
        }

        /// <summary>
        /// 获取AB打包后的输出路径
        /// 1、平台（PC/移动端）路径
        /// 2、平台的名称
        /// </summary>
        /// <returns></returns>
        public static string GetABoutPath()
        {
            return GetPlatformPath() + "/" + GetPlatformName();
        }

        /// <summary>
        /// 平台（PC/移动端）路径
        /// </summary>
        /// <returns></returns>
        private static string GetPlatformPath()
        {
            string strReturnPlayformPath = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    strReturnPlayformPath = Application.streamingAssetsPath;
                    break;
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Android:
                    strReturnPlayformPath = Application.persistentDataPath;
                    break;
            }
            return strReturnPlayformPath;
        }

        /// <summary>
        /// 平台的名称，便于管理
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformName()
        {
            string strReturnPlayformName = string.Empty;
            switch (Application.platform)
            {

                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    strReturnPlayformName = "Window";
                    break;
                case RuntimePlatform.OSXEditor:
                    strReturnPlayformName = "MacOS";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    strReturnPlayformName = "IPhone";
                    break;
                case RuntimePlatform.Android:
                    strReturnPlayformName = "Android";
                    break;
            }
            return strReturnPlayformName;
        }

        /// <summary>
        /// 获取基于WWW协议的 AB 下载路径
        /// 本地下载
        /// </summary>
        /// <returns></returns>
        public static string GetABwwwPath()
        {
           string url =String.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    url = "file://" + GetABoutPath();
                    break;
                case RuntimePlatform.Android:
                    url = "jar:file://" + GetABoutPath();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    url = GetABoutPath()+"/Raw/";
                    break;
                default:
                    break;
            }
            return url;
        }

    }
}
