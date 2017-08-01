using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LNB.CollisionManager
{
    public class CollisionManager {

        #region static
        private static CollisionManager instance;
        internal static CollisionManager Instance
        {
            get{
                if(instance == null)
                {
                    instance = new CollisionManager();
                }

                return instance;
            }
        }

        /// <summary>
        /// 为某组增加一个GameObject,包含其下所有Collider
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="go">Go.</param>
        public static void AddGroupGameObject(string groupId, GameObject go)
        {
            Instance.LAddGroupGameObject(groupId, go);
        }

        /// <summary>
        /// 为某组移除一个GameObject,包含其下所有Collider
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="go">Go.</param>
        public static void RemoveGroupGameObject(string groupId, GameObject go)
        {
            Instance.LRemoveGroupGameObject(groupId, go);
        }

        /// <summary>
        /// 为某组增加一个碰撞
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="col">Col.</param>
        public static void AddGroupCollider(string groupId, Collider col)
        {
            Instance.LAddGroupCollider(groupId, col);
        }

        /// <summary>
        /// 为某组减少一个碰撞。
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="col">Col.</param>
        public static void RemoveGroupCollider(string groupId, Collider col)
        {
            Instance.LRemoveGroupCollider(groupId, col);
        }

        /// <summary>
        /// 注册一个组
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        public static void RegisterGroup(string groupId)
        {
            Instance.LRegisterGroup(groupId);   
        }

        /// <summary>
        /// 取消注册一个组
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        public static void UnregisterGroup(string groupId)
        {
            Instance.LUnregisterGroup(groupId);
        }


        #endregion

        /// <summary>
        /// 缓存所有组的关系。
        /// </summary>
        private Dictionary<string, List<Collider>> colliderDic;


        public CollisionManager()
        {
            colliderDic = new Dictionary<string, List<Collider>>();
        }


        /// <summary>
        /// 注释一个组
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        internal void LRegisterGroup(string groupId)
        {
            if(colliderDic.ContainsKey(groupId))
            {
                Debug.Log(groupId +" has been register");
                return;
            }

            colliderDic.Add(groupId, new List<Collider>());
        }

        /// <summary>
        /// 卸载一个组
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        internal void LUnregisterGroup(string groupId)
        {
            if(!colliderDic.ContainsKey(groupId))
            {
                return;
            }

            List<Collider> tempCols = colliderDic[groupId];

            int count = tempCols.Count;

            for (int i = 0; i < count; i++)
            {
                RemoveCollision(groupId, tempCols[i]);
            }

            colliderDic.Remove(groupId);
        }

        /// <summary>
        /// 增加一个GameObject,包含其下所有Collider
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="go">Go.</param>
        internal void LAddGroupGameObject(string groupId, GameObject go)
        {

            if(!HadGroup(groupId))
            {
                LRegisterGroup(groupId);
            }

            Collider[] cols = go.GetComponentsInChildren<Collider>(true);

            int count = cols.Length;

            for (int i = 0; i < count; i++)
            {

                LAddGroupCollider(groupId, cols[count]);

            }
        }


        /// <summary>
        /// 增加一个碰撞。体
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="col">Col.</param>
        internal void LAddGroupCollider(string groupId, Collider col)
        {
            if(!HadGroup(groupId))
            {
                LRegisterGroup(groupId);
            }

            if(AddCollider(groupId, col))
            {
                SetCollision(groupId, col);
            }

        }

        /// <summary>
        /// 移除一个GameObject,包含其下所有Collider
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="go">Go.</param>
        internal void LRemoveGroupGameObject(string groupId, GameObject go)
        {

            if(!HadGroup(groupId))
            {
                LRegisterGroup(groupId);
            }

            Collider[] cols = go.GetComponentsInChildren<Collider>(true);

            int count = cols.Length;

            for (int i = 0; i < count; i++)
            {

                LRemoveGroupCollider(groupId, cols[count]);

            }
        }


        /// <summary>
        /// 移除一个碰撞体。
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="col">Col.</param>
        internal void LRemoveGroupCollider(string groupId, Collider col)
        {
            if(!HadGroup(groupId))
            {
                return;
            }

            if(RemoveCollider(groupId, col))
            {
                RemoveCollision(groupId, col);
            }

        }

        /// <summary>
        /// 增加一个碰撞
        /// </summary>
        /// <returns><c>true</c>, if collider was added, <c>false</c> otherwise.</returns>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="col">Col.</param>
        private bool AddCollider(string groupId, Collider col)
        {
            List<Collider> groupCols = GetColliders(groupId);

            if(groupCols.Contains(col))
            {
                return false;
            }

            groupCols.Add(col);

            return true;

        }

        /// <summary>
        /// 移除一个碰撞
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="col">Col.</param>
        private bool RemoveCollider(string groupId, Collider col)
        {
            List<Collider> groupCols = GetColliders(groupId);


            if(groupCols.Contains(col))
            {
                groupCols.Remove(col);

                if(groupCols.Count == 0)
                {
                    LUnregisterGroup(groupId);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置碰撞关系
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="col">Col.</param>
        private void SetCollision(string groupId, Collider col)
        {

            if(col == null)
            {
                return;
            }

            foreach (var item in colliderDic)
            {
                if(item.Key == groupId)
                {
                    continue;
                }

                if(item.Value == null)
                {
                    continue;
                }

                int count = item.Value.Count;

                for (int i = 0; i < count; i++)
                {

                    if(item.Value[i] == null)
                    {
                        continue;
                    }

                    Physics.IgnoreCollision(col, item.Value[i]);
                }
            }
        }

        /// <summary>
        /// 移除碰撞关系
        /// </summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="col">Col.</param>
        private void RemoveCollision(string groupId, Collider col)
        {

            if(col == null)
            {
                return;
            }

            foreach (var item in colliderDic)
            {
                if(item.Key == groupId)
                {
                    continue;
                }

                if(item.Value == null)
                {
                    continue;
                }

                int count = item.Value.Count;

                for (int i = 0; i < count; i++)
                {
                    if(item.Value[i] == null)
                    {
                        continue;
                    }

                    Physics.IgnoreCollision(col, item.Value[i], false);
                }
            }
        }

        /// <summary>
        /// 是否已经有该组。
        /// </summary>
        /// <returns><c>true</c>, if group was haded, <c>false</c> otherwise.</returns>
        /// <param name="groupId">Group identifier.</param>
        private bool HadGroup(string groupId)
        {
            return colliderDic.ContainsKey(groupId);
        }

        /// <summary>
        /// 获得该组的所有碰撞
        /// </summary>
        /// <returns>The colliders.</returns>
        /// <param name="groupId">Group identifier.</param>
        private List<Collider> GetColliders(string groupId)
        {

            if(!HadGroup(groupId))
            {
                return new List<Collider>();
            }

            return colliderDic[groupId];
        }

    }
}