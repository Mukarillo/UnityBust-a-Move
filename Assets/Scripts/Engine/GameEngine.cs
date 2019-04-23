using System;
using System.Threading.Tasks;

namespace BAMEngine
{
    public class GameEngine
    {
        private const int TIME_TO_STEP_DOWN = 5;
        public bool DEBUG { get; private set; } = true;

        public Board board { get; private set; }

        private readonly Action onCreateNextPiece;
        private readonly Action onStepDown;
        private readonly Action onGameOver;

        private Task mTask;
        private bool mGameOver;

        public GameEngine(Action onCreateNextPiece, Action onStepDown, Action onGameOver)
        {
            MainThread.Initiate();
            MainThread.SetMainThread();

            board = new Board(this);
            this.onCreateNextPiece = onCreateNextPiece;
            this.onStepDown = onStepDown;
            this.onGameOver = onGameOver;

            StartCounting();
        }

        ~GameEngine()
        {
            mTask?.Dispose();
        }

        private void StartCounting()
        {
            mTask = Task.Delay(TIME_TO_STEP_DOWN * 1000).ContinueWith(t =>
            {
                if (!mGameOver)
                {
                    StartCounting();
                    MainThread.Invoke(OnStepDown);
                }
            });
        }

        private void OnStepDown()
        {
            board.StepDown();
            onStepDown?.Invoke();
        }

        public void UpdatePiecePosition(Piece piece, int line, int position)
        {
            board.UpdatePiecePosition(piece, line, position);
        }

        public void LockPiece(Piece piece)
        {
            board.LockPiece(piece);
            onCreateNextPiece?.Invoke();
        }

        public Piece GetNextPiece()
        {
            var piece = NormalPiece.GetRandom();
            piece.UpdatePosition(board.lines[Board.MAX_LINES - 1], (Board.MAX_PIECES_PER_LINE / 2) - 1);
            return piece;
        }

        internal void GameOver()
        {
            mGameOver = true;
            onGameOver?.Invoke();
        }
    }
}
