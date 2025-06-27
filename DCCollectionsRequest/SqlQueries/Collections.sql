
SELECT 


       CONVERT(datetime, DATEFROMPARTS(YEAR(base_month), MONTH(base_month),
              CASE WHEN DEDUCTIONDAY > DAY(EOMONTH(base_month))
                   THEN DAY(EOMONTH(base_month)) ELSE DEDUCTIONDAY END), 23) AS RequestedCollectionDate,
       DEDUCTIONDAY,
	   mmb.MANDATEUSERREF+'/'+   convert( varchar(10),
	   CONVERT(datetime, DATEFROMPARTS(YEAR(base_month), MONTH(base_month),
              CASE WHEN DEDUCTIONDAY > DAY(EOMONTH(base_month))
                   THEN DAY(EOMONTH(base_month)) ELSE DEDUCTIONDAY END), 23),23)  AS PaymentInformation,
       3 AS TrackingPeriod,
       'RCUR' AS DebitSequence,
       '0021' AS EntryClass,
       InstructedAmount AS InstructedAmount,
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
INNER JOIN MEMB_MASTERS AS mm ON mmb.MEMBID = mm.MEMBID and getdate() between mmb.FROMDATE and isnull(mmb.TODATE, getdate ())
inner join    GetsavviDEBICheck dbc
 on mm.subssn = dbc.subssn and (DeductionDate_RequestedCollectionDate = N'2025-04-28 12:00:00')
 inner join MEMB_HPHISTS mh on mm.MEMBID = mh.membid 
 
 and getdate() between mh.OPFROMDT and isnull(mh.OPTHRUDT,getdate())
WHERE DEDUCTIONDAY =@DEDUCTIONDAY and [day]=@DEDUCTIONDAY

and mm.subssn not in (
'MGS605275',
'MGS589298',
'MGS174660',
'MGS695365',
'MGS708141',
'MGS845016',
'MGS778135',
'MGS541901',
'MGS844959',
'MGS844977',
'MGS844985',
'MGS845241',
'MGS845243',
'MGS845249',
'MGS845591',
'MGS845842',
'MGS846383',
'MGS846429',
'MGS846497',
'MGS849411',
'MGS589298')