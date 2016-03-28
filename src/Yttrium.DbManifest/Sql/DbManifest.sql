if exists ( select id from sysobjects where name = 'DbManifest' and xtype = 'P' )
    drop procedure DbManifest;
go

-- $Id: DbManifest.sql 34 2014-10-23 16:40:59Z lft $
create procedure DbManifest
as
begin
    set nocount on;

    --
    -- Reference for: sys.objects
    -- http://msdn.microsoft.com/en-us/library/ms190324.aspx
    --


    --
    -- 0, Database
    --
    select name, collation_name, is_fulltext_enabled, is_broker_enabled
    from sys.databases
    where name = db_name();


    --
    -- 1+2, Tables and Columns
    --
    select T.object_id, T.name
    from sys.tables as T
    order by T.name;

    select T.object_id, C.name, IST.name as system_type, C.max_length,
        C.precision, C.scale, C.is_nullable, C.collation_name,
        CC.value as description
    from sys.objects as T
    inner join sys.columns as C
        on ( T.object_id = C.object_id )
    left outer join sys.extended_properties CC
        on ( CC.major_id = T.object_id and CC.minor_id = C.column_id and CC.name = 'MS_Description' )
    inner join sys.types as ST
        on ( C.system_type_id = ST.system_type_id and C.user_type_id = ST.user_type_id )
    inner join sys.types as IST
        on ( ST.system_type_id = IST.system_type_id and IST.system_type_id = IST.user_type_id )
    where T.type = 'U'
    order by T.name, C.column_id;


    --
    -- 3+4, Indexes
    --
    select T.object_id, I.index_id, I.name, I.type, I.is_primary_key, I.is_unique_constraint
    from sys.tables T
    inner join sys.indexes I
        on ( T.object_id = I.object_id and I.is_hypothetical = 0 and I.type <> 0 )
    where T.is_ms_shipped = 0
    order by T.name;

    select T.object_id, I.index_id, C.name, IC.is_descending_key, IC.is_included_column
    from sys.tables T
    inner join sys.indexes I
        on ( T.object_id = I.object_id and I.is_hypothetical = 0 and I.type <> 0 )
    inner join sys.index_columns IC
        on ( IC.object_id = I.object_id and IC.index_id = I.index_id )
    inner join sys.columns C
        on ( T.object_id = C.object_id and IC.column_id = C.column_id )
    where T.is_ms_shipped = 0
    order by T.name, IC.index_column_id;


    --
    -- 5+6, Views and Columns
    --
    select V.object_id, V.name, M.definition, checksum( M.definition ) as checksum
    from sys.views V
    inner join sys.sql_modules M
        on ( V.object_id = M.object_id );

    select V.object_id, C.name, IST.name as system_type, C.max_length, C.precision, C.scale, C.is_nullable, C.collation_name
    from sys.views as V
    inner join sys.columns as C
        on ( V.object_id = C.object_id )
    inner join sys.types as ST
        on ( C.system_type_id = ST.system_type_id and C.user_type_id = ST.user_type_id )
    inner join sys.types as IST
        on ( ST.system_type_id = IST.system_type_id and IST.system_type_id = IST.user_type_id )
    order by V.name, C.column_id;


    --
    -- 7+8, Procedures and Parameters
    --
    select P.object_id, P.name, M.definition, checksum( M.definition ) as checksum
    from sys.procedures as P
    inner join sys.sql_modules M
        on ( P.object_id = M.object_id )
    order by P.name;

    select P.object_id, V.name, IST.name as system_type, V.max_length, V.precision, V.scale, V.is_output
    from sys.procedures as P
    inner join sys.parameters as V
        on ( P.object_id = V.object_id )
    inner join sys.types as ST
        on ( V.system_type_id = ST.system_type_id and V.user_type_id = ST.user_type_id )
    inner join sys.types as IST
        on ( ST.system_type_id = IST.system_type_id and IST.system_type_id = IST.user_type_id )
    order by P.name, V.parameter_id;


    --
    -- 9+10, Functions and Parameters
    --
    select F.object_id, F.name, F.type, M.definition, checksum( M.definition ) as checksum
    from sys.objects as F
    inner join sys.sql_modules M
        on ( F.object_id = M.object_id )
    where F.type in ( 'FN', 'IF', 'TF', 'FS', 'FT' )
        and F.is_ms_shipped = 0
    order by F.name;

    select P.object_id, V.name, IST.name as system_type, V.max_length, V.precision, V.scale, V.is_output
    from sys.objects as P
    inner join sys.parameters as V
        on ( P.object_id = V.object_id )
    inner join sys.types as ST
        on ( V.system_type_id = ST.system_type_id and V.user_type_id = ST.user_type_id )
    inner join sys.types as IST
        on ( ST.system_type_id = IST.system_type_id and IST.system_type_id = IST.user_type_id )
    where P.type in ( 'FN', 'IF', 'TF', 'FS', 'FT' )
        and P.is_ms_shipped = 0
    order by P.name, V.parameter_id;

    return 0;
end
go

/* eof */