namespace IMNOTEPIX.Framework.fInterfaces
{
    public interface IScene
    {
        void Start();
        void Update();
        void Stop();
        bool IsFinished { get; }
    }
}