using UnityEngine;
using System.Collections.Generic;

namespace TypeRogue
{
    /// <summary>
    /// 武器状态面板视图：管理武器列表
    /// 功能：
    /// 1. 维护一个 WeaponStatusItemView 的列表
    /// 2. 提供添加、获取武器 Item 的接口
    /// </summary>
    public sealed class WeaponStatusPanelView : MonoBehaviour
    {
        [SerializeField] private WeaponStatusItemView itemPrefab;
        [SerializeField] private Transform container; // 挂载 VerticalLayoutGroup 的父节点

        private readonly List<WeaponStatusItemView> items = new List<WeaponStatusItemView>();

        // 临时：为了简单起见，我们目前假设只有一个武器，或者通过索引访问
        // 在更复杂的系统中，可以使用 Dictionary<string, WeaponStatusItemView> 或 WeaponID
        
        public void Initialize()
        {
            if (container == null) return;
            
            // 清理现有的（如果是开发模式下的测试数据）
            // 注意：如果在编辑器下运行，Destroy可能无法立即清理，但在Start中通常是安全的
            // 为了避免修改正在遍历的集合，我们先收集再销毁，或者反向遍历，或者直接Destroy(child.gameObject)也是Unity允许的
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
            items.Clear();
        }

        public WeaponStatusItemView AddWeapon(string weaponName)
        {
            if (itemPrefab == null || container == null) return null;

            var itemObj = Instantiate(itemPrefab, container);
            
            // 修复 Unity 断言错误：Assertion failed on expression: '!(o->TestHideFlag(Object::kDontSaveInEditor)...'
            // 某些情况下，实例化的对象可能会继承错误的 hideFlags（例如如果 Prefab 本身带有 DontSave 标记），导致 Unity 试图保存它时报错
            // 显式重置为 None 可以解决此问题
            itemObj.gameObject.hideFlags = HideFlags.None;
            
            // 确保缩放正确（UI实例化有时会受到父级缩放影响）
            itemObj.transform.localScale = Vector3.one;
            
            itemObj.Initialize(weaponName);
            items.Add(itemObj);
            return itemObj;
        }

        public WeaponStatusItemView GetItem(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                return items[index];
            }
            return null;
        }
    }
}
