using BAMEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Testor : MonoBehaviour
    {
        public Board board;

        public InputField lineInput;
        public InputField indexInput;
        public InputField typeInput;

        public void Start()
        {
            board = new Board();
        }

        public void Execute()
        {
            board.PlacePiece(new NormalPiece((NormalPiece.PieceType)int.Parse(typeInput.text)), int.Parse(lineInput.text), int.Parse(indexInput.text));
        }
    }
}