/* Checks if the current database version is greater
 * than the version of this patch. To use this in the
 * SSMS the SQLCMD mode should be enabled.
 */
:on error exit

declare @continue bit,
  @objectname varchar(120),
  @objectversion int

set @objectname = 'mtc_add_tag' /* the name of the object related with the script */
set @objectversion = 1 /* the current object version */

exec @continue = nohros_updateversion @objectname=@objectname, @objectversion=@objectversion
if @continue != 1
begin /* version guard */
  raiserror(
    'The version of the database is greater than the version of this script. The batch will be stopped', 11, 1
  )
end /* version guard */

/* create and empty procedure with the name [@pbjectname]. So, we
 * can use the [alter proc [@objectname] statement]. This simulates
 * the behavior of the ALTER OR REPLACE statement that exists in other
 * datbases products. */
exec nohros_createproc @name = @objectname
go

/**
 * Copyright (c) 2011 by Nohros Inc, All rights reserved.
 */
alter proc mtc_add_tag (
  @name varchar(800),
  @value varchar(800),
  @serie_id bigint
)
as

declare @tag_id bigint

select @tag_id = tag_id
from mtc_tag
where tag_name = @name
  and tag_value = @value

if (@tag_id is null)
begin
  insert into mtc_tag(
     tag_name
    ,tag_value
  ) values (
     @name
    ,@value
  )

  set @tag_id = scope_identity()
end

if not exists(
  select tag_id
  from mtc_tag_serie
  where serie_id = @serie_id
    and tag_id = @tag_id
)
begin
  insert into mtc_tag_serie(
     tag_id
    ,serie_id
  ) values (
     @tag_id
    ,@serie_id
  )
end

select @tag_id