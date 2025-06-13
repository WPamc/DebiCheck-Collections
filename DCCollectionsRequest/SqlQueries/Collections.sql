SELECT 

mmb.MANDATEUSERREF AS PaymentInformation,
       CONVERT(datetime, DATEFROMPARTS(YEAR(base_month), MONTH(base_month),
              CASE WHEN DEDUCTIONDAY > DAY(EOMONTH(base_month))
                   THEN DAY(EOMONTH(base_month)) ELSE DEDUCTIONDAY END), 23) AS RequestedCollectionDate,
       DEDUCTIONDAY,
       3 AS TrackingPeriod,
       'RCUR' AS DebitSequence,
       '0021' AS EntryClass,
       1501 AS InstructedAmount,
       mmb.AUTHORISATIONREF AS MandateReference,
       mmb.BRANCHCODE AS DebtorBankBranch,
       mm.FIRSTNM,
       mm.LASTNM AS DebtorName,
       mmb.ACCNR AS DebtorAccountNumber,
       mmb.ACCTYPE AS AccountType,
       mmb.MANDATEUSERREF AS ContractReference,
       GETDATE() AS RelatedCycleDate
FROM (
    SELECT *,
           DATEADD(month,
                   CASE WHEN DAY(GETDATE()) <= DEDUCTIONDAY THEN 0 ELSE 1 END,
                   DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS base_month
    FROM MEMBMANDATE_BANKHIST
) AS mmb
INNER JOIN MEMB_MASTERS AS mm ON mmb.MEMBID = mm.MEMBID
WHERE DEDUCTIONDAY = @DEDUCTIONDAY;
