using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ekka.blendShapeBuilder
{
    [Serializable]
    public struct BlendShapeBlueprint
    {
        [Serializable]
        public struct BlendShapeSource
        {
            public string name;
            public float weight;

            public BlendShapeSource(string name = "", float weight = 100f)
            {
                this.name = name;
                this.weight = weight;
            }
        }

        public string newBlendShapeName;
        public List<BlendShapeSource> blendShapeSources;

        public BlendShapeBlueprint(string newBlendShapeName)
        {
            this.newBlendShapeName = newBlendShapeName;
            blendShapeSources = new List<BlendShapeSource>();// { new Blendshape(mmdBlendShapeName, 100f) };
        }
    }

    [CreateAssetMenu(fileName = "blend_shape_blueprint_list", menuName = "EKKA/Blend Shape Blueprint List")]
    public class BlendShapeBlueprintList : ScriptableObject
    {
        private static readonly string[] MMDBlendShapesNameAnimasa = {
            "真面目",
            "困る",
            "にこり",
            "怒り",
            "上",
            "下",
            "まばたき",
            "笑い",
            "ウィンク",
            "ウィンク２",
            "ウィンク右",
            "ｳｨﾝｸ２右",
            "はぅ",
            "なごみ",
            "びっくり",
            "じと目",
            "なぬ！",
            "あ",
            "い",
            "う",
            "お",
            "▲",
            "∧",
            "ω",
            "ω□",
            "はんっ！",
            "えー",
            "にやり",
            "瞳小",
            "ぺろっ",
        };

        // Ref=https://docs.google.com/spreadsheets/d/1mfE8s48pUfjP_rBIPN90_nNkAIBUNcqwIxAdVzPBJ-Q/
        private static readonly string[] MMDBlendShapesName = {
            /* Mouth */
            "あ",
            "い",
            "う",
            "え",
            "お",
            "にやり",
            "∧",
            "ワ",
            "ω",
            "▲",
            "口角上げ",
            "口角下げ",
            "口横広げ",
            
            /* Eye */
            "まばたき",
            "笑い",
            "はぅ",
            "瞳小",
            "ｳｨﾝｸ２右",
            "ウィンク２",
            "ウィンク",
            "ウィンク右",
            "なごみ",
            "じと目",
            "びっくり",
            "ｷﾘｯ",
            "はぁと",
            "星目",

            /* Eyebrow */
            "にこり",
            "上",
            "下",
            "真面目",
            "困る",
            "怒り",
            "前",

            /* Other */
            "照れ",
            "にやり２",
            "ん",
            "あ2",
            "恐ろしい子！",
            "歯無し下",
            "涙",
        };

        public List<BlendShapeBlueprint> blendShapes = new List<BlendShapeBlueprint>();

        public BlendShapeBlueprintList()
        {
            //InitilizeBlendShapesNameList();
        }

        public void InitilizeBlendShapesNameList()
        {
            blendShapes.Clear();
            foreach (string name in MMDBlendShapesName)
            {
                blendShapes.Add(new BlendShapeBlueprint(name));
            }
        }
    }
}
