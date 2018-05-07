/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *              工具类
 *              
 *      Description:
 *              功能:
 *                  1、所有的常量定义
 *                  2、所有的委托定义
 *                  3、所有的枚举定义
 *                  
 *              
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *           
 */
using UnityEngine;
namespace AssetBundleFormWork
{
    /* 委托定义区 */
    public delegate void DelLoadComplete(string abName);
    /* 枚举定义区 */
    public class AbDefine
    {
        /* 框架常量 */
        public const string SYS_ASSETBUNDLE_MANIFEST = "AssetBundleManifest";
        /* 系统方法 */
    }
}
