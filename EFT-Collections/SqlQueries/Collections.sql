
 SELECT  z.SUBSSN,
  isnull(mm.BRANCHCODE,'') HomingBranch,
 isnull(mm.ACCNR, '') HomingAccountNumber ,
mm.FIRSTNM + ' ' + mm.LASTNM HomingAccountName ,
1 AccountType ,
z.AMOUNTREQUESTED Amount ,
'AUL       ' +substring(z.SUBSSN,4,10)+'_'+convert(varchar(4), year(getdate()))+'-'+convert(varchar(4), month(getdate())) UserReference 
FROM (select 'MGS125485'	as SUBSSN					   , 1283 AMOUNTREQUESTED
union all
select 'MGS807510'						   , 977	
union all
select 'MGS846812'						   , 1283
union all
select 'MGS849410'						   , 1501) z
  inner JOIN MEMB_MASTERS MM ON MM.SUBSSN =  z.SUBSSN AND RLSHIP = 1 
 