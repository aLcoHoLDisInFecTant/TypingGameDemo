using System.Collections.Generic;
using UnityEngine;
using TypeRogue.Data; // For BuffData
using TypeRogue;      // For BuffInstance

namespace TypeRogue.UI
{
    public class BuffStatusPanelView : MonoBehaviour
    {
        [SerializeField] private Transform buffContainer;
        [SerializeField] private BuffStatusItemView buffItemPrefab;

        private List<BuffStatusItemView> activeItems = new List<BuffStatusItemView>();

        public void Initialize(IEnumerable<BuffData> buffs)
        {
            // 清理旧的
            if (buffContainer != null)
            {
                foreach (Transform child in buffContainer)
                {
                    Destroy(child.gameObject);
                }
            }
            activeItems.Clear();

            if (buffContainer == null || buffItemPrefab == null || buffs == null) return;

            // 创建新的
            foreach (var buffData in buffs)
            {
                var item = Instantiate(buffItemPrefab, buffContainer);
                // 显示 TriggerWord 而不是 BuffName，或者两者都显示？
                // 用户输入的是 TriggerWord，所以显示 TriggerWord 可能更直观
                // 但通常 UI 显示 Buff Name (e.g. "Precise Shot")
                // 这里暂时用 BuffName
                item.Initialize(buffData.BuffName);
                
                // 确保缩放正确
                item.transform.localScale = Vector3.one;
                
                activeItems.Add(item);
            }
        }

        public void UpdateBuffState(List<BuffInstance> buffInstances)
        {
            // Sync UI items with data instances
            // If we have more data than UI items, create new UI items
            while (activeItems.Count < buffInstances.Count)
            {
                var newBuffIndex = activeItems.Count;
                var newBuffData = buffInstances[newBuffIndex].Data;
                
                if (buffItemPrefab != null && buffContainer != null)
                {
                    var item = Instantiate(buffItemPrefab, buffContainer);
                    item.Initialize(newBuffData.BuffName);
                    item.transform.localScale = Vector3.one;
                    activeItems.Add(item);
                }
                else
                {
                    break; // Should not happen if initialized correctly
                }
            }

            // Update state for all items
            for (int i = 0; i < activeItems.Count; i++)
            {
                if (i < buffInstances.Count)
                {
                    var instance = buffInstances[i];
                    if (instance != null && instance.Data != null)
                    {
                        activeItems[i].SetState(instance.CurrentCooldown, instance.Data.Cooldown, instance.IsPrimed);
                    }
                }
                else
                {
                    // If we have more UI items than data (e.g. removed buff), disable or hide?
                    // For now, TypeRogue doesn't seem to support removing buffs, so this is fine.
                    // If needed: activeItems[i].gameObject.SetActive(false);
                }
            }
        }
        
        // 废弃旧接口
        public void UpdateBuffs(IEnumerable<string> buffNames) 
        {
            // Do nothing
        }
    }
}
