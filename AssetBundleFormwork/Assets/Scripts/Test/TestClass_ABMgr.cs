/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *              框架内部验证测试
 *      Description:
 *              功能:
 *                  框架整体验证测试 
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *           
 */
using UnityEngine;
namespace AssetBundleFormWork
{
    public class TestClass_ABMgr:MonoBehaviour
    {
        // 场景名称
        private string _SceneName = "scene_1";
        // AB包名
        private string _AsserBundleName = "prefabs_1.ab"; // "scene_1/prefabs_1.ab"
        // 资源名称
        private string _AssetName = "sphere.prefab";

        private void Awake()
        {
            // 加载 AB 包 
            StartCoroutine(AssetBundleManager.GetInstance().LoadAssetBundlePack(_SceneName, _AsserBundleName, LoadAllComplete));
        }
        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="abName"></param>
        private void LoadAllComplete(string abName)
        {
            Object tmpObj = null;
            tmpObj = AssetBundleManager.GetInstance().LoadAsset(_SceneName, abName, _AssetName, false) as Object;
            if (tmpObj!=null)
            {
                Instantiate(tmpObj);
            }
        }
        // 测试销毁资源
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                AssetBundleManager.GetInstance().DisposeAllAssets(_SceneName);
            }
        }

    }
}
