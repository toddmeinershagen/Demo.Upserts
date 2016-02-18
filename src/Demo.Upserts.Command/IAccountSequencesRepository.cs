namespace Demo.Upserts.Command
{
    public interface IAccountSequencesRepository
    {
        Version GetVersionFor(int accountNumber);
        Version Increment(int accountNumber);
        void RemoveCounts();
        void LoadEmptyAccounts(int numberOfAccounts);
    }
}