using System.ComponentModel;

namespace IMNOTEPIX.Framework.fInterfaces
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IScene
    {
        void Start();
        void Update();
        void Stop();
        bool IsFinished { get; }
    }
}