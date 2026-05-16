using GTA.Native;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fWorld.Cutscene
{
    public class RegisterEntityChunk
    {
        public RegisterEntityChunk(GTA.Entity ent, string cHandle, Cutscene.CutsceneUsage usage, int modelNames, Cutscene.CutsceneEntityOptionFlag entityOptionsFlag)
        {
            Ent = ent;
            CHandle = cHandle;
            Usage = usage;
            ModelNames = modelNames;
            EntityOptionsFlag = entityOptionsFlag;
        }

        public RegisterEntityChunk(int ent, string cHandle, Cutscene.CutsceneUsage usage, int modelNames, Cutscene.CutsceneEntityOptionFlag entityOptionsFlag)
        {
            EntInt = ent;
            CHandle = cHandle;
            Usage = usage;
            ModelNames = modelNames;
            EntityOptionsFlag = entityOptionsFlag;
        }

        public void GetCutscenePedOutfit(GTA.Ped NonCutscene)
        {
            CutscenePedPropData = new Dictionary<int, PedPropData>
            {
                { 0, new PedPropData(Function.Call<int>(Hash.GET_PED_PROP_INDEX, NonCutscene, 0), Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, NonCutscene, 0)) },
                { 1, new PedPropData(Function.Call<int>(Hash.GET_PED_PROP_INDEX, NonCutscene, 1), Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, NonCutscene, 1)) },
                { 2, new PedPropData(Function.Call<int>(Hash.GET_PED_PROP_INDEX, NonCutscene, 2), Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, NonCutscene, 2)) },
                { 6, new PedPropData(Function.Call<int>(Hash.GET_PED_PROP_INDEX, NonCutscene, 6), Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, NonCutscene, 6)) },
                { 7, new PedPropData(Function.Call<int>(Hash.GET_PED_PROP_INDEX, NonCutscene, 7), Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, NonCutscene, 7)) }
            };
            CutscenePedVariationData = new Dictionary<int, PedVariationData>
            {
                { 0, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 0), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 0)) },
                { 1, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 1), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 1)) },
                { 2, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 2), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 2)) },
                { 3, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 3), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 3)) },
                { 4, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 4), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 4)) },
                { 5, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 5), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 5)) },
                { 6, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 6), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 6)) },
                { 7, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 7), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 7)) },
                { 8, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 8), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 8)) },
                { 9, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 9), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 9)) },
                { 10, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 10), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 10)) },
                { 11, new PedVariationData(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, NonCutscene, 11), Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, NonCutscene, 11)) },
            };
        }

        public void SetCutscenePedOutfit(GTA.Ped NonCutscene)
        {
            foreach (var data in CutscenePedVariationData)
            {
                int componentId = data.Key;
                int drawable = data.Value.Drawable;
                int texture = data.Value.Texture;

                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, NonCutscene, componentId, drawable, texture, 1);
            }
            foreach (var data in CutscenePedPropData)
            {
                int componentId = data.Key;
                int drawable = data.Value.Drawable;
                int texture = data.Value.Texture;

                Function.Call(Hash.SET_​PED_​PROP_​INDEX, NonCutscene, componentId, drawable, texture, true, 0);
            }
        }

        public GTA.Entity Ent;

        public int EntInt = -1;

        public string CHandle;

        public Cutscene.CutsceneUsage Usage;

        public int ModelNames;

        public Cutscene.CutsceneEntityOptionFlag EntityOptionsFlag;

        private Dictionary<int, PedPropData> CutscenePedPropData = new Dictionary<int, PedPropData>
        {
                { 0, new PedPropData(0, 0) },
                { 1, new PedPropData(0, 0) },
                { 2, new PedPropData(0, 0) },
                { 6, new PedPropData(0, 0) },
                { 7, new PedPropData(0, 0) }
        };

        private Dictionary<int, PedVariationData> CutscenePedVariationData = new Dictionary<int, PedVariationData>
        {
                { 0, new PedVariationData(0, 0) },
                { 1, new PedVariationData(0, 0) },
                { 2, new PedVariationData(0, 0) },
                { 3, new PedVariationData(0, 0) },
                { 4, new PedVariationData(0, 0) },
                { 5, new PedVariationData(0, 0) },
                { 6, new PedVariationData(0, 0) },
                { 7, new PedVariationData(0, 0) },
                { 8, new PedVariationData(0, 0) },
                { 9, new PedVariationData(0, 0) },
                { 10, new PedVariationData(0, 0) },
                { 11, new PedVariationData(0, 0) },
        };

        public struct PedPropData
        {
            public int Drawable;
            public int Texture;

            public PedPropData(int propDrawable, int propTexture)
            {
                Drawable = propDrawable;
                Texture = propTexture;
            }
        }

        public struct PedVariationData
        {
            public int Drawable;
            public int Texture;

            public PedVariationData(int drawable, int texture)
            {
                Drawable = drawable;
                Texture = texture;
            }
        }
    }
}
