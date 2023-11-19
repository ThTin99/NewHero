using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentItem", menuName = "Beginner Code/Equipment Item", order = -999)]
public class EquipmentItem : Item
{
    public enum EquipmentSlot
    {
        Head,
        Torso,
        Legs,
        Pet1,
        Pet2,
        Weapon = 666
    }

    public abstract class EquippedEffect : ScriptableObject
    {
        public string Description;
        //return true if could be used, false otherwise.
        public abstract void Equipped(CharacterData user);
        public abstract void Removed(CharacterData user);

        public virtual string GetDescription()
        {
            return Description;
        }
    }

    public EquipmentSlot Slot;

    [Header("Minimum Stats")]
    public int MinimumStrength;
    public int MinimumAgility;
    public int MinimumDefense;

    public List<EquippedEffect> EquippedEffects;

    public ushort Level { get => level; private set => level = value; }
    public ushort Star { get => star; private set => star = value; }

    public bool equiped;

    [Range(1, 100)]
    public ushort level = 1;

    [Range(1, 5)]
    [SerializeField]
    private ushort star = 1;
    public bool CanEnhance => star < 5;

    public uint UpgradeCost;


    public uint CurrentUpgradeCost => UpgradeCost * level;

    public ushort LevelLimit => (ushort)(star * 5);

    public override bool UsedBy(CharacterData user)
    {
        var userStat = user.Stats.CurrentStats;

        if (userStat.agility < MinimumAgility
            || userStat.strength < MinimumStrength
            || userStat.defense < MinimumDefense)
        {
            return false;
        }

        user.Equipment.Equip(this);

        return true;
    }

    public override string GetDescription()
    {
        var desc = "";
        foreach (var effect in EquippedEffects)
            desc += effect.GetDescription();

        //bool requireStrength = MinimumStrength > 0;
        //bool requireDefense = MinimumDefense > 0;
        //bool requireAgility = MinimumAgility > 0;

        //if (requireStrength || requireAgility || requireDefense)
        //{
        //    desc += "\nRequire : \n";

        //    if (requireStrength)
        //        desc += $"Strength : {MinimumStrength}";

        //    if (requireAgility)
        //    {
        //        if (requireStrength) desc += " & ";
        //        desc += $"Defense : {MinimumDefense}";
        //    }

        //    if (requireDefense)
        //    {
        //        if (requireStrength || requireAgility) desc += " & ";
        //        desc += $"Agility : {MinimumAgility}";
        //    }
        //}

        return desc;
    }


    public void EquippedBy(CharacterData user)
    {
        foreach (var effect in EquippedEffects)
            effect.Equipped(user);
    }

    public void UnequippedBy(CharacterData user)
    {
        foreach (var effect in EquippedEffects)
            effect.Removed(user);
    }

    public void Upgrade()
    {
        if (!IsReachLevelLimit)
        {
            GameFlowManager.Instance.UserProfile.UpdateCurrency(ItemType.GOLD, (int)-UpgradeCost);
            level++;
            foreach (var item in EquippedEffects)
            {
                var eff = (StatChangeEquipEffect)item;
                eff.Modifier.Stats.level = level;
            }
            InventoryUI.Instance.PlayUpgradeVFX();
        }
    }

    public bool IsReachLevelLimit => level >= LevelLimit;

    public void Enhance()
    {
        if (IsReachLevelLimit && CanEnhance && IsHaveEnoughMaterial())
        {
            InventoryUI.Instance.ShowEnhanceVFX();
            GameFlowManager.Instance.UserProfile.UpdateCurrency(ItemType.GOLD, (int)-CurrentUpgradeCost * 2);
            InventoryUI.Instance.ShowEnhancePopupSuccess(this);
            Star++;
            Level = 1;
            foreach (var item in EquippedEffects)
            {
                var eff = (StatChangeEquipEffect)item;
                eff.Modifier.Stats.level = Level;
            }
        }
        else
        {
            NotificationManager.Instance.ShowNotifyWithContent("Not enough Material!");
        }
    }

    public bool IsHaveEnoughMaterial()
    {
        var requirement = EquipmentDatabaseManager.Instance.EnhanceRequirements[star - 1];
        var canEnhance = true;
        for (int i = 0; i < requirement.RequiredMaterials.Count; i++)
        {
            if (GameFlowManager.Instance.UserProfile.EquipmentItems.Find(x => x.Item.ItemName == requirement.RequiredMaterials[i].MaterialItem.ItemName).Count < requirement.RequiredMaterials[i].Amount)
            {
                canEnhance = false;
            }
        }
        //subtract the required amount here for convenience
        if (canEnhance == true)
        {
            for (int i = 0; i < requirement.RequiredMaterials.Count; i++)
            {
                GameFlowManager.Instance.UserProfile.EquipmentItems.Find(x => x.Item.ItemName == requirement.RequiredMaterials[i].MaterialItem.ItemName).Count -= requirement.RequiredMaterials[i].Amount;
            }
        }
        return canEnhance;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EquipmentItem))]
public class EquipmentItemEditor : Editor
{
    EquipmentItem m_Target;

    ItemEditor m_ItemEditor;

    List<string> m_AvailableEquipEffectType;
    SerializedProperty m_EquippedEffectListProperty;

    SerializedProperty m_SlotProperty;
    SerializedProperty m_UpgradeCost;
    SerializedProperty m_Level;

    SerializedProperty m_MinimumStrengthProperty;
    SerializedProperty m_MinimumAgilityProperty;
    SerializedProperty m_MinimumDefenseProperty;

    void OnEnable()
    {
        m_Target = target as EquipmentItem;
        m_EquippedEffectListProperty = serializedObject.FindProperty(nameof(EquipmentItem.EquippedEffects));

        m_SlotProperty = serializedObject.FindProperty(nameof(EquipmentItem.Slot));
        m_UpgradeCost = serializedObject.FindProperty(nameof(EquipmentItem.UpgradeCost));
        m_Level = serializedObject.FindProperty(nameof(EquipmentItem.level));

        m_MinimumStrengthProperty = serializedObject.FindProperty(nameof(EquipmentItem.MinimumStrength));
        m_MinimumAgilityProperty = serializedObject.FindProperty(nameof(EquipmentItem.MinimumAgility));
        m_MinimumDefenseProperty = serializedObject.FindProperty(nameof(EquipmentItem.MinimumDefense));

        m_ItemEditor = new ItemEditor();
        m_ItemEditor.Init(serializedObject);

        var lookup = typeof(EquipmentItem.EquippedEffect);
        m_AvailableEquipEffectType = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
            .Select(type => type.Name)
            .ToList();
    }

    public override void OnInspectorGUI()
    {
        m_ItemEditor.GUI();

        EditorGUILayout.PropertyField(m_SlotProperty);
        EditorGUILayout.PropertyField(m_UpgradeCost);
        EditorGUILayout.PropertyField(m_Level);

        EditorGUILayout.PropertyField(m_MinimumStrengthProperty);
        EditorGUILayout.PropertyField(m_MinimumAgilityProperty);
        EditorGUILayout.PropertyField(m_MinimumDefenseProperty);

        int choice = EditorGUILayout.Popup("Add new Effect", -1, m_AvailableEquipEffectType.ToArray());

        if (choice != -1)
        {
            var newInstance = ScriptableObject.CreateInstance(m_AvailableEquipEffectType[choice]);

            AssetDatabase.AddObjectToAsset(newInstance, target);

            m_EquippedEffectListProperty.InsertArrayElementAtIndex(m_EquippedEffectListProperty.arraySize);
            m_EquippedEffectListProperty.GetArrayElementAtIndex(m_EquippedEffectListProperty.arraySize - 1).objectReferenceValue = newInstance;
        }

        Editor ed = null;
        int toDelete = -1;
        for (int i = 0; i < m_EquippedEffectListProperty.arraySize; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            var item = m_EquippedEffectListProperty.GetArrayElementAtIndex(i);
            SerializedObject obj = new SerializedObject(item.objectReferenceValue);

            Editor.CreateCachedEditor(item.objectReferenceValue, null, ref ed);

            ed.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("-", GUILayout.Width(32)))
            {
                toDelete = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (toDelete != -1)
        {
            var item = m_EquippedEffectListProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
            DestroyImmediate(item, true);

            //need to do it twice, first time just nullify the entry, second actually remove it.
            m_EquippedEffectListProperty.DeleteArrayElementAtIndex(toDelete);
            m_EquippedEffectListProperty.DeleteArrayElementAtIndex(toDelete);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif