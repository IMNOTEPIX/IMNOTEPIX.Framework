namespace IMNOTEPIX.Framework.fPlayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using GTA;
    using GTA.Native;

    /// <summary>
    /// A weapon loadout that lives as a variable in your code.
    /// Create it, apply it to the player, and optionally save/load it to XML.
    ///
    /// Usage:
    ///     var combatLoadout = new WeaponLoadout(
    ///         (WeaponHash.Pistol,   100),
    ///         (WeaponHash.Shotgun,   50),
    ///         (WeaponHash.SMG,      200)
    ///     );
    ///     combatLoadout.Apply();
    ///     combatLoadout.Apply(clearExisting: true);
    ///     combatLoadout.Save("Combat", "scripts/IMNOTEPIX/Loadouts");
    ///
    ///     // Snapshot the player's current weapons as a loadout variable.
    ///     var snapshot = WeaponLoadout.FromPlayer();
    ///     snapshot.Print();
    ///     snapshot.Save("MySnapshot");
    /// </summary>
    public class WeaponLoadout
    {
        private readonly List<(WeaponHash Hash, int Ammo)> _weapons;

        /// <summary>
        /// Creates a loadout from any number of (WeaponHash, ammo) pairs.
        /// </summary>
        public WeaponLoadout(params (WeaponHash Hash, int Ammo)[] weapons)
        {
            if (weapons == null || weapons.Length == 0)
                throw new ArgumentException("A loadout must contain at least one weapon.");

            _weapons = new List<(WeaponHash, int)>(weapons);
        }

        // Private constructor used by Load().
        private WeaponLoadout(List<(WeaponHash, int)> weapons)
        {
            _weapons = weapons;
        }

        /// <summary>
        /// Gives this loadout to the player.
        /// </summary>
        /// <param name="clearExisting">
        ///     If true, strips all of the player's current weapons before applying.
        /// </param>
        public void Apply(bool clearExisting = false)
        {
            var player = Game.Player.Character;

            if (clearExisting)
                Function.Call(Hash.REMOVE_ALL_PED_WEAPONS, player.Handle, true);

            foreach (var (hash, ammo) in _weapons)
            {
                bool alreadyHas = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, player.Handle, (uint)hash, false);

                if (!alreadyHas)
                {
                    Function.Call(Hash.GIVE_WEAPON_TO_PED, player.Handle, (uint)hash, ammo, false, true);
                }
                else
                {
                    Function.Call(Hash.ADD_AMMO_TO_PED, player.Handle, (uint)hash, ammo);
                }
            }
        }

        /// <summary>
        /// Saves this loadout to an XML file.
        /// </summary>
        /// <param name="fileName">Name for the file (no extension needed).</param>
        /// <param name="directory">Folder to save into. Created if it doesn't exist.</param>
        public void Save(string fileName, string directory = "scripts/IMNOTEPIX/Loadouts")
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var root = new XElement("Loadout");

            foreach (var (hash, ammo) in _weapons)
            {
                root.Add(new XElement("Weapon", new XAttribute("hash", (uint)hash), new XAttribute("ammo", ammo)));
            }

            string path = Path.Combine(directory, $"{fileName}.xml");
            new XDocument(root).Save(path);
        }

        /// <summary>
        /// Builds a WeaponLoadout from whatever the player is currently carrying.
        /// The result is a normal loadout variable — you can apply, save, or inspect it.
        /// Returns null if the player has no weapons.
        /// </summary>
        public static WeaponLoadout FromPlayer()
        {
            var player = Game.Player.Character;
            var entries = new List<(WeaponHash, int)>();

            foreach (WeaponHash hash in Enum.GetValues(typeof(WeaponHash)))
            {
                if (hash == WeaponHash.Unarmed)
                    continue;

                bool hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, player.Handle, (uint)hash, false);

                if (!hasWeapon)
                    continue;

                int ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, player.Handle, (uint)hash);

                entries.Add((hash, ammo));
            }

            if (entries.Count <= 1)
            {
                GTA.UI.Notification.PostTicker("~y~WeaponLoadout: player is carrying no weapons.", false);
                return null;
            }

            return new WeaponLoadout(entries);
        }

        /// <summary>
        /// Prints every weapon and its ammo count to the screen as a GTA notification.
        /// </summary>
        public void Print()
        {
            var lines = new System.Text.StringBuilder();
            lines.AppendLine("~b~── Weapon Loadout ──");

            foreach (var (hash, ammo) in _weapons)
                lines.AppendLine($"~w~{hash} ~s~| Ammo: ~y~{ammo}");

            GTA.UI.Notification.PostTicker(lines.ToString(), false);
        }

        /// <summary>
        /// Returns a read-only snapshot of the weapons in this loadout.
        /// Useful if you want to loop over them in your own code.
        /// </summary>
        public IReadOnlyList<(WeaponHash Hash, int Ammo)> GetWeapons() => _weapons.AsReadOnly();

        /// <summary>
        /// Loads a saved XML file and returns it as a new WeaponLoadout you can use like any other.
        /// </summary>
        /// <param name="fileName">Name of the file to load (no extension needed).</param>
        /// <param name="directory">Folder to load from.</param>
        /// <returns>A WeaponLoadout built from the file, or null if loading failed.</returns>
        public static WeaponLoadout Load(string fileName, string directory = "scripts/IMNOTEPIX/Loadouts")
        {
            string path = Path.Combine(directory, $"{fileName}.xml");

            if (!File.Exists(path))
            {
                GTA.UI.Notification.PostTicker($"~r~WeaponLoadout: '{fileName}.xml' not found.", false);
                return null;
            }

            try
            {
                var entries = new List<(WeaponHash, int)>();
                var root = XDocument.Load(path).Root;

                foreach (var el in root.Elements("Weapon"))
                {
                    var hash = (WeaponHash)uint.Parse((string)el.Attribute("hash"));
                    int ammo = int.Parse((string)el.Attribute("ammo"));
                    entries.Add((hash, ammo));
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
