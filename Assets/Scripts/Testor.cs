using System;
using BAMEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Testor : MonoBehaviour
    {
        public GameEngine engine;

        public InputField lineInput;
        public InputField indexInput;
        public InputField typeInput;

        public void Start()
        {
            engine = new GameEngine(OnCreateNextPiece);

        }

        private void OnCreateNextPiece()
        {

        }

        public void Execute()
        {
            //board.LockPiece(new NormalPiece((NormalPiece.PieceType)int.Parse(typeInput.text)), int.Parse(lineInput.text), int.Parse(indexInput.text));
        }
    }
}