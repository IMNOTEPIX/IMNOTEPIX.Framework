using System.ComponentModel;

namespace IMNOTEPIX.Framework.Abstracts
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class AInterior
    {
        public bool IsLoaded { get; private set; } = false;

        public void Load()
        {
            if (IsLoaded)
                return;

            OnLoad();

            IsLoaded = true;
        }

        public void Refresh()
        {
            if (!IsLoaded)
            {
                Load();
                return;
            }

            OnLoad();
        }

        public void Unload(bool force = false)
        {
            if (!force && !IsLoaded)
                return;

            fWorld.Map.Interior.ClearInteriorStateOfEntity(fPlayer.Player.Character);

            OnUnload();

            IsLoaded = false;
        }

        protected abstract void OnLoad();

        protected abstract void OnUnload();
    }
}