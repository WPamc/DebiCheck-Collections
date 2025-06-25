SELECT TOP 1
    ClientCode,
    ClientName,
    BankservUserCode,
    CreditorBranch,
    CreditorAccount,
    CreditorAbbreviation,
    TypeOfService
FROM dbo.CreditorDefaults
WHERE CreditorId = @CREDITORID;
