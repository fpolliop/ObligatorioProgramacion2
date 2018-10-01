using Repository;
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
        public bool hasFinished{ get; set; }
        public bool hasStarted;
        private readonly object lockBoard = new object();

        public SlasherMatch()
        {
            board = new Player[8, 8];
            round = 1;
            hasStarted = false;
            hasFinished = false;
            playersInMatch = new List<Player>();
        }
        public void StartMatch()
        {
            hasStarted = true;
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
            hasFinished = true;
        }

        public void AddPlayer(Player newPlayer)
        {
            if (playersInMatch.Contains(newPlayer))
            {
                throw new Exception("Error. El jugador ya se ha unido a la partida.");
            }
            playersInMatch.Add(newPlayer);

            Random random = new Random();
            int playerRow = -1;
            int playerColumn = -1;
            while(!PositionIsValid(playerRow, playerColumn))
            {
                playerRow = random.Next(0, 7);
                playerColumn = random.Next(0, 7);
            }
            board[playerRow, playerColumn] = newPlayer;
            
        }

        public string MovePlayer(Player player, PlayerGameAction gameAction)
        {
            lock (lockBoard)
            {
                if (player.Movements < 2)
                {
                    Tuple<int,int> actualPosition = GetPlayerPosition(player);
                    CheckInBounds(actualPosition, gameAction);
                    CheckFreeSpace(actualPosition, gameAction);
                    PlayerMovement(actualPosition, gameAction, player);
                    Tuple<int, int> newPosition = GetPlayerPosition(player);
                    string nearPlayers = GetNearPlayers(newPosition);
                    return nearPlayers;
                }
                else
                {
                    throw new Exception("No tiene mas moviminetos, espere as que termine el turno actual");
                }
            }
        }

        private string GetNearPlayers(Tuple<int,int> newPosition)
        {
            string nearPlayers = "Coordenadas jugador: " + newPosition.Item1 + "," + newPosition.Item2 + " ------ Jugadores cercanos: ";
            if (newPosition.Item1 != 0 && board[newPosition.Item1 - 1, newPosition.Item2] != null)
            {
                Player nearPlayer = board[newPosition.Item1 - 1, newPosition.Item2];
                nearPlayers = CheckRole(nearPlayers, nearPlayer);
            }
            if (newPosition.Item2 != 7 && board[newPosition.Item1, newPosition.Item2 + 1] != null)
            {
                Player nearPlayer = board[newPosition.Item1 , newPosition.Item2 + 1];
                nearPlayers = CheckRole(nearPlayers, nearPlayer);
            }
            if (newPosition.Item1 != 7 && board[newPosition.Item1 + 1, newPosition.Item2] != null)
            {
                Player nearPlayer = board[newPosition.Item1 + 1, newPosition.Item2];
                nearPlayers = CheckRole(nearPlayers, nearPlayer);
            }
            if (newPosition.Item2 != 0 && board[newPosition.Item1, newPosition.Item2 - 1] != null)
            {
                Player nearPlayer = board[newPosition.Item1, newPosition.Item2 - 1];
                nearPlayers = CheckRole(nearPlayers, nearPlayer);
            }
            return nearPlayers;
        }

        private string CheckRole(string nearPlayers, Player nearPlayer)
        {
            if (nearPlayer.Role == Role.Survivor)
            {
                nearPlayers += nearPlayer.Nickname + ": Sobreviviente, vida restante: " + nearPlayer.Health + ", ";
            }
            else
            {
                nearPlayers += nearPlayer.Nickname + ": Monstruo, vida restante: " + nearPlayer.Health + ", ";
            }

            return nearPlayers;
        }

        private void CheckTurns()
        {
            bool turnIsFinished = true;
            foreach (Player player in playersInMatch)
            {
                if (player.Movements < 2)
                {
                    turnIsFinished = false;
                }            
            }
            if (turnIsFinished)
            {
                foreach (Player player in playersInMatch)
                {
                    player.Movements = 0;
                }
            }
        }

        private void PlayerMovement(Tuple<int, int> actualPosition, PlayerGameAction gameAction, Player player)
        {
            if (gameAction.Equals(PlayerGameAction.MoveUp))
            {
                board[actualPosition.Item1 - 1, actualPosition.Item2] = player;
                board[actualPosition.Item1, actualPosition.Item2] = null;                
            }
            else if (gameAction.Equals(PlayerGameAction.MoveRight))
            {
                board[actualPosition.Item1, actualPosition.Item2 +1] = player;
                board[actualPosition.Item1, actualPosition.Item2] = null;
            }
            else if (gameAction.Equals(PlayerGameAction.MoveDown))
            {
                board[actualPosition.Item1 + 1, actualPosition.Item2] = player;
                board[actualPosition.Item1, actualPosition.Item2] = null;
            }
            else if (gameAction.Equals(PlayerGameAction.MoveLeft))
            {
                board[actualPosition.Item1, actualPosition.Item2 - 1] = player;
                board[actualPosition.Item1, actualPosition.Item2] = null;
            }
            player.Movements = player.Movements + 1;
            CheckTurns();
        }

        private void CheckInBounds(Tuple<int, int> actualPosition, PlayerGameAction gameAction)
        {
            if (gameAction.Equals(PlayerGameAction.MoveUp))
            {
                if (actualPosition.Item1 == 0 )
                {
                    throw new Exception("No se puede mover fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveRight))
            {
                if (actualPosition.Item2 == 7)
                {
                    throw new Exception("No se puede mover fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveDown))
            {
                if (actualPosition.Item1 == 7)
                {
                    throw new Exception("No se puede mover fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveLeft))
            {
                if (actualPosition.Item2 == 0)
                {
                    throw new Exception("No se puede mover fuera de los limites del tablero");
                }
            }
        }

        private void CheckFreeSpace(Tuple<int, int> actualPosition, PlayerGameAction gameAction)
        {
            if (gameAction.Equals(PlayerGameAction.MoveUp))
            {
                if(board[actualPosition.Item1 - 1, actualPosition.Item2] != null)
                {
                    throw new Exception("Hay otro jugador ocupando esa posicion");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveRight))
            {
                if (board[actualPosition.Item1, actualPosition.Item2 + 1 ] != null)
                {
                    throw new Exception("Hay otro jugador ocupando esa posicion");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveDown))
            {
                if (board[actualPosition.Item1 + 1, actualPosition.Item2] != null)
                {
                    throw new Exception("Hay otro jugador ocupando esa posicion");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveLeft))
            {
                if (board[actualPosition.Item1, actualPosition.Item2 - 1] != null)
                {
                    throw new Exception("Hay otro jugador ocupando esa posicion");
                }
            }
        }

        private Tuple<int, int> GetPlayerPosition(Player player)
        {
            Tuple<int,int> ret = null;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i,j] != null && board[i, j].Equals(player))
                    {
                        ret = new Tuple<int, int>(i, j);
                    }
                }
            }
            return ret;
        }

        public Player GetPlayer(User user)
        {
            Player ret = null;
            foreach (Player player in playersInMatch)
            {
                if (player.Nickname.Equals(user.Nickname))
                {
                    ret = player;
                }
            }
            return ret;
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

