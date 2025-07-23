
SELECT CONVERT(datetime, DATEFROMPARTS(YEAR(mmb.base_month), MONTH(mmb.base_month), 
CASE WHEN mmb.DEDUCTIONDAY > DAY(EOMONTH(base_month)) THEN DAY(EOMONTH(base_month)) ELSE mmb.DEDUCTIONDAY END), 23) AS RequestedCollectionDate, mmb.DEDUCTIONDAY, 
             mmb.MANDATEUSERREF + '/' + CONVERT(varchar(10), CONVERT(datetime, 
             DATEFROMPARTS(YEAR(mmb.base_month), MONTH(mmb.base_month), CASE WHEN mmb.DEDUCTIONDAY > DAY(EOMONTH(base_month)) THEN DAY(EOMONTH(base_month)) ELSE mmb.DEDUCTIONDAY END), 23), 
             23) AS PaymentInformation, 3 AS TrackingPeriod, 'RCUR' AS DebitSequence, '0021' AS EntryClass, mmb.AUTHORISATIONREF AS MandateReference, mmb.BRANCHCODE AS DebtorBankBranch, mm.FIRSTNM, mm.LASTNM AS DebtorName, mmb.ACCNR AS DebtorAccountNumber, 
             mmb.ACCTYPE AS AccountType, mmb.MANDATEUSERREF AS ContractReference, GETDATE() AS RelatedCycleDate, TOTAL AS InstructedAmount
FROM   (SELECT MEMBID, BANK, BRANCHCODE, ACCNR, ACCTYPE, ACCHOLDNAME, DEDUCTIONDAY, MANDATEUSERREF, AUTHORISATIONREF, CURRHIST, FROMDATE, TODATE, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE, SEQUENCE, ROWID, 
                           DATEADD(month, CASE WHEN DAY(GETDATE()) <= DEDUCTIONDAY THEN 0 ELSE 1 END, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS base_month
             FROM    MEMB_MANDATEHIST) AS mmb INNER JOIN
             MEMB_MASTERS AS mm ON mmb.MEMBID = mm.MEMBID AND GETDATE() BETWEEN mmb.FROMDATE AND ISNULL(mmb.TODATE, GETDATE()) INNER JOIN
             MEMB_HPHISTS AS mh ON mm.MEMBID = mh.MEMBID AND GETDATE() BETWEEN mh.OPFROMDT AND ISNULL(mh.OPTHRUDT, GETDATE())

			LEFT OUTER JOIN SUBSSN_LAST_BILLING_AMOUNT SM ON MM.SUBSSN = SM.SUBSSN 
WHERE (mmb.DEDUCTIONDAY =@DeductionDay) and isnull(mmb.AUTHORISATIONREF,'') <> ''
and @EffectiveBillingsDate   between opfromdt and isnull(opthrudt,@EffectiveBillingsDate)