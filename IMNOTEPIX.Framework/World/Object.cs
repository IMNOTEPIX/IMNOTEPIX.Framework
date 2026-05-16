using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fWorld
{
    public static class Object
    {
        public static void DeleteObjectsInList(List<GTA.Prop> objectList)
        {
            foreach (var obj in objectList)
            {
                if (obj != null)
                    obj.Delete();
            }
            objectList.Clear();
        }

        public static void CreateObjectForList(List<GTA.Prop> objectList, Model model, Vector3 pos, Vector3 rot, bool dynamic, bool placeOnGround = true)
        {
            GTA.Prop obj = GTA.World.CreateProp(model, pos, rot, dynamic, placeOnGround);

            if (obj != null && !obj.Exists())
                GTA.Script.Yield();

            if (obj != null && !objectList.Contains(obj))
                objectList.Add(obj);
        }
        public static void CreateObjectForList(List<GTA.Prop> objectList, Model model, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, bool dynamic, bool placeOnGround = true)
        {
            GTA.Prop obj = GTA.World.CreateProp(model, new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), dynamic, placeOnGround);

            if (obj != null && !obj.Exists())
                GTA.Script.Yield();

            if (obj != null && !objectList.Contains(obj))
                objectList.Add(obj);
        }

        public static void CreateObjectNoOffsetForList(List<GTA.Prop> objectList, Model model, Vector3 pos, Vector3 rot, bool dynamic)
        {
            GTA.Prop obj = GTA.World.CreatePropNoOffset(model, pos, rot, dynamic);

            if (obj != null && !obj.Exists())
                GTA.Script.Yield();

            if (obj != null && !objectList.Contains(obj))
                objectList.Add(obj);
        }

        public static void CreateObjectNoOffsetForList(List<GTA.Prop> objectList, Model model, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, bool dynamic)
        {
            GTA.Prop obj = GTA.World.CreatePropNoOffset(model, new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), dynamic);

            if (obj != null && !obj.Exists())
                GTA.Script.Yield();

            if (obj != null && !objectList.Contains(obj))
                objectList.Add(obj);
        }

        [Obsolete("Call fWorld.CreateProp directly.")]
        public static GTA.Prop CreateObject(Model model, Vector3 pos, Vector3 rot, bool dynamic, bool placeOnGround)
            => GTA.World.CreateProp(model, pos, rot, dynamic, placeOnGround);

        [Obsolete("Call fWorld.CreatePropNoOffset directly.")]
        public static GTA.Prop CreateObjectNoOffset(Model model, Vector3 pos, Vector3 rot, bool dynamic)
            => GTA.World.CreatePropNoOffset(model, pos, rot, dynamic);
    }
}
