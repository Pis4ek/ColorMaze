using MainMenu;
using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "SkinsConfig", menuName = "Configs/SkinsConfig")]
    public class SkinsConfig : ScriptableObject
    {
        [field: SerializeField] public List<Skin> SkinList { get; private set; }

        public Skin GetNodeBySkinID(int id)
        {
            for (var i = 0; i < SkinList.Count; i++)
            {
                if (SkinList[i].ID == id)
                {
                    return SkinList[i];
                }
            }
            throw new System.Exception($"{GetType().Name} has not skin with such ID({id}).");
        }
    }
}