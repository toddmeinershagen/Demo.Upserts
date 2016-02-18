select	AccountNumber, 
		Sequence 
from	dbo.AccountSequences with (nolock)
where	Sequence > 0

select	sum(Sequence)
from	dbo.AccountSequences with (nolock)
where	Sequence > 0