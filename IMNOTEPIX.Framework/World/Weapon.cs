using GTA;
using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Weapon
    {
        public static void GiveWeaponToPed(GTA.Ped ped, int weaponHash, int ammoCount, bool isHidden, bool bForceInHand)
            => Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, weaponHash, ammoCount, isHidden, bForceInHand);

        public static bool HasPedGotWeapon(GTA.Ped ped, int weaponHash, bool p2 = false)
            => Function.Call<bool>(Hash.HAS_​PED_​GOT_​WEAPON, ped, weaponHash, p2);
    }
}
