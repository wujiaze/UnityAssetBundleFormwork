/*
 * 
 *      Title:   "AssetBundle简单框架" 项目
 *      
 *      Description:
 *              功能: 打包所有被标记 AB 包名的资源
 *              
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *      
 *      
 */

using System;
using UnityEngine;
using UnityEditor; // 引入 Unity 编辑器，命名空间
using System.IO;

namespace AssetBundleFormWork
{
    public class BuildAssetBundle
    {
        private static string outputPath = PathTools.GetABoutPath();

        

        [MenuItem("AssetBundleTool/DeleteAllAssetBundle")]
        static void DeleteAllAssetBundle()
        {
            if (Directory.Exists(outputPath))
            {
                // 防止多余的文件存在
                // 删除目录 true:表示删除给空文件 false：表示不删除非空文件
                Directory.Delete(outputPath, true);
            }
            AssetDatabase.Refresh();
        }

        static void CreatAssetBundlePath()
        {
            // 删除旧文件
            DeleteAllAssetBundle();
            // 目录类---创建目录/文件夹
            Directory.CreateDirectory(outputPath);
        }
        

        /// <summary>
        /// 打包生成所有的AssetBundle(包)
        /// 功能：所有被添加AsstBundle标记的对象，都会被打包
        /// </summary>
        [MenuItem("AssetBundleTool/BuildforWinDdow")]                                                                                                                                            
        static void BuildAbsforWinDdow()
        {
            // 打包后的存储路径---提前创建StreamingAssets文件夹/或代码创建文件夹
            CreatAssetBundlePath();
            // AssetBundle 打包   
            // outputPath 存放地址
            // BuildAssetBundleOptions.None--默认的打包方式，
            // BuildTarget.StandaloneWindows64：打包后适用的平台
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            // 自动更新Asset文件夹
            AssetDatabase.Refresh();
        }

        [MenuItem("AssetBundleTool/BuildforMacOSX")]
        static void BuildAbsforMac()
        {
            CreatAssetBundlePath();
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXIntel64);
            AssetDatabase.Refresh();
        }
        [MenuItem("AssetBundleTool/BuildforAndroid")]
        static void BuildAbsforAndroid()
        {
            CreatAssetBundlePath();
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.Android);
            AssetDatabase.Refresh();
        }
        [MenuItem("AssetBundleTool/BuildforIOS")]
        static void BuildAbsforIOS()
        {
            CreatAssetBundlePath();
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.iOS);
            AssetDatabase.Refresh();
        }
    }
}

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                