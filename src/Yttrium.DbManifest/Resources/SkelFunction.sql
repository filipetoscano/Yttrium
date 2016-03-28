if exists ( select name from sys.objects where name = '{0}' and xtype in ( 'FN', 'IF', 'TF', 'FS', 'FT' ) )
    drop function {0};
go

{1}
go

/* eof */