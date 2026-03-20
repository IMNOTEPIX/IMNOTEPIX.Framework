using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.Funcs
{
    /// <summary>
    /// Manages a single scripted or entity-attached fire instance.
    /// Use the static helpers for world queries and bulk creation.
    /// </summary>
    public class Fire
    {
        // ─────────────────────────────────────────────────────────────────────
        // Instance state
        // ─────────────────────────────────────────────────────────────────────

        public int Id { get; private set; }
        public Vector3 Position { get; private set; }
        public Entity Entity { get; private set; }
        public bool GasPowered { get; private set; }

        private int _spreadLimit;

        /// <summary>Clamped to 0–25 by the setter.</summary>
        public int SpreadLimit
        {
            get => _spreadLimit;
            set => _spreadLimit = Math.Max(0, Math.Min(25, value));
        }

        public bool IsValid => Id != 0;

        /// <summary>
        /// For entity fires: queries IS_ENTITY_ON_FIRE.
        /// For script fires: relies on IsValid — no native exists to query a
        /// script fire handle's active state directly.
        /// </summary>
        public bool IsActive => Entity != null
            ? Function.Call<bool>(Hash.IS_ENTITY_ON_FIRE, Entity)
            : IsValid;

        private bool IsEntityFire => Entity != null;

        // ─────────────────────────────────────────────────────────────────────
        // Construction
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Script fire at a world position.</summary>
        public Fire(Vector3 position, int spreadLimit = 1, bool gasPowered = false)
        {
            Position = position;
            SpreadLimit = spreadLimit;
            GasPowered = gasPowered;
        }

        /// <summary>Fire attached to an entity.</summary>
        public Fire(Entity entity)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            Position = entity.Position;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Lifecycle
        // ─────────────────────────────────────────────────────────────────────

        public void Start()
        {
            if (IsEntityFire)
                Id = Function.Call<int>(Hash.START_ENTITY_FIRE, Entity);
            else
                Id = Function.Call<int>(Hash.START_SCRIPT_FIRE,
                    Position.X, Position.Y, Position.Z, SpreadLimit, GasPowered);
        }

        public void Remove()
        {
            if (IsEntityFire)
                Function.Call(Hash.STOP_ENTITY_FIRE, Entity);
            else
                Function.Call(Hash.REMOVE_SCRIPT_FIRE, Id);

            Id = 0;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Static world queries
        // ─────────────────────────────────────────────────────────────────────

        public static bool IsEntityOnFire(Entity entity)
            => Function.Call<bool>(Hash.IS_ENTITY_ON_FIRE, entity);

        public static int GetCountInRange(Vector3 position, float radius)
            => Function.Call<int>(Hash.GET_NUMBER_OF_FIRES_IN_RANGE,
                position.X, position.Y, position.Z, radius);

        public static void SetSpreadRate(float multiplier)
            => Function.Call(Hash.SET_FLAMMABILITY_MULTIPLIER, multiplier);

        public static void StopInRange(Vector3 position, float radius)
            => Function.Call(Hash.STOP_FIRE_IN_RANGE,
                position.X, position.Y, position.Z, radius);

        public static unsafe bool TryGetClosestPosition(Vector3 position, out Vector3 firePosition)
        {
            Vector3 result = default;
            bool found = Function.Call<bool>(Hash.GET_CLOSEST_FIRE_POS,
                &result, position.X, position.Y, position.Z);
            firePosition = found ? result : default;
            return found;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Static bulk creation
        // ─────────────────────────────────────────────────────────────────────

        // Shared RNG — static to avoid identical seeds when called rapidly
        private static readonly Random _rng = new Random();

        /// <summary>
        /// Spawns fires scattered randomly within a circle around <paramref name="center"/>.
        /// Returns all created instances so they can be removed later.
        /// </summary>
        public static List<Fire> CreateInRadius(
            Vector3 center,
            float radius,
            int count,
            int spreadLimit = 1,
            bool gasPowered = false)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Must be greater than 0.");

            var fires = new List<Fire>(count);

            for (int i = 0; i < count; i++)
            {
                double angle = _rng.NextDouble() * Math.PI * 2;
                double distance = Math.Sqrt(_rng.NextDouble()) * radius; // sqrt = uniform disc

                var pos = new Vector3(
                    center.X + (float)(Math.Cos(angle) * distance),
                    center.Y + (float)(Math.Sin(angle) * distance),
                    center.Z);

                var fire = new Fire(pos, spreadLimit, gasPowered);
                fire.Start();
                fires.Add(fire);
            }

            return fires;
        }

        /// <summary>
        /// Spawns fires in a uniform grid across a rectangle defined by two corners.
        /// Ground Z is sampled at each grid point.
        /// </summary>
        public static List<Fire> CreateInArea(
            Vector3 min,
            Vector3 max,
            float spacing,
            int spreadLimit = 1,
            bool gasPowered = false)
        {
            if (spacing <= 0)
                throw new ArgumentOutOfRangeException(nameof(spacing), "Must be greater than 0.");

            var fires = new List<Fire>();

            for (float x = min.X; x <= max.X; x += spacing)
            {
                for (float y = min.Y; y <= max.Y; y += spacing)
                {
                    var pos = new Vector3(x, y, SampleGroundZ(x, y, min.Z));
                    var fire = new Fire(pos, spreadLimit, gasPowered);
                    fire.Start();
                    fires.Add(fire);
                }
            }

            return fires;
        }

        /// <summary>
        /// Removes all fires in the list.
        /// The list is cleared after removal — do not rely on its contents afterwards.
        /// </summary>
        public static void RemoveAll(List<Fire> fires)
        {
            foreach (var fire in fires)
                fire.Remove();

            fires.Clear();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────

        private static unsafe float SampleGroundZ(float x, float y, float searchStartZ)
        {
            float gz = 0f;
            Function.Call(Hash.GET_GROUND_Z_FOR_3D_COORD,
                x, y, searchStartZ + 100f, &gz, false);
            return gz;
        }
    }
}
