/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *      
 *      Description:
 *              功能: 自动给资源文件添加标记
 *              开发思路：
 *              1、定义需要打包资源的文件根目录 ： 这里是 AB_Res
 *              2、遍历每个 “场景” 文件夹（目录）
 *                  A: 遍历本场景目录下所有的目录或文件
 *                     如果是目录，则继续 “递归” 访问里面的文件，直到定位到文件
 *                  B：找到文件，则使用 AssetImport 类，标记 “包名” 与 “后缀名”
 *                     包名(无论是哪个子目录中的文件)：统一  “场景名/功能目录名”
 *                     后缀名 ：一般用于更新升级
 *              
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *           
 */


using UnityEngine;
using UnityEditor;  // 编辑器命名空间
using System.IO;    // 文件与目录的操作空间 
namespace AssetBundleFormWork
{
    public class AutoSetLabel
    {
        /// <summary>
        /// 设置 AB 包名
        /// 注意点：本方法使用的前提是所有需要打包的资源必须在“场景”目录中
        /// </summary>
        [MenuItem("AssetBundleTool/Creat AB Label")]
        static void CreatABLabel()
        {
            SetABLabel(true);
            AssetDatabase.Refresh();
        }
        [MenuItem("AssetBundleTool/Clear All AB Label")]
        static void ClearAllABLabel()
        {
            SetABLabel(false);
            AssetDatabase.Refresh();
        }
        [MenuItem("AssetBundleTool/Remove Unused AB Label")]
        static void RemoveUnusedAssetBundleNames()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }
        private static void SetABLabel(bool isCreat)
        {
            /* 方法局部变量 */
            // 需要给AB做标记的根目录
            string strNeedSetLabelRoot = string.Empty;
            // 目录信息（场景目录信息数组，表示根目录下所有的场景目录）
            DirectoryInfo[] dirSceneDirArray = null; 

            // 清空没有用到的 AB 标记
            AssetDatabase.RemoveUnusedAssetBundleNames();

            // 1、定义需要打包资源的文件夹根目录
            strNeedSetLabelRoot = PathTools.GetABResourcesPath(); 
            // 2、遍历每个 “场景” 文件夹（目录）
            // 获取目录信息
            dirSceneDirArray = new DirectoryInfo(strNeedSetLabelRoot).GetDirectories();
            foreach (DirectoryInfo currentDirInfo in dirSceneDirArray)
            {
                // 2.1、遍历本场景目录下所有的目录或文件
                //      如果是目录，则继续 “递归” 访问里面的文件，直到定位到文件
                // 2.2、找到文件，则使用 AssetImport 类，标记 “包名” 与 “后缀名”
                JudgeDirOrFileByRecursive(currentDirInfo, isCreat);
            }
            // 刷新
            AssetDatabase.Refresh();
            // 提示信息，标记包名完成
            Debug.Log("AssetBundle 本次操作设置标记完成！");
        }

        /// <summary>
        /// 递归判断是否为目录或文件，修改 AssetBundle 的标记
        /// </summary>
        /// <param name="currenInfo">目录信息</param>
        private static void JudgeDirOrFileByRecursive(DirectoryInfo currenInfo, bool isCreat)
        {
            if (!currenInfo.Exists)
            {
                Debug.LogError("文件或目录名称 ：" + currenInfo + "不存在");
                return;
            }
            else
            {
                FileSystemInfo[] fileSysArray = currenInfo.GetFileSystemInfos();
                foreach (FileSystemInfo info in fileSysArray)
                {
                    // FileSystemInfo 文件系统信息 可以转换为  FileInfo（文件信息） 或  DirectoryInfo（目录信息）
                    FileInfo fileinfo = info as FileInfo;
                    if (fileinfo == null)
                    {
                        // 说明是目标类型，则递归调用
                        JudgeDirOrFileByRecursive((DirectoryInfo)info, isCreat);
                    }
                    else
                    {
                        // 文件类型，则修改此文件的 AB 标签
                        SetFileABLabel(fileinfo, isCreat);
                    }
                }
            }
        }

        /// <summary>
        /// 对指定的文件设置 AB 包名
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        private static void SetFileABLabel(FileInfo fileInfo,bool isCreat)
        {
            // 参数检查
            if (fileInfo.Extension == ".meta") return;
            /* 局部变量 */
           // 文件路径（需要相对路径）
            string strAssetFilePath = string.Empty;
            
            // 获取文件路径
            int tmpIndx = fileInfo.FullName.IndexOf("Assets");
            strAssetFilePath = fileInfo.FullName.Substring(tmpIndx);
            // 给资源文件设置 AB 名称以及后缀名
            AssetImporter tmpImportObj = AssetImporter.GetAtPath(strAssetFilePath);
            // AB 包名
            string strABname = string.Empty;
            if (isCreat)
            {
                // 获取包名
                strABname = GetABname(fileInfo);
                tmpImportObj.assetBundleName = strABname;
                // 后缀名
                if (fileInfo.Extension == ".unity")
                {
                    tmpImportObj.assetBundleVariant = "scene";
                }
                else
                {
                    tmpImportObj.assetBundleVariant = "ab";
                }
            }
            tmpImportObj.assetBundleName = strABname;
        }

        /// <summary>
        /// 生成AB包名
        /// AB 包形成规则：
        ///     文件AB包名称 = “所在二级目录名称”（场景名称）+“三级目录名称”（功能文件夹名称）
        ///     若AB包名只在二级目录下，则 文件AB包名称 = “所在二级目录名称”（场景名称）+“所在二级目录名称”（场景名称）;
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private static string GetABname(FileInfo fileInfo)
        {
            string strABname = string.Empty;
            int tmpIndx = fileInfo.FullName.IndexOf("Assets");
            string strAssetFilePath = fileInfo.FullName.Substring(tmpIndx);
            string[] tmpstr = strAssetFilePath.Split('\\'); // win路径是‘\’分割的

            if (tmpstr.Length == 4) 
            {
                strABname = tmpstr[2] + "/" + tmpstr[2]; // unity路径是通过‘/’ 分割的
            }
            else if(tmpstr.Length >=5)
            {
                strABname = tmpstr[2] + "/" + tmpstr[3];
            }
            return strABname;
        }

       
       
    }
}
