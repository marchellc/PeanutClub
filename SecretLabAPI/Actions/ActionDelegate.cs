namespace SecretLabAPI.Actions
{
    /// <summary>
    /// Represents a method that performs an action on a specified target using the provided action information and
    /// context.
    /// </summary>
    /// <param name="target">The object on which the action is performed. This parameter represents the target of the action and may be used
    /// to determine the context or recipient.</param>
    /// <param name="info">An ActionInfo instance containing details about the action to be performed. Provides metadata or parameters
    /// relevant to the action.</param>
    /// <param name="index">The zero-based index of the current action within the actions list. Used to identify the position of the action
    /// in the sequence.</param>
    /// <param name="actions">A list of ActionInfo objects representing all available actions in the current context. May be used to access
    /// related or sequential actions.</param>
    /// <returns>true if the action was successfully performed; otherwise, false.</returns>
    public delegate bool ActionDelegate(ref object target, ActionInfo info, int index, List<ActionInfo> actions);
}