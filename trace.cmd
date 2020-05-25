start cmd /k "sxstrace trace -logfile:sxs.trace && sxstrace parse -logfile:sxs.trace -outfile:trace.log && start trace.log"
SysTrayApp.exe
