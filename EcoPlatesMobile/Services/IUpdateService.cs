public interface IUpdateService
{
    /// <summary>
    /// Checks the latest.json file for updates and prompts the user if needed.
    /// Uses AlertService to show dialogs and opens store URL if user agrees.
    /// </summary>
    Task CheckAndPromptAsync();
}