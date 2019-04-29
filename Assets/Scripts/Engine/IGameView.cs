namespace BAMEngine
{
    public interface IGameView
    {
        bool ShouldStepDown { get; }
        int TimeToStepDown { get; }
        void OnCreateNextPiece();
        void OnStepDown();
        bool CanStepDown();
        void OnGameOver();
    }
}
