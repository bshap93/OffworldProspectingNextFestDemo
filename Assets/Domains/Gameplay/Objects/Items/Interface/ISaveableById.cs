namespace Domains.Items.Interface
{
    public interface ISaveableById
    {
        string UniqueID { get; }
        void RestoreState(bool wasPreviouslyDestroyed);
    }
}