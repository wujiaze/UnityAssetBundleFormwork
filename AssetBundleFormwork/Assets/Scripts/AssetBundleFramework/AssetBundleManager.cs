/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *              框架主流层（第四层）：所有 “场景”的 AB包管理
 *      Description:
 *              本框架使用前提：1、必须将资源放入Asset下的二级目录及以下
 *                             
 *              功能:
 *                  1、读取 “Manifest”清单文件，缓存本脚本
 *                  2、以'场景'为单位，管理整个项目中所有的AB包
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *           
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AssetBundleFormWork
{
    public class AssetBundleManager:MonoBehaviour
    {
        // 本类的单例
        private static AssetBundleManager _Instance;
        // 场景集合
        private Dictionary<string,MultiABManager> _DicAllScenes =new Dictionary<string, MultiABManager>();
        // AB清单文件 系统类
        private AssetBundleManifest _ManifestObj = null;


        /// <summary>
        /// 得到本类实例
        /// </summary>
        /// <returns></returns>
        public static AssetBundleManager GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("_AssetBundleMgr").AddComponent<AssetBundleManager>();
            }
            return _Instance;
        }

        void Awake()
        {
            StartCoroutine(ABManifestLoader.GetInstance().LoadManifestFile());
        }

        /// <summary>
        /// 下载指定场景，指定包
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="abName">assetBundle 名称</param>
        /// <param name="loadAllCompleteHandle">加载完成之后，执行的方法</param>
        /// <returns></returns>
        public IEnumerator LoadAssetBundlePack(string sceneName, string abName, DelLoadComplete loadAllCompleteHandle) 
        {
           
            // 参数检查
            if (string.IsNullOrEmpty(sceneName) ||string.IsNullOrEmpty(abName) || loadAllCompleteHandle==null)
            {
                Debug.LogError(GetType()+ "/LoadAssetBundlePack()/sceneName or abName or loadAllCompleteHandle =null");
                yield  break;
            }
            string _CurrentSceneName = sceneName.ToLower();
            string _CurrentAB =String.Empty;
            if (abName.Contains("/"))
            {
                _CurrentAB = abName;
            }
            else
            {
                _CurrentAB = _CurrentSceneName + "/" + abName.ToLower();
            }
            // 等待Manifest清单文件加载完成
            while (!ABManifestLoader.GetInstance().IsLoadFinish)
            {
                yield return null;
            }
            _ManifestObj = ABManifestLoader.GetInstance().GetABManifest();
            if (_ManifestObj ==null)
            {
                Debug.LogError(GetType() + "/LoadAssetBundlePack()/_ManifestObj =null");
                yield break; 
            }
            // 把当前场景加入集合
            if (!_DicAllScenes.ContainsKey(_CurrentSceneName))
            {
                MultiABManager multiMgrObj =new MultiABManager(_CurrentSceneName, _CurrentAB, loadAllCompleteHandle);
                _DicAllScenes.Add(_CurrentSceneName, multiMgrObj);
            }
            // 调用下一层（MultiABManager）
            MultiABManager tmpMultiAbManager = _DicAllScenes[_CurrentSceneName];
            if (tmpMultiAbManager == null)
            {
                Debug.LogError(GetType() + "/LoadAssetBundlePack()/tmpMultiAbManager=null");
                yield break;
            }
            else
            {
                // 调用“多包管理类”来加载指定AB包
                yield return tmpMultiAbManager.LoadAssetBundle(_CurrentAB); 
            }
        }


        /// <summary>
        /// 加载所需资源
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="abName">assetBundle 名称</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="isCache">是否缓存</param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string sceneName, string abName,string assetName,bool isCache)
        {
            string tmpsceneName = sceneName.ToLower();
            string tmpassetName = assetName.ToLower();
            string tmpabName =String.Empty;
            if (abName.Contains("/"))
            {
                tmpabName = abName.ToLower();
            }
            else
            {
                tmpabName = tmpsceneName+"/"+ abName.ToLower();
            }
            
            if (_DicAllScenes.ContainsKey(tmpsceneName))
            {
                MultiABManager multiObj = _DicAllScenes[tmpsceneName];
                return multiObj.LoadAsset(tmpabName, tmpassetName, isCache);
            }
            Debug.LogError(GetType()+ "/LoadAsset() sceneName 可能为空，或者未使用 LoadAssetBundlePack");
            return null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public void  DisposeAllAssets(string sceneName)
        {
            if (_DicAllScenes.ContainsKey(sceneName))
            {
                MultiABManager multiObj = _DicAllScenes[sceneName];
                multiObj.DisposeAllAsset();
            }
            else
            {
                Debug.LogError(GetType() + "/DisposeAllAssets() sceneName 可能为空");
            }
        }

    }
}
