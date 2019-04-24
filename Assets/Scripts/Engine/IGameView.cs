namespace BAMEngine
{
    public interface IGameView
    {
        bool ShouldStepDown { get; }
        void OnCreateNextPiece();
        void OnStepDown();
        bool CanStepDown();
        void OnGameOver();
    }
}
