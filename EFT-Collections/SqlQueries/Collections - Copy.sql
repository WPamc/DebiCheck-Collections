
 SELECT  'MGS'+REFERENCENO SUBSSN,
  isnull(mm.BRANCHCODE,'') HomingBranch,
 isnull(mm.ACCNR, '') HomingAccountNumber ,
mm.FIRSTNM + ' ' + mm.LASTNM HomingAccountName ,
1 AccountType ,
g.[AMOUNT TO BE BILLED] Amount ,
'AUL       ' +REFERENCENO+'_'+convert(varchar(4), year(getdate()))+'-'+convert(varchar(4), month(getdate())) UserReference 
FROM GetsavviDeductions G LEFT OUTER JOIN [BILLING_COLLECTIONREQUESTS] B 
  ON G.REFERENCENO = B.REFERENCE 
  inner JOIN MEMB_MASTERS MM ON MM.SUBSSN =  'MGS'+REFERENCENO AND RLSHIP = 1 
  WHERE B.REFERENCE IS NULL 
  AND [AMOUNT TO BE BILLED] > 0  