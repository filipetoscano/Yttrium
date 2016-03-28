if exists ( select name from sys.procedures where name = '{0}' )
    drop procedure {0};
go

{1}
go

/* eof */