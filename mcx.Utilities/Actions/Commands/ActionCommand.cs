using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

using mcx.Utilities.Actions.Targets;

using UnityEngine;

namespace mcx.Utilities.Actions.Commands
{
    [Command("action", "Base command for all action commands")]
    public class ActionCommand : CommandBase, IServerSideCommand
    {
        [CommandOverload("list", "Lists all available actions", null)]
        public void List()
        {
            Ok(x =>
            {
                x.AppendLine();

                ActionManager.AppendDebug(x);
            });
        }

        [CommandOverload("trigger", "Triggers an action for a player.", null)]
        public void Trigger(
            [CommandParameter("ID", "The ID of the action to invoke.")] string id,
            [CommandParameter("Player", "The player to trigger the action for.")] ExPlayer player,
            [CommandParameter("Parameters", "The parameters for the action.")] Dictionary<string, string>? parameters = null)
        {
            parameters ??= new();

            var action = new ActionInfo() { Id = id };

            foreach (var p in parameters)
                action.Parameters[p.Key] = [p.Value];

            if (action.TriggerSingle(null, new TargetPlayer(player)))
                Ok($"Triggered action '{id}' for player {player.ToCommandString()}");
            else
                Fail($"Action '{id}' could not be triggered");
        }

        [CommandOverload("triggerat", "Triggers an action at a position.", null)]
        public void TriggerAt(
            [CommandParameter("ID", "The ID of the action to invoke.")] string id, 
            [CommandParameter("Position", "The position to trigger the action at.")] Vector3 position,  
            [CommandParameter("Parameters", "The parameters for the action.")] Dictionary<string, string>? parameters = null)
        {
            parameters ??= new();

            var action = new ActionInfo() { Id = id };

            foreach (var p in parameters)
                action.Parameters[p.Key] = [p.Value];

            if (action.TriggerSingle(null, new TargetPosition(position)))
                Ok($"Triggered action '{id}' at {position.ToPreciseString()}");
            else
                Fail($"Action '{id}' could not be triggered");
        }
    }
}