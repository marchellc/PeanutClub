using SecretLabAPI.Actions.API;
using SecretLabAPI.Actions.Attributes;

namespace SecretLabAPI.Actions.Functions
{
    /// <summary>
    /// Contains player functions.
    /// </summary>
    public static class PlayerFunctions
    {
        /// <summary>
        /// Bans one or more players from the server using the specified reason and duration.
        /// </summary>
        /// <remarks>The ban reason and duration are retrieved from the context parameters. All selected
        /// players in the context will be banned for the specified duration. Ensure that the context is properly
        /// compiled with valid parameters before calling this method.</remarks>
        /// <param name="context">The action context containing player selection and ban parameters. Must include a reason and a duration (in
        /// seconds) for the ban.</param>
        /// <returns>An ActionResultFlags value indicating the result of the ban operation. Returns SuccessDispose if the action
        /// completes successfully.</returns>
        [Action("Ban", "Bans players from the server.")]
        [ActionParameter("Reason", "Reason for the ban.")]
        [ActionParameter("Duration", "Duration of the ban (in seconds).")]
        public static ActionResultFlags BanPlayers(ref ActionContext context)
        {
            context.EnsureCompiled((index, parameter) =>
            {
                return index switch
                {
                    0 => parameter.EnsureCompiled("No reason specified."),
                    1 => parameter.EnsureCompiled<long>(long.TryParse, 0),

                    _ => false
                };
            });

            var reason = context.GetMemoryOrValue("BanReason", 0);
            var duration = context.GetMemoryOrValue<long>("BanDuration", 1);

            context.ForEach(p => p.Ban(reason, duration));
            return ActionResultFlags.SuccessDispose;
        }

        /// <summary>
        /// Kicks one or more players from the server using the specified action context.
        /// </summary>
        /// <remarks>The reason for the kick is retrieved from the context using the key "KickReason". All
        /// players specified in the context are kicked with the same reason.</remarks>
        /// <param name="context">A reference to the action context containing information about the players to kick and the reason for the
        /// kick. Must be compiled before use.</param>
        /// <returns>An ActionResultFlags value indicating the result of the operation. Returns SuccessDispose if the players
        /// were kicked successfully.</returns>
        [Action("Kick", "Kicks a player from the server.")]
        [ActionParameter("Reason", "The reason for the kick")]
        public static ActionResultFlags KickPlayers(ref ActionContext context)
        {
            context.EnsureCompiled((index, parameter) =>
            {
                return index switch
                {
                    0 => parameter.EnsureCompiled("No reason specified."),
                    _ => false,
                };
            });

            var reason = context.GetMemoryOrValue("KickReason", 0);

            context.ForEach(p => p.Kick(reason));
            return ActionResultFlags.SuccessDispose;
        }
    }
}