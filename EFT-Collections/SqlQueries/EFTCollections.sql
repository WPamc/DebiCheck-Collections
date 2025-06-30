


declare @deductionday as int = 31
SELECT 
     mm.subssn,CONVERT(datetime, DATEFROMPARTS(YEAR(base_month), MONTH(base_month),
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
       GETDATE() AS RelatedCycleDate into #z
FROM (
    SELECT *,
           DATEADD(month,
                   CASE WHEN DAY(GETDATE()) <= DEDUCTIONDAY THEN 0 ELSE 1 END,
                   DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS base_month
    FROM MEMBMANDATE_BANKHIST
) AS mmb
INNER JOIN MEMB_MASTERS AS mm ON mmb.MEMBID = mm.MEMBID and getdate() between mmb.FROMDATE and isnull(mmb.TODATE, getdate ())
inner join    GetsavviDEBICheck dbc
 on mm.subssn = dbc.subssn and (DeductionDate_RequestedCollectionDate = N'2025-05-31 12:00:00')
 inner join MEMB_HPHISTS mh on mm.MEMBID = mh.membid 
 
 and getdate() between mh.OPFROMDT and isnull(mh.OPTHRUDT,getdate())
WHERE DEDUCTIONDAY =@DEDUCTIONDAY and [day]=@DEDUCTIONDAY

and mm.subssn >= 'MGS173464' and mm.subssn < 'MGS594673'

and mm.subssn not in ('MGS171817','MGS174660','MGS694952','MGS125485','MGS695365','MGS846141','MGS791031','MGS620074','MGS708141','MGS609368','MGS845016',
'MGS739507','MGS605275','MGS778135','MGS541901','MGS622051','MGS807510','MGS823038','MGS838723','MGS842938','MGS844959','MGS844977','MGS844985','MGS845241',
'MGS845243','MGS845249','MGS845251','MGS845345','MGS845370','MGS845591','MGS845842','MGS846383','MGS846429','MGS846471','MGS846497','MGS846812','MGS849410','MGS849411','MGS589298')
--select * from #z

DECLARE @DATEREQUESTED AS DATE = '2025/06/30'

 SELECT  #z.SUBSSN,
  isnull(mm.BRANCHCODE,'') HomingBranch,
 isnull(mm.ACCNR, '') HomingAccountNumber ,
mm.FIRSTNM + ' ' + mm.LASTNM HomingAccountName ,
1 AccountType ,
#z.instructedamount Amount ,
'AUL       ' +substring(#z.SUBSSN,4,10)+'_'+convert(varchar(4), year(getdate()))+'-'+convert(varchar(4), month(getdate())) UserReference 
, @DATEREQUESTED DATEREQUESTED

from  #z
  inner JOIN MEMB_MASTERS MM ON MM.SUBSSN =  #z.SUBSSN AND RLSHIP = 1 
    inner join MEMB_HPHISTS mh on mm.MEMBID = mh.membid 
 
 and getdate() between mh.OPFROMDT and isnull(mh.OPTHRUDT,getdate()) where mm.subssn not in (select subssn from BILLING_COLLECTIONREQUESTS where DATEREQUESTED between '2025/06/01' and '2025/06/30')
 
 drop table #z