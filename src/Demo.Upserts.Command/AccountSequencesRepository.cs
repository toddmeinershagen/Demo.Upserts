using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

using Dapper;

namespace Demo.Upserts.Command
{
    public class AccountSequencesRepository : IAccountSequencesRepository
    {
        public Version GetVersionFor(int accountNumber)
        {
            const string sql = "select Sequence from dbo.AccountSequences where AccountNumber = @accountNumber";

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                return (Version)connection.Query<int>(sql, new {accountNumber}).FirstOrDefault();
            }
        }

        public Version Increment(int accountNumber)
        {
            const string sql = @"merge AccountSequences as target
using (select @accountNumber) as source (AccountNumber)
on (target.AccountNumber = source.AccountNumber)
when matched then 
	update set Sequence = target.Sequence + 1
when not matched THEN   
	insert (AccountNumber, Sequence) values (source.AccountNumber, 1)
output inserted.Sequence;";

            using (var connection = new SqlConnection(ConnectionString))
            {
                return (Version)connection.ExecuteScalar<int>(sql, new {accountNumber}, commandTimeout: State.CommandTimeoutInSeconds);
            }
        }

        public void RemoveCounts()
        {
            const string sql = @"if not exists (select * from sys.tables where name = 'AccountSequences')
begin
	create table [dbo].[AccountSequences](
		[AccountNumber] [int] NOT NULL,
		[Sequence] [int] NULL,
	primary key clustered 
	(
		[AccountNumber] ASC
	) with (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) on [PRIMARY]
end;

truncate table dbo.AccountSequences;";

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Execute(sql, commandTimeout: State.CommandTimeoutInSeconds);
            }
        }

        public void LoadEmptyAccounts(int numberOfAccounts)
        {
            const string sql = @"set nocount on;
declare @counter int = 0

while (@counter < @rowsToProcess)
begin
	insert into dbo.AccountSequences (AccountNumber, Sequence) values (@counter, 0)
	set @counter = @counter + 1
end

set nocount off;";

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute(sql, new { rowsToProcess = numberOfAccounts }, commandTimeout: State.CommandTimeoutInSeconds);
            }
        }

        public string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Test"].ConnectionString; }
        }
    }
}