README: orapack
=========================================================================

orapack is a command-line utility which aggregates procedures, functions
and type declarations (each versioned seperately in the source control
repository) into a single Oracle package.


Usage
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

> orapack
orapack v1.0
usage: orapack [OPTIONS] FILES [FILES [...]]

  p, package    Required. Name of Oracle package [Required]
  s             Place header/body into separate files (pks, pkb)
  help          Display help screen


The standard command-line use is:

> orapack --package=PackageName Private\*.sql Public\*.sql

Please note that this statement assumes that the database structure in
the code repository is the following:

   Database/
     Bootstrap/
     Data/
     PackageName/   <-- Run command from here
       Private/
       Public/

In cases where a single database contains only a single package, then
the database structure can take a simplified form:

   Database/
     Bootstrap/
     Data/
     Private/
     Public/



File structure: Public globals (variables, types)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The format of the public globals file is as follows:

------------ 8< ------------ 8< ------------ 8< ------------ 8< ---------
-- $Id: README-orapack.txt 23 2014-10-20 16:32:24Z lft $
-- [orapack:globals]

type r_record is record (
    Column varchar(50)
);

type t_record is table of r_record;

/* eof */
------------ 8< ------------ 8< ------------ 8< ------------ 8< ---------



File structure: Public procedures/functions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The format of a public procedure/function is as follows:

------------ 8< ------------ 8< ------------ 8< ------------ 8< ---------
-- $Id: README-orapack.txt 23 2014-10-20 16:32:24Z lft $
create or replace procedure ProcedureName
(
)
as
begin

    -- TODO

end ProcedureName;

/* eof */
------------ 8< ------------ 8< ------------ 8< ------------ 8< ---------



File structure: Private procedures/functions
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The format of a private procedure/function is as follows:

------------ 8< ------------ 8< ------------ 8< ------------ 8< ---------
-- $Id: README-orapack.txt 23 2014-10-20 16:32:24Z lft $
-- [orapack:private]
create or replace procedure PrivateProcedureName
(
)
as
begin

    -- TODO

end PrivateProcedureName;

/* eof */
------------ 8< ------------ 8< ------------ 8< ------------ 8< ---------


You'll want copy-paste the contents inside the --- 8< --- cut here markers.
The first line must be -- $Id: README-orapack.txt 23 2014-10-20 16:32:24Z lft $, and the last line must be /* eof */


Warnings
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The file-name *MUST* be the name of the object contained inside, otherwise
the file WILL be ignored.

If the object being packaged is a function/procedure overload, then the
file-name can end in _\d+.

   GetTime_1.sql
   GetTime_2.sql

The formatting must be respected *EXACTLY*: if the is/as keywords do not
start at the beginning of the line, then orapack will place the entire 
contents in the header. You've been warned! :)

/* eof */