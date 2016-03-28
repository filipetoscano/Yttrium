if exists ( select name from sys.views where name = '{0}' )
    drop view {0};
go

{1}
go

/* eof */