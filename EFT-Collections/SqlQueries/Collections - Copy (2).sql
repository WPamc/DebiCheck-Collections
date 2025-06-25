
 SELECT  z.SUBSSN,
  isnull(mm.BRANCHCODE,'') HomingBranch,
 isnull(mm.ACCNR, '') HomingAccountNumber ,
mm.FIRSTNM + ' ' + mm.LASTNM HomingAccountName ,
1 AccountType ,
z.AMOUNTREQUESTED Amount ,
'AUL       ' +substring(z.SUBSSN,4,10)+'_'+convert(varchar(4), year(getdate()))+'-'+convert(varchar(4), month(getdate())) UserReference 
FROM (SELECT        BILLING_COLLECTIONREQUESTS.subssn, AMOUNTREQUESTED, BILLING_COLLECTIONREQUESTS.DATEREQUESTED
FROM            BILLING_COLLECTIONREQUESTS INNER JOIN
                             (SELECT        r.SUBSSN, MAX(r.DATEREQUESTED) AS DATEREQUESTED
                               FROM            EDI_BANK_FILES AS bf INNER JOIN
                                                         BILLING_COLLECTIONREQUESTS AS r ON r.EDIBANKFILEROWID = bf.ROWID
                               WHERE        (r.SUBSSN IN ('MGS694952', 'MGS125485', 'MGS620074', 'MGS739507', 'MGS605275', 'MGS807510', 'MGS846812', 'MGS849410'))
                               GROUP BY r.SUBSSN) AS z ON BILLING_COLLECTIONREQUESTS.DATEREQUESTED = z.DATEREQUESTED AND BILLING_COLLECTIONREQUESTS.SUBSSN = z.SUBSSN
							   ) z
  inner JOIN MEMB_MASTERS MM ON MM.SUBSSN =  z.SUBSSN AND RLSHIP = 1 
 