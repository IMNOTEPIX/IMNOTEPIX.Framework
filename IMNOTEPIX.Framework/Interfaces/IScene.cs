namespace IMNOTEPIX.Framework.Interfaces
{
    public interface IScene
    {
        void Start();
        void Update();
        void Stop();
        bool IsFinished { get; }
    }
}