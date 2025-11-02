using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

using mcx.Levels.API;

namespace mcx.Levels.Commands
{
    [Command("level", "Base command for level management")]
    public class LevelCommand : CommandBase, IServerSideCommand
    {
        [CommandOverload("Shows the level of a specific player.", null)]
        public void Show(
            [CommandParameter("Target", "The player to show the level of")] ExPlayer? target = null)
        {
            target ??= Sender;

            var levelData = target.GetSavedLevel();

            if (levelData == null)
            {
                Fail($"Could not load level data of player {target.ToCommandString()}");
                return;
            }

            Ok($"Player {target.ToCommandString()} has level '{levelData.Level}' ({levelData.Experience} XP / {levelData.RequiredExperience} XP)");
        }

        [CommandOverload("setlevel", "Sets the level of a player.", null)]
        public void SetLevel(string userId, int level)
        {
            if (LevelManager.SetLevel(userId, level))
            {
                Ok($"Set level of player '{userId}' to '{level}'");
            }
            else
            {
                Fail($"Could not set level of player '{userId}' to '{level}'");
            }
        }

        [CommandOverload("setxp", "Sets the experience of a player.", null)]
        public void SetExperience(string userId, int exp)
        {
            if (LevelManager.SetExperience(userId, exp))
            {
                Ok($"Set XP of player '{userId}' to '{exp}'");
            }
            else
            {
                Fail($"Could not set XP of player '{userId}' to '{exp}'");
            }
        }

        [CommandOverload("addxp", "Adds experience points to a player.", null)]
        public void AddExperience(string userId, int exp)
        {
            if (LevelManager.AddExperience(userId, "Command", exp))
            {
                Ok($"Added '{exp}' XP to player '{userId}'");
            }
            else
            {
                Fail($"Could not add '{exp}' XP to player '{userId}'");
            }
        }

        [CommandOverload("subxp", "Subtracts experience points from a player.", null)]
        public void SubtractExperience(string userId, int exp)
        {
            if (LevelManager.SubstractExperience(userId, "Command", exp))
            {
                Ok($"Removed '{exp}' XP from player '{userId}'");
            }
            else
            {
                Fail($"Could not remove '{exp}' XP from player '{userId}'");
            }
        }

        [CommandOverload("reset", "Resets the level of a player.", null)]
        public void ResetLevel(string userId)
        {
            if (LevelManager.ResetLevel(userId))
            {
                Ok($"Reset level of player '{userId}' to 1");
            }
            else
            {
                Fail($"Could not reset level of player '{userId}' to default");
            }
        }

        [CommandOverload("resetall", "Resets the level of a player.", null)]
        public void ResetLevels()
        {
            if (LevelManager.ResetLevels())
            {
                Ok($"Reset levels of all players to 1");
            }
            else
            {
                Fail($"Could not reset levels of all players to default");
            }
        }
    }
}