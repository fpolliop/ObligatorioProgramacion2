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
                throw new Exception("Error. El jugador ya ha muerto en la partida activa. Espere a que comienze una nueva partida");
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
                    if (actualPosition == null) {
                        throw new Exception("Estas muerto");
                    }
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
            string nearPlayers = "Coordenadas jugador: " + newPosition.Item1 + "," + newPosition.Item2 + " ------ Jugadores proximos: ";
            if (newPosition.Item1 != 0 && board[newPosition.Item1 - 1, newPosition.Item2] != null)
            {
                Player nearPlayer = board[newPosition.Item1 - 1, newPosition.Item2];
                nearPlayers += "(Arriba) ";
                nearPlayers = CheckRole(nearPlayers, nearPlayer);
            }
            if (newPosition.Item2 != 7 && board[newPosition.Item1, newPosition.Item2 + 1] != null)
            {
                Player nearPlayer = board[newPosition.Item1 , newPosition.Item2 + 1];
                nearPlayers += "(Derecha) ";
                nearPlayers = CheckRole(nearPlayers, nearPlayer);
            }
            if (newPosition.Item1 != 7 && board[newPosition.Item1 + 1, newPosition.Item2] != null)
            {
                Player nearPlayer = board[newPosition.Item1 + 1, newPosition.Item2];
                nearPlayers += "(Abajo) ";
                nearPlayers = CheckRole(nearPlayers, nearPlayer);
            }
            if (newPosition.Item2 != 0 && board[newPosition.Item1, newPosition.Item2 - 1] != null)
            {
                Player nearPlayer = board[newPosition.Item1, newPosition.Item2 - 1];
                nearPlayers += "(Izquierda) ";
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
                if (player.Movements < 2 && !player.IsDead)
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

        public string AttackPlayer(Player player, PlayerGameAction gameAction)
        {
            lock (lockBoard)
            {
                if (player.Movements < 2)
                {
                    Tuple<int, int> actualPosition = GetPlayerPosition(player);
                    if (actualPosition == null)
                    {
                        throw new Exception("Estas muerto");
                    }
                    CheckAttackInBounds(actualPosition, gameAction);
                    CheckAttackNotEmptySpace(actualPosition, gameAction);
                    Attack(actualPosition, gameAction, player);
                    
                    string nearPlayers = GetNearPlayers(actualPosition);
                    return nearPlayers;
                }
                else
                {
                    throw new Exception("No tiene mas movimientos, espere as que termine el turno actual");
                }
            }
        }

        private void Attack(Tuple<int, int> actualPosition, PlayerGameAction gameAction, Player player)
        {
            if (gameAction.Equals(PlayerGameAction.AttackUp))
            {
                Tuple<int, int> positionAttacked = new Tuple<int, int>(actualPosition.Item1 - 1, actualPosition.Item2);
                Player playerAttacked = GetPlayerByPosition(positionAttacked);
                ExecuteAttack(player, playerAttacked);
            }
            else if (gameAction.Equals(PlayerGameAction.AttackRight))
            {
                Tuple<int, int> positionAttacked = new Tuple<int, int>(actualPosition.Item1, actualPosition.Item2 + 1);
                Player playerAttacked = GetPlayerByPosition(positionAttacked);
                ExecuteAttack(player, playerAttacked);
            }
            else if (gameAction.Equals(PlayerGameAction.AttackDown))
            {
                Tuple<int, int> positionAttacked = new Tuple<int, int>(actualPosition.Item1 + 1, actualPosition.Item2);
                Player playerAttacked = GetPlayerByPosition(positionAttacked);
                ExecuteAttack(player, playerAttacked);
            }
            else if (gameAction.Equals(PlayerGameAction.AttackLeft))
            {
                Tuple<int, int> positionAttacked = new Tuple<int, int>(actualPosition.Item1, actualPosition.Item2 -1);
                Player playerAttacked = GetPlayerByPosition(positionAttacked);
                ExecuteAttack(player, playerAttacked);
            }
            player.Movements = player.Movements + 1;
            CheckTurns();
        }

        private void ExecuteAttack(Player player, Player playerAttacked)
        {
            if (player.Role == Role.Survivor)
            {
                if (playerAttacked.Role == Role.Monster)
                {
                    SubstractHealthFromPlayer(playerAttacked, player.SurvivorPowerAttack);
                }
                else
                {
                    throw new Exception("Los sobrevivientes no pueden atacarse entre ellos");
                }
            }
            else if (player.Role == Role.Monster)
            {
                SubstractHealthFromPlayer(playerAttacked, player.MonsterPowerAttack);
            }
        }

        private void SubstractHealthFromPlayer(Player playerAttacked, int attackPower)
        {
            playerAttacked.Health = playerAttacked.Health - attackPower;
            if (playerAttacked.Health <= 0)
            {
                RemovePlayerFromActiveMatch(playerAttacked);
            }
        }

        private void RemovePlayerFromActiveMatch(Player playerAttacked)
        {
            Tuple<int, int> position = GetPlayerPosition(playerAttacked);
            board[position.Item1, position.Item2] = null;
            playerAttacked.Movements = 0;
            playerAttacked.IsDead = true;
        }

        private Player GetPlayerByPosition(Tuple<int, int> attackedPosition)
        {
            return board[attackedPosition.Item1, attackedPosition.Item2];
        }

        private void CheckAttackInBounds(Tuple<int, int> actualPosition, PlayerGameAction gameAction)
        {
            if (gameAction.Equals(PlayerGameAction.AttackUp))
            {
                if (actualPosition.Item1 == 0)
                {
                    throw new Exception("No se puede atacar fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackRight))
            {
                if (actualPosition.Item2 == 7)
                {
                    throw new Exception("No se puede atacar fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackDown))
            {
                if (actualPosition.Item1 == 7)
                {
                    throw new Exception("No se puede atacar fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackLeft))
            {
                if (actualPosition.Item2 == 0)
                {
                    throw new Exception("No se puede atacar fuera de los limites del tablero");
                }
            }
        }

        private void CheckAttackNotEmptySpace(Tuple<int, int> actualPosition, PlayerGameAction gameAction)
        {
            if (gameAction.Equals(PlayerGameAction.AttackUp))
            {
                if (board[actualPosition.Item1 - 1, actualPosition.Item2] == null)
                {
                    throw new Exception("No hay ningun jugador en la posicion atacada");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackRight))
            {
                if (board[actualPosition.Item1, actualPosition.Item2 + 1] == null)
                {
                    throw new Exception("No hay ningun jugador en la posicion atacada");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackDown))
            {
                if (board[actualPosition.Item1 + 1, actualPosition.Item2] == null)
                {
                    throw new Exception("No hay ningun jugador en la posicion atacada");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackLeft))
            {
                if (board[actualPosition.Item1, actualPosition.Item2 - 1] == null)
                {
                    throw new Exception("No hay ningun jugador en la posicion atacada");
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

