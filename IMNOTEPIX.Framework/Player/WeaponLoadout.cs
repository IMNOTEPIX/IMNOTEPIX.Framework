namespace IMNOTEPIX.Framework.fPlayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using GTA;
    using GTA.Native;

    public class WeaponLoadout
    {
        private readonly List<(WeaponHash Hash, int Ammo, List<WeaponComponentHash> Components)> _weapons;

        public WeaponLoadout(params (WeaponHash Hash, int Ammo)[] weapons)
        {
            if (weapons == null || weapons.Length == 0)
                throw new ArgumentException("A loadout must contain at least one weapon.");

            _weapons = new List<(WeaponHash, int, List<WeaponComponentHash>)>();
            foreach (var (hash, ammo) in weapons)
                _weapons.Add((hash, ammo, new List<WeaponComponentHash>()));
        }

        private WeaponLoadout(List<(WeaponHash, int, List<WeaponComponentHash>)> weapons)
        {
            _weapons = weapons;
        }

        /// <summary>
        /// Attaches a component to a weapon already in this loadout. No-op if the weapon isn't present.
        /// </summary>
        public void AddComponent(WeaponHash weapon, WeaponComponentHash component)
        {
            for (int i = 0; i < _weapons.Count; i++)
            {
                if (_weapons[i].Hash == weapon)
                {
                    _weapons[i].Components.Add(component);
                    return;
                }
            }
        }
        private bool IsWeaponMK2(WeaponHash weapon)
        {
            if (weapon == WeaponHash.PistolMk2 || weapon == WeaponHash.SMGMk2 || weapon == WeaponHash.AssaultrifleMk2 || weapon == WeaponHash.CarbineRifleMk2 || weapon == WeaponHash.CombatMGMk2 ||
                weapon == WeaponHash.HeavySniperMk2 || weapon == WeaponHash.BullpupRifleMk2 || weapon == WeaponHash.MarksmanRifleMk2 || weapon == WeaponHash.PumpShotgunMk2 || weapon == WeaponHash.RevolverMk2 ||
                weapon == WeaponHash.SNSPistolMk2 || weapon == WeaponHash.SpecialCarbineMk2)
                return true;
            else
                return false;
        }

        public void Apply(bool clearExisting = false)
        {
            var player = Game.Player.Character;

            if (clearExisting)
                Function.Call(Hash.REMOVE_ALL_PED_WEAPONS, player.Handle, true);

            foreach (var (hash, ammo, components) in _weapons)
            {
                bool alreadyHas = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, player.Handle, (uint)hash, false);
                if (!alreadyHas)
                    Function.Call(Hash.GIVE_WEAPON_TO_PED, player.Handle, (uint)hash, 0, false, true);

                foreach (var component in components)
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, player.Handle, (uint)hash, (uint)component);

                if (IsWeaponMK2(hash))
                {
                    uint ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, player.Handle, (uint)hash);
                    if (Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, player.Handle, (uint)hash) <= 0)
                        Function.Call(Hash.ADD_PED_AMMO_BY_TYPE, player.Handle, ammoType, ammo);
                    else
                    {
                        Function.Call(Hash.SET_PED_AMMO, player.Handle, (uint)hash, 0);
                        Function.Call(Hash.ADD_PED_AMMO_BY_TYPE, player.Handle, ammoType, ammo);
                    }
                }
                else
                {
                    Function.Call(Hash.SET_PED_AMMO, player.Handle, (uint)hash, ammo);
                }
            }
        }

        public void Save(string fileName, string directory = "scripts/IMNOTEPIX/Data")
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var root = new XElement("Loadout");

            foreach (var (hash, ammo, components) in _weapons)
            {
                var weaponEl = new XElement("Weapon",
                    new XAttribute("hash", (uint)hash),
                    new XAttribute("ammo", ammo));

                foreach (var component in components)
                    weaponEl.Add(new XElement("Component", new XAttribute("hash", (uint)component)));

                root.Add(weaponEl);
            }

            string path = Path.Combine(directory, $"{fileName}.xml");
            new XDocument(root).Save(path);
        }

        public static WeaponLoadout FromPlayer()
        {
            var player = Game.Player.Character;
            var entries = new List<(WeaponHash, int, List<WeaponComponentHash>)>();

            foreach (WeaponHash hash in Enum.GetValues(typeof(WeaponHash)))
            {
                if (hash == WeaponHash.Unarmed)
                    continue;

                bool hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, player.Handle, (uint)hash, false);
                if (!hasWeapon)
                    continue;

                int ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, player.Handle, (uint)hash);

                var components = new List<WeaponComponentHash>();
                foreach (WeaponComponentHash component in Enum.GetValues(typeof(WeaponComponentHash)))
                {
                    bool hasComponent = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT,
                        player.Handle, (uint)hash, (uint)component);

                    if (hasComponent)
                        components.Add(component);
                }

                entries.Add((hash, ammo, components));
            }

            if (entries.Count <= 1)
            {
#if DEBUG
                GTA.UI.Notification.PostTicker("~y~WeaponLoadout: player is carrying no weapons.", false);
#endif
                return null;
            }

            return new WeaponLoadout(entries);
        }

        public void Print()
        {
            var lines = new System.Text.StringBuilder();
            lines.AppendLine("~b~── Weapon Loadout ──");

            foreach (var (hash, ammo, components) in _weapons)
            {
                lines.AppendLine($"~w~{hash} ~s~| Ammo: ~y~{ammo}");
                foreach (var component in components)
                    lines.AppendLine($"~s~   + ~g~{component}");
            }

            GTA.UI.Notification.PostTicker(lines.ToString(), false);
        }

        public IReadOnlyList<(WeaponHash Hash, int Ammo, IReadOnlyList<WeaponComponentHash> Components)> GetWeapons()
        {
            var result = new List<(WeaponHash, int, IReadOnlyList<WeaponComponentHash>)>();
            foreach (var (hash, ammo, components) in _weapons)
                result.Add((hash, ammo, components.AsReadOnly()));
            return result.AsReadOnly();
        }

        public static WeaponLoadout Load(string fileName, string directory = "scripts/IMNOTEPIX/Data")
        {
            string path = Path.Combine(directory, $"{fileName}.xml");

            if (!File.Exists(path))
            {
                GTA.UI.Notification.PostTicker($"~r~WeaponLoadout: '{fileName}.xml' not found.", false);
                return null;
            }

            try
            {
                var entries = new List<(WeaponHash, int, List<WeaponComponentHash>)>();
                var root = XDocument.Load(path).Root;

                foreach (var weaponEl in root.Elements("Weapon"))
                {
                    var hash = (WeaponHash)uint.Parse((string)weaponEl.Attribute("hash"));
                    int ammo = int.Parse((string)weaponEl.Attribute("ammo"));

                    var components = new List<WeaponComponentHash>();
                    foreach (var componentEl in weaponEl.Elements("Component"))
                    {
                        var component = (WeaponComponentHash)uint.Parse((string)componentEl.Attribute("hash"));
                        components.Add(component);
                    }

                    entries.Add((hash, ammo, components));
                }

                return new WeaponLoadout(entries);
            }
            catch (Exception ex)
            {
                GTA.UI.Notification.PostTicker($"~r~WeaponLoadout: failed to load '{fileName}': {ex.Message}", false);
                return null;
            }
        }
    }
}