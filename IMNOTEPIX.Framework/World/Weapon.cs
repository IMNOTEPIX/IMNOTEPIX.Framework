using GTA;
using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Weapon
    {
        public static void GiveWeaponToPed(GTA.Ped ped, int weaponHash, int ammoCount, bool isHidden, bool bForceInHand)
            => Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, weaponHash, ammoCount, isHidden, bForceInHand);
    }
}
