 SELECT  z.SUBSSN,
  isnull(mm.BRANCHCODE,'') HomingBranch,
 isnull(mm.ACCNR, '') HomingAccountNumber ,
mm.FIRSTNM + ' ' + mm.LASTNM HomingAccountName ,
1 AccountType ,
z.AMOUNTREQUESTED Amount ,
'AUL       ' +substring(z.SUBSSN,4,10)+'_'+CONVERT(varchar(10), @DATEREQUESTED,112) UserReference 
, CASE
        WHEN EffectiveBillingMonth IS NULL
          OR DeductionDay        IS NULL       -- either column missing ⇒ no date
        THEN NULL

        -- if the requested day exists in the month, use it
        WHEN DeductionDay <= DAY(EOMONTH(EffectiveBillingMonth))
        THEN DATEFROMPARTS(
                 YEAR(EffectiveBillingMonth),
                 MONTH(EffectiveBillingMonth),
                 DeductionDay)

        -- otherwise fall back to the month-end
        ELSE EOMONTH(EffectiveBillingMonth) end DATEREQUESTED

from BILLING_SPECINSTRUCTIONS z
  inner JOIN MEMB_MASTERS MM ON MM.SUBSSN =  z.SUBSSN AND RLSHIP = 1 
  AND  CASE
        WHEN EffectiveBillingMonth IS NULL
          OR DeductionDay        IS NULL       -- either column missing ⇒ no date
        THEN NULL

        -- if the requested day exists in the month, use it
        WHEN DeductionDay <= DAY(EOMONTH(EffectiveBillingMonth))
        THEN DATEFROMPARTS(
                 YEAR(EffectiveBillingMonth),
                 MONTH(EffectiveBillingMonth),
                 DeductionDay)

        -- otherwise fall back to the month-end
        ELSE EOMONTH(EffectiveBillingMonth) end = @DATEREQUESTED
