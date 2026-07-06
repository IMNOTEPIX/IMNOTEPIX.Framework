using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using static IMNOTEPIX.Framework.fWorld.Train;

namespace IMNOTEPIX.Framework.fWorld
{
    public static class TrainVariationModels
    {
        public static readonly Dictionary<TrainVariation, Model[]> Required = new Dictionary<TrainVariation, Model[]>
        {
            { TrainVariation.FreightConfig0,  new[] { new Model("freight"), new Model("freightcar") } },
            { TrainVariation.FreightConfig1,  new[] { new Model("freight"), new Model("freightcar") } },
            { TrainVariation.FreightConfig2,  new[] { new Model("freight"), new Model("freightcont2"), new Model("freightcont1"), new Model("freightgrain"), new Model("tankercar"), new Model("freightcar") } },
            { TrainVariation.FreightConfig3,  new[] { new Model("freight"), new Model("freightcar"), new Model("tankercar") } },
            { TrainVariation.FreightConfig4,  new[] { new Model("freight"), new Model("freightgrain"), new Model("freightcar") } },
            { TrainVariation.FreightConfig5,  new[] { new Model("freight"), new Model("freightcar"), new Model("freightgrain") } },
            { TrainVariation.FreightConfig6,  new[] { new Model("freight"), new Model("freightcont1"), new Model("freightcar"), new Model("tankercar") } },
            { TrainVariation.FreightConfig7,  new[] { new Model("freight"), new Model("freightcar"), new Model("freightcont2") } },
            { TrainVariation.FreightConfig8,  new[] { new Model("freight"), new Model("freightcont2"), new Model("freightcar"), new Model("tankercar") } },
            { TrainVariation.FreightConfig9,  new[] { new Model("freight"), new Model("freightcar"), new Model("freightgrain") } },
            { TrainVariation.FreightConfig10, new[] { new Model("freight"), new Model("freightcont1"), new Model("freightcont2"), new Model("freightcar") } },
            { TrainVariation.FreightConfig11, new[] { new Model("freight"), new Model("freightcont1"), new Model("freightcar") } },
            { TrainVariation.FreightConfig12, new[] { new Model("freight"), new Model("freightgrain"), new Model("freightcar") } },
            { TrainVariation.FreightConfig13, new[] { new Model("freight"), new Model("freightcar"), new Model("tankercar") } },
            { TrainVariation.FreightConfig14, new[] { new Model("freight"), new Model("freightcont1"), new Model("freightcar") } },
            { TrainVariation.FreightConfig15, new[] { new Model("freight"), new Model("freightgrain"), new Model("freightcar"), new Model("freightcont1") } },
            { TrainVariation.FreightConfig16, new[] { new Model("freight"), new Model("freightcar"), new Model("tankercar") } },
            { TrainVariation.FreightConfig17, new[] { new Model("freight"), new Model("freightcont2") } },
            { TrainVariation.FreightConfig18, new[] { new Model("freight") } },
            { TrainVariation.FreightConfig19, new[] { new Model("freight"), new Model("tankercar") } },
            { TrainVariation.FreightConfig20, new[] { new Model("freight"), new Model("freightcont2") } },
            { TrainVariation.FreightConfig21, new[] { new Model("metrotrain") } },
            { TrainVariation.FreightConfig22, new[] { new Model("freight"), new Model("freightcar") } },
            { TrainVariation.FreightConfig23, new[] { new Model("freight"), new Model("freightcar"), new Model("freightcont1") } },
            { TrainVariation.FreightConfig24, new[] { new Model("freight"), new Model("freightcar2"), new Model("freightgrain") } },
            { TrainVariation.FreightConfig25, new[] { new Model("freight"), new Model("freightgrain"), new Model("freightcar2"), new Model("tankercar") } },
            { TrainVariation.FreightConfig26, new[] { new Model("freight2"), new Model("freightcar2") } },
            { TrainVariation.FreightConfig27, new[] { new Model("freight"), new Model("freightcar3") } },

            { TrainVariation.MetroConfig0,    new[] { new Model("metrotrain") } },
        };
    }

    public class Train
    {
        public enum TrainDirection { Forward = 0, Backward = 1 }
        public enum TrainVariation
        {
            // Freight configs
            /// <summary>freight x1, freightcar x2</summary>
            FreightConfig0 = 0,
            /// <summary>freight x1, freightcar x8</summary>
            FreightConfig1 = 1,
            /// <summary>freight x1, freightcont2 x1, freightcont1 x1, freightgrain x1, tankercar x1, freightcar x1</summary>
            FreightConfig2 = 2,
            /// <summary>freight x1, freightcar x1, tankercar x3, freightcar x2, tankercar x2</summary>
            FreightConfig3 = 3,
            /// <summary>freight x1, freightgrain x2, freightcar x1, freightgrain x1, freightcar x3, freightgrain x2</summary>
            FreightConfig4 = 4,
            /// <summary>freight x1, freightcar x4, freightgrain x1, freightcar x1</summary>
            FreightConfig5 = 5,
            /// <summary>freight x1, freightcont1 x2, freightcar x1, tankercar x1</summary>
            FreightConfig6 = 6,
            /// <summary>freight x1, freightcar x3, freightcont2 x3, freightcar x2</summary>
            FreightConfig7 = 7,
            /// <summary>freight x1, freightcont2 x2, freightcar x1, freightcont2 x2, freightcar x1, tankercar x1</summary>
            FreightConfig8 = 8,
            /// <summary>freight x1, freightcar x1, freightgrain x2, freightcar x2, freightgrain x1</summary>
            FreightConfig9 = 9,
            /// <summary>freight x1, freightcont1 x3, freightcont2 x2, freightcar x3</summary>
            FreightConfig10 = 10,
            /// <summary>freight x1, freightcont1 x1, freightcar x1, freightcont1 x5</summary>
            FreightConfig11 = 11,
            /// <summary>freight x1, freightgrain x3, freightcar x1, freightgrain x2</summary>
            FreightConfig12 = 12,
            /// <summary>freight x1, freightcar x2, tankercar x3, freightcar x3</summary>
            FreightConfig13 = 13,
            /// <summary>freight x1, freightcont1 x1, freightcar x2, freightcont1 x3, freightcar x1, freightcont1 x1</summary>
            FreightConfig14 = 14,
            /// <summary>freight x1, freightgrain x3, freightcar x1, freightcont1 x2, freightgrain x1, freightcar x1</summary>
            FreightConfig15 = 15,
            /// <summary>freight x1, freightcar x1, tankercar x3, freightcar x1, tankercar x2</summary>
            FreightConfig16 = 16,
            /// <summary>freight x1, freightcont2 x40 (no_random_spawn)</summary>
            FreightConfig17 = 17,
            /// <summary>freight x1 only (no_random_spawn)</summary>
            FreightConfig18 = 18,
            /// <summary>freight x1, tankercar x2 (no_random_spawn)</summary>
            FreightConfig19 = 19,
            /// <summary>freight x1, freightcont2 x3 (no_random_spawn)</summary>
            FreightConfig20 = 20,
            /// <summary>metrotrain x1 only (no_random_spawn)</summary>
            FreightConfig21 = 21,
            /// <summary>freight x1, freightcar x12 (no_random_spawn)</summary>
            FreightConfig22 = 22,
            /// <summary>freight x1, alternating freightcar/freightcont1 x6 each (no_random_spawn)</summary>
            FreightConfig23 = 23,
            /// <summary>freight x1, freightcar2 x4, freightgrain x1, freightcar2 x1 (no_random_spawn)</summary>
            FreightConfig24 = 24,
            /// <summary>freight x1, mixed freightgrain/freightcar2/tankercar x15 (no_random_spawn)</summary>
            FreightConfig25 = 25,
            /// <summary>freight2 x1, freightcar2 x5 (no_random_spawn)</summary>
            FreightConfig26 = 26,
            /// <summary>freight x1, freightcar3 x6 (no_random_spawn)</summary>
            FreightConfig27 = 27,

            // Metro configs
            /// <summary>metrotrain x1, metrotrain x1 (flipped)</summary>
            MetroConfig0 = 28,
        }

        public GTA.Vehicle Handle { get; private set; }
        public bool IsValid => !(Handle == null || !Handle.Exists());

        public bool Derailed 
        { set => Function.Call(Hash.SET_RENDER_TRAIN_AS_DERAILED, IsValid ? Handle : throw new NullReferenceException("Cannot set a non-existing train as derailed!"), value);  }

        public float Speed
        { 
            get => Handle.Speed;
            set
            {
                Function.Call(Hash.SET_TRAIN_SPEED, Handle, value);
            }
        }

        public float CruiseSpeed
        {
            set
            {
                Function.Call(Hash.SET_TRAIN_CRUISE_SPEED, Handle, value);
            }
        }

        public Vector3 Position 
        {
            get
            {
                if (!IsValid)
                    throw new NullReferenceException("Cannot get the position of a train that does not exist!");

                return Handle.Position;
            } 
            set
            {
                if (!IsValid)
                    return;

                Function.Call(Hash.SET_​MISSION_​TRAIN_​COORDS, Handle, value.X, value.Y, value.Z);
            } 
        }

        private readonly Vector3 SpawnPosition;
        private readonly TrainVariation Variation;
        private readonly TrainDirection Direction;
        private readonly Model[] Models;

        internal bool IsNetwork = false;
        internal bool NetMissionEntity = false;

        public Train(TrainVariation variation, Vector3 spawnPosition, TrainDirection direction)
        {
            Variation = variation;
            SpawnPosition = spawnPosition;
            Direction = direction;
            Models = TrainVariationModels.Required[variation];
        }

        public Train(TrainVariation variation, float spawnX, float spawnY, float spawnZ, TrainDirection direction)
        {
            Variation = variation;
            SpawnPosition = new Vector3(spawnX, spawnY, spawnZ);
            Direction = direction;
            Models = TrainVariationModels.Required[variation];
        }


        public void Create()
        {
            if (IsValid) return;

            if ((int)Variation < 0 || (int)Variation > 28)
                throw new ArgumentOutOfRangeException("Cannot set TrainVariation below 0 or above 28.");

            foreach (var model in Models)
            {
                model.Request();
                while (!model.IsLoaded)
                {
                    Script.Yield();
                    model.Request();
                }
            }

            Handle = Function.Call<GTA.Vehicle>(Hash.CREATE_MISSION_TRAIN, (int)Variation, SpawnPosition.X, SpawnPosition.Y, SpawnPosition.Z, (int)Direction, IsNetwork, NetMissionEntity);
            while (Handle == null)
                Script.Yield();

            foreach (var model in Models)
                model.MarkAsNoLongerNeeded();
        }

        public GTA.Vehicle[] GetCarriagesArray()
        {
            if (!IsValid) return null;

            var carriages = new List<GTA.Vehicle>();

            int index = 0;
            while (true)
            {
                int handle = Function.Call<int>(Hash.GET_TRAIN_CARRIAGE, Handle.Handle, index);

                if (handle == 0) break;

                GTA.Vehicle carriage = (GTA.Vehicle)GTA.Entity.FromHandle(handle);
                if (carriage == null || !carriage.Exists()) break;

                carriages.Add(carriage);
                index++;
            }

            return carriages.ToArray();
        }

        public List<GTA.Vehicle> GetCarriagesList()
        {
            if (!IsValid) return null;

            var carriages = new List<GTA.Vehicle>();

            int index = 0;
            while (true)
            {
                int handle = Function.Call<int>(Hash.GET_TRAIN_CARRIAGE, Handle.Handle, index);

                if (handle == 0) break;

                GTA.Vehicle carriage = (GTA.Vehicle)GTA.Entity.FromHandle(handle);
                if (carriage == null || !carriage.Exists()) break;

                carriages.Add(carriage);
                index++;
            }

            return carriages;
        }

        public void MarkAsNoLongerNeeded()
        {
            if (!IsValid) return;

            foreach (var var in GetCarriagesList())
            {
                if (var == null || !var.Exists()) continue;

                var.AttachedBlip?.Delete();
                var.MarkAsNoLongerNeeded();
            }

            Handle.AttachedBlip?.Delete();
            Handle.MarkAsNoLongerNeeded();
        }

        public void Delete()
        {
            if (!IsValid) return;

            foreach (var var in GetCarriagesList())
            {
                if (var == null || !var.Exists()) continue;

                var.AttachedBlip?.Delete();
                var.Delete();
            }

            Handle.AttachedBlip?.Delete();
            Handle.Delete();
            Handle = null;

        }


    }
}
