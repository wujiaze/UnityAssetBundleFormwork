/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *              工具辅助类：
 *                  AssetBundle 关系类
 *      Description:
 *              功能:
 *                  1、存储指定AB包的所有依赖关系
 *                  2、存储指定AB包的所有引用关系
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *           
 */

using System.Collections.Generic;
namespace AssetBundleFormWork
{
#pragma warning disable 414
    public class ABRelation
    {
        // 当前 AB 名称
        private string _ABname;
        // 当前AB包的所有依赖包集合
        private List<string> _ListAllDependenceAB;

        // 当前AB包的所有引用包集合
        private List<string> _ListAllRefercenAB;

        public ABRelation(string abName)
        {
            if (!string.IsNullOrEmpty(abName))
            {
                 _ABname = abName;
                 _ListAllDependenceAB =new List<string>();
                 _ListAllRefercenAB = new List<string>();
            }
        }
        /* 依赖关系 */
        /// <summary>
        /// 增加依赖关系
        /// </summary>
        /// <param name="abName">需要添加的依赖包</param>
        public void AddDenpendce(string abName)
        {
            if (!_ListAllDependenceAB.Contains(abName))
            {
                _ListAllDependenceAB.Add(abName);
            }
        }

        /// <summary>
        /// 移除依赖关系
        /// </summary>
        /// <param name="abName">需要移除的包名</param>
        /// <returns>
        /// true：当前 AB 包中没有依赖项
        /// false：当期 AB 包中还有依赖项
        /// </returns>
        public bool RemoveDenpendce(string abName)
        {
            if (_ListAllDependenceAB.Contains(abName))
            {
                _ListAllDependenceAB.Remove(abName);
            }
            if (_ListAllDependenceAB.Count>0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取所有依赖关系
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllDependence()
        {
            return _ListAllDependenceAB;
        }

        /* 引用关系 */
        /// <summary>
        /// 增加引用关系
        /// </summary>
        /// <param name="abName">需要添加的引用包</param>
        public void AddReference(string abName)
        {
            if (!_ListAllRefercenAB.Contains(abName))
            {
                _ListAllRefercenAB.Add(abName);
            }
        }

        /// <summary>
        /// 移除引用关系
        /// </summary>
        /// <param name="abName">需要移除的包名</param>
        /// <returns>
        /// true：当前 AB 包中没有引用项
        /// false：当期 AB 包中还有引用项
        /// </returns>
        public bool RemoveReference(string abName)
        {
            if (_ListAllRefercenAB.Contains(abName))
            {
                _ListAllRefercenAB.Remove(abName);
            }
            if (_ListAllRefercenAB.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取所有引用关系
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllReference()
        {
            return _ListAllRefercenAB;
        }
    }
}
