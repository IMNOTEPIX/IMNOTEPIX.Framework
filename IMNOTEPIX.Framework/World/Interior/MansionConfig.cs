using GTA.Math;

namespace IMNOTEPIX.Framework.fWorld.Interior
{
    /// <summary>
    /// Pure data for one mansion safehouse instance.
    /// Replace the 66 flat static fields on MPMansion.
    /// </summary>
    public class MansionConfig
    {
        public MansionInterior.MansionID ID { get; set; }
        public bool Enabled { get; set; } = true;

        // Floor lookup coords — resolved to interior IDs at call time
        public Vector3 MainFloorCoords { get; set; }
        public Vector3 LowerFloorCoords { get; set; }
        public Vector3 GarageFloorCoords { get; set; }

        // Combat / AI
        public int AI { get; set; } = 1;   // 0–2

        // Appearance
        public int DecorStyle { get; set; } = 1;   // 0 Loft | 1 Cali | 2 Regency
        public int WallpaperStyle { get; set; } = 1;   // 0 None | 1–6
        public int Style { get; set; } = 1;   // tint index

        // Vault
        public int VaultCash { get; set; } = 0;   // 0 = empty | 1–10
        public bool VaultRoom { get; set; } = true;

        // Rooms
        public bool ArcadeRoom { get; set; } = true;
        public bool PodiumRoom { get; set; } = true;
        public bool ArmouryRoom { get; set; } = true;
        public bool VehicleModShopRoom { get; set; } = true;
        public bool WindowsBlackedOut { get; set; } = false;
        public bool AfterParty { get; set; } = false;

        // Pets
        public bool KennelCat { get; set; } = false;
        public bool KennelDog { get; set; } = true;

        // Seasonal
        public bool DecorationsXmas { get; set; } = false;
        public bool DecorationsHalloween { get; set; } = false;
        public bool DecorationsNewYear { get; set; } = false;
        public bool DecorationsLunarNewYear { get; set; } = false;
        public bool DecorationsBirthday { get; set; } = false;

        // Radio
        public int MansionRadio { get; set; }
        public int MansionClubRadio { get; set; }
    }
}
