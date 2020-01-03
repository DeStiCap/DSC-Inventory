using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSC.InventorySystem
{
    public class BaseInventoryManager<InventoryData> : MonoBehaviour where InventoryData : struct, BaseInventoryData
    {
        #region Variable

        #region Variable - Property

        protected static BaseInventoryManager<InventoryData> instance
        {
            get
            {
                if (m_hInstance == null && m_bAppStart && !m_bAppQuit)
                    Debug.LogWarning("Don't have InventoryManager in scene.");

                return m_hInstance;
            }
        }

        #endregion

        protected static BaseInventoryManager<InventoryData> m_hInstance;
        protected static bool m_bAppStart;
        protected static bool m_bAppQuit;

        protected Dictionary<Transform, InventoryData> m_dicInventory = new Dictionary<Transform, InventoryData>();

        #endregion

        #region Base - Mono

        protected virtual void Awake()
        {
            if (m_hInstance == null)
            {
                m_hInstance = this;
            }
            else if (m_hInstance != this)
            {
                Destroy(this);
                return;
            }

            Application.quitting += OnAppQuit;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected static void OnAppStart()
        {
            m_bAppStart = true;
            m_bAppQuit = false;
        }

        protected void OnAppQuit()
        {
            Application.quitting -= OnAppQuit;

            m_bAppStart = false;
            m_bAppQuit = true;
        }

        #endregion

        #region Main

        public static void RegisterInventory(Transform hTransform, InventoryData hInventory)
        {
            if (instance == null)
                return;

            m_hInstance.MainRegisterInventory(hTransform, hInventory);
        }

        protected virtual void MainRegisterInventory(Transform hTransform, InventoryData hInventory)
        {
            if (HasInventoryKey(hTransform))
                return;

            m_dicInventory.Add(hTransform, hInventory);
        }

        public static void UnregisterInventory(Transform hTransform)
        {
            instance?.MainUnregisterInventory(hTransform);
        }

        protected virtual void MainUnregisterInventory(Transform hTransform)
        {
            if (!HasInventoryKey(hTransform))
                return;

            m_dicInventory.Remove(hTransform);
        }

        public static InventoryData GetInventory(Transform hTransform)
        {
            if (instance == null)
                return default;

            return m_hInstance.MainGetInventory(hTransform);
        }

        protected virtual InventoryData MainGetInventory(Transform hTransform)
        {
            if (!HasInventoryKey(hTransform))
                return default;

            return m_dicInventory[hTransform];
        }

        public static bool TryGetInventory(Transform hTransform, out InventoryData hOutInventory)
        {
            if (instance == null)
            {
                hOutInventory = default;
                return false;
            }

            return m_hInstance.MainTryGetInventory(hTransform, out hOutInventory);
        }

        protected virtual bool MainTryGetInventory(Transform hTransform, out InventoryData hOutInventory)
        {
            if (!HasInventoryKey(hTransform))
            {
                hOutInventory = default;
                return false;
            }

            hOutInventory = GetInventory(hTransform);
            return true;
        }

        public static void SetInventory(Transform hTransform, InventoryData hInventory)
        {
            instance?.MainSetInventory(hTransform, hInventory);
        }

        protected virtual void MainSetInventory(Transform hTransform, InventoryData hInventory)
        {
            if (!HasInventoryKey(hTransform))
                return;

            m_dicInventory[hTransform] = hInventory;
        }

        #endregion

        #region Helper

        protected bool HasInventoryKey(Transform hTransform)
        {
            return m_dicInventory.ContainsKey(hTransform);
        }

        #endregion
    }
}