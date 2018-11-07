using Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Entities
{
    public class Match
    {
        public List<Player> PlayersInMatch { get; set; }
        public Player[,] Board { get; set; }
        Stopwatch gameTime = new Stopwatch();
        public bool HasFinished { get; set; }
        public bool TimerHasFinished { get; set; }
        public bool HasStarted { get; set; }
        private const int FIRST_ROW = 0;
        private const int LAST_ROW = 7;
        private const int FIRST_COLUMN = 0;
        private const int LAST_COLUMN = 7;
        private readonly object lockBoard = new object();

        public Match()
        {
            Board = new Player[8, 8];
            HasStarted = false;
            HasFinished = false;
            TimerHasFinished = false;
            PlayersInMatch = new List<Player>();
        }

        public void StartMatch()
        {
            HasStarted = true;
            gameTime.Start();
            Thread timerThread = new Thread(() => ControllTimer());
            timerThread.Start();
        }

        private void ControllTimer()
        {
            //while (gameTime.Elapsed.Minutes < 3)
            while(gameTime.Elapsed.Seconds < 15)
            {

            }
            if (!HasFinished)
            {
                gameTime.Stop();
                TimerHasFinished = true;
                FinishMatch();
            }
        }

        private void FinishMatch()
        {
            HasFinished = true;
            gameTime.Stop();
            ClientLog.FinishMatch();
            ServerLog.UpdateLog();
        }

        public void AddPlayer(Player newPlayer)
        {
            if (PlayersInMatch.Contains(newPlayer))
            {
                throw new Exception("Error. El jugador ya ha muerto en la partida activa. Espere a que comienze una nueva partida");
            }
            PlayersInMatch.Add(newPlayer);

            Random random = new Random();
            int playerRow = -1;
            int playerColumn = -1;
            while (!PositionIsValid(playerRow, playerColumn))
            {
                playerRow = random.Next(0, 7);
                playerColumn = random.Next(0, 7);
            }
            Board[playerRow, playerColumn] = newPlayer;
            ClientLog.AddPlayer(newPlayer.Nickname, newPlayer.Role.ToString());

        }

        public string MovePlayer(Player player, PlayerGameAction gameAction)
        {
            lock (lockBoard)
            {
                if (TimerHasFinished)
                {
                    if (SurvivorsAlive())
                        throw new Exception("Partida terminada, han ganado los sobrevivientes");
                    else
                        throw new Exception("Partida terminada, nadie ha ganado");
                }
                CheckMatchHasFinished();
                if (player.Movements < 2)
                {
                    Tuple<int, int> actualPosition = GetPlayerPosition(player);
                    if (actualPosition == null)
                    {
                        throw new Exception("Estas muerto");
                    }
                    CheckInBounds(actualPosition, gameAction);
                    CheckFreeSpace(actualPosition, gameAction);
                    PlayerMovement(actualPosition, gameAction, player);
                    ClientLog.PlayerAction(player.Nickname, GameActionString(gameAction));
                    CheckMatchHasFinished();
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

        private string GameActionString(PlayerGameAction gameAction)
        {
            switch (gameAction)
            {
                case PlayerGameAction.AttackDown:
                    return "Atacar hacia abajo";
                    break;
                case PlayerGameAction.AttackLeft:
                    return "Atacar hacia izquierda";
                    break;
                case PlayerGameAction.AttackRight:
                    return "Atacar hacia derecha";
                    break;
                case PlayerGameAction.AttackUp:
                    return "Atacar hacia arriba";
                    break;
                case PlayerGameAction.MoveDown:
                    return "Mover hacia abajo";
                    break;
                case PlayerGameAction.MoveLeft:
                    return "Mover hacia izquierda";
                    break;
                case PlayerGameAction.MoveRight:
                    return "Mover hacia derecha";
                    break;
                case PlayerGameAction.MoveUp:
                    return "Mover hacia arriba";
                default:
                    return "";
                    

            }
        }

        private void CheckMatchHasFinished()
        {
            if (!JustSurvivorsInGame())
            {
                if (JustSurvivorsLeft())
                {
                    FinishMatch();
                    throw new Exception("Partida terminada, han ganado los sobrevivientes");
                }
                else if (OneMonsterLeft())
                {
                    FinishMatch();
                    throw new Exception("Partida terminada, ha ganado el monstruo");
                }
            }
        }

        private bool SurvivorsAlive()
        {
            bool survivorsAlive = false;
            foreach (Player player in PlayersInMatch)
            {
                if (player.Role == Role.Survivor && !player.IsDead)
                {
                    survivorsAlive = true;
                }
            }
            return survivorsAlive;
        }

        private bool JustSurvivorsInGame()
        {
            bool justSurvivors = true;
            foreach (Player player in PlayersInMatch)
            {
                if (player.Role == Role.Monster)
                {
                    justSurvivors = false;
                }
            }
            if (PlayersInMatch.Count > 1)
            {
                return justSurvivors;
            }
            else
                return true;
        }

        private bool JustSurvivorsLeft()
        {
            bool justSurvivorsLeft = true;
            foreach (Player player in PlayersInMatch)
            {
                if (player.Role == Role.Monster && !player.IsDead)
                    justSurvivorsLeft = false;
            }
            return justSurvivorsLeft;
        }

        private bool OneMonsterLeft()
        {
            bool oneMonsterLeft = true;
            int countMonsters = 0;
            foreach (Player player in PlayersInMatch)
            {
                if (player.Role == Role.Survivor && !player.IsDead)
                    oneMonsterLeft = false;
                else if (player.Role == Role.Monster && !player.IsDead)
                    countMonsters = countMonsters + 1;
            }
            if (countMonsters > 1)
                oneMonsterLeft = false;
            return oneMonsterLeft;
        }

        private string GetNearPlayers(Tuple<int, int> newPosition)
        {
            string nearPlayers = "Coordenadas jugador: " + newPosition.Item1 + "," + newPosition.Item2 + " ------ Jugadores proximos: ";
            if (newPosition.Item1 != FIRST_ROW && Board[newPosition.Item1 - 1, newPosition.Item2] != null)
            {
                Player nearPlayer = Board[newPosition.Item1 - 1, newPosition.Item2];
                nearPlayers += "(Arriba) ";
                nearPlayers = CheckRole(nearPlayers, nearPlayer);
            }
            if (newPosition.Item2 != LAST_COLUMN && Board[newPosition.Item1, newPosition.Item2 + 1] != null)
            {
                Player nearPlayer = Board[newPosition.Item1, newPosition.Item2 + 1];
                nearPlayers += "(Derecha) ";
                nearPlayers = CheckRole(nearPlayers, nearPlayer);
            }
            if (newPosition.Item1 != LAST_ROW && Board[newPosition.Item1 + 1, newPosition.Item2] != null)
            {
                Player nearPlayer = Board[newPosition.Item1 + 1, newPosition.Item2];
                nearPlayers += "(Abajo) ";
                nearPlayers = CheckRole(nearPlayers, nearPlayer);
            }
            if (newPosition.Item2 != FIRST_COLUMN && Board[newPosition.Item1, newPosition.Item2 - 1] != null)
            {
                Player nearPlayer = Board[newPosition.Item1, newPosition.Item2 - 1];
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
            foreach (Player player in PlayersInMatch)
            {
                if (player.Movements < 2 && !player.IsDead)
                {
                    turnIsFinished = false;
                }
            }
            if (turnIsFinished)
            {
                foreach (Player player in PlayersInMatch)
                {
                    player.Movements = 0;
                }
            }
        }

        private void PlayerMovement(Tuple<int, int> actualPosition, PlayerGameAction gameAction, Player player)
        {
            if (gameAction.Equals(PlayerGameAction.MoveUp))
            {
                Board[actualPosition.Item1 - 1, actualPosition.Item2] = player;
                Board[actualPosition.Item1, actualPosition.Item2] = null;
            }
            else if (gameAction.Equals(PlayerGameAction.MoveRight))
            {
                Board[actualPosition.Item1, actualPosition.Item2 + 1] = player;
                Board[actualPosition.Item1, actualPosition.Item2] = null;
            }
            else if (gameAction.Equals(PlayerGameAction.MoveDown))
            {
                Board[actualPosition.Item1 + 1, actualPosition.Item2] = player;
                Board[actualPosition.Item1, actualPosition.Item2] = null;
            }
            else if (gameAction.Equals(PlayerGameAction.MoveLeft))
            {
                Board[actualPosition.Item1, actualPosition.Item2 - 1] = player;
                Board[actualPosition.Item1, actualPosition.Item2] = null;
            }
            player.Movements = player.Movements + 1;
            CheckTurns();
        }

        private void CheckInBounds(Tuple<int, int> actualPosition, PlayerGameAction gameAction)
        {
            if (gameAction.Equals(PlayerGameAction.MoveUp))
            {
                if (actualPosition.Item1 == FIRST_ROW)
                {
                    throw new Exception("No se puede mover fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveRight))
            {
                if (actualPosition.Item2 == LAST_COLUMN)
                {
                    throw new Exception("No se puede mover fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveDown))
            {
                if (actualPosition.Item1 == LAST_ROW)
                {
                    throw new Exception("No se puede mover fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveLeft))
            {
                if (actualPosition.Item2 == FIRST_COLUMN)
                {
                    throw new Exception("No se puede mover fuera de los limites del tablero");
                }
            }
        }

        private void CheckFreeSpace(Tuple<int, int> actualPosition, PlayerGameAction gameAction)
        {
            if (gameAction.Equals(PlayerGameAction.MoveUp))
            {
                if (Board[actualPosition.Item1 - 1, actualPosition.Item2] != null)
                {
                    throw new Exception("Hay otro jugador ocupando esa posicion");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveRight))
            {
                if (Board[actualPosition.Item1, actualPosition.Item2 + 1] != null)
                {
                    throw new Exception("Hay otro jugador ocupando esa posicion");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveDown))
            {
                if (Board[actualPosition.Item1 + 1, actualPosition.Item2] != null)
                {
                    throw new Exception("Hay otro jugador ocupando esa posicion");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.MoveLeft))
            {
                if (Board[actualPosition.Item1, actualPosition.Item2 - 1] != null)
                {
                    throw new Exception("Hay otro jugador ocupando esa posicion");
                }
            }
        }

        public string AttackPlayer(Player player, PlayerGameAction gameAction)
        {
            lock (lockBoard)
            {
                if (TimerHasFinished)
                {
                    if (SurvivorsAlive())
                        throw new Exception("Partida terminada, han ganado los sobrevivientes");
                    else
                        throw new Exception("Partida terminada, nadie ha ganado");
                }
                CheckMatchHasFinished();
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
                    CheckMatchHasFinished();
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
                Tuple<int, int> positionAttacked = new Tuple<int, int>(actualPosition.Item1, actualPosition.Item2 - 1);
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
                    SubstractHealthFromPlayer(playerAttacked, player.GetPowerAttack());
                }
                else
                {
                    throw new Exception("Los sobrevivientes no pueden atacarse entre ellos");
                }
            }
            else if (player.Role == Role.Monster)
            {
                SubstractHealthFromPlayer(playerAttacked, player.GetPowerAttack());
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

        public void RemovePlayerFromActiveMatch(Player playerAttacked)
        {
            Tuple<int, int> position = GetPlayerPosition(playerAttacked);
            Board[position.Item1, position.Item2] = null;
            playerAttacked.Movements = 0;
            playerAttacked.IsDead = true;
        }

        private Player GetPlayerByPosition(Tuple<int, int> attackedPosition)
        {
            return Board[attackedPosition.Item1, attackedPosition.Item2];
        }

        private void CheckAttackInBounds(Tuple<int, int> actualPosition, PlayerGameAction gameAction)
        {
            if (gameAction.Equals(PlayerGameAction.AttackUp))
            {
                if (actualPosition.Item1 == FIRST_ROW)
                {
                    throw new Exception("No se puede atacar fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackRight))
            {
                if (actualPosition.Item2 == LAST_COLUMN)
                {
                    throw new Exception("No se puede atacar fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackDown))
            {
                if (actualPosition.Item1 == LAST_ROW)
                {
                    throw new Exception("No se puede atacar fuera de los limites del tablero");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackLeft))
            {
                if (actualPosition.Item2 == FIRST_COLUMN )
                {
                    throw new Exception("No se puede atacar fuera de los limites del tablero");
                }
            }
        }

        private void CheckAttackNotEmptySpace(Tuple<int, int> actualPosition, PlayerGameAction gameAction)
        {
            if (gameAction.Equals(PlayerGameAction.AttackUp))
            {
                if (Board[actualPosition.Item1 - 1, actualPosition.Item2] == null)
                {
                    throw new Exception("No hay ningun jugador en la posicion atacada");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackRight))
            {
                if (Board[actualPosition.Item1, actualPosition.Item2 + 1] == null)
                {
                    throw new Exception("No hay ningun jugador en la posicion atacada");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackDown))
            {
                if (Board[actualPosition.Item1 + 1, actualPosition.Item2] == null)
                {
                    throw new Exception("No hay ningun jugador en la posicion atacada");
                }
            }
            else if (gameAction.Equals(PlayerGameAction.AttackLeft))
            {
                if (Board[actualPosition.Item1, actualPosition.Item2 - 1] == null)
                {
                    throw new Exception("No hay ningun jugador en la posicion atacada");
                }
            }
        }

        private Tuple<int, int> GetPlayerPosition(Player player)
        {
            Tuple<int, int> ret = null;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j] != null && Board[i, j].Equals(player))
                    {
                        ret = new Tuple<int, int>(i, j);
                    }
                }
            }
            return ret;
        }

        public Player GetPlayer(string nickname)
        {
            Player ret = null;
            foreach (Player player in PlayersInMatch)
            {
                if (player.Nickname.Equals(nickname))
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
            if (Board[playerRow, playerColumn] != null)
                return false;
            return true;
        }
    }
}

public enum PlayerGameAction
{
    MoveUp, MoveRight, MoveDown, MoveLeft, AttackUp, AttackRight, AttackDown, AttackLeft
}
