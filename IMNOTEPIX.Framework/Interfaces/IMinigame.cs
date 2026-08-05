using System.ComponentModel;

namespace IMNOTEPIX.Framework.fInterfaces
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IMinigame
    {
        void Update();
        void Dispose();
        void PushResetOnDeath();
    }
}
