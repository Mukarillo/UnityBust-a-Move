using System.Threading.Tasks;

namespace BAMEngine
{
    public class GameEngine
    {
        public bool DEBUG { get; private set; } = true;

        public Board board { get; private set; }
        private IGameView mGameView;

        private Task mTask;
        private bool mGameOver;

        public GameEngine(IGameView gameView)
        {
            MainThread.Initiate();
            MainThread.SetMainThread();

            board = new Board(this);

            mGameView = gameView;

            if (mGameView.ShouldStepDown)
                StartCounting();
        }

        ~GameEngine()
        {
            mTask?.Dispose();
        }

        private void StartCounting()
        {
            mTask = Task.Delay(mGameView.TimeToStepDown * 1000).ContinueWith(t =>
            {
                if (!mGameOver)
                    MainThread.Invoke(StepDown);
            });
        }

        public void StepDown()
        {
            if (mGameView.CanStepDown())
            {
                board.StepDown();
                mGameView.OnStepDown();
                StartCounting();
            }
        }

        public void UpdatePiecePosition(Piece piece, int line, int position)
        {
            board.UpdatePiecePosition(piece, line, position);
        }

        public void LockPiece(Piece piece)
        {
            board.LockPiece(piece);
            mGameView.OnCreateNextPiece();
        }

        public Piece GetNextPiece()
        {
            var piece = NormalPiece.GetRandom();
            return piece;
        }

        internal void GameOver()
        {
            mGameOver = true;
            mGameView.OnGameOver();
        }
    }
}
