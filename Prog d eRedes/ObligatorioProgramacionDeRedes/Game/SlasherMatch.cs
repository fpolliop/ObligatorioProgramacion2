using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class SlasherMatch
    {
        List<Player> playersInMatch;
        int round;

        Player[,] board;

        Stopwatch gameTime = new Stopwatch();
        public bool MatchHasFinished { get; set; }

        public SlasherMatch()
        {
            board = new Player[8, 8];
            round = 1;
        }
        public void StartMatch()
        {
            MatchHasFinished = false;
            gameTime.Start();
            Thread timerThread = new Thread(() => ControllTimer());
        }

        private void ControllTimer()
        {
            while(gameTime.Elapsed.Minutes < 3)
            {
                
            }
            gameTime.Stop();
            FinishMatch();
        }

        private void FinishMatch()
        {
            MatchHasFinished = true;
        }

        public void AddPlayer(Player newPlayer)
        {
            playersInMatch.Add(newPlayer);

            Random random = new Random();
            int playerRow = -1;
            int playerColumn = -1;
            while(!PositionIsValid(playerRow, playerColumn))
            {
                playerRow = random.Next(0, 7);
                playerColumn = random.Next(0, 7);
            }
            
        }

        public bool PositionIsValid(int playerRow, int playerColumn)
        {
            if (playerRow.Equals(-1) || playerColumn == -1)
                return false;
            if (board[playerRow, playerColumn] != null)
                return false;
            return true;
        }

        public void HandlePlayerAction()
        {

        }
    }
}

public enum PlayerGameAction
{
    MoveUp,
    MoveRight,
    MoveDown,
    MoveLeft,
    AttackUp,
    AttackRight,
    AttackDown,
    AttackLeft
}

