namespace BAMEngine
{
    public class SpecialPiece : Piece
    {
        public virtual void Execute()
        {

        }

        public override void Break()
        {
            throw new System.NotImplementedException();
        }

        public override void Fall()
        {
            throw new System.NotImplementedException();
        }
    }
}