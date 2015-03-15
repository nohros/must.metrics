if exists (select name from sys.tables where name = 'mtc_tag')
begin
  if (select count(*) from mtc_tag) > 0
  begin
    raiserror('A tabela [mtc_tag] contem dados e não pode ser excluida.',16,1)
  end
  drop table mtc_tag
end
go

create table mtc_tag (
  tag_id bigint identity(1,1) not null,
  tag_name varchar(64) not null,
  tag_value varchar(128) not null
)

alter table mtc_tag
add constraint PK_mtc_tag
primary key (
  tag_id
)

create unique nonclustered index IX_mtc_tag_natural
on mtc_tag (
   tag_name
  ,tag_value
)
